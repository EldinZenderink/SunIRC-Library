using System;
using System.Diagnostics;
using SimpleIRCLib;
using System.IO;
using System.Threading;
using WebSocketSharp.Server;
using WebSocketSharp;
using Newtonsoft.Json;
using System.Collections.Generic; // C:\ 
using System.Reflection;
// home/myfodler/<-  

namespace SunIRCLibrary
{
    class WebSocketHandler : WebSocketBehavior
    {
        private SimpleIRC irc;
        private Thread checkMessagesToSend = null;
        private UtitlityMethods utilityMethods = null;
        private Thread makeSureConnection = null;
        private bool isLocal;

        public WebSocketHandler() 
        {
            isLocal = SunIRCInit.isLocal;
            irc = SharedData.irc;
            utilityMethods = new UtitlityMethods();
            SharedData.AddToMessageList("HELLO LITTLE WEEB");
            checkMessagesToSend = new Thread(new ThreadStart(messagesToSend));
            checkMessagesToSend.Start();
        }
        
        private void messagesToSend()
        {
            while(SharedData.messageToSendWS != null)
            {
                Thread.Sleep(100);
                string messageToSend = SharedData.getAndRemoveFromMessageList();
                if (messageToSend != "" && messageToSend != null)
                {
                    while (true)
                    {
                        try
                        {
                            Send(messageToSend);
                            //Debug.WriteLine("DEBUG-WEBSOCKETHANDLER - MSG SEND: " + messageToSend);
                            break;
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine("WSDEBUG-WEBSOCKETHANDLER: COULD NOT SEND DATA: " + messageToSend);
                            Debug.WriteLine("WSDEBUG-WEBSOCKETHANDLER: COULD NOT SEND DATA: " + e.ToString());
                        }
                    }
                    
                }

            }

        }

        protected override void OnClose(CloseEventArgs e)
        {
            if (!isLocal)
            {
                Debug.WriteLine("WSDEBUG-WEBSOCKETHANDLER: CLIENT DISCONNECTED!");
                Sessions.CloseSession(this.ID);
                disconnectIrc();
                checkMessagesToSend.Abort();

            }
        }

        private void getIrcData()
        {
            JsonIrcUpdate update = new JsonIrcUpdate();
            update.connected = SharedData.irc.isClientRunning();
            update.downloadlocation = SharedData.currentDownloadLocation;
            try
            {
                update.server = SharedData.irc.newIP + ":" + SharedData.irc.newPort;
                update.user = SharedData.irc.newUsername;
                update.channel = SharedData.irc.newChannel;

            }
            catch (Exception e)
            {
                Debug.WriteLine("WSDEBUG-WEBSOCKETHANDLER: no irc connection yet.");
                update.server = "";
                update.user = "";
                update.channel = "";
            }

            update.local = isLocal;
            SharedData.AddToMessageList(JsonConvert.SerializeObject(update, Formatting.Indented));
        }

        private void getDownloads()
        {
            string[] filePaths = Directory.GetFiles(SharedData.currentDownloadLocation);
            int a = 0;
            JsonAlreadyDownloaded alreadyDownloadedList = new JsonAlreadyDownloaded();

            List<JsonDownloadUpdate> listWithFiles = new List<JsonDownloadUpdate>();

            foreach (string filePath in filePaths)
            {
                if (utilityMethods.IsMediaFile(filePath)){

                    string filename = Path.GetFileName(filePath);
                    FileInfo info = new FileInfo(filePath);
                    int filesize = (int)(info.Length / 1048576);

                    JsonDownloadUpdate alreadyDownloaded = new JsonDownloadUpdate();
                    alreadyDownloaded.id = a.ToString();
                    alreadyDownloaded.progress = "100";
                    alreadyDownloaded.speed = "0";
                    alreadyDownloaded.status = "ALREADYDOWNLOADED";
                    alreadyDownloaded.filename = filename;
                    alreadyDownloaded.filesize = filesize.ToString();
                    listWithFiles.Add(alreadyDownloaded);

                    a++;
                }
            }
            alreadyDownloadedList.alreadyDownloaded = listWithFiles;

            SharedData.AddToMessageList(JsonConvert.SerializeObject(alreadyDownloadedList, Formatting.Indented));
        }

        private void addDownload(dynamic download)
        {
            
            try
            {
                string dlId = download.id.ToString();
                string dlPack = download.pack.ToString();
                string dlBot = download.bot.ToString();

                dlData d = new dlData();
                d.dlId = dlId;
                d.dlBot = dlBot;
                d.dlPack = dlPack;
                d.dlIndex = SharedData.downloadList.Count;
                SharedData.AddToDownloadList(d);

                //Debug.WriteLine("DEBUG-WEBSOCKETHANDLER:DONE ADDING BATCH TO DOWLOADS: ID:" + dlId + " XDCC: /msg " + dlBot + " xdcc send #" + dlPack);

            }
            catch (Exception ex)
            {
                Debug.WriteLine("DEBUG-WEBSOCKETHANDLER:ERROR: " + ex.ToString());
            }
        }

        private void abortDownload()
        {
            try
            {
                SharedData.irc.stopXDCCDownload();
            }
            catch
            {
                Debug.WriteLine("DEBUG-WEBSOCKETHANDLER: ERROR: tried to stop download but there isn't anything downloading or no connection to irc");
            }
        }

        private void deleteDownload(dynamic download)
        {
            string dlId = download.id;
            string fileName = download.filename;
            if (SharedData.currentDownloadId == dlId)
            {
                SharedData.removeIfDownloadIsInDownloadList(dlId);
                try
                {
                    //Debug.WriteLine("I guess I should Delete stuff");
                    SharedData.irc.stopXDCCDownload();

                }
                catch
                {
                    Debug.WriteLine("DEBUG-WEBSOCKETHANDLER: ERROR: tried to stop download but there isn't anything downloading or no connection to irc");
                }
            }
            else
            {
                // Debug.WriteLine("DEBUG-WEBSOCKETHANDLER: QUEU LENGTH BEFORE REMOVING: " + SharedData.downloadList.Count);

                SharedData.removeIfDownloadIsInDownloadList(dlId);
                // Debug.WriteLine("DEBUG-WEBSOCKETHANDLER: QUEU LENGTH AFTER REMOVING: " + SharedData.downloadList.Count);


                try
                {
                    // Debug.WriteLine("DEBUG-WEBSOCKETHANDLER: YOU MORON... no actually, THIS SHOULD ONLY HAPPEN... well.. when you actually want to delete stuff x)");
                    File.Delete(SharedData.currentDownloadLocation + "\\" + fileName);
                }
                catch (IOException ex)
                {
                    Debug.WriteLine("DEBUG-WEBSOCKETHANDLER: ERROR:  We've got a problem :( -> " + ex.ToString());
                }
            }
        }

        private void openDownloadDirectory()
        {
            Process.Start(SharedData.currentDownloadLocation);
        }

        private void setDownloadDirectoryV2(string path)
        {
            try
            {
                // Debug.WriteLine("DEBUG-WEBSOCKETHANDLER: opening file dialog.");
                SharedData.currentDownloadLocation = path;
                SharedData.irc.setCustomDownloadDir(SharedData.currentDownloadLocation);
                SharedData.settings.saveSettings();

                SharedData.httpserver.downloadDir = SharedData.currentDownloadLocation;

                JsonIrcUpdate update = new JsonIrcUpdate();
                update.connected = SharedData.irc.isClientRunning();
                update.downloadlocation = SharedData.currentDownloadLocation;
                try
                {
                    update.server = SharedData.irc.newIP + ":" + SharedData.irc.newPort;
                    update.user = SharedData.irc.newUsername;
                    update.channel = SharedData.irc.newChannel;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("WSDEBUG-WEBSOCKETHANDLER: no irc connection yet.");
                    update.server = "";
                    update.user = "";
                    update.channel = "";
                }


                update.local = isLocal;

                SharedData.AddToMessageList(JsonConvert.SerializeObject(update, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DEBUG-WEBSOCKETHANDLER:ERROR: " + ex.ToString());
            }
        }

        private void openFile(dynamic download)
        {
            string fileName = download.filename;
            string fileLocation = Path.Combine(SharedData.currentDownloadLocation, fileName);
            try
            {
                 Debug.WriteLine("DEBUG-WEBSOCKETHANDLER: Trying to open file: " + fileLocation);
                Thread fileopener = new Thread(new ThreadStart(delegate
                {
                    Process.Start(fileLocation);
                }));
                fileopener.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DEBUG-WEBSOCKETHANDLER:ERROR: We've got another problem: " + ex.ToString());
            }
        }

        private void closeEverything()
        {
            Debug.WriteLine("DEBUG-WEBSOCKETHANDLER: CLOSING SHIT");
            SharedData.closeBackend = true;
        }

        private void getDirectories(string path)
        {
            Debug.WriteLine("DEBUG-WEBSOCKETHANDLER: GETTING DIRECTORIES FROM PATH: " + path);
            try
            {
                string[] dirs = Directory.GetDirectories(path);

                List<JsonDirectory> directorieswithpath = new List<JsonDirectory>();
                foreach (string directory in dirs)
                {
                    JsonDirectory directorywithpath = new JsonDirectory();
                    directorywithpath.dirname = directory.Replace(Path.GetDirectoryName(directory) + Path.DirectorySeparatorChar, "");
                    directorywithpath.path = directory;
                    directorieswithpath.Add(directorywithpath);
                }
                JsonDirectories tosendover = new JsonDirectories();
                tosendover.directories = directorieswithpath;
                SharedData.AddToMessageList(JsonConvert.SerializeObject(tosendover, Formatting.Indented));


            }
            catch (Exception e)
            {
                Debug.WriteLine("DEBUG-WEBSOCKETHANDLER: COULD NOT FIND DIRS IN  : " + path);
                Debug.WriteLine("DEBUG-WEBSOCKETHANDLER: " + e.ToString());
               
            }
           
        }

        private void getDrives()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            List<JsonDirectory> directorieswithpath = new List<JsonDirectory>();
            foreach (DriveInfo drive in allDrives)
            {
                JsonDirectory directorywithpath = new JsonDirectory();
                directorywithpath.dirname = drive.Name;
                directorywithpath.path = drive.Name;
                directorieswithpath.Add(directorywithpath);
            }
            JsonDirectories tosendover = new JsonDirectories();
            tosendover.directories = directorieswithpath;
            SharedData.AddToMessageList(JsonConvert.SerializeObject(tosendover, Formatting.Indented));
        }

        private void createDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);


                string[] dirs = Directory.GetDirectories(path);
                List<JsonDirectory> directorieswithpath = new List<JsonDirectory>();
                foreach (string dir in dirs)
                {
                    JsonDirectory directorywithpath = new JsonDirectory();
                    directorywithpath.dirname = dir.Split('\\')[dir.Split('\\').Length - 1];
                    directorywithpath.path = dir;
                    directorieswithpath.Add(directorywithpath);
                }
                JsonDirectories tosendover = new JsonDirectories();
                tosendover.directories = directorieswithpath;
                SharedData.AddToMessageList(JsonConvert.SerializeObject(tosendover, Formatting.Indented));
            }
        }

        private void connectIrc(dynamic extra)
        {
            Debug.WriteLine("DEBUG-WEBSOCKETHANDLER: GOT MESSAGE TO CONNECT TO IRC CLIENT!");
            if (SharedData.ircHandler.isConnected)
            {
                disconnectIrc();

            }
            Thread.Sleep(1000);

            string address = extra.address;
            string username = extra.username;
            string channels = extra.channels;

            makeSureConnection = new Thread(new ThreadStart(delegate ()
            {
                SharedData.ircHandler.startIrc(address, username, channels);
            }));
            makeSureConnection.Start();
            Debug.WriteLine("DEBUG-WEBSOCKETHANDLER: STARTED IRC CLIENT WITH FOLLOWING INFORMATION:");
            Debug.WriteLine("DEBUG-WEBSOCKETHANDLER: IRC ADDRESS = " + address);
            Debug.WriteLine("DEBUG-WEBSOCKETHANDLER: IRC USERNAME = " + username);
            Debug.WriteLine("DEBUG-WEBSOCKETHANDLER: IRC CHANNEL(S) = " + channels);

        }

        private void disconnectIrc()
        {
            Debug.WriteLine("DEBUG-WEBSOCKETHANDLER: GOT MESSAGE TO CLOSE IRC CLIENT!");
            SharedData.ircHandler.closeIrcClient();
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            var json = JsonConvert.DeserializeObject<dynamic>(e.Data);
            Debug.WriteLine("DEBUG-WEBSOCKETHANDLER: " + ((object)JsonConvert.SerializeObject(json)).ToString());

            switch (json.action.ToString())
            {
                case "get_irc_data":
                    getIrcData();
                    break;
                case "get_downloads":
                    getDownloads();
                    break;
                case "add_download":
                    addDownload(json.extra);
                    break;
                case "abort_download":
                    abortDownload();
                    break;
                case "delete_file":
                    deleteDownload(json.extra);
                    break;
                case "open_download_directory":
                    openDownloadDirectory();
                    break;
                case "set_download_directory":
                    string path = json.extra;
                    setDownloadDirectoryV2(path);
                    break;
                case "open_file":
                    openFile(json.extra);
                    break;
                case "get_directories":
                    if(json.extra != null)
                    {
                        if(json.extra.ToString() == "DRIVES" || json.extra.ToString() == "/")
                        {
                            Debug.WriteLine("DEBUG-WEBSOCKETHANDLER: GETTING DRIVES");
                            getDrives();
                        }
                        else
                        {
                            Debug.WriteLine("DEBUG-WEBSOCKETHANDLER: GETTING DIRECTORIES");
                            string pathtoset = json.extra.ToString();
                            getDirectories(pathtoset);
                        }
                    } else
                    {
                        Debug.WriteLine("DEBUG-WEBSOCKETHANDLER: GETTING DRIVES");
                        getDrives();
                    }
                    break;
                case "create_directory":
                    string pathtocreate= json.extra;
                    createDirectory(pathtocreate);
                    break;
                case "connect_irc":
                    connectIrc(json.extra);
                    break;
                case "disconnect_irc":
                    disconnectIrc();
                    break;
                case "enablechat_irc":
                    SharedData.enableIrcChat = true;
                    break;
                case "disablechat_irc":
                    SharedData.enableIrcChat = false;
                    break;
                case "getuserlist_irc":
                    JsonIRCUsersList newuserlist = new JsonIRCUsersList();
                    newuserlist.users = SharedData.userList;
                    SharedData.AddToMessageList(JsonConvert.SerializeObject(newuserlist, Formatting.Indented));
                    break;
                case "sendmessage_irc":
                    SharedData.ircHandler.sendMessage(json.extra.message);
                    break;
                case "close":
                    closeEverything();
                    break;
                default:
                    Debug.WriteLine("DEBUG-WEBSOCKETHANDLER: RECEIVED UNKNOWN JSON ACTION: " + ((object)json.action).ToString());
                    break;
               
            }
        }


    }
}

using System;
using SimpleIRCLib;
using System.Threading;
using System.Diagnostics;
using WebSocketSharp.Server;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace SunIRCLibrary
{
    class IrcHandler : WebSocketBehavior
    {
        private SimpleIRC irc;
        private WebSocketServer websocketserver;
        private UtitlityMethods usefullstuff;
        private bool shouldStopClient = false;
        private bool isLocal;
        private int channelcount = 0;
        public bool isBussyConnecting = false;
        public bool isConnected = false;

        private Thread downloaderLogicThread = null;
        public IrcHandler()
        {

            isLocal = SunIRCInit.isLocal;
            websocketserver = SharedData.websocketserver;
            irc = SharedData.irc;

            usefullstuff = new UtitlityMethods();


            Debug.WriteLine("DEBUG-IRCHANDLER: STARTING DOWNLOADER LOGIC");
            downloaderLogicThread = new Thread(new ThreadStart(downloaderLogic));
            downloaderLogicThread.Start();

        }

        public void getIrcData()
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

        public void closeIrcClient()
        {
            SharedData.joinedChannel = false;
            shouldStopClient = true;
            try
            {
                irc.stopXDCCDownload();
                irc.stopClient();
                isConnected = irc.isClientRunning();
            }
            catch (Exception e) { Debug.WriteLine("DEBUG-IRCHANDLER: ERROR: Could not shut down IRC client: " + e.ToString()); }


        }

        public void Shutdown()
        {
            SharedData.closeBackend = true;
            SharedData.currentlyDownloading = false;
            try
            {

                downloaderLogicThread.Abort();
            }
            catch (Exception e) { Debug.WriteLine("DEBUG-IRCHANDLER: ERROR: Could not shut down Downoloader Logic: " + e.ToString()); }
            try
            {
                irc.stopXDCCDownload();
                irc.stopClient();
            }
            catch (Exception e) { Debug.WriteLine("DEBUG-IRCHANDLER: ERROR: Could not shut down IRC client: " + e.ToString()); }
        }

        public void startIrc(string address, string username, string channels)
        {
            channelcount = channels.Split(',').Length;
            shouldStopClient = false;
            SharedData.joinedChannel = false;
            SharedData.userList.Clear();
            Debug.WriteLine("IRCDEBUG-IRCHANDLER: STARTING CONNECTION TO IRC SERVER!");
            int i = 0;
            while (!SharedData.joinedChannel && !isBussyConnecting)
            {
                isBussyConnecting = true;
                try
                {
                    irc.stopXDCCDownload();
                    irc.stopClient();
                }
                catch (Exception e) { Debug.WriteLine("DEBUG-IRCHANDLER: ERROR: Could not shut down IRC client: " + e.ToString()); }
                if (username == "")
                {
                    username = "SunIRC_" + usefullstuff.RandomString(6);
                }
                irc.setupIrc(address, 6667, username, "", channels, chatOutputCallback);
                irc.setDebugCallback(debugOutputCallback);
                irc.setDownloadStatusChangeCallback(downloadStatusCallback);
                irc.setUserListReceivedCallback(userListReceivedCallback);
                irc.setCustomDownloadDir(SharedData.currentDownloadLocation);
                irc.startClient();

                int x = 3;
                while (x > 0)
                {
                    if (shouldStopClient)
                    {
                        break;
                    }
                    Thread.Sleep(1000);
                    x--;
                }

                if (shouldStopClient)
                {

                    Debug.WriteLine("DEBUG-IRCHANDLER: SHOULD STOP RETRYING!");
                    shouldStopClient = false;
                    break;
                }


                if (!SharedData.joinedChannel)
                {
                    Debug.WriteLine("DEBUG-IRCHANDLER: DID NOT JOIN CHANNEL, RETRY!");
                } else
                {
                    isConnected = true;
                    JsonIrcUpdate update = new JsonIrcUpdate();
                    update.connected = true;
                    update.downloadlocation = SharedData.currentDownloadLocation;
                    update.server = irc.newIP + ":" + irc.newPort;
                    update.user = irc.newUsername;
                    update.channel = irc.newChannel;
                    update.local = isLocal;
                    SharedData.AddToMessageList(JsonConvert.SerializeObject(update, Formatting.Indented));
                    break;
                }
                i++;
                if (i > 10)
                {
                    Debug.WriteLine("DEBUG-IRCHANDLER: I AM DONE TRYING TO CONNECT!");
                    break;
                } else
                {

                    Debug.WriteLine("DEBUG-IRCHANDLER: NOT CONNECTED TO IRC SERVER");
                }
            }
            isBussyConnecting = false;

        }

        private void chatOutputCallback(string user, string message)
        {

            try
            {

                using (StreamWriter sw = File.AppendText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ircchatlog.txt")))
                {
                    try
                    {

                        sw.WriteLine(user + " | " + message);
                    }
                    catch
                    {

                    }
                }
            }
            catch
            {

            }

            JsonIRCChatMessage chatmessageobj = new JsonIRCChatMessage();
            chatmessageobj.user = user;
            chatmessageobj.message = message;
            if (SharedData.enableIrcChat)
            {
                SharedData.AddToMessageList(JsonConvert.SerializeObject(chatmessageobj, Formatting.Indented));
            }

        }

        private void debugOutputCallback(string debug)
        {
            Debug.WriteLine("DEBUG-IRCHANDLER: IRCDEBUG: " + debug);

        }

        private void downloadStatusCallback() //see below for definition of each index in this array
        {
            Object progress = irc.getDownloadProgress("progress");
            Object speedkbps = irc.getDownloadProgress("kbps");
            Object status = irc.getDownloadProgress("status");
            Object filename = irc.getDownloadProgress("filename");
            Object filesize = irc.getDownloadProgress("size");
            long filesizeinmb = (long.Parse(filesize.ToString().Trim()) / 1048576);
            if (status.ToString().Contains("DOWNLOADING") && status.ToString().Contains("WAITING"))
            {
                SharedData.currentlyDownloading = true;
            }
            else if (status.ToString().Contains("FAILED") || status.ToString().Contains("COMPLETED") || status.ToString().Contains("ABORTED"))
            {
                SharedData.currentlyDownloading = false;
            }

            JsonDownloadUpdate update = new JsonDownloadUpdate();
            update.id = SharedData.currentDownloadId;
            update.progress = progress.ToString();
            update.speed = speedkbps.ToString();
            update.status = status.ToString();
            update.filename = filename.ToString();
            update.filesize = filesizeinmb.ToString();


            SharedData.AddToMessageList(JsonConvert.SerializeObject(update, Formatting.Indented));

            Debug.WriteLine("DEBUG-IRCHANDLER: Download upsate: " + JsonConvert.SerializeObject(update, Formatting.Indented));
        }

        private void userListReceivedCallback(string[] users) //see below for definition of each index in this array
        {
            SharedData.userList.Add(users);

            channelcount--;

            if (channelcount == 0)
            {
                Debug.WriteLine("DEBUG-IRCHANDLER: GOT USER LIST FOR ALL CHANNELS, SUCCESFULLY CONNECTED");
                SharedData.joinedChannel = true;
                getIrcData();
            }

        }

        public void sendMessage(string msg)
        {
            if (irc.isClientRunning())
            {
                irc.sendMessage(msg);
            } else
            {
                Debug.WriteLine("DEBUG-IRCHANDLER: TRIED TO SEND MESSAGE: " + msg + " WHILE NOT BEING CONNECTED :(");
            }
        }

        private void downloaderLogic()
        {
            Debug.WriteLine("DEBUG-IRCHANDLER: DOWNLOADER LOGIC STARTED");
            while (!SharedData.closeBackend)
            {
                Thread.Sleep(1);
                if (irc.isClientRunning() && !isBussyConnecting && SharedData.joinedChannel)
                {
                    Thread.Sleep(1000);
                    if (!irc.checkIfDownload() && !SharedData.currentlyDownloading)
                    {
                        Thread.Sleep(500);
                        if (!irc.didErrorHappen)
                        {

                            //Debug.WriteLine("DEBUG-IRCHANDLER: QUEU LENGTH BEFORE  TAKING: " + SharedData.downloadList.Count);
                            dlData data = SharedData.getAndRemoveFromDownloadList();
                            if (data != null)
                            {
                                SharedData.currentDownloadId = data.dlId;
                                bool succes = false;
                                try
                                {

                                    if (data.dlBot != "undefined" && data.dlPack != "undefined")
                                    {
                                        irc.sendMessage("/msg " + data.dlBot + " xdcc send " + data.dlPack);
                                        succes = true;
                                    }
                                }
                                catch
                                {
                                    Debug.WriteLine("DEBUG-IRCHANDLER: ERROR:  NOT CONNECTED TO IRC, CAN'T DOWNLOAD FILE :(");
                                }

                                if (succes)
                                {
                                    //SharedData.AddToMessageList("DOWNLOADSTARTED");
                                    Debug.WriteLine("DEBUG-IRCHANDLER: Started a download: " + "/msg " + data.dlBot + " xdcc send " + data.dlPack);
                                    Console.WriteLine("Started a download: " + "/msg " + data.dlBot + " xdcc send " + data.dlPack);
                                    SharedData.currentlyDownloading = true;
                                }
                            }
                            // Debug.WriteLine("DEBUG-IRCHANDLER: QUEU LENGTH AFTER TAKING: " + SharedData.downloadList.Count);
                        }
                    }
                }
            }
        }
    }
}

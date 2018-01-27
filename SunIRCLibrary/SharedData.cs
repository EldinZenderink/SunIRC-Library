using SimpleIRCLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using WebSocketSharp.Server;

namespace SunIRCLibrary
{
    class SharedData
    {
        public static bool joinedChannel { get; set; }
        public static bool closeBackend { get; set; }
        public static bool currentlyDownloading { get; set; }
        public static string currentDownloadLocation { get; set; }
        public static string currentDownloadId { get; set; }
        public static string operatingSystem { get; set; }
        public static List<string> messageToSendWS { get; set; }
        public static List<dlData> downloadList { get; set; }
        public static List<string[]> userList { get; set; }
        public static SimpleIRC irc { get; set; }
        public static IrcHandler ircHandler { get; set; }
        public static WebSocketServer websocketserver { get; set; }
        public static SettingsHandler settings { get; set; }
        public static SimpleWebServer httpserver { get; set; }
        public static bool enableIrcChat { get; set; }



        public static void AddToDownloadList(dlData data)
        {
            while (true)
            {
                if (!downloadList.Contains(data))
                {
                    downloadList.Add(data);
                    break;
                }
            }
           // Debug.WriteLine("SHAREDDATA-DEBUG: added download: " + data.dlPack + " at index: " + (downloadList.Count - 1));
        }

        public static void AddToMessageList(string data)
        {
            while (true)
            {

                if (!messageToSendWS.Contains(data))
                {
                    messageToSendWS.Add(data);
                    break;
                }
            }
          //  Debug.WriteLine("SHAREDDATA-DEBUG: added message: " + data+ " at index: " + (messageToSendWS.Count - 1));
        }

        public static dlData getAndRemoveFromDownloadList()
        {

            if (downloadList.Count > 0)
            {
                while (true)
                {
                    try
                    {
                        //Debug.WriteLine("SHAREDDATA-DEBUG: returning: " + downloadList[0].dlPack + " from first index ");
                        dlData toreturn = downloadList[0];
                        downloadList.RemoveAt(0);
                        return toreturn; // this should prevent from reading non existent downloads, if there is no available next time will be.

                    }
                    catch (Exception e)
                    {

                        Debug.WriteLine("SHAREDDATA-DEBUG: Could not return and remove downloaditem, retrying until it can");
                    }
                }
            } else
            {
                //Debug.WriteLine("SHAREDDATA-DEBUG: could not return latestdownloadadded, returning: null");
                return null;
            }
           
           
        }

        public static void removeIfDownloadIsInDownloadList(string id)
        {
            int index = 0;
            while (true)
            {
                try
                {
                    foreach (var dl in downloadList)
                    {
                        if (dl.dlId == id)
                        {
                            downloadList.RemoveAt(index);
                            //Debug.WriteLine("SHAREDDATA-DEBUG:removed download at index: " + index);
                            break;
                        }
                        index++;
                    }
                    break;
                } catch (Exception E)
                {
                    Debug.WriteLine("SHAREDDATA-DEBUG:could not remove data because the list has changed :X, will try again.");
                }
            }
            
        }

        public static string getAndRemoveFromMessageList()
        {
            if (messageToSendWS.Count > 0)
            {

                while (true)
                {
                    try
                    {
                        //Debug.WriteLine("SHAREDDATA-DEBUG: returning: " + messageToSendWS[0] + " from first index ");
                        string toreturn = messageToSendWS[0];
                        messageToSendWS.RemoveAt(0);
                        return toreturn; // this should prevent from reading non existent downloads, if there is no available next time will be.

                    } catch (Exception e)
                    {
                        Debug.WriteLine("SHAREDDATA-DEBUG:could not remove message because the list has changed :X, will try again.");
                    }                    

                }
            }
            else
            {
                //Debug.WriteLine("SHAREDDATA-DEBUG: could not return latest message, returning: null");
                return null;
            }
        }
    }
}

using SimpleIRCLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using WebSocketSharp.Server;

namespace SunIRCLibrary
{
    public class SunIRCInit
    {
        private UtitlityMethods usefullstuff;
        private WebSocketServer websocketserver;
        private SimpleIRC irc;
        private IrcHandler irchandler;
        private SettingsHandler settings;
        private SimpleWebServer httpserver;
        public static bool isLocal{ get; set; }

        public SunIRCInit(bool local)
        {
            isLocal = local;
            //initialize debugging
            InitializeDebugging();
            //initialize sharing between classes
            InitializeSharedData();
            //initialize settings and load them
            InitializeSettings();
            //initialize websockets
            InitializeWebSocketSever();
            //initialize webserver
            InitializeWebServer();
            //initialize irc
            InitializeSimpleIRC();

            Debug.WriteLine("DEBUG-MAIN: INITIALIZED LITTLEWEEB :D v0.3.0");
            Debug.WriteLine("DEBUG-MAIN: CURRENT DIRECTORY IS: " + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).ToString());
            Debug.WriteLine("DEBUG-MAIN: WAITING FOR CONNECTION WITH INTERFACE!");
        }


        private void InitializeWebServer()
        {
            httpserver = new SimpleWebServer(6010);
            httpserver.SetFileDir("GUI");
            httpserver.SetDefaultPage("index.html");
            httpserver.downloadDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).ToString();
            httpserver.Start();
        }


        private void InitializeDebugging()
        {
            var listeners = new TraceListener[] { new TextWriterTraceListener(Console.Out) };
            Debug.Listeners.AddRange(listeners);
            if(!File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "awesomelogofc.txt")))
            {
                File.Create(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "awesomelogofc.txt"));
            }
            TraceListener listener = new DelimitedListTraceListener(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "awesomelogofc.txt"));

            // Add listener.
            Debug.Listeners.Add(listener);

            // Write and flush.
            Debug.WriteLine("DEBUG-MAIN: Starting to debug!");
        }

        private void InitializeWebSocketSever()
        {

            Debug.WriteLine("DEBUG-MAIN: Starting websocket server");
            websocketserver = new WebSocketServer(1515);
            websocketserver.AddWebSocketService<WebSocketHandler>("/");
            websocketserver.Start();

        }

        private void InitializeSimpleIRC()
        {
            try
            {
                if (irc.isClientRunning())
                {
                    irc.stopClient();
                }
            }
            catch
            {

            }
            irc = new SimpleIRC();
            SharedData.irc = irc;
            irchandler = new IrcHandler();
            SharedData.ircHandler = irchandler;
        }

        private void InitializeSharedData()
        {
            SharedData.websocketserver = websocketserver;
            SharedData.irc = irc;
            SharedData.settings = settings;
            SharedData.joinedChannel = false;
            SharedData.closeBackend = false;
            SharedData.currentlyDownloading = false;
            SharedData.downloadList = new List<dlData>();
            SharedData.messageToSendWS = new List<string>();
            SharedData.currentDownloadId = "";
            SharedData.currentDownloadLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).ToString();
            SharedData.httpserver = this.httpserver;
        }

        private void InitializeSettings()
        {
            settings = new SettingsHandler();
            SharedData.settings = settings;
            settings.loadSettings();
        }

        public void Shutdown()
        {
            SharedData.closeBackend = true;
            httpserver.StopServer();
            try
            {
                irchandler.Shutdown();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DEBUG-MAIN:  Could not close irc :( ");
                Debug.WriteLine(ex.ToString());
            }
            try
            {
                websocketserver.Stop();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DEBUG-MAIN:  Could not close websocketserver :( ");
                Debug.WriteLine(ex.ToString());
            }

            Debug.Flush();
            Debug.Close();
            Environment.Exit(0);

        }
    }
}

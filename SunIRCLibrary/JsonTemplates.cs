using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunIRCLibrary
{
    class JsonDirectories
    {
        public string type = "directories";
        public List<JsonDirectory> directories { get; set; }
    }

    class JsonDirectory
    {
        public string type = "directory";
        public string path { get; set; }
        public string dirname { get; set; }
    }

    class JsonSettingsUpdate
    {
        public string type = "settings";
        public string currentDir { get; set; }
    }

    class JsonIrcUpdate
    {
        public string type = "irc_data";
        public bool connected { get; set; }
        public string channel { get; set; }
        public string server { get; set; }
        public string user { get; set; }
        public string downloadlocation { get; set; }
        public bool local { get; set; }
    }

    class JsonDownloadUpdate
    {
        public string type = "download_update"; //used for identifying json
        public string id { get; set; }
        public string progress { get; set; }
        public string speed { get; set; }
        public string status { get; set; }
        public string filename { get; set; }
        public string filesize { get; set; }

    }

    class JsonAlreadyDownloaded
    {
        public string type = "already_downloaded"; //used for identifying json
        public List<JsonDownloadUpdate> alreadyDownloaded { get; set; }
    }

    class JsonIRCChatMessage
    {
        public string type = "chat_message"; //used for identifying json
        public string user { get; set; }
        public string message { get; set; }
    }

    class JsonIRCUsersList
    {
        public string type = "user_list"; //used for identifying json
        public List<string[]> users { get; set; }
    }

}

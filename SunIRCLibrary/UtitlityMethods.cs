using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SunIRCLibrary
{
    class UtitlityMethods
    {
        public UtitlityMethods()
        {

        }
        public bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

        public string getOS()
        {
            string launchDirectory = Assembly.GetEntryAssembly().CodeBase;
            if (!IsLinux)
            {
                Console.WriteLine("UTILITYDEBUG: THIS IS OBVIOUSLY WINDOWS :D");
                return "Windows";
            } else
            {
                Debug.WriteLine("UTILITYDEBUG: THIS IS OBVIOUSLY NOT WINDOWS :D");
                return "Not Windows";
            }
        }

        public string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }

        public string GetValidFileName(string fileName)
        {
            // remove any invalid character from the filename.
            String ret = Regex.Replace(fileName.Trim(), "[^A-Za-z0-9_. ]+", "");
            return ret.Replace(" ", String.Empty);
        }


        private Random random = new Random();
        public string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public bool IsMediaFile(string filename)
        {
            string[] fileExtensions = new string[] { ".mkv", ".mp4", ".avi", ".flak", ".mp3", ".aac", ".exe", ".tar", ".aaf", ".3gp", ".asf", ".avchd", ".avi", ".bik", ".dat", ".flv", ".mpeg", ".m4v", ".mkv", ".mp4", ".mts", ".wmv", ".vp9", ".vp8", ".webm" };
            string extension = Path.GetExtension(filename.ToLower());

            int inArray = Array.IndexOf(fileExtensions, extension);
            if (inArray > -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

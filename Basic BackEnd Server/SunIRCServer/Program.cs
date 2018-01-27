using System;
using System.Net;
using System.Net.Sockets;
using SunIRCLibrary;
using System.Reflection;
using System.IO;

namespace SunIRCServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to SunIRC's server!");
            Console.WriteLine("=================================");

            string ip = GetLocalIPAddress();

            Console.WriteLine("Running webserver with base directory: /GUI at following address:");
            Console.WriteLine("http://" + ip + ":6010");
            Console.WriteLine("=================================");
            Console.WriteLine("Running websocketserver with base address: / at following address:");
            Console.WriteLine("http://" + ip + ":1515");

            //run locally
            SunIRCInit init = new SunIRCInit(false);
            Console.WriteLine("Press a key to exit!");
            Console.ReadLine();
            init.Shutdown();

        }

        private static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}

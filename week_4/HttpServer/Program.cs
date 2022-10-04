using System;
using System.Buffers;
using System.Net;
using System.IO;
using HttpServer;

namespace NetConsoleApp
{
    class Program
    {
        private static bool _keepRunning = true;
        static void Main(string[] args)
        {
            var url = "http://localhost:2323/google/";
            HttpServer.HttpServer server = new HttpServer.HttpServer(url);
            server.Start();
            while (_keepRunning)
            {
                var request = Console.ReadLine()?.ToLower();
                if (request == "start")
                    server.Start();
                if (request == "stop")
                    server.Stop();
                if (request == "exit")
                    _keepRunning = false;
            }
            Console.WriteLine("Exiting gracefully...");
            Console.Read();
        }
    }
}
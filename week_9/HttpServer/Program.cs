using System;
using System.Buffers;
using System.Net;
using System.IO;
using HttpServer;

namespace HttpServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using (HttpServer server = new HttpServer())
            {
                RequestHandler handler = new RequestHandler(server) { KeepRunning = true };
                handler.HandleRequests("start");
                while (handler.KeepRunning)
                    handler.HandleRequests(Console.ReadLine()?.ToLower());
            }
            Console.Read();
            Console.Clear();
        }
    }
}
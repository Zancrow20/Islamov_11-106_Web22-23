using System.Diagnostics;
using System.Net;

namespace HttpServer;

public class RequestHandler
{
    public bool KeepRunning { get; set; }

    private readonly HttpServer Server;

    public RequestHandler(HttpServer server)
    {
        Server = server;
    }
    public void HandleRequests(string? request)
    {
        if (request == "start")
            Server.Start();
        if (request == "stop")
            Server.Stop();
        if (request == "restart")
        {
            Server.Stop();
            Server.Start();
        }
        if (request == "exit")
            KeepRunning = false;
    }
}

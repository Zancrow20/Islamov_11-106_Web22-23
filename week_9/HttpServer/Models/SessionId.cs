using System.Text;
using System.Text.Json;

namespace HttpServer;

public class SessionId
{
    public Guid Guid { get; }

    public SessionId(int id, string nickname, string password)
    {
        var identifier = $"{id}_{nickname}_{password}";
        var bytes = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(identifier));
        var guidBytes = new byte[16];
        Array.Copy(bytes,0,guidBytes,0,16);
        Guid = new Guid(guidBytes);
    }
    
    public SessionId() => Guid = new Guid();
    
}
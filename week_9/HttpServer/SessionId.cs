namespace HttpServer;

public class SessionId
{
    public bool IsAuthorized { get; }
    public int? Id { get; }

    public SessionId(bool isAuthorized, int? id)
    {
        IsAuthorized = isAuthorized;
        Id = id;
    }
}
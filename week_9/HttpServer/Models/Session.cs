namespace HttpServer.Models;

public class Session
{
    public int AccountId { get; }
    public Guid Guid { get; set; }
    public string Nickname { get; }
    public DateTime CreateDateTime { get; }

    public Session(Guid guid,int accountId, string nickname, DateTime createDateTime)
    {
        Guid = guid;
        AccountId = accountId;
        Nickname = nickname;
        CreateDateTime = createDateTime;
    }
}
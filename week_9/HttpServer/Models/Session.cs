namespace HttpServer.Models;

public class Session
{
    public int AccountId { get; }
    public int? Id { get; }
    public string Nickname { get; }
    public DateTime CreateDateTime { get; }

    public Session(int? id, int accountId, string nickname, DateTime createDateTime)
    {
        Id = id;
        AccountId = accountId;
        Nickname = nickname;
        CreateDateTime = createDateTime;
    }
}
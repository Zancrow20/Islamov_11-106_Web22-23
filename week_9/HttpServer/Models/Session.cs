namespace HttpServer.Models;

public class Session
{
    public int AccountId { get; }
    public int? Id { get; set; }
    public string Nickname { get; }
    public DateTime CreateDateTime { get; }

    public Session(int accountId, string nickname, DateTime createDateTime)
    {
        AccountId = accountId;
        Nickname = nickname;
        CreateDateTime = createDateTime;
    }
}
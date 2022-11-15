using System.Net;
using System.Text.RegularExpressions;
using HttpServer.Attributes;
using HttpServer.Models;
using HttpServer.ORM;

namespace HttpServer.Controllers;


[HttpController("accounts")]
public class Accounts
{
    private readonly AccountRepository _accountRepo;
        
    public Accounts() => _accountRepo = new AccountRepository();

    [HttpGET("")]
    public List<Account>? GetAccounts(string? cookieValue)
    {
        var authorization = cookieValue?.Split(' ')[0];
        return authorization is "IsAuthorized=True" ? _accountRepo.GetAccounts() : null;
    }

    [HttpGET("[1-9][0-9]+")]
    public Account? GetAccountInfo(int id, string cookie)
    {
        var cookieInfo = cookie.Split(' ');
        if (cookieInfo[0] is "IsAuthorized=True")
            if (int.TryParse(cookieInfo[1].Split('=')[^1], out var idFromCookie) && idFromCookie == id)
                return _accountRepo.GetById(id);
        
        return null;
    }
    
    [HttpPOST("account")]
    public SessionId Login(string nickname, string password)
    {
        var account = _accountRepo.GetAccountByProperties(nickname, password);

        var cookie = account == null
            ? new SessionId(false, null)
            : new SessionId(true, account.Id);
        return cookie;
    }
}
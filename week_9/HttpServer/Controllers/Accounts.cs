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
        try
        {
            var authorization = cookieValue?.Split(' ');
            var canParse = int.TryParse(authorization[^1].Split('=')[1],out var id);
            if (canParse && authorization[0] is "IsAuthorized=True" && SessionManager.CheckSession(id))
                return _accountRepo.GetAccounts();
        }
        catch (KeyNotFoundException e)
        {
            return null;
        }
        
        return null;
    }

    [HttpGET("[1-9][0-9]+")]
    public Account? GetAccountInfo(int id, string cookie)
    {
        try
        { 
            var cookieInfo = cookie.Split(' ');
            if (cookieInfo[0] is "IsAuthorized=True" && SessionManager.CheckSession(id))
                return _accountRepo.GetById(SessionManager.GetInfo(id)!.AccountId);
        }
        catch (KeyNotFoundException e)
        {
            return null;
        }
        return null;
    }
    
    [HttpPOST("account")]
    public SessionId Login(string nickname, string password)
    {
        var account = _accountRepo.GetAccountByProperties(nickname, password);
        if (account == null) return new SessionId(false, null);
        SessionManager.GetOrAdd(account.Id, () => new Session(account.Id, nickname, DateTime.Now));
        return new SessionId(true, account.Id);
    }
}
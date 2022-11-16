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
    public List<Account>? GetAccounts(string? cookie)
    {
        try
        {
            var cookieInfo = cookie?.Split('=')[^1];
            if (Guid.TryParse(cookieInfo, out var guid) && SessionManager.CheckSession(guid))
                return _accountRepo.GetAccounts();
        }
        catch (KeyNotFoundException e)
        {
            return null;
        }
        
        return null;
    }

    [HttpGET("[1-9][0-9]+")]
    public Account? GetAccountInfo(string cookie)
    {
        try
        { 
            var cookieInfo = cookie.Split('=')[^1];
            if (Guid.TryParse(cookieInfo, out var guid) && SessionManager.CheckSession(guid))
                return _accountRepo.GetById(SessionManager.GetInfo(guid)!.AccountId);
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
        if (account == null) return null;
        var sessionId = new SessionId(account.Id, nickname,password);
        SessionManager.GetOrAdd(sessionId.Guid, () 
            => new Session(sessionId.Guid, account.Id, nickname, DateTime.Now));
        return sessionId;
    }
}
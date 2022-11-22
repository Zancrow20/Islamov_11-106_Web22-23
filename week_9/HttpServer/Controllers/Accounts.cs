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
    public async Task<List<Account>?> GetAccounts(string? cookie)
    {
        try
        {
            var cookieInfo = cookie?.Split('=')[^1];
            if (Guid.TryParse(cookieInfo, out var guid) && await SessionManager.CheckSession(guid))
                return await _accountRepo.GetAccounts();
        }
        catch (KeyNotFoundException e)
        {
            return null;
        }
        
        return null;
    }

    [HttpGET("[1-9][0-9]+")]
    public async Task<Account?> GetAccountInfo(string cookie)
    {
        try
        { 
            var cookieInfo = cookie.Split('=')[^1];
            if (Guid.TryParse(cookieInfo, out var guid) && await SessionManager.CheckSession(guid))
                return await _accountRepo.GetById((await SessionManager.GetInfo(guid))!.AccountId);
        }
        catch (KeyNotFoundException e)
        {
            return null;
        }
        return null;
    }
    
    [HttpPOST("account")]
    public async Task<SessionId> Login(string nickname, string password)
    {
        var account = await _accountRepo.GetAccountByProperties(nickname, password);
        if (account == null) return null;
        var sessionId = new SessionId(account.Id, nickname,password);
        await SessionManager.GetOrAdd(sessionId.Guid, () 
            => Task.FromResult(new Session(sessionId.Guid, account.Id, nickname, DateTime.Now)));
        return sessionId;
    }
}
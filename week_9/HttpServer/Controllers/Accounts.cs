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
    public List<Account> GetUsers()
    {
        return _accountRepo.GetAccounts();
    }

    [HttpGET("[1-9][0-9]*$")]
    public Account? GetUserById(int id)
    {
        return _accountRepo.GetById(id);
    }
    
    [HttpPOST("account$")]
    public void SaveUser(string query)
    {
        var accountData = query.Split('&')
            .Select(pair => pair?.Split('=')[1]).ToArray();
        if(accountData[0] == null || accountData[1] == null)
            return;
        var account = new Account() {Nickname = accountData[0], Password = accountData[1]};
        _accountRepo.Create(account);
    }

    [HttpPOST("login")]
    public void Login(string email, string password)
    {
        //TODO
    }
}
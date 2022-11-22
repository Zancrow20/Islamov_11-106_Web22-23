using HttpServer.Models;
using HttpServer.ORM;

namespace HttpServer.ORM;


public interface IRepository<TEntity>
{
    Task<Account?> GetById(int id);
    Task Create(TEntity entity);
    Task Update(TEntity entity);
    Task Delete(TEntity entity);
    Task<List<Account>> GetAccounts();
}

public class AccountRepository : IRepository<Account>
{
    private static readonly ORM DB = new 
        (@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True");
    
    public async Task<Account?> GetById(int id)
        => await DB.Select<Account>(id);

    public async Task Create(Account account)
        => await DB.Insert(account);

    public async Task Update(Account account)
        => await DB.Update(account);

    public async Task Delete(Account account)
        => await DB.Delete(account);

    public async Task<List<Account>> GetAccounts()
        => await DB.Select<Account>();
    
    public async Task<Account?> GetAccountByProperties(string nickname, string password)
    {
        var listByNicknames = await DB.Select<Account>(nickname, "nickname");
        var listByPasswords = await DB.Select<Account>(password, "password");
        Account? account = null;
        foreach (var accountByName in 
                 from accountByName in listByNicknames 
                 from accountByPassword in listByPasswords 
                 where accountByName.Id == accountByPassword.Id 
                 select accountByName)
            account = accountByName;
        return account;
    }
}
using HttpServer.Models;
using HttpServer.ORM;

namespace HttpServer.ORM;


public interface IRepository
{
    Account? GetById(int id);
    void Create(Account account);
    void Update(Account account);
    void Delete(Account account);
    List<Account> GetAccounts();
}

public class AccountRepository : IRepository
{
    private static readonly ORM DB = new 
        ORM(@"Data Source=DESKTOP-Q9MBLGB\SQLEXPRESS;Initial Catalog=SteamDB;Integrated Security=True");
    
    public Account? GetById(int id) => DB.Select<Account>(id);

    public void Create(Account account) => DB.Insert(account);

    public void Update(Account account) => DB.Update(account);

    public void Delete(Account account) => DB.Delete(account);

    public List<Account> GetAccounts() => DB.Select<Account>();
}
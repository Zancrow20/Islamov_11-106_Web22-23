using HttpServer.Models;
using HttpServer.ORM;

namespace HttpServer.ORM;

public class AccountRepository
{
    private static readonly ORM Orm = new 
        ORM(@"Data Source=DESKTOP-Q9MBLGB\SQLEXPRESS;Initial Catalog=SteamDB;Integrated Security=True");
    
    public Account? GetById(int id) => Orm.Select<Account>(id);

    public void Create(Account account) => Orm.Insert(account);

    public void Update(Account account) => Orm.Update(account);

    public void Delete(Account account) => Orm.Delete(account);

    public List<Account> GetAccounts() => Orm.Select<Account>();
}
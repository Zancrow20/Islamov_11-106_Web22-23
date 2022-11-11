using HttpServer.Models;

namespace HttpServer.ORM;


public interface IAccountDAO
{
    List<Account?> GetAccounts();
    Account? GetAccountById(int id);
    void Add(Account account);
    void Delete(Account account);
}
public class AccountDAO: IAccountDAO
{
    private ORM DB;

    private readonly string connectionString =
        @"Data Source=DESKTOP-Q9MBLGB\SQLEXPRESS;Initial Catalog=SteamDB;Integrated Security=True";
    public AccountDAO()
    {
        DB = new ORM(connectionString);
    }

    public List<Account?> GetAccounts() 
        => DB.Select<Account>();

    public Account? GetAccountById(int id) 
        => DB.Select<Account>(id);

    public void Add(Account account)
        => DB.Insert(account);

    public void Delete(Account account)
        => DB.Delete(account);
}
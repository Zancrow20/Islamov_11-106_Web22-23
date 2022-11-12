using HttpServer.Models;

namespace HttpServer.ORM;


public interface IAccountDAO
{
    List<Account> GetAccounts();
    Account? GetAccountById(int id);
    void Add(string nickname, string password);
    void Delete(int id);
}
public class AccountDAO: IAccountDAO
{
    private ORM DB;

    private readonly string connectionString =
        @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True";
    
    public AccountDAO()
    {
        DB = new ORM(connectionString);
    }

    public List<Account> GetAccounts() 
        => DB.Select<Account>();

    public Account? GetAccountById(int id) 
        => DB.Select<Account>(id);

    public void Add(string nickname, string password) =>
        DB.Insert<Account>(nickname, password);

    public void Delete(int id)
    {
        var account = DB.Select<Account>(id);
        DB.Delete(account);
    }
}
using HttpServer.Models;

namespace HttpServer.ORM;


public interface IAccountDAO
{
    Task<List<Account>> GetAccounts();
    Task<Account?> GetAccountById(int id);
    Task Add(string nickname, string password);
    Task Delete(int id);
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

    public async Task<List<Account>> GetAccounts() 
        => await DB.Select<Account>();

    public async Task<Account?> GetAccountById(int id) 
        => await DB.Select<Account>(id);

    public async Task Add(string nickname, string password) =>
        await DB.Insert<Account>(nickname, password);

    public async Task Delete(int id)
    {
        var account = DB.Select<Account>(id);
        await DB.Delete(account);
    }
}
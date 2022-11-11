using HttpServer.Models;

namespace HttpServer.ORM;


public interface IAccountDAO
{
    List<Account> GetAll();
    Account? GetById(int id);
    void Add(Account account);
    void Delete(Account account);
}

public class AccountDAO : IAccountDAO
{
    private ORM DB;

    public AccountDAO()
    {
        DB = new ORM(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True");
    }

    public List<Account> GetAll() 
        => DB.Select<Account>();

    public Account? GetById(int id) 
        => DB.Select<Account>(id);

    public void Add(Account account) 
        => DB.Insert(account);

    public void Delete(Account account) 
        => DB.Delete(account);
}
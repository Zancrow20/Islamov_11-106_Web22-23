using HttpServer.Models;
using HttpServer.ORM;

namespace HttpServer.ORM;

public interface IRepository<TEntity>
{
    TEntity? GetById(int id);
    void Create(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
}
public class AccountRepository : IRepository<Account>
{
    private  readonly ORM DB;

    public AccountRepository()
    {
        DB = new ORM(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True");
    }
    public Account? GetById(int id) 
        => DB.Select<Account>(id);

    public void Create(Account account) 
        => DB.Insert(account);

    public void Update(Account account) 
        => DB.Update(account);

    public void Delete(Account account) 
        => DB.Delete(account);

    public List<Account> GetAccounts() 
        => DB.Select<Account>();
}
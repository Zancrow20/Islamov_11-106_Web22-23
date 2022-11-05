using System.Data.SqlClient;
using System.Reflection;
using System.Runtime.CompilerServices;
using HttpServer.Attributes;
using HttpServer.Models;

namespace HttpServer.Controllers;


[HttpController("accounts")]
public class Accounts
{
    [HttpGET("")]
    public List<Account> GetUsers()
                                         
    {
        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True";
        var users = DBLogic.GetUsers(connectionString);
        
        return users;
    }

    [HttpGET("[1-9][0-9]*$")]
    public Account GetUserById(int id)
    {
        
        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True";
        Account user = DBLogic.GetUserById(connectionString, id);
        return user;
    }
    
    [HttpPOST("account$")]
    public void SaveUser(string query)
    {
        var accountData = query.Split('&')
            .Select(pair => pair.Split('=')[1]).ToArray();
        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True";
        DBLogic.SaveUser(connectionString, accountData[0], accountData[1]);
    }
}
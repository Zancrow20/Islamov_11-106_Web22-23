using System.Data.SqlClient;
using HttpServer.Models;

namespace HttpServer;

public class DBLogic
{
    public static List<Account> GetUsers(string connectionString)
    {
        var Users = new List<Account>();
        string sqlExpression = "SELECT * FROM SteamDB.Accounts";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
 
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            SqlDataReader reader = command.ExecuteReader();
 
            if(reader.HasRows) // если есть данные
            {
                // выводим названия столбцов
                Console.WriteLine("{0}\t{1}", reader.GetName(1), reader.GetName(2));
                
                while (reader.Read()) // построчно считываем данные
                    Users.Add(new Account{Nickname = (string)reader.GetValue(1), 
                        Password = (string)reader.GetValue(2)});
            }
         
            reader.Close();
        }

        return Users;
    }
    
    public static Account GetUserById(string connectionString,int id)
    {
        Account user = new Account();
        
        
        string sqlExpression = $"SELECT * FROM SteamDB.Accounts u where u.id = {id}";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
 
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            SqlDataReader reader = command.ExecuteReader();
            
            if(reader.HasRows) // если есть данные
            {
                // выводим названия столбцов
                Console.WriteLine("{0}\t{1}", reader.GetName(1), reader.GetName(2));
 
                while (reader.Read()) // построчно считываем данные
                {
                    object nickname = reader.GetValue(1);
                    object password = reader.GetValue(2);
                    user = new Account {Nickname = (string) nickname, Password = (string) password};
                    Console.WriteLine("{0} \t{1}", nickname, password);
                }
            }
         
            reader.Close();
        }

        return user;
    }
    
    public static void SaveUser(string connectionString,string nickname, string password)
    {
        // выражение SQL для добавления данных
        string sqlExpression = "INSERT INTO SteamDB.Accounts (nickname, password) VALUES (@nickname, @password)";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            SqlCommand command = new SqlCommand(sqlExpression, connection);

            // создаем параметр для имени
            SqlParameter nicknameParam = new SqlParameter("@nickname", nickname);
            // добавляем параметр к команде
            command.Parameters.Add(nicknameParam);
            // создаем параметр для возраста
            SqlParameter passwordParam = new SqlParameter("@password", password);
            // добавляем параметр к команде
            command.Parameters.Add(passwordParam);

            int number = command.ExecuteNonQuery();
            Console.WriteLine($"Добавлено объектов: {number}");

            // вывод данных
             
            command.CommandText = "SELECT * FROM SteamDB.Accounts";
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows) // если есть данные
                {
                    // выводим названия столбцов
                    string columnName1 = reader.GetName(0);
                    string columnName2 = reader.GetName(1);
                    //string columnName3 = reader.GetName(2);
                    //Console.WriteLine($"{columnName1}\t{columnName2}\t{columnName3}");
                    Console.WriteLine($"{columnName1}\t{columnName2}");

                    while (reader.Read()) // построчно считываем данные
                    {
                        //object id = reader.GetValue(0);
                        object nickname2 = reader.GetValue(0);
                        object password2 = reader.GetValue(1);

                        Console.WriteLine($"{nickname2} \t{password2}");
                        //Console.WriteLine($"{id} \t{nickname2} \t{password2}");
                    }
                }
            }
        }
    }
}
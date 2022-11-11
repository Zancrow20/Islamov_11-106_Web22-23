using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using HttpServer.Models;

namespace HttpServer.ORM;

public class ORM
{
    private IDbConnection _connection;
    private IDbCommand _command = new SqlCommand();

    public ORM(string connectionString)
    {
        _connection = new SqlConnection(connectionString);
        _command = _connection.CreateCommand();
    }

    private IEnumerable<T> ExecuteQuery<T>(string query)
    {
        IList<T> list = new List<T>();
        Type type = typeof(T);

        using (_connection)
        {
            _command.CommandText = query;
            _connection.Open();
            var reader = _command.ExecuteReader();
            while (reader.Read())
            {
                var obj = Activator.CreateInstance<T>();
                type.GetProperties().ToList().ForEach(p=>
                    p.SetValue(obj, reader[p.Name.ToLower()]));
                
                list.Add(obj);
            }
        }

        return list;
    }

    private int ExecuteNonQuery<T>(string query)
    {
        int noAffectedRows;
        using (_connection)
        {
            _command.CommandText = query;
            _connection.Open();
            noAffectedRows = _command.ExecuteNonQuery();
        }
        
        return noAffectedRows;
    }

    public List<T> Select<T>()
    {
        var query = $"SELECT * FROM {typeof(T).Name}s";
        return ExecuteQuery<T>(query).ToList();
    }

    public T? Select<T>(int id)
    {
        var query = $"SELECT * FROM {typeof(T).Name}s WHERE id = id";
        return ExecuteQuery<T>(query).ToList().FirstOrDefault();
    }

    public void Update<T>(T entity)
    {
        var sb = new StringBuilder();
        var id = GetId(entity);
        var properties = entity?.GetType().GetProperties();
        foreach (var property in properties!)
            sb.Append($"{property.Name} = {property.GetValue(entity)}, ");
        
        string nonQuery = $"UPDATE {typeof(T).Name}s SET {sb} WHERE id = {id}";
        ExecuteNonQuery<T>(nonQuery);
    }

    public void Delete<T>(T entity)
    {
        var id = GetId(entity);
        string nonQuery = $"DELETE FROM {typeof(T).Name}s + WHERE id = {id}";
        ExecuteNonQuery<T>(nonQuery);
    }

    public void Insert<T>(T entity)
    {
        var args = GetPropertiesValues(entity);
        var values = args.Select(value => $"@{value}").ToArray();
        string nonQuery = $"SET IDENTITY_INSERT {typeof(T).Name}s ON" +
                          $"INSERT INTO {typeof(T).Name}s VALUES ({string.Join(", ", values)})" +
                          $"SET IDENTITY_INSERT {typeof(T).Name} OFF";
        ExecuteNonQuery<T>(nonQuery);
    }

    private static IEnumerable<object?> GetPropertiesValues<T>(T entity) =>
        typeof(T).GetProperties().Select(property => property.GetValue(entity));

    private static object? GetId<T>(T entity) =>
        typeof(T).GetProperty("id")?.GetValue(entity);
}
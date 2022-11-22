using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using HttpServer.Models;

namespace HttpServer.ORM;

public class ORM
{
    private SqlConnection _connection;
    private SqlCommand _command = new SqlCommand();

    public ORM(string connectionString)
    {
        _connection = new SqlConnection(connectionString);
        _command = _connection.CreateCommand();
    }

    private async Task<IEnumerable<T>> ExecuteQuery<T>(string query)
    {
        IList<T> list = new List<T>();
        Type type = typeof(T);
        
        _command.CommandText = query;
        await _connection.OpenAsync();
        var reader = await _command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var obj = Activator.CreateInstance<T>();
            type.GetProperties().ToList().ForEach(p=>
                p.SetValue(obj, reader[p.Name.ToLower()]));
            
            list.Add(obj);
        }
        await _connection.CloseAsync();

        return list;
    }

    private async Task<int> ExecuteNonQuery<T>(string query)
    {
        _command.CommandText = query;
        await _connection.OpenAsync();
        var noAffectedRows = await _command.ExecuteNonQueryAsync(); 
        _command.Parameters.Clear();
        await _connection.CloseAsync();
        return noAffectedRows;
    }

    public async Task<List<T>> Select<T>()
    {
        var query = $"SELECT * FROM {typeof(T).Name}s";
        return (List<T>) await ExecuteQuery<T>(query);
    }

    public async Task<List<T>> Select<T>(string propertyValue, string propertyName)
    {
        var query = $"SELECT * FROM {typeof(T).Name}s " +
                    $"WHERE {propertyName.ToLower()} = '{propertyValue}'";
        return (List<T>) await ExecuteQuery<T>(query);
    }

    public async Task<T?> Select<T>(int id)
    {
        var query = $"SELECT * FROM {typeof(T).Name}s WHERE id = {id}";
        return ((List<T>) await ExecuteQuery<T>(query)).FirstOrDefault();
    }

    public async Task Update<T>(T entity)
    {
        var sb = new StringBuilder();
        var id = GetId(entity);
        var properties = entity?.GetType().GetProperties();
        foreach (var property in properties!)
            sb.Append($"{property.Name} = {property.GetValue(entity)}, ");
        
        var nonQuery = $"UPDATE {typeof(T).Name}s SET {sb} WHERE id = {id}";
        await ExecuteNonQuery<T>(nonQuery);
    }

    public async Task Delete<T>(T entity)
    {
        var id = GetId(entity);
        var nonQuery = $"DELETE FROM {typeof(T).Name}s + WHERE id = {id}";
        await ExecuteNonQuery<T>(nonQuery);
    }

    public async Task Insert<T>(T entity)
    {
        var args = GetProperties(entity);
        var values = args.Select(value => $"@{value.GetValue(entity)}").ToArray();
        foreach (var parameter in args)
        {
            var sqlParameter = new SqlParameter($"@{parameter.Name}", parameter.GetValue(entity));
            _command.Parameters.Add(sqlParameter);
        }
        
        var nonQuery = $"SET IDENTITY_INSERT {typeof(T).Name}s ON" +
                       $"INSERT INTO {typeof(T).Name}s VALUES ({string.Join(", ", values)})" +
                       $"SET IDENTITY_INSERT {typeof(T).Name}s OFF";
        await ExecuteNonQuery<T>(nonQuery);
    }

    public async Task Insert<T>(params object[] args)
    {
        var values = args.Select(value => $"@{value}").ToArray();
        var nonQuery = $"INSERT INTO {typeof(T).Name}s VALUES ({string.Join(", ", values)})";
        await ExecuteNonQuery<T>(nonQuery);
    }

    private static IEnumerable<object?> GetPropertiesValues<T>(T entity) =>
        typeof(T).GetProperties().Select(property => property.GetValue(entity));

    private static PropertyInfo[] GetProperties<T>(T entity) =>
        typeof(T).GetProperties();

    private static object? GetId<T>(T entity) =>
        typeof(T).GetProperty("id")?.GetValue(entity);
}
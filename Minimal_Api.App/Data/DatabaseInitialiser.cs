using Dapper;

namespace Minimal_Api.App.Data;

public class DatabaseInitialiser
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DatabaseInitialiser(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task InitialiseAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        
        const string sql = "CREATE TABLE IF NOT EXISTS books (isbn TEXT PRIMARY KEY, title TEXT, author TEXT, short_description TEXT, page_count INT, release_date DATE)";
        await connection.ExecuteAsync(sql);
        
    }
}
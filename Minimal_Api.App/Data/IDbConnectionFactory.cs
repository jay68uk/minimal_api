using System.Data;

namespace Minimal_Api.App.Data;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync();
}
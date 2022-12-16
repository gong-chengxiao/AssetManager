using MySqlConnector;
using AssetManager.Core.Models;

namespace AssetManager.Helpers;

static class SqlConnector
{
    private static MySqlConnection? _sqlConnection;

    public static MySqlConnection? SqlConnection => _sqlConnection;

    public static async Task GetSqlConnectionAsync()
    {
        try
        {
            var _connectString = AppSettings.GetConnectionString();

            _sqlConnection = new MySqlConnection(_connectString);
            await _sqlConnection.OpenAsync();

        }
        catch
        {
            throw;
        }
    }

    public static async Task GetSqlConnectionAsync(string username, string password)
    {
        try
        {
            var _connectString = AppSettings.GetConnectionString(username, password);

            _sqlConnection = new MySqlConnection(_connectString);
            await _sqlConnection.OpenAsync();

        }
        catch
        {
            throw;
        }
    }
}

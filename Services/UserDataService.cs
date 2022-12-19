using AssetManager.Contracts.Services;
using AssetManager.Core.Helpers;
using AssetManager.Core.Models;
using AssetManager.Models;
using MySqlConnector;

namespace AssetManager.Services;
public class UserDataService : IUserDataService
{
    private readonly string _view = AppSettings.UserView;
    private readonly string _baseTable = AppSettings.UserTable;
    private readonly int _rowLimit = AppSettings.RowLimit;
    private List<UserInfo> _userInfo;
    private List<KeyValuePair<string, UserInfo>> _updateList = new();
    public void AddToUpdateList(string key, UserInfo asset)
    {
        _updateList.Add(new KeyValuePair<string, UserInfo>(key, asset));
    }
    public async Task ActivateUpdateList()
    {
        try
        {
            var connection = await SqlConnector.RefreshConnectionAsync();
            foreach (var i in _updateList)
            {
                var key = i.Key;
                var value = i.Value;
                
                // a表
                var queryString = 
                    $"update {_baseTable} set " +
                    $"{AppSettings.UserTableColumns[1]} = '{value.UserName}'," +
                    $"{AppSettings.UserTableColumns[2]} = '{value.UserDepartment}'," +
                    $"{AppSettings.UserTableColumns[3]} = '{value.UserPhonenumber}' " +
                    $"where {AppSettings.UserTableColumns[0]} = {key}";
                using (var command = new MySqlCommand(queryString, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        catch { throw; }
        finally
        {
            _updateList.Clear();
        }
    }
    public async Task ActivateAdd(UserInfo asset)
    {
        try
        {
            var connection = await SqlConnector.RefreshConnectionAsync();
            var i = asset;
            var queryString =
                $"insert into {_baseTable} " +
                $"value(" +
                $"'{i.UserID}'," +
                $"'{i.UserName}'," +
                $"'{i.UserDepartment}'," +
                $"'{i.UserPhonenumber}');";
            using (var command = new MySqlCommand(queryString, connection))
            {
                await command.ExecuteNonQueryAsync();
            };

        }
        catch
        {
            throw;
        }

        await Task.CompletedTask;
    }

    private static Task<UserInfo> ParseFromReader(MySqlDataReader reader)
    {
        var asset = new UserInfo
        {
            UserID = reader.GetInt32(0),
            UserName = reader.GetString(1),
            UserDepartment = reader.GetString(2),
            UserPhonenumber = reader.GetString(3)
        };
        return Task.FromResult(asset);
    }
    public async Task<IEnumerable<UserInfo>> GetGridDataAsync()
    {
        if (_userInfo == null)
        {
            _userInfo = new List<UserInfo>();
            try
            {
                var connection = await SqlConnector.RefreshConnectionAsync();
                var queryString = $"select * from {_view} limit {_rowLimit}";
                using (var command = new MySqlCommand(queryString, connection))
                {
                    var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        _userInfo.Add(await ParseFromReader(reader));
                    }
                };
            }
            catch
            {
                throw;
            }

        }

        await Task.CompletedTask;
        return _userInfo;
    }
    public async Task<IEnumerable<UserInfo>> GetFilterGridDataAsync(string column, string value)
    {
        _userInfo = new List<UserInfo>();
        try
        {
            var connection = await SqlConnector.RefreshConnectionAsync();
            var queryString = $"select * from {_view} where {column} like '%{value}%' limit {_rowLimit}";
            using (var command = new MySqlCommand(queryString, connection))
            {
                var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    _userInfo.Add(await ParseFromReader(reader));
                }
            };
        }
        catch
        {
            throw;
        }

        await Task.CompletedTask;
        return _userInfo;
    }
    public async Task<IEnumerable<UserInfo>> GetRefreshGridDataAsync()
    {
        _userInfo = new List<UserInfo>();
        try
        {
            var connection = await SqlConnector.RefreshConnectionAsync();
            var queryString = $"select * from {_view} limit {_rowLimit}";
            using (var command = new MySqlCommand(queryString, connection))
            {
                var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    _userInfo.Add(await ParseFromReader(reader));
                }
            };
        }
        catch
        {
            throw;
        }

        await Task.CompletedTask;
        return _userInfo;
    }
    public async Task<IEnumerable<UserInfo>> GetSearchGridDataAsync(string key)
    {
        _userInfo = new List<UserInfo>();
        try
        {
            foreach (var column in AppSettings.UserTableColumns)
            {
                var connection = await SqlConnector.RefreshConnectionAsync();
                var queryString = $"select * from {_view} where {column} like '%{key}%' limit {_rowLimit}";
                using (var command = new MySqlCommand(queryString, connection))
                {
                    var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        _userInfo.Add(await ParseFromReader(reader));
                    }
                };
            }
        }
        catch
        {
            throw;
        }

        await Task.CompletedTask;
        return _userInfo;
    }
    
    public async Task DeleteRowAsync(int key)
    {
        try
        {
            var connection = await SqlConnector.RefreshConnectionAsync();
            var queryString = $"delete from {_baseTable} where {AppSettings.UserTableColumns[0]} = {key}";
            using (var command = new MySqlCommand(queryString, connection))
            {
                await command.ExecuteNonQueryAsync();
            }
        }
        catch
        {
            throw;
        }
    }
}

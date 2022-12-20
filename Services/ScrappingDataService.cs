using AssetManager.Contracts.Services;
using AssetManager.Core.Helpers;
using AssetManager.Core.Models;
using AssetManager.Helpers;
using AssetManager.Models;
using MySqlConnector;

namespace AssetManager.Services;
public class ScrappingDataService : IScrappingDataService
{
    private readonly string _view = AppSettings.ScrappingView;
    private readonly string _baseTable = AppSettings.ScrappingTable;
    private readonly int _rowLimit = AppSettings.RowLimit;

    private List<ScrappingInfo> _scrappingInfo;
    private List<KeyValuePair<string, ScrappingInfo>> _updateList = new();

    public void AddToUpdateList(string key, ScrappingInfo asset)
    {
        _updateList.Add(new KeyValuePair<string, ScrappingInfo>(key, asset));
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

                var queryString =
                    $"update {_baseTable} set " +
                    $"{AppSettings.ScrappingTableColumns[1]} = '{value.AssetID}'," +
                    $"{AppSettings.ScrappingTableColumns[2]} = '{value.ScrapVendorID}'," +
                    $"{AppSettings.ScrappingTableColumns[3]} = '{value.ExecutorID}'," +
                    $"{AppSettings.ScrappingTableColumns[4]} = '{value.ScrappingDate}'," +
                    $"{AppSettings.ScrappingTableColumns[5]} = '{value.ScrappingRemark}';"
                    ;

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
    public async Task ActivateAdd(ScrappingInfo asset)
    {
        try
        {
            var connection = await SqlConnector.RefreshConnectionAsync();
            var i = asset;
            var queryString =
                $"insert into {_baseTable} " +
                $"value(" +
                $"'{i.ScrappingID}'," +
                $"'{i.AssetID}'," +
                $"'{i.ScrapVendorID}'," +
                $"'{i.ExecutorID}'," +
                $"'{i.ScrappingDate}'," +
                $"'{i.ScrappingRemark}');"
                ;

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

    private static Task<ScrappingInfo> ParseFromReader(MySqlDataReader reader)
    {
        var asset = new ScrappingInfo
        {
            ScrappingID = reader.GetInt32(0),
            AssetID = reader.GetInt32(1),
            AssetName = reader.GetString(2),
            ScrapVendorID = reader.GetInt32(3),
            ScrapVendorName = reader.GetString(4),
            ExecutorID = reader.GetInt32(5),
            ExecutorName = reader.GetString(6),
            ScrappingDate = reader.GetDateTime(7).ToString(),
            ScrappingRemark = reader.GetString(8),

        };
        return Task.FromResult(asset);
    }
    public async Task<IEnumerable<ScrappingInfo>> GetGridDataAsync()
    {
        if (_scrappingInfo == null)
        {
            _scrappingInfo = new List<ScrappingInfo>();
            try
            {
                var connection = await SqlConnector.RefreshConnectionAsync();
                var queryString = $"select * from {_view} limit {_rowLimit}";
                using (var command = new MySqlCommand(queryString, connection))
                {
                    var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        _scrappingInfo.Add(await ParseFromReader(reader));
                    }
                };
            }
            catch
            {
                throw;
            }

        }

        await Task.CompletedTask;
        return _scrappingInfo;
    }
    public async Task<IEnumerable<ScrappingInfo>> GetFilterGridDataAsync(string column, string value)
    {
        _scrappingInfo = new List<ScrappingInfo>();
        try
        {
            var connection = await SqlConnector.RefreshConnectionAsync();
            var queryString = $"select * from {_view} where {column} = '{value}' limit {_rowLimit}";
            using (var command = new MySqlCommand(queryString, connection))
            {
                var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    _scrappingInfo.Add(await ParseFromReader(reader));
                }
            };
        }
        catch
        {
            throw;
        }

        await Task.CompletedTask;
        return _scrappingInfo;
    }
    public async Task<IEnumerable<ScrappingInfo>> GetRefreshGridDataAsync()
    {
        _scrappingInfo = new List<ScrappingInfo>();
        try
        {
            var connection = await SqlConnector.RefreshConnectionAsync();
            var queryString = $"select * from {_view} limit {_rowLimit}";
            using (var command = new MySqlCommand(queryString, connection))
            {
                var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    _scrappingInfo.Add(await ParseFromReader(reader));
                }
            };
        }
        catch
        {
            throw;
        }

        await Task.CompletedTask;
        return _scrappingInfo;
    }
    public async Task<IEnumerable<ScrappingInfo>> GetSearchGridDataAsync(string key)
    {
        _scrappingInfo = new List<ScrappingInfo>();
        try
        {
            foreach (var column in AppSettings.ScrappingViewColumns)
            {
                var connection = await SqlConnector.RefreshConnectionAsync();
                var queryString = $"select * from {_view} where {column} like '%{key}%' limit {_rowLimit}";
                using (var command = new MySqlCommand(queryString, connection))
                {
                    var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        _scrappingInfo.Add(await ParseFromReader(reader));
                    }
                };
            }
        }
        catch
        {
            throw;
        }

        await Task.CompletedTask;
        return _scrappingInfo;
    }
    public async Task<IEnumerable<ScrappingInfo>> GetLastMonthGridDataAsync(string month)
    {
        _scrappingInfo = new List<ScrappingInfo>();
        try
        {
            var connection = await SqlConnector.RefreshConnectionAsync();
            var queryString = $"call get_s_by_time({month});";
            
            using (var command = new MySqlCommand(queryString, connection))
            {
                var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    _scrappingInfo.Add(await ParseFromReader(reader));
                }
            };

        }
        catch
        {
            throw;
        }

        await Task.CompletedTask;
        return _scrappingInfo;
    }
    public async Task DeleteRowAsync(int key)
    {
        try
        {
            var connection = await SqlConnector.RefreshConnectionAsync();
            var queryString = $"delete from {_baseTable} where {AppSettings.ScrappingTableColumns[0]} = {key}";
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

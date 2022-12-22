using AssetManager.Contracts.Services;
using AssetManager.Core.Helpers;
using AssetManager.Core.Models;
using AssetManager.Helpers;
using AssetManager.Models;
using MySqlConnector;

namespace AssetManager.Services;
public class MaintenanceDataService : IMaintenanceDataService
{
    private readonly string _view = AppSettings.MaintenanceView;
    private readonly string _baseTable = AppSettings.MaintenanceTable;
    private readonly string[] _baseTableColumns = AppSettings.MaintenanceTableColumns;
    private readonly int _rowLimit = AppSettings.RowLimit;

    private List<MaintenanceInfo> _maintenanceInfo;
    private List<KeyValuePair<string, MaintenanceInfo>> _updateList = new();

    public enum DateType
    {
        MaintenanceDate,
        NextMaintenanceDate
    }
    public void AddToUpdateList(string key, MaintenanceInfo asset)
    {
        _updateList.Add(new KeyValuePair<string, MaintenanceInfo>(key, asset));
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
                    $"{AppSettings.MaintenanceTableColumns[1]} = '{value.AssetId}'," +
                    $"{AppSettings.MaintenanceTableColumns[2]} = '{value.InChargePersonID}'," +
                    $"{AppSettings.MaintenanceTableColumns[3]} = '{value.MaintenanceContent}'," +
                    $"{AppSettings.MaintenanceTableColumns[4]} = '{value.IsNormal}'," +
                    $"{AppSettings.MaintenanceTableColumns[5]} = '{value.MaintenanceDate}'," +
                    $"{AppSettings.MaintenanceTableColumns[6]} = '{value.NextMaintenanceDate}' " +
                    $"where {AppSettings.MaintenanceTableColumns[0]} = '{key}'";

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
    public async Task ActivateAdd(MaintenanceInfo asset)
    {
        try
        {
            var connection = await SqlConnector.RefreshConnectionAsync();
            var i = asset;

            // 将该固定资产之前记录的mndate（预计下次维护时间）置为null，
            // 表示这件资产有了更新的维护记录
            var queryString =
                $"update {_baseTable} set {_baseTableColumns[6]} = null " +
                $"where {_baseTableColumns[0]} = '{i.MaintenanceID}'";

            using (var command = new MySqlCommand(queryString, connection))
            {
                await command.ExecuteNonQueryAsync();
            };

            queryString =
                $"insert into {_baseTable} " +
                $"value(" +
                $"'{i.MaintenanceID}', " +
                $"'{i.AssetId}', " +
                $"'{i.InChargePersonID}', " +
                $"'{i.MaintenanceContent}'," +
                $"'{i.IsNormal}', " +
                $"'{i.MaintenanceDate}', " +
                $"'{i.NextMaintenanceDate}'" +
                $");"
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
    public async Task ActivateAddWithoutPriKey(MaintenanceInfo asset)
    {
        try
        {
            var connection = await SqlConnector.RefreshConnectionAsync();
            var i = asset;
            // 将该固定资产之前记录的mndate（预计下次维护时间）置为null，
            // 表示这件资产有了更新的维护记录
            var queryString =
                $"update {_baseTable} set {_baseTableColumns[6]} = null " +
                $"where {_baseTableColumns[0]} = '{i.MaintenanceID}'";

            using (var command = new MySqlCommand(queryString, connection))
            {
                await command.ExecuteNonQueryAsync();
            };

            // 正常插入操作
            queryString =
                $"insert into {_baseTable} (" +
                $"{_baseTableColumns[1]}, " +
                $"{_baseTableColumns[2]}, " +
                $"{_baseTableColumns[3]}, " +
                $"{_baseTableColumns[4]}, " +
                $"{_baseTableColumns[5]}, " +
                $"{_baseTableColumns[6]} " +
                $") " +
                $"value(" +
                $"'{i.AssetId}', " +
                $"'{i.InChargePersonID}', " +
                $"'{i.MaintenanceContent}'," +
                $"'{i.IsNormal}', " +
                $"'{i.MaintenanceDate}', " +
                $"'{i.NextMaintenanceDate}'" +
                $");"
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

    private static Task<MaintenanceInfo> ParseFromReader(MySqlDataReader reader)
    {
        var asset = new MaintenanceInfo
        {
            MaintenanceID = reader.GetInt32(0),
            AssetId = reader.GetInt32(1),
            AssetName = reader.GetString(2),
            InChargePersonID = reader.GetInt32(3),
            InChargePersonName = reader.GetString(4),
            MaintenanceContent = reader.GetString(5),
            IsNormal = reader.GetByte(6),
            MaintenanceDate = reader.GetDateTime(7).ToString(),
            NextMaintenanceDate = reader.IsDBNull(8) ? "null" : reader.GetDateTime(8).ToString(),
            Department = reader.GetString(9),
        };
        return Task.FromResult(asset);
    }
    public async Task<IEnumerable<MaintenanceInfo>> GetGridDataAsync()
    {
        if (_maintenanceInfo == null)
        {
            _maintenanceInfo = new List<MaintenanceInfo>();
            try
            {
                var connection = await SqlConnector.RefreshConnectionAsync();
                var queryString = $"select * from {_view} limit {_rowLimit}";
                using (var command = new MySqlCommand(queryString, connection))
                {
                    var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        _maintenanceInfo.Add(await ParseFromReader(reader));
                    }
                };
            }
            catch
            {
                throw;
            }

        }

        await Task.CompletedTask;
        return _maintenanceInfo;
    }
    public async Task<IEnumerable<MaintenanceInfo>> GetFilterGridDataAsync(string column, string value)
    {
        _maintenanceInfo = new List<MaintenanceInfo>();
        try
        {
            var connection = await SqlConnector.RefreshConnectionAsync();
            var queryString = $"select * from {_view} where {column} = '{value}' limit {_rowLimit}";
            using (var command = new MySqlCommand(queryString, connection))
            {
                var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    _maintenanceInfo.Add(await ParseFromReader(reader));
                }
            };
        }
        catch
        {
            throw;
        }

        await Task.CompletedTask;
        return _maintenanceInfo;
    }
    public async Task<IEnumerable<MaintenanceInfo>> GetRefreshGridDataAsync()
    {
        _maintenanceInfo = new List<MaintenanceInfo>();
        try
        {
            var connection = await SqlConnector.RefreshConnectionAsync();
            var queryString = $"select * from {_view} limit {_rowLimit}";
            using (var command = new MySqlCommand(queryString, connection))
            {
                var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    _maintenanceInfo.Add(await ParseFromReader(reader));
                }
            };
        }
        catch
        {
            throw;
        }

        await Task.CompletedTask;
        return _maintenanceInfo;
    }
    public async Task<IEnumerable<MaintenanceInfo>> GetSearchGridDataAsync(string key)
    {
        _maintenanceInfo = new List<MaintenanceInfo>();
        try
        {
            foreach (var column in AppSettings.MaintenanceViewColumns)
            {
                var connection = await SqlConnector.RefreshConnectionAsync();
                var queryString = $"select * from {_view} where {column} like '%{key}%' limit {_rowLimit}";
                using (var command = new MySqlCommand(queryString, connection))
                {
                    var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        _maintenanceInfo.Add(await ParseFromReader(reader));
                    }
                };
            }
        }
        catch
        {
            throw;
        }

        await Task.CompletedTask;
        return _maintenanceInfo;
    }
    public async Task<IEnumerable<MaintenanceInfo>> GetLastMonthGridDataAsync(DateType type, string month)
    {
        _maintenanceInfo = new List<MaintenanceInfo>();
        try
        {
            var connection = await SqlConnector.RefreshConnectionAsync();
            var queryString = "";
            if (type == DateType.MaintenanceDate)
            {
                queryString = $"call get_m_by_time({month});";
            }
            else if (type == DateType.NextMaintenanceDate)
            {
                queryString = $"call get_mn_by_time({month});";
            }
            else
            {
                throw new Exception("Fatal error in Maintenance.GetLastMonthGridDataAsync(): Arg 'Type' doesn't match any DataType eumn.".GetLocalized());
            }
            using (var command = new MySqlCommand(queryString, connection))
            {
                var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    _maintenanceInfo.Add(await ParseFromReader(reader));
                }
            };

        }
        catch
        {
            throw;
        }

        await Task.CompletedTask;
        return _maintenanceInfo;
    }
    public async Task<IEnumerable<MaintenanceInfo>> GetComingWeekGridDateAsync()
    {
        _maintenanceInfo = new List<MaintenanceInfo>();
        try
        {
            var connection = await SqlConnector.RefreshConnectionAsync();
            var queryString = $"call get_mn_by_week(1);";
            using (var command = new MySqlCommand(queryString, connection))
            {
                var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    _maintenanceInfo.Add(await ParseFromReader(reader));
                }
            };

        }
        catch
        {
            throw;
        }

        await Task.CompletedTask;
        return _maintenanceInfo;
    }
    public async Task DeleteRowAsync(int key)
    {
        try
        {
            var connection = await SqlConnector.RefreshConnectionAsync();
            var queryString = $"delete from {_baseTable} where {AppSettings.MaintenanceTableColumns[0]} = {key}";
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

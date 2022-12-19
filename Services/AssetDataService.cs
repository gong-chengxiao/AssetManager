using AssetManager.Contracts.Services;
using AssetManager.Core.Helpers;
using AssetManager.Core.Models;
using AssetManager.Models;
using MySqlConnector;

namespace AssetManager.Services;
public class AssetDataService : IAssetDataService
{
    private readonly string _view = AppSettings.AssetView;
    private readonly string _assetTable = AppSettings.AssetTable;
    private readonly string _vendorTable = AppSettings.VendorTable;
    private readonly string _userTable = AppSettings.UserTable;

    private readonly int _rowLimit = AppSettings.RowLimit;

    private readonly string _vidText = AppSettings.VendorTableColumns[0];
    private readonly string _vnameText = AppSettings.VendorTableColumns[1];
    private readonly string _uidText = AppSettings.UserTableColumns[0];
    private readonly string _unameText = AppSettings.UserTableColumns[1];


    private List<SchoolAsset> _schoolAssets;
    private List<KeyValuePair<string, SchoolAsset>> _updateList = new();
    public void AddToUpdateList(string key, SchoolAsset asset)
    {
        _updateList.Add(new KeyValuePair<string, SchoolAsset>(key, asset));
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
                    $"begin;" +
                    $"select {_vidText} into @_vid from {_vendorTable} where {_vnameText} = {value.AssetVendorName};" +
                    $"select {_uidText} into @_gtuid from {_userTable} where {_unameText} = {value.GetterName};" +
                    $"select {_uidText} into @_usuid from {_userTable} where {_unameText} = {value.UserName};" +
                    $"update {_assetTable} set " +
                    $"{AppSettings.AssetTableColumns[0]} = '{value.AssetID}'," +
                    $"{AppSettings.AssetTableColumns[1]} = '{value.AssetName}'," +
                    $"{AppSettings.AssetTableColumns[2]} = '{value.AssetSpecification}'," +
                    $"{AppSettings.AssetTableColumns[3]} = '{value.AssetType}'," +
                    $"{AppSettings.AssetTableColumns[4]} = '{value.AssetSerialNumber}'," +
                    $"{AppSettings.AssetTableColumns[5]} = '{value.AssetPurchaseDate}'," +
                    $"{AppSettings.AssetTableColumns[6]} = '{value.AssetPurchasePrice}'," +
                    $"{AppSettings.AssetTableColumns[7]} = '{value.AssetPurchaseOrderNumber}'," +
                    $"{AppSettings.AssetTableColumns[8]} = @_vid," +
                    $"{AppSettings.AssetTableColumns[9]} = @_gtuid," +
                    $"{AppSettings.AssetTableColumns[10]} = @_usuid " +
                    $"where {AppSettings.AssetTableColumns[0]} = {key};" +
                    $"commit;";

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
    public async Task ActivateAdd(SchoolAsset asset)
    {
        try
        {
            var connection = await SqlConnector.RefreshConnectionAsync();
            var i = asset;
            var queryString =
                $"begin;" +
                $"select vid into @_vid from v where vname='{i.AssetVendorName}';" +
                $"select uid into @_gtid from u where uname='{i.GetterName}';" +
                $"select uid into @_usid from u where uname='{i.UserName}';" +
                $"insert into {_assetTable} " +
                $"value(" +
                $"'{i.AssetID}'," +
                $"'{i.AssetName}'," +
                $"'{i.AssetSpecification}'," +
                $"'{i.AssetType}'," +
                $"'{i.AssetSerialNumber}'," +
                $"'{i.AssetPurchaseDate}'," +
                $"'{i.AssetPurchasePrice}'," +
                $"'{i.AssetPurchaseOrderNumber}'," +
                $"@_vid, @_gtid, @_usid);" +
                $"commit;";
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

    private static Task<SchoolAsset> ParseFromReader(MySqlDataReader reader)
    {
        var asset = new SchoolAsset
        {
            AssetID = reader.GetInt32(0),
            AssetName = reader.GetString(1),
            AssetSpecification = reader.GetString(2),
            AssetType = reader.GetString(3),
            AssetSerialNumber = reader.GetString(4),
            AssetPurchaseDate = reader.GetDateTime(5).ToString(),
            AssetPurchasePrice = reader.GetDouble(6),
            AssetPurchaseOrderNumber = reader.GetString(7),
            AssetVendorName = reader.GetString(8),
            GetterName = reader.GetString(9),
            UserName = reader.GetString(10),
            DepartmentName = reader.GetString(11)

        };
        return Task.FromResult(asset);
    }
    public async Task<IEnumerable<SchoolAsset>> GetGridDataAsync()
    {
        if (_schoolAssets == null)
        {
            _schoolAssets = new List<SchoolAsset>();
            try
            {
                var connection = await SqlConnector.RefreshConnectionAsync();
                var queryString = $"select * from {_view} limit {_rowLimit}";
                using (var command = new MySqlCommand(queryString, connection))
                {
                    var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        _schoolAssets.Add(await ParseFromReader(reader));
                    }
                };
            }
            catch
            {
                throw;
            }

        }

        await Task.CompletedTask;
        return _schoolAssets;
    }
    public async Task<IEnumerable<SchoolAsset>> GetFilterGridDataAsync(string column, string value)
    {
        _schoolAssets = new List<SchoolAsset>();
        try
        {
            var connection = await SqlConnector.RefreshConnectionAsync();
            var queryString = $"select * from {_view} where {column} like '%{value}%' limit {_rowLimit}";
            using (var command = new MySqlCommand(queryString, connection))
            {
                var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    _schoolAssets.Add(await ParseFromReader(reader));
                }
            };
        }
        catch
        {
            throw;
        }

        await Task.CompletedTask;
        return _schoolAssets;
    }
    public async Task<IEnumerable<SchoolAsset>> GetRefreshGridDataAsync()
    {
        _schoolAssets = new List<SchoolAsset>();
        try
        {
            var connection = await SqlConnector.RefreshConnectionAsync();
            var queryString = $"select * from {_view} limit {_rowLimit}";
            using (var command = new MySqlCommand(queryString, connection))
            {
                var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    _schoolAssets.Add(await ParseFromReader(reader));
                }
            };
        }
        catch
        {
            throw;
        }

        await Task.CompletedTask;
        return _schoolAssets;
    }
    public async Task<IEnumerable<SchoolAsset>> GetSearchGridDataAsync(string key)
    {
        _schoolAssets = new List<SchoolAsset>();
        try
        {
            foreach (var column in AppSettings.AssetViewColumns)
            {
                var connection = await SqlConnector.RefreshConnectionAsync();
                var queryString = $"select * from {_view} where {column} like '%{key}%' limit {_rowLimit}";
                using (var command = new MySqlCommand(queryString, connection))
                {
                    var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        _schoolAssets.Add(await ParseFromReader(reader));
                    }
                };
            }
        }
        catch
        {
            throw;
        }

        await Task.CompletedTask;
        return _schoolAssets;
    }
    public async Task<IEnumerable<SchoolAsset>> GetLastMonthGridDataAsync(string month)
    {
        _schoolAssets = new List<SchoolAsset>();
        try
        {
            foreach (var column in AppSettings.AssetViewColumns)
            {
                var connection = await SqlConnector.RefreshConnectionAsync();
                var queryString = $"call get_a_by_time({month});";
                using (var command = new MySqlCommand(queryString, connection))
                {
                    var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        _schoolAssets.Add(await ParseFromReader(reader));
                    }
                };
            }
        }
        catch
        {
            throw;
        }

        await Task.CompletedTask;
        return _schoolAssets;
    }
    public async Task DeleteRowAsync(int key)
    {
        try
        {
            var connection = await SqlConnector.RefreshConnectionAsync();
            var queryString = $"delete from {_assetTable} where {AppSettings.AssetViewColumns[0]} = {key}";
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

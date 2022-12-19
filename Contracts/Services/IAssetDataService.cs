using AssetManager.Core.Models;

namespace AssetManager.Core.Contracts.Services;
public interface IAssetDataService
{
    Task<IEnumerable<SchoolAsset>> GetGridDataAsync();
    Task<IEnumerable<SchoolAsset>> GetFilterGridDataAsync(string column, string value);
    Task<IEnumerable<SchoolAsset>> GetRefreshGridDataAsync();
    Task<IEnumerable<SchoolAsset>> GetSearchGridDataAsync(string key);
    Task<IEnumerable<SchoolAsset>> GetLastMonthGridDataAsync(string month);
    Task ActivateUpdateList();
    void AddToUpdateList(string key, SchoolAsset asset);
    Task DeleteRowAsync(int key);
    Task ActivateAdd(SchoolAsset asset);
}

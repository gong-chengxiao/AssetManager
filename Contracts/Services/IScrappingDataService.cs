using AssetManager.Models;

namespace AssetManager.Contracts.Services;
public interface IScrappingDataService
{
    Task<IEnumerable<ScrappingInfo>> GetGridDataAsync();
    Task<IEnumerable<ScrappingInfo>> GetFilterGridDataAsync(string column, string value);
    Task<IEnumerable<ScrappingInfo>> GetRefreshGridDataAsync();
    Task<IEnumerable<ScrappingInfo>> GetSearchGridDataAsync(string key);
    Task<IEnumerable<ScrappingInfo>> GetLastMonthGridDataAsync(string month);
    Task ActivateUpdateList();
    void AddToUpdateList(string key, ScrappingInfo maintenanceInfo);
    Task DeleteRowAsync(int key);
    Task ActivateAdd(ScrappingInfo maintenanceInfo);
}

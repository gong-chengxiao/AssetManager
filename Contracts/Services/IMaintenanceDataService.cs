using AssetManager.Models;
using static AssetManager.Services.MaintenanceDataService;

namespace AssetManager.Contracts.Services;
public interface IMaintenanceDataService
{
    Task<IEnumerable<MaintenanceInfo>> GetGridDataAsync();
    Task<IEnumerable<MaintenanceInfo>> GetFilterGridDataAsync(string column, string value);
    Task<IEnumerable<MaintenanceInfo>> GetRefreshGridDataAsync();
    Task<IEnumerable<MaintenanceInfo>> GetSearchGridDataAsync(string key);
    Task<IEnumerable<MaintenanceInfo>> GetLastMonthGridDataAsync(DateType type, string month);
    Task ActivateUpdateList();
    void AddToUpdateList(string key, MaintenanceInfo maintenanceInfo);
    Task DeleteRowAsync(int key);
    Task ActivateAdd(MaintenanceInfo maintenanceInfo);
}

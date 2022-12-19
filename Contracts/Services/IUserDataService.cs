using AssetManager.Models;

namespace AssetManager.Contracts.Services;
public interface IUserDataService
{
    Task<IEnumerable<UserInfo>> GetGridDataAsync();
    Task<IEnumerable<UserInfo>> GetFilterGridDataAsync(string column, string value);
    Task<IEnumerable<UserInfo>> GetRefreshGridDataAsync();
    Task<IEnumerable<UserInfo>> GetSearchGridDataAsync(string key);
    Task ActivateUpdateList();
    void AddToUpdateList(string key, UserInfo asset);
    Task DeleteRowAsync(int key);
    Task ActivateAdd(UserInfo asset);
}

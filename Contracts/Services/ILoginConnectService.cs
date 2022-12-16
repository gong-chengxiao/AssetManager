using AssetManager.Helpers;

namespace AssetManager.Contracts.Services;

public interface ILoginConnectService
{
    Task SetSqlConnectFromLogin(LoginForm form);
    Task NavigateToShellPageAsync();
}

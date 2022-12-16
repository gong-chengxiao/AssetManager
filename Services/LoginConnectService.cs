using AssetManager.Contracts.Services;
using AssetManager.Core.Models;
using AssetManager.Helpers;

namespace AssetManager.Services;

public class LoginConnectService : ILoginConnectService
{
    public async Task SetSqlConnectFromLogin(LoginForm login)
    {
        try
        {
            if (login.IsUseOtherDb)
            {
                AppSettings.Host = login.Host;
                AppSettings.Port = login.Port;
                AppSettings.DbName = login.DbName;
                await SqlConnector.GetSqlConnectionAsync(login.Username, login.Password);
            }
            else
            {
                await SqlConnector.GetSqlConnectionAsync(login.Username, login.Password);
            }
        }
        catch { throw; }
    }

}

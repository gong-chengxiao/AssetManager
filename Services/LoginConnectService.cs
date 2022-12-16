using AssetManager.Contracts.Services;
using AssetManager.Core.Models;
using AssetManager.Helpers;
using AssetManager.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

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

    public async Task NavigateToShellPageAsync()
    {
        UIElement shell = App.GetService<ShellPage>();
        App.MainWindow.Content = shell ?? new Frame();
    }

}

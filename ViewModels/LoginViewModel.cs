using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using AssetManager.Helpers;
using AssetManager.Core.Models;
using AssetManager.Contracts.Services;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Controls;

namespace AssetManager.ViewModels;
public class LoginViewModel : INotifyPropertyChanged
{

    public readonly ILoginConnectService _loginConnectService;
    public ICommand LoginCommand
    {
        get;
    }

    public LoginForm LoginForm
    {
        get;
        set;
    }
    public bool IsUseOtherDb
    {
        get;
        set;
    }


    private InfoBar? _messageInfoBar;
    public InfoBar? MessageInfoBar
    {
        get => _messageInfoBar;
        set
        {
            _messageInfoBar = value;
            OnPropertyChanged();
        }
    }


    private bool _progressRingActive;
    public bool ProgressRingActive
    {
        get => _progressRingActive;
        set
        {
            _progressRingActive = value;
            OnPropertyChanged();
        }
    }


    public LoginViewModel(ILoginConnectService loginConnectService)
    {
        LoginForm = new();
        IsUseOtherDb = false;
        _loginConnectService = loginConnectService;
        ProgressRingActive = false;

        LoginCommand = new RelayCommand(
            async () =>
            {
                ProgressRingActive = true;
                if (LoginForm != null)
                {
                    try
                    {
                        await Task.Delay(1);
                        await _loginConnectService.SetSqlConnectFromLogin(LoginForm);
                        await _loginConnectService.NavigateToShellPageAsync();
                    }
                    catch (Exception e)
                    {
                        MessageInfoBar = new InfoBar
                        {
                            Title = NotifyHelper.ErrorTitle,
                            Message = e.Message,
                            Severity = InfoBarSeverity.Error,
                            IsClosable = true,
                            Visibility = Visibility.Visible,
                            IsOpen = true
                        };
                    }
                }
                ProgressRingActive = false;
            });
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

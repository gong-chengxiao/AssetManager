using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using AssetManager.Contracts.ViewModels;
using AssetManager.Core.Contracts.Services;
using AssetManager.Core.Models;
using AssetManager.Helpers;
using AssetManager.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AssetManager.ViewModels;

public class AssetTableViewModel : ObservableRecipient, INavigationAware, INotifyPropertyChanged
{
    private readonly IAssetDataService _assetDataService;

    private Visibility _progressBarVisibility;
    public Visibility ProgressBarVisibility
    {
        get => _progressBarVisibility;
        set
        {
            _progressBarVisibility = value;
            OnPropertyChanged();
        }
    }
    public ICommand RefreshCommand
    {
        get;
    }
    public ICommand FileterCommand
    {
        get;
    }

    public ObservableCollection<SchoolAsset> Source { get; } = new ObservableCollection<SchoolAsset>();

    public AssetTableViewModel(IAssetDataService assetDataService)
    {
        _assetDataService = assetDataService;
        FileterCommand = new RelayCommand<KeyValueStringPair>(
            async (param) =>
            {
                ProgressBarVisibility = Visibility.Visible;
                try
                {
                    var data = await _assetDataService.GetFilterGridDataAsync(param.Key, param.Value);
                    Source.Clear();
                    foreach (var asset in data)
                    {
                        Source.Add(asset);
                    }
                }
                catch (Exception e)
                {
                    await NotifyHelper.ShowNotifyDialog(NotifyHelper.ErrorTitle, e.Message);
                }
                finally
                {
                    ProgressBarVisibility = Visibility.Collapsed;
                }
            });
        RefreshCommand = new RelayCommand(
            async () =>
            {
                ProgressBarVisibility = Visibility.Visible;
                try
                {
                    var data = await _assetDataService.RefreshGridDataAsync();
                    Source.Clear();
                    foreach (var asset in data)
                    {
                        Source.Add(asset);
                    }
                }
                catch (Exception e)
                {
                    await NotifyHelper.ShowNotifyDialog(NotifyHelper.ErrorTitle, e.Message);
                }
                finally
                {
                    ProgressBarVisibility = Visibility.Collapsed;
                }
            });
    }

    public async void OnNavigatedTo(object parameter)
    {
        Source.Clear();
        try
        {      
            var data = await _assetDataService.GetGridDataAsync();

            foreach (var item in data)
            {
                Source.Add(item);
            }
        }
        catch (Exception e)
        {
            await NotifyHelper.ShowNotifyDialog(NotifyHelper.ErrorTitle, e.Message);
        }
    }

    public void OnNavigatedFrom()
    {
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

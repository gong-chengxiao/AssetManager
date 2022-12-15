using System.Collections.ObjectModel;

using AssetManager.Contracts.ViewModels;
using AssetManager.Core.Contracts.Services;
using AssetManager.Core.Models;
using AssetManager.Helpers;
using AssetManager.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AssetManager.ViewModels;

public class AssetTableViewModel : ObservableRecipient, INavigationAware
{
    private readonly IAssetDataService _assetDataService;
    

    public ObservableCollection<SchoolAsset> Source { get; } = new ObservableCollection<SchoolAsset>();

    public AssetTableViewModel(IAssetDataService assetDataService)
    {
        _assetDataService = assetDataService;
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
}

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using AssetManager.Contracts.ViewModels;
using AssetManager.Contracts.Services;
using AssetManager.Models;
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
    
    // 新增item的数量，每次提交更改后重置
    public int NewItemNumber { get; set; } = 0;

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
    public ICommand TimeFilterCommand
    {
        get;
    }
    public ICommand SubmittCommand
    {
        get;
    }
    public ICommand DeleteRowCommand
    {
        get;
    }
    public ICommand AddCommand
    {
        get;
    }
    public ObservableCollection<SchoolAsset> Source { get; } = new ObservableCollection<SchoolAsset>();
    public SchoolAsset SelectedRow
    {
        get; set;
    }
    public AssetTableViewModel(IAssetDataService assetDataService)
    {
        ProgressBarVisibility = Visibility.Collapsed;
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
                    var data = await _assetDataService.GetRefreshGridDataAsync();
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
        TimeFilterCommand = new RelayCommand<string>(
           async (param) =>
           {
               ProgressBarVisibility = Visibility.Visible;
               try
               {
                   var data = await _assetDataService.GetLastMonthGridDataAsync(param);
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
        SubmittCommand = new RelayCommand(
            async () =>
            {
                ProgressBarVisibility = Visibility.Visible;
                try
                {
                    await _assetDataService.ActivateUpdateList();
                    for(var i = 0; i < NewItemNumber; i++)
                    {
                        await _assetDataService.ActivateAdd(Source[i]);
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
        DeleteRowCommand = new RelayCommand(
            async () =>
            {
                ProgressBarVisibility = Visibility.Visible;
                try
                {
                    await _assetDataService.DeleteRowAsync(SelectedRow.AssetID);
                    var data = await _assetDataService.GetRefreshGridDataAsync();
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
        AddCommand = new RelayCommand(
            () =>
            {
                NewItemNumber++;
                Source.Insert(0, new SchoolAsset{
                    AssetID = Source.Last().AssetID + 1 
                });
            });
    }
    public async Task SearchBoxQuery(string param)
    {
        if (param != "")
        {
            ProgressBarVisibility = Visibility.Visible;
            try
            {
                await Task.Delay(1);
                var data = await _assetDataService.GetSearchGridDataAsync(param);
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
        }
    }
    public void AddToUpdateList(string key, SchoolAsset value)
    {
        _assetDataService.AddToUpdateList(key, value);
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

    public new event PropertyChangedEventHandler? PropertyChanged;
    protected virtual new void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

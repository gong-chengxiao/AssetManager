using System.Collections.ObjectModel;
using System.ComponentModel;
using AssetManager.Contracts.Services;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AssetManager.Contracts.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using AssetManager.Models;
using AssetManager.Helpers;

namespace AssetManager.ViewModels;

public class MaintenanceTableViewModel : ObservableRecipient, INavigationAware, INotifyPropertyChanged
{
    private readonly IMaintenanceDataService _maintenanceDataService;

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
    public ICommand NextTimeFilterCommand
    {
        get;
    }
    public ObservableCollection<MaintenanceInfo> Source { get; } = new ObservableCollection<MaintenanceInfo>();
    public MaintenanceInfo SelectedRow
    {
        get; set;
    }
    public MaintenanceTableViewModel(IMaintenanceDataService maintenanceDataService)
    {
        ProgressBarVisibility = Visibility.Collapsed;
        _maintenanceDataService = maintenanceDataService;

        FileterCommand = new RelayCommand<KeyValueStringPair>(
            async (param) =>
            {
                ProgressBarVisibility = Visibility.Visible;
                try
                {
                    var data = await _maintenanceDataService.GetFilterGridDataAsync(param.Key, param.Value);
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
                    var data = await _maintenanceDataService.GetRefreshGridDataAsync();
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
                   var data = await _maintenanceDataService.GetLastMonthGridDataAsync(Services.MaintenanceDataService.DateType.MaintenanceDate, param);
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
                    await _maintenanceDataService.ActivateUpdateList();
                    for (var i = 0; i < NewItemNumber; NewItemNumber--)
                    {
                        await _maintenanceDataService.ActivateAdd(Source[i]);
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
                    await _maintenanceDataService.DeleteRowAsync(SelectedRow.MaintenanceID);
                    var data = await _maintenanceDataService.GetRefreshGridDataAsync();
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
                Source.Insert(0, new MaintenanceInfo
                {
                    MaintenanceID = Source.Last().MaintenanceID + 1
                });
            });
        NextTimeFilterCommand = new RelayCommand<string>(
           async (param) =>
           {
               ProgressBarVisibility = Visibility.Visible;
               try
               {
                   var data = await _maintenanceDataService.GetLastMonthGridDataAsync(Services.MaintenanceDataService.DateType.NextMaintenanceDate, param);
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
    public async Task SearchBoxQuery(string param)
    {
        if (param != "")
        {
            ProgressBarVisibility = Visibility.Visible;
            try
            {
                await Task.Delay(1);
                var data = await _maintenanceDataService.GetSearchGridDataAsync(param);
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
    public void AddToUpdateList(string key, MaintenanceInfo value)
    {
        _maintenanceDataService.AddToUpdateList(key, value);
    }

    public async void OnNavigatedTo(object parameter)
    {
        Source.Clear();
        try
        {
            var data = await _maintenanceDataService.GetGridDataAsync();

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

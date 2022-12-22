using System.Collections.ObjectModel;
using AssetManager.Core.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AssetManager.Contracts.Services;
using AssetManager.Contracts.ViewModels;
using AssetManager.Helpers;
using AssetManager.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel.DataTransfer;
using CommunityToolkit.Mvvm.Input;
using Windows.Storage.Pickers;
using Windows.Storage;

namespace AssetManager.ViewModels;

public class HomeViewModel : ObservableRecipient, INavigationAware, INotifyPropertyChanged
{
    private readonly IMaintenanceDataService _maintenanceDataService;
    private readonly IDatabaseBackupService _databaseBackupService;
    private DateTime _lastBackupTime;
    private string _greetingMessage;
    public bool[] TableChecked
    {
        get; set;
    } =
    {
        true,
        true,
        true,
        true,
        true,
    };
    public string[] Tables =
    {
        AppSettings.AssetTable,
        AppSettings.MaintenanceTable,
        AppSettings.UserTable,
        AppSettings.ScrappingTable,
        AppSettings.VendorTable,
    };
    private string _backupFilePath;
    public string BackupFilePath
    {
        get => _backupFilePath;
        set
        {
            _backupFilePath = value;
            OnPropertyChanged();
        }
    }
    public string GreetingMessage
    {
        get => _greetingMessage;
    }
    public ObservableCollection<MaintenanceInfo> Source { get; } = new();
    public ObservableCollection<MaintenanceInfo> Target { get; } = new();
    public DateTimeOffset CurMtDate
    {
        get; set;
    }
    public DateTimeOffset NextMtDate
    {
        get; set;
    }
    public string MtContent
    {
        get; set;
    }
    public bool IsNormalMt
    {
        set; get;
    }
    public ICommand SubmitMiCommand
    {
        get;
    }
    public ICommand BackupCommand
    {
        get;
    }
    public ICommand ExploreCommand
    {
        get;
    }
    public HomeViewModel(IMaintenanceDataService maintenanceDataService, IDatabaseBackupService databaseBackupService)
    {
        _greetingMessage = GetGreetingMessage();
        _maintenanceDataService = maintenanceDataService;
        _databaseBackupService = databaseBackupService;
        IsNormalMt = true;
        CurMtDate = DateTimeOffset.Now;
        NextMtDate = DateTimeOffset.Now;
        MtContent = "";
        _lastBackupTime = databaseBackupService.LastBackupTime;
        SubmitMiCommand = new RelayCommand(
            async () =>
            {
                try
                {
                    foreach (var i in Target)
                    {
                        i.NextMaintenanceDate = NextMtDate.DateTime.ToString();
                        i.MaintenanceDate = CurMtDate.DateTime.ToString();
                        i.MaintenanceContent = MtContent;
                        i.IsNormal = (byte)(IsNormalMt ? 1 : 0);
                        await _maintenanceDataService.ActivateAddWithoutPriKey(i);
                    }
                    Target.Clear();
                }
                catch (Exception e)
                {
                    await NotifyHelper.ShowError(e.Message);
                }
            });
        BackupCommand = new RelayCommand(
            async () =>
            {
                try
                {
                    string[] selectedTables = await GetSelectedTable();
                    if (selectedTables.Length == 0)
                    {
                        throw new Exception("No table selected".GetLocalized());
                    }
                    if (BackupFilePath == null)
                    {
                        throw new Exception("No backup file path selected".GetLocalized());
                    }
                    await databaseBackupService.BackupDatabaseAsync(BackupFilePath, selectedTables);
                }
                catch (Exception e)
                {
                    await NotifyHelper.ShowError(e.Message);
                }
            });
        ExploreCommand = new RelayCommand(
            async () =>
            {
                // 浏览并选择文件
                try
                {
                    var window = new Microsoft.UI.Xaml.Window();
                    var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

                    var picker = new FileSavePicker();
                    picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                    picker.FileTypeChoices.Add("SQL File", new List<string>() { ".sql" });
                    picker.SuggestedFileName = $"AssetManager_Data_Backup_{DateTime.Now}";
                    WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
                    var file = await picker.PickSaveFileAsync();
                    if (file != null)
                    {
                        BackupFilePath = file.Path;
                    }
                }

                catch (Exception e)
                {
                    await NotifyHelper.ShowError(e.Message);
                }
            });

    }
    private string GetGreetingMessage()
    {
        var hour = DateTime.Now.Hour;
        var greeting = hour < 12 ? "Home_MorningGreeting".GetLocalized()
                        : hour < 18 ? "Home_AfternoonGreeting".GetLocalized()
                        : "Home_EveningGreeting".GetLocalized();
        return $"{greeting} {Environment.UserName}!";
    }

    private async Task<string[]> GetSelectedTable()
    {
        List<string> selectedTable = new();
        for (var i = 0; i < Tables.Length; i++)
        {
            if (TableChecked[i])
            {
                selectedTable.Add(Tables[i]);
            }
        }

        await Task.CompletedTask;
        return selectedTable.ToArray();
    }
    private int GetLastBackupPassDays()
    {
        return (DateTime.Now - _lastBackupTime).Days;
    }
    public async void OnNavigatedTo(object parameter)
    {
        Source.Clear();
        try
        {
            var data = await _maintenanceDataService.GetComingWeekGridDateAsync();

            foreach (var item in data)
            {
                Source.Add(item);
            }
            MiInfo = Source.Count.ToString();
        }
        catch (Exception e)
        {
            await NotifyHelper.ShowNotifyDialog(NotifyHelper.ErrorTitle, e.Message);
        }
    }
    public void OnNavigatedFrom()
    {
    }
    public async Task MiListViewDrop(object sender, Microsoft.UI.Xaml.DragEventArgs e)
    {
        ListView target = (ListView)sender;

        if (e.DataView.Contains(StandardDataFormats.Text))
        {
            DragOperationDeferral def = e.GetDeferral();
            string s = await e.DataView.GetTextAsync();
            string[] items = s.Split('\n');
            foreach (string item in items)
            {

                // Create MaintenanceInfo object from string, add to existing target ListView
                string[] info = item.Split("&", 6);
                MaintenanceInfo temp = new MaintenanceInfo
                {
                    MaintenanceID = int.Parse(info[0]),
                    AssetId = int.Parse(info[1]),
                    AssetName = info[2],
                    InChargePersonID = int.Parse(info[3]),
                    NextMaintenanceDate = info[4],
                    Department = info[5],
                };

                // Find the insertion index:
                Windows.Foundation.Point pos = e.GetPosition(target.ItemsPanelRoot);

                // If the target ListView has items in it, use the height of the first item
                //      to find the insertion index.
                int index = 0;
                if (target.Items.Count != 0)
                {
                    // Get a reference to the first item in the ListView
                    ListViewItem sampleItem = (ListViewItem)target.ContainerFromIndex(0);

                    // Adjust itemHeight for margins
                    double itemHeight = sampleItem.ActualHeight + sampleItem.Margin.Top + sampleItem.Margin.Bottom;

                    // Find index based on dividing number of items by height of each item
                    index = Math.Min(target.Items.Count - 1, (int)(pos.Y / itemHeight));

                    // Find the item being dropped on top of.
                    ListViewItem targetItem = (ListViewItem)target.ContainerFromIndex(index);

                    // If the drop position is more than half-way down the item being dropped on
                    //      top of, increment the insertion index so the dropped item is inserted
                    //      below instead of above the item being dropped on top of.
                    Windows.Foundation.Point positionInItem = e.GetPosition(targetItem);
                    if (positionInItem.Y > itemHeight / 2)
                    {
                        index++;
                    }

                    // Don't go out of bounds
                    index = Math.Min(target.Items.Count, index);
                }
                // Only other case is if the target ListView has no items (the dropped item will be
                //      the first). In that case, the insertion index will remain zero.

                // Find correct source list
                if (target.Name == "SourceListView")
                {
                    foreach (MaintenanceInfo i in Source)
                    {
                        if (i.MaintenanceID == temp.MaintenanceID)
                        {
                            Source.Remove(i);
                            break;
                        }
                    }
                    // Find the ItemsSource for the target ListView and insert
                    Source.Insert(index, temp);
                    //Go through source list and remove the items that are being moved
                    foreach (MaintenanceInfo i in Target)
                    {
                        if (i.MaintenanceID == temp.MaintenanceID)
                        {
                            Target.Remove(i);
                            break;
                        }
                    }
                }
                else if (target.Name == "TargetListView")
                {
                    foreach (MaintenanceInfo i in Target)
                    {
                        if (i.MaintenanceID == temp.MaintenanceID)
                        {
                            Target.Remove(i);
                            break;
                        }
                    }
                    Target.Insert(index, temp);
                    foreach (MaintenanceInfo i in Source)
                    {
                        if (i.MaintenanceID == temp.MaintenanceID)
                        {
                            Source.Remove(i);
                            break;
                        }
                    }
                }
            }

            e.AcceptedOperation = DataPackageOperation.Move;
            def.Complete();
        }
    }


    public new event PropertyChangedEventHandler? PropertyChanged;
    protected virtual new void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    private string _updateMaintenanceText = "Update maintenance record".GetLocalized();
    public string UpdateMaintenanceText => _updateMaintenanceText;

    private string _miDatePickerHeader = "Select maintenance date".GetLocalized();
    private string _nMiDatePickerHeader = "Select next maintenance arrangement date".GetLocalized();
    public string MiDatePickerHeader => _miDatePickerHeader;
    public string NMiDatePickerHeader => _nMiDatePickerHeader;

    private string _submitMiText = "Submit maintenance record".GetLocalized();
    public string SubmitMiText => _submitMiText;

    private string _miInfoPre = "In the coming week, you have".GetLocalized();
    private string _miInfoPost = "maintenance arrangements".GetLocalized();
    private int _miInfoCount = -1;
    public string MiInfo
    {
        get => $"{_miInfoPre} {_miInfoCount} {_miInfoPost}";
        set
        {
            try
            {
                _miInfoCount = int.Parse(value);
                OnPropertyChanged();
            }
            catch { }
        }
    }


    private string _backupPre = "You haven't done a backup in".GetLocalized();
    private string _backupPost = "days".GetLocalized();
    public string BackupInfo => $"{_backupPre} {GetLastBackupPassDays()} {_backupPost}";

    private string _submitContentText = "Write maintenance content:".GetLocalized();
    public string SubmitContentText => _submitContentText;

    private string _isNormalText = "Normal Maintenance".GetLocalized();
    public string IsNormalText => _isNormalText;

    private string _mtArrangementText = "Maintenance Arrangement".GetLocalized();
    public string MtArrangementText => _mtArrangementText;

    public string DatabaseBackupText => "Database backup".GetLocalized();
    public string BackupRightnowText => "Backup right now".GetLocalized();
    public string SelectFilePathText => "Select file path".GetLocalized();
    public string ExploreText => "Explore".GetLocalized();
}

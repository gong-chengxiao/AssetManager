using AssetManager.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace AssetManager.Views;

// TODO: Change the grid as appropriate for your app. Adjust the column definitions on DataGridPage.xaml.
// For more details, see the documentation at https://docs.microsoft.com/windows/communitytoolkit/controls/datagrid.
public sealed partial class MaintenanceTablePage : Page
{
    public MaintenanceTableViewModel ViewModel
    {
        get;
    }

    public MaintenanceTablePage()
    {
        ViewModel = App.GetService<MaintenanceTableViewModel>();
        InitializeComponent();
    }
}

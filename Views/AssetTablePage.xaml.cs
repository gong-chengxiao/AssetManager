using AssetManager.ViewModels;
using AssetManager.Helpers;

using Microsoft.UI.Xaml.Controls;
using AssetManager.Models;
using Microsoft.UI.Xaml;

namespace AssetManager.Views;

// TODO: Change the grid as appropriate for your app. Adjust the column definitions on DataGridPage.xaml.
// For more details, see the documentation at https://docs.microsoft.com/windows/communitytoolkit/controls/datagrid.
public sealed partial class AssetTablePage : Page
{
    public AssetTableViewModel ViewModel
    {
        get;
    }

    public AssetTablePage()
    {
        ViewModel = App.GetService<AssetTableViewModel>();
        InitializeComponent();
    }

    private async void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        await ViewModel.SearchBoxQuery(args.QueryText);
    }

    private void DataGrid_RowEditEnded(object sender, CommunityToolkit.WinUI.UI.Controls.DataGridRowEditEndedEventArgs e)
    {
        if (e.Row.GetIndex() >= ViewModel.NewItemNumber)
        {
            SchoolAsset row = (SchoolAsset)e.Row.DataContext;
            ViewModel.AddToUpdateList(row.AssetID.ToString(), row);
        }
    }

    private void DataGrid_RightTapped(object sender, Microsoft.UI.Xaml.Input.RightTappedRoutedEventArgs e)
    {
        ViewModel.SelectedRow = (e.OriginalSource as FrameworkElement).DataContext as SchoolAsset;
    }
}

using AssetManager.Models;
using AssetManager.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AssetManager.Views;

// TODO: Change the grid as appropriate for your app. Adjust the column definitions on DataGridPage.xaml.
// For more details, see the documentation at https://docs.microsoft.com/windows/communitytoolkit/controls/datagrid.
public sealed partial class UserTablePage : Page
{
    public UserTableViewModel ViewModel
    {
        get;
    }

    public UserTablePage()
    {
        ViewModel = App.GetService<UserTableViewModel>();
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
            UserInfo row = (UserInfo)e.Row.DataContext;
            ViewModel.AddToUpdateList(row.UserID.ToString(), row);
        }
    }

    private void DataGrid_RightTapped(object sender, Microsoft.UI.Xaml.Input.RightTappedRoutedEventArgs e)
    {
        ViewModel.SelectedRow = (e.OriginalSource as FrameworkElement).DataContext as UserInfo;
    }
}

using AssetManager.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;
using AssetManager.Models;
using System.Text;
using Windows.ApplicationModel.Contacts;

namespace AssetManager.Views;

public sealed partial class HomePage : Page
{
    public HomeViewModel ViewModel
    {
        get;
    }

    public HomePage()
    {
        ViewModel = App.GetService<HomeViewModel>();
        InitializeComponent();
    }

    private async void ListView_Drop(object sender, Microsoft.UI.Xaml.DragEventArgs e)
    {
        await ViewModel.MiListViewDrop(sender, e);
    }
    private void Source_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
    {
        // Prepare a string with one dragged item per line
        StringBuilder items = new StringBuilder();
        foreach (MaintenanceInfo item in e.Items)
        {
            if (items.Length > 0) { items.Append('\n'); }
            if (item.ToString() != null)
            {
                // Append name from contact object onto data string
                items.Append(
                    item.MaintenanceID.ToString() + "&" +
                    item.AssetId.ToString() + "&" +
                    item.AssetName + "&" +
                    item.InChargePersonID.ToString() + "&" +
                    item.NextMaintenanceDate.ToString() + "&" +
                    item.Department
                    );
            }
        }
        // Set the content of the DataPackage
        e.Data.SetText(items.ToString());

        e.Data.RequestedOperation = DataPackageOperation.Move;

    }
    private void Source_DragOver(object sender, DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.Move;
    }
    private void Target_DragOver(object sender, DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.Move;
    }
    private void Target_DragEnter(object sender, DragEventArgs e)
    {
        // We don't want to show the Move icon
        e.DragUIOverride.IsGlyphVisible = false;
    }
    private void Target_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
    {
         // Prepare a string with one dragged item per line
        StringBuilder items = new StringBuilder();
        foreach (MaintenanceInfo item in e.Items)
        {
            if (items.Length > 0) { items.Append('\n'); }
            if (item.ToString() != null)
            {
                // Append name from contact object onto data string
                items.Append(
                    item.MaintenanceID.ToString() + "&" +
                    item.AssetId.ToString() + "&" +
                    item.AssetName + "&" +
                    item.InChargePersonID.ToString() + "&" +
                    item.NextMaintenanceDate.ToString() + "&" +
                    item.Department
                    );
            }
        }
        // Set the content of the DataPackage
        e.Data.SetText(items.ToString());

        e.Data.RequestedOperation = DataPackageOperation.Move;
    }
}

using AssetManager.Views;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;

namespace AssetManager.Helpers
{
    class NotifyHelper
    {
        private static readonly string _errorTitle = "Dialog_ErrorTittle".GetLocalized();
        public static string ErrorTitle => _errorTitle;
        private static readonly string _warningTitle = "Dialog_WarningTitle".GetLocalized();
        public static string WarningTitle => _warningTitle;

        public static XamlRoot? XamlRoot
        {
            get; set;
        }

        public static async Task<ContentDialogResult> ShowNotifyDialog(string title, string content)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = title,
                Content = content,
                PrimaryButtonText = "OK".GetLocalized(),
                CloseButtonText = "Cancel".GetLocalized(),
                SecondaryButtonText = "Copy to clipboard".GetLocalized(),
                SecondaryButtonCommand = new RelayCommand(
                () =>
                {
                    DataPackage dataPackage = new DataPackage();
                    dataPackage.SetText(content);
                    Clipboard.SetContent(dataPackage);
                }),
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = XamlRoot

            };
            return await dialog.ShowAsync();
        }

        public static async Task<ContentDialogResult> ShowError(string content)
        {
            return await ShowNotifyDialog(_errorTitle, content);
        }

        public static async Task<ContentDialogResult> ShowWarning(string content)
        {
            return await ShowNotifyDialog(_warningTitle, content);
        }
    }
}

using AssetManager.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace AssetManager.Helpers
{
    class NotifyHelper
    {
        private static readonly string _errorTitle = "Dialog_ErrorTittle".GetLocalized();
        public static string ErrorTitle => _errorTitle;

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
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = XamlRoot

            };
            return await dialog.ShowAsync();
        }

        public static async Task<ContentDialogResult> ShowError(string content)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = _errorTitle,
                Content = content,
                PrimaryButtonText = "OK".GetLocalized(),
                CloseButtonText = "Cancel".GetLocalized(),
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = XamlRoot

            };
            return await dialog.ShowAsync();
        }
    }
}

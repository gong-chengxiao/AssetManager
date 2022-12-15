using AssetManager.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace AssetManager.Helpers
{
    class NotifyHelper
    {
        private static string _errorTitle = "Dialog_ErrorTittle".GetLocalized();
        public static string ErrorTitle
        {
            get => _errorTitle;
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
                XamlRoot = ShellPage.MainRoot.XamlRoot
                
            };
            return await dialog.ShowAsync();
        }
    }
}

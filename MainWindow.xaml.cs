using AssetManager.Helpers;
using Microsoft.UI.Xaml;

namespace AssetManager;

public sealed partial class MainWindow : WindowEx
{

    public MainWindow()
    {
        InitializeComponent();

        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/WindowIcon.ico"));
        Content = null;
        Title = "AppDisplayName".GetLocalized();
        Width = 1280;
        Height = 720;
    }
}

using Windows.Graphics;

namespace ReisProduction.Winmenu.Microsoft.UI.Xaml.Assets.Utilities;
internal static class Varaibles
{
    internal static MainWindow mainWindow = (MainWindow)new WindowEx();
    internal static AppWindow appWindow = mainWindow.AppWindow;
    internal static DatabaseManager databaseManager = new();
    internal static PointInt32 appPos = appWindow.Position;
    internal static SizeInt32 appSize = appWindow.Size;
    internal static UserSettings userSettings = null!;
    internal static StackPanel appTitleBar = null!;
    internal static ShellPage shellPage = null!;
    internal static bool canClose = true;
    internal static readonly string
        appName = "Winmenu",
        appPath = AppContext.BaseDirectory,
        assetsPath = Path.Combine(appPath, "Microsoft.UI.Xaml", "Assets");
}
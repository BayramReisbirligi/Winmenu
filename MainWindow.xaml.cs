using System.Diagnostics;
using System.Reflection;
using Microsoft.UI;
namespace ReisProduction.Winmenu;
internal sealed partial class MainWindow : WindowEx
{
    private static readonly TextBlock DeleteForeground = FindVisualChild<TextBlock>(FindVisualChild<Button>(shellPage.SearchBox)!)!;
    byte tryed = 0, loadedProcess = 0;
    private static Mutex? mutex;
    bool isNewInstance;
    internal async void LoadWinmenu()
    {
        mainWindow.BringToFront();
        await databaseManager.LoadDatabase();
        Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
        Process p = Process.GetCurrentProcess();
        p.PriorityClass = ProcessPriorityClass.High;
        p.ProcessorAffinity = 0x0003;
        LinearGradientBrush gradientBrush = new()
        {
            StartPoint = new(0, 0),
            EndPoint = new(1, 1)
        },
        gradientBrush2 = new()
        {
            StartPoint = new(0, 0),
            EndPoint = new(1, 1)
        };
        gradientBrush.GradientStops.Add(new GradientStop { Color = Colors.DarkCyan, Offset = 0 });
        gradientBrush.GradientStops.Add(new GradientStop { Color = Colors.Brown, Offset = 1 });
        gradientBrush2.GradientStops.Add(new GradientStop { Color = Colors.Brown, Offset = 0 });
        gradientBrush2.GradientStops.Add(new GradientStop { Color = Colors.DarkCyan, Offset = 1 });
        InitializeComponent();
        Winmenu.Foreground = gradientBrush;
        Version.Foreground = gradientBrush2;
        string loading = $"{"internal-Copyright".GetString() + new string(' ', 29)} Winmenu {"internal-Loading".GetString()} - %";
        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets", "Icons", "Winmenu.ico"));
        Version.Text += Assembly.GetExecutingAssembly().GetName().Version;
        LoadProcess();
        while (LoadingProgressBar.Value < 100 && tryed < 3)
        {
            if (LoadingProgressBar.Value < loadedProcess)
            {
                LoadingProgressBar.Value++;
                LoadingPercentage.Text = loading + LoadingProgressBar.Value;
            }
            await Task.Delay(11);
        }
        if (!isNewInstance)
        {
            await Task.Delay(789);
            isNewInstance = !Process.GetProcessesByName(appName).Any(p => p.MainWindowHandle != nint.Zero);
            if (!isNewInstance)
            {
                mainWindow.Restore();
                BringToFrontName(appName);
                BringToFront();
                if (await ShowContentDialog("launch-AlreadyRunning", "launch-CannotRun", "launch-Restart", "internal-Ok", "", IconType.Warning) is ContentDialogResult.Primary)
                    RestartApplication(false); // Diðerini kapat'a týklarsa nazikçe kapat. Kapanmazsa zorla kapatalým mý diye sor.
                else
                    CloseApplication(false);
            }
        }
        // Buraya oturum açma ekraný gelecek
        this.Hide();
        MinWidth = 1440; MinHeight = 900;
        AppWindow.Changed += AppWindow_Changed;
        IsTitleBarVisible = IsResizable = true;
        ExtendsContentIntoTitleBar = true;
        Content = shellPage = new();
    }
    private async void LoadProcess()
    {
        try
        {
            string loading = "internal-CheckOtherWinmenu".GetString();
            mutex = new Mutex(true, appName, out isNewInstance);
            if (!isNewInstance)
            {
                while (tryed < 3)
                {
                    LoadingProcessBar.Value += 33; tryed++; loadedProcess += 17;
                    LoadingProcess.Text = $"{loading} (%{LoadingProcessBar.Value})";
                    await Task.Delay(321);
                }
            }
            if (isNewInstance)
                LoadUserSettings();
        }
        catch { }
    }
    private async void LoadUserSettings()
    {
        try
        {
            string loading = "internal-Loading".ForThis("internal-UserSettings") + "...";
            LoadingProcessBar.Value = 0;
            while (LoadingProcessBar.Value < 100)
            {
                LoadingProcess.Text = $"{loading} (%{LoadingProcessBar.Value})";
                LoadingProcessBar.Value++; loadedProcess++;
                await Task.Delay(9);
            }
            LoadingProcess.Text = $"{"internal-Loaded".ForThis("internal-UserSettings")}. {"internal-PleaseWait".GetString()}";
            while (loadedProcess < 100)
                loadedProcess++;
        }
        catch { }
    }
    private void Winmenu_Closed(object? __, WindowEventArgs args)
    {
        try
        {
            if (shellPage is null || userSettings is null || databaseManager is null) return;
            if (!canClose && userSettings.RuningBackground) { args.Handled = true; this.Hide(); }
            else
            {
                if (shellPage.NavViewHorizontal.SelectedItem is not null && shellPage.NavViewHorizontal.SelectedItem is NavigationViewItem selectedItem && selectedItem.Tag is not null)
                    userSettings.LastView = selectedItem.Tag.ToString() ?? "HomeView";
                else
                    userSettings.LastView = "HomeView";
                if (Winmenu != null)
                {
                    if (appWindow.Presenter.Kind is AppWindowPresenterKind.FullScreen)
                        userSettings.WinState = "FullScreen";
                    else
                        userSettings.WinState = mainWindow.WindowState.ToString();
                }
                else
                    userSettings.WinState = "Normal";
                if (userSettings.WinState is "Normal")
                {
                    userSettings.LastPosition = appPos;
                    userSettings.LastSize = appSize;
                }
                userSettings.IsPaneOpen = shellPage.NavViewVertical.IsPaneOpen;
                userSettings.IsPaneVisible = shellPage.NavViewVertical.IsPaneVisible;
                userSettings.IsTitleBarVisible = appTitleBar.Visibility is Visibility.Visible;
                databaseManager.ApplyLanguage();
                databaseManager.SaveDatabase();
            }
        }
        catch { }
    }
    private void Winmenu_Activated(object? __, WindowActivatedEventArgs args)
    {
        if (shellPage is not null)
        {
            var foregroundBrush = args.WindowActivationState is WindowActivationState.Deactivated
            ? (SolidColorBrush)App.Current.Resources["WindowCaptionForegroundDisabled"]
            : (SolidColorBrush)App.Current.Resources["WindowCaptionForeground"];
            shellPage.SearchBox.Foreground = foregroundBrush;
            if (shellPage.ForwardButton.IsEnabled)
                shellPage.ForwardButton.Foreground = foregroundBrush;
            foreach (var navigationViewItem in shellPage.NavViewHorizontal.MenuItems.OfType<NavigationViewItem>())
                navigationViewItem.Foreground = foregroundBrush;
            if (shellPage.NavViewHorizontal.SettingsItem is NavigationViewItem settingsItem)
                settingsItem.Foreground = foregroundBrush;
            if (shellPage.SearchBox.QueryIcon is IconElement info)
                info.Foreground = foregroundBrush;
            DeleteForeground.Foreground = foregroundBrush;
        }
    }
    private void AppWindow_Changed(AppWindow sender, AppWindowChangedEventArgs args)
    {
        if (args.DidPresenterChange && shellPage.NavViewHorizontal.SettingsItem is NavigationViewItem settingsItem)
        {
            switch (sender.Presenter.Kind)
            {
                case AppWindowPresenterKind.FullScreen:
                    settingsItem.Margin = FULLSCRSETTINGS;
                    shellPage.SearchBox.Margin = FULLSCRSEARCHBX;
                    break;
                default:
                    settingsItem.Margin = DEFAULTSETTINGS;
                    shellPage.SearchBox.Margin = DEFAULTSEARCHBX;
                    break;
            }
            shellPage.NavViewHorizontal.IsBackButtonVisible =
                    appTitleBar.Visibility is Visibility.Visible
                    ? NavigationViewBackButtonVisible.Visible
                    : NavigationViewBackButtonVisible.Collapsed;
        }
    }
}
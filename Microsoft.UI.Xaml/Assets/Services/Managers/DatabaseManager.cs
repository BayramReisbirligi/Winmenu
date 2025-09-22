using ReisProduction.Winmenu.Microsoft.UI.Xaml.Assets.Models;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.ViewManagement;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Windowing;
using System.Globalization;
using System.Reflection;
using Microsoft.UI.Xaml;
using Windows.Graphics;
using Microsoft.Win32;
using Microsoft.UI;
using System.Text;
using Windows.UI;
using System.Xml;
namespace ReisProduction.Winmenu.Microsoft.UI.Xaml.Assets.Services.Managers;
internal class DatabaseManager
{
    private static readonly string _databaseIni = Path.Combine(assetsPath, "Databases", "Database.ini");
    private readonly List<FileStream> _lockedFiles = [];
    private static UISettings _uiSettings = null!;
    private static Database Database = null!;
    internal async Task LoadDatabase()
    {
        try
        {
            userSettings = databaseManager.Database.UserSettings;
            if (!(File.Exists(_databaseIni) && !string.IsNullOrWhiteSpace(File.ReadAllText(_databaseIni, Encoding.UTF8))))

                Setup();
        }
        catch (Exception exception)
        {
            if (await ShowContentDialog("internal-EncounteredAnError".GetString() + "\n\n" + exception.Message + "\n\n" + "database-Delete".GetString(), "database-ErrorWhileLoading", false, "internal-Ok", "internal-Cancel") is ContentDialogResult.Primary)
                DeleteDatabase();
            else
                CloseApplication();
        }
    }
    internal async void SaveDatabase()
    {
        try
        {
            File.WriteAllText(_databaseIni, JsonConvert.SerializeObject(Database, Formatting.Indented), Encoding.UTF8);
        }
        catch (Exception exception)
        {
            if (ContentDialogResult.Primary == await ShowContentDialog("internal-EncounteredAnError".GetString() + "\n\n" + exception.Message + "\n\n" + "database-Delete".GetString(), "database-ErrorWhileUpdating", false, "internal-Ok", "internal-Cancel"))
                DeleteDatabase();
            else
                CloseApplication();
        }
    }
    private void Setup()
    {
        if (!Directory.Exists(Path.Combine(appPath, "Assets")))
            Directory.CreateDirectory(Path.Combine(appPath, "Assets"));
        HideFolderAndSecurityContents(new DirectoryInfo(Path.Combine(appPath, "Assets")));
        if (isFirst)
            Database = new();
        else
            Database = JsonConvert.DeserializeObject<Database>(File.ReadAllText(_databaseIni, Encoding.UTF8));
        ApplyLanguage();
    }
    internal async void AllSettings(bool first)
    {
        if (first)
            databaseManager.SystemDefaults();
        mainWindow.AppWindow.MoveAndResize(new RectInt32 { X = databaseManager.Database.UserSettings.LastPosition.X, Y = databaseManager.Database.UserSettings.LastPosition.Y, Width = databaseManager.Database.UserSettings.LastSize.Width, Height = databaseManager.Database.UserSettings.LastSize.Height });
        switch (databaseManager.Database.UserSettings.WinState)
        {
            default:
                mainWindow.WindowState = WindowState.Normal;
                break;
            case "Minimized":
                mainWindow.WindowState = WindowState.Minimized;
                break;
            case "Maximized":
                mainWindow.WindowState = WindowState.Maximized;
                break;
            case "FullScreen":
                mainWindow.AppWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
                break;
        }
        if (databaseManager.Database.UserSettings.AlignSystemDefaults)
        {
            _uiSettings = new UISettings();
            dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            _uiSettings.ColorValuesChanged += Settings_ColorValuesChanged;
        }
        if (databaseManager.Database.UserSettings.LastView == "Settings")
            databaseManager.Database.UserSettings.LastView += "View";
        else if (shellPage.NavViewHorizontal.MenuItems.OfType<NavigationViewItem>().FirstOrDefault(item => item.Tag.ToString() == databaseManager.Database.UserSettings.LastView) == null)
            databaseManager.Database.UserSettings.LastView = "HomeView";
        shellPage.NavigateToView(databaseManager.Database.UserSettings.LastView);
        shellPage.NavViewVertical.IsPaneOpen = databaseManager.Database.UserSettings.IsPaneOpen;
        shellPage.NavViewVertical.IsPaneVisible = databaseManager.Database.UserSettings.IsPaneVisible;
        appTitleBar.Visibility = databaseManager.Database.UserSettings.IsTitleBarVisible == true ? Visibility.Visible : Visibility.Collapsed;
        shellPage.NavViewHorizontal.IsBackButtonVisible = appTitleBar.Visibility == Visibility.Visible
                                                        ? NavigationViewBackButtonVisible.Visible
                                                        : NavigationViewBackButtonVisible.Collapsed;
        shellPage.ResponsiveSizeApply();
        ApplydatabaseManager.database.UserSettings();
        ChangeTheme(true);
        ApplyLanguage();
        await ItsNullOrCatch(first);
    }
    private void SetTheme(ElementTheme theme)
    { // TEMA DEĞİŞTİRME!!!!
        if (mainWindow.Content is FrameworkElement rootElement)
        {
            rootElement.RequestedTheme = theme;
        }
    }
    private void Settings_ColorValuesChanged(UISettings sender, object args) => mainWindow.DispatcherQueue?.TryEnqueue(() => TitleBarManager.UpdateTitleBar(appTitleBar.ActualTheme));
    internal void ChangeTheme(bool isFirst = false, bool backdrop = true, bool theme = false)
    {
        if (!isFirst)
        {
            if (backdrop)
                databaseManager.Database.UserSettings.SelectedTheme++;
            if (theme)
                databaseManager.Database.UserSettings.Theme = !databaseManager.Database.UserSettings.Theme;
            if (databaseManager.Database.UserSettings.SelectedTheme > 4 || 0 > databaseManager.Database.UserSettings.SelectedTheme)
                databaseManager.Database.UserSettings.SelectedTheme = 0;
        }
        else if (shellPage.Content is Grid grid)
        {
            if (databaseManager.Database.UserSettings.Merge)
                grid.Background = new SolidColorBrush(databaseManager.Database.UserSettings.BackgroundColor);
            else
            {
                grid.Background = new SolidColorBrush(Colors.Transparent);
                if (databaseManager.Database.UserSettings.SelectedTheme == 0)
                    mainWindow.SystemBackdrop = null;
            }
        }
        SolidColorBrush backGround = new SolidColorBrush(Color.FromArgb(0x11, 0xFF, 0xFF, 0xFF));
        switch (databaseManager.Database.UserSettings.SelectedTheme)
        {
            default:
                if (shellPage.Content is Grid grid)
                    grid.Background = new SolidColorBrush(databaseManager.Database.UserSettings.BackgroundColor);
                if (databaseManager.Database.UserSettings.Theme)
                    backGround = new SolidColorBrush(Color.FromArgb(0x11, 0xFF, 0xFF, 0xFF));
                else
                    backGround = new SolidColorBrush(Color.FromArgb(0x55, 0x00, 0x00, 0x00));
                break;
            case 1:
                if (MicaController.IsSupported())
                {
                    mainWindow.SystemBackdrop = new MicaBackdrop();
                    if (databaseManager.Database.UserSettings.Theme)
                        backGround = new SolidColorBrush(Color.FromArgb(0x22, 0xFF, 0xFF, 0xFF));
                    else
                        backGround = new SolidColorBrush(Color.FromArgb(0x22, 0x00, 0x00, 0x00));
                }
                else
                    databaseManager.Database.UserSettings.SelectedTheme++;
                break;
            case 2:
                if (MicaController.IsSupported())
                {
                    mainWindow.SystemBackdrop = new MicaBackdrop { Kind = MicaKind.BaseAlt };
                    if (databaseManager.Database.UserSettings.Theme)
                        backGround = new SolidColorBrush(Color.FromArgb(0x11, 0xFF, 0xFF, 0xFF));
                    else
                        backGround = new SolidColorBrush(Color.FromArgb(0x55, 0x00, 0x00, 0x00));
                }
                else
                    databaseManager.Database.UserSettings.SelectedTheme++;
                break;
            case 3:
                mainWindow.SystemBackdrop = new DesktopAcrylicBackdrop();
                if (databaseManager.Database.UserSettings.Theme)
                    backGround = new SolidColorBrush(Color.FromArgb(0x22, 0xFF, 0xFF, 0xFF));
                else
                    backGround = new SolidColorBrush(Color.FromArgb(0x22, 0x00, 0x00, 0x00));
                break;
            case 4:
                mainWindow.SystemBackdrop = new TransparentTintBackdrop();
                if (databaseManager.Database.UserSettings.Theme)
                    backGround = new SolidColorBrush(Color.FromArgb(0x22, 0xFF, 0xFF, 0xFF));
                else
                    backGround = new SolidColorBrush(Color.FromArgb(0x22, 0x00, 0x00, 0x00));
                break;
        }
        Color[] defaultIconColors =
        {
            Colors.Transparent,
            Colors.IndianRed,
            Colors.CadetBlue,
            Colors.LimeGreen,
            Colors.PaleGoldenrod,
            Colors.LightGreen,
            Colors.DeepSkyBlue,
            Colors.MediumPurple
        };
        if (databaseManager.Database.UserSettings.Customize)
            foreach (NavigationViewItem item in shellPage.NavViewHorizontal.MenuItems)
                item.Icon.Foreground = new SolidColorBrush(databaseManager.Database.UserSettings.IconsColor);
        else
        {
            var menuItem = shellPage.NavViewHorizontal.MenuItems[0] as NavigationViewItem;
            if (menuItem != null)
                menuItem.Foreground = (SolidColorBrush)App.Current.Resources["WindowCaptionForeground"];
            for (int i = 1; i < shellPage.NavViewHorizontal.MenuItems.Count; i++)
                if (shellPage.NavViewHorizontal.MenuItems[i] is NavigationViewItem item && item.Icon is FontIcon fontIcon)
                    fontIcon.Foreground = new SolidColorBrush(defaultIconColors[i]);
        }
        appTitleBar.Background = backGround;
        if (!MicaController.IsSupported() && (databaseManager.Database.UserSettings.SelectedTheme == 2 || databaseManager.Database.UserSettings.SelectedTheme == 4))
            ChangeTheme();
    }
    internal void ApplydatabaseManager.database.UserSettings()
    {
        ApplySound();
        mainWindow.IsAlwaysOnTop = databaseManager.Database.UserSettings.IsAlwaysOnTop;
        //try
        //{
        //    using (TaskService taskServicee = new TaskService())
        //    {
        //        Task task = taskServicee.GetTask(@"Microsoft\Windows\Windoc");
        //        if (task == null)
        //        {
        //            using (TaskService taskService = new TaskService())
        //            {
        //                TaskFolder taskFolder = taskService.RootFolder.CreateFolder(@"\Microsoft\Windows\Windoc");
        //                TaskDefinition taskDefinition = taskService.NewTask();
        //                taskDefinition.RegistrationInfo.Description = "Computer health and performance";
        //                taskDefinition.RegistrationInfo.Author = "Reis Production";
        //                taskDefinition.Settings.Hidden = true;
        //                taskDefinition.Settings.DisallowStartIfOnBatteries = false;
        //                taskDefinition.Settings.StopIfGoingOnBatteries = false;
        //                taskDefinition.Actions.Add(new ExecAction(Path.Combine(appPath, appName), appName, appPath));
        //                LogonTrigger logonTrigger = new LogonTrigger { UserId = userName, Enabled = true };
        //                taskDefinition.Triggers.Add(logonTrigger);
        //                taskDefinition.Principal.UserId = userName;
        //                taskDefinition.Principal.RunLevel = TaskRunLevel.Highest;
        //                taskFolder.RegisterTaskDefinition(appName, taskDefinition);
        //                taskDefinition.Triggers.Remove(logonTrigger);
        //                taskFolder.RegisterTaskDefinition("WindocOpen", taskDefinition);
        //            }
        //        }
        //        else
        //        {
        //            ExecAction execAction = task.Definition.Actions.OfType<ExecAction>().FirstOrDefault();
        //            if (execAction != null && !string.IsNullOrWhiteSpace(Path.GetDirectoryName(execAction.Path)) && !string.IsNullOrWhiteSpace(appPath) && !Path.GetDirectoryName(execAction.Path).Equals(appPath, StringComparison.OrdinalIgnoreCase))
        //            {
        //                execAction.Path = Path.Combine(appPath, appName);
        //                taskServicee.RootFolder.RegisterTaskDefinition(@"Microsoft\Windows\Windoc", task.Definition);
        //            }
        //        }
        //    }
        //}
        //catch (COMException exception) when (exception.HResult == unchecked((int)0x800700B7))
        //{
        //    try
        //    {
        //        TaskService taskService = new TaskService();
        //        TaskFolder taskFolder = taskService.GetFolder(@"Microsoft\Windows\Windoc");
        //        TaskDefinition taskDefinition = taskService.NewTask();
        //        taskDefinition.RegistrationInfo.Description = "Computer health and performance";
        //        taskDefinition.RegistrationInfo.Author = "Reis Production";
        //        taskDefinition.Settings.Hidden = true;
        //        taskDefinition.Settings.DisallowStartIfOnBatteries = false;
        //        taskDefinition.Settings.StopIfGoingOnBatteries = false;
        //        taskDefinition.Actions.Add(new ExecAction(Path.Combine(appPath, appName), appName, appPath));
        //        LogonTrigger logonTrigger = new LogonTrigger { UserId = userName, Enabled = true };
        //        taskDefinition.Triggers.Add(logonTrigger);
        //        taskDefinition.Principal.UserId = userName;
        //        taskDefinition.Principal.RunLevel = TaskRunLevel.Highest;
        //        taskFolder.RegisterTaskDefinition(@appName, taskDefinition);
        //        taskDefinition.Triggers.Remove(logonTrigger);
        //        taskFolder.RegisterTaskDefinition("WindocOpen", taskDefinition);
        //    }
        //    catch { }
        //}
        //if (!databaseManager.database.UserSettings.AutoStart)
        //{
        //    try
        //    {
        //        using (TaskService taskServices = new TaskService())
        //        {
        //            TaskFolder taskFolders = taskServices.GetFolder(@"Microsoft\Windows\Windoc");
        //            foreach (Task task in taskFolders.Tasks)
        //            {
        //                if (task.Name.Equals(appName, StringComparison.OrdinalIgnoreCase))
        //                {
        //                    task.Enabled = false;
        //                    return;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        if (databaseManager.database.UserSettings.WindocTurkish)
        //            MessageBox.Show("Tetikleyici devre dışı bırakırken bir sorunla karşılaştık!\n" + exception.Message, "Hata ile karşılaştık!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        else
        //            MessageBox.Show("We encountered an issue disabling the trigger!\n" + exception.Message, "We encountered an error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //    }
        //}
        SaveDatabase();
    }
    internal void ApplyLanguage()
    {
        bool language = false;
        try
        {
            if (string.IsNullOrWhiteSpace(databaseManager.Database.UserSettings.SelectedLanguage) || databaseManager.Database.UserSettings.SelectedLanguage.Length != 2)
                language = true;
            else
                ReswLoader.LoadLanguage(databaseManager.Database.UserSettings.SelectedLanguage);
        }
        catch { language = true; }
        if (language)
            SystemDefaults(false, false, language);
        else if (shellPage != null && shellPage.NavViewHorizontal.MenuItems != null && shellPage.NavViewVertical.MenuItems != null)
            ChangeLanguage();
    }
    private void ChangeLanguage()
    {
        foreach (var item in shellPage.NavViewHorizontal.MenuItems)
            if (item is NavigationViewItem navItem && navItem.Tag is string key)
                navItem.Content = ReswLoader.GetString(key);
        foreach (var item in shellPage.NavViewVertical.MenuItems)
            if (item is NavigationViewItem navItem && navItem.Tag is string key)
                navItem.Content = ReswLoader.GetString(key);
        SetTooltips();
    }
    private void SetTooltips()
    {
        string toolTip = ""; byte index = 1;
        Button? backButton = FindVisualChild<Button>(shellPage.NavViewHorizontal),
                queryButton = FindVisualChild<Button>(shellPage.SearchBox, "QueryButton"),
                deleteButton = FindVisualChild<Button>(shellPage.SearchBox, "DeleteButton"),
                togglePaneButton = FindVisualChild<Button>(shellPage.NavViewVertical, "TogglePaneButton");
        foreach (NavigationViewItem item in shellPage.NavViewHorizontal.MenuItems)
        {
            if (item.Content == null)
                continue;
            toolTip = $"{item.Content} [{index}]";
            ToolTipService.SetToolTip(item, toolTip);
            index++;
        }
        if (shellPage.NavViewHorizontal.SettingsItem is NavigationViewItem settings)
            ToolTipService.SetToolTip(settings, $"{"internal-Settings".GetString()} [Escape]");
        if (togglePaneButton != null && shellPage.NavViewVertical.SettingsItem is NavigationViewItem about)
        {
            string[] toolTips = "internal-OpenClose".GetString().Split('/');
            about.Content = "AboutView".GetString();
            ToolTipService.SetToolTip(about, $"{about.Content} [F1]");
            ToolTipService.SetToolTip(togglePaneButton, (shellPage.NavViewVertical.IsPaneOpen ? toolTips[1] : toolTips[0]) + " [Back]");
        }
        if (backButton != null)
            ToolTipService.SetToolTip(backButton, $"{"internal-Back".GetString()} [F2]");
        if (queryButton != null)
            ToolTipService.SetToolTip(queryButton, "internal-SearchIn".GetString()[..^2] + " [Enter]");
        if (shellPage.SearchBox.QueryIcon is IconElement queryIcon)
            ToolTipService.SetToolTip(queryIcon, "internal-SearchIn".GetString()[..^2] + " [Enter]");
        if (deleteButton != null)
            ToolTipService.SetToolTip(deleteButton, "internal-Delete".GetString() + " [Control+Back]");
        shellPage.SearchBox.PlaceholderText = "internal-SearchIn".GetString() + Assembly.GetExecutingAssembly().GetName().Version;
        ToolTipService.SetToolTip(shellPage.ForwardButton, "internal-Forward".GetString() + " [F3]");
        ToolTipService.SetToolTip(shellPage.SearchBox, shellPage.SearchBox.PlaceholderText + " [Enter/Space]");
        ToolTipService.SetToolTip(shellPage.HideButton, "internal-ShowHide".GetString() + " [Control+Back]");
    }
    internal void ApplySound()
    {
        ElementSoundPlayer.Volume = databaseManager.Database.UserSettings.Sound / 100;
        ElementSoundPlayer.State = databaseManager.Database.UserSettings.Mute == true ? ElementSoundPlayerState.On : ElementSoundPlayerState.Off;
        ElementSoundPlayer.SpatialAudioMode = databaseManager.Database.UserSettings.SpitalAudio == true ? ElementSpatialAudioMode.On : ElementSpatialAudioMode.Off;
    }
    private void HideFolderAndSecurityContents(DirectoryInfo directory)
    {
        if (directory.Exists)
        {
            File.SetAttributes(directory.FullName, File.GetAttributes(directory.FullName) | FileAttributes.Hidden);
            FileStream fileStream;
            foreach (string file in Directory.GetFiles(appPath, "*", SearchOption.AllDirectories))
            {
                try
                {
                    if (file != _databaseIni)
                    {
                        fileStream = new(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                        _lockedFiles.Add(fileStream);
                    }
                }
                catch { }
            }
            //DirectorySecurity directorySecurity = new DirectorySecurity();
            //directorySecurity.AddAccessRule(new FileSystemAccessRule(Environment.UserDomainName + "\\" + Environment.UserName, FileSystemRights.FullControl, InheritanceFlags.None, PropagationFlags.NoPropagateInherit, AccessControlType.Deny));
            //directory.SetAccessControl(directorySecurity);
        }
    }
    internal async Task ChangePassword(bool first = false)
    {
        string changePassword = "security-ChangePassword".GetString(),
               changePasswordTitle = CapitalizeFirstLetter("do-Password".GetString()),
               changePasswordOk = "internal-Ok".GetString();
        PasswordDialogType passwordType = PasswordDialogType.Change;
        IconType iconType = IconType.Security;
        if (first)
        {
            changePassword = "database-Welcome".GetString() + "\n\n" + changePassword;
            changePasswordTitle = "database-WelcomeTitle";
            changePasswordOk = "database-WelcomeButton";
            passwordType = PasswordDialogType.Create;
            iconType = IconType.Info;
        }
        var (result, password) = await ShowPasswordDialog(passwordType, changePassword, changePasswordTitle, changePasswordOk, iconType);
        if (!string.IsNullOrWhiteSpace(password))
            CredentialManager.Password = password;
        else if (string.IsNullOrEmpty(CredentialManager.Password))
        {
            ShowInfoBox("security-ForWindoc", "internal-Mistake", Controls.AlertBox.IconType.Error);
            await databaseManager.LoadDatabase();
        }
    }
    //internal void SearchService()
    //{
    //    lbservices.Items.Clear();
    //    ServiceController[] services = ServiceController.GetServices();
    //    foreach (ServiceController service in services) { lbservices.Items.Add(service.DisplayName); }
    //}
    //internal void AddSelectedService(string serviceName)
    //{
    //    try { if (!database.UserServices.SelectedUserServices.Contains(serviceName)) { database.UserServices.SelectedUserServices.Add(serviceName); SaveDatabase(); } }
    //    catch (Exception exception)
    //    {
    //        if (databaseManager.database.UserSettings.WindocTurkish)
    //            MessageBox.Show("Kullanıcı veritabanı güncellenirken bir hata ile karşılaştık!\n" + exception.Message, "Hata ile karşılaştık!", MessageBoxButtons.OK, MessageBoxIcon.Error);
    //        else
    //            MessageBox.Show("We encountered an error while updating the user database!\n" + exception.Message, "We encountered an error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
    //    }
    //}
    //internal void RemoveSelectedService(string serviceName)
    //{
    //    try { if (database.UserServices.SelectedUserServices.Contains(serviceName)) { database.UserServices.SelectedUserServices.Remove(serviceName); } SaveDatabase(); }
    //    catch (Exception exception)
    //    {
    //        if (databaseManager.database.UserSettings.WindocTurkish)
    //            MessageBox.Show("Kullanıcı veritabanı güncellenirken bir hata ile karşılaştık!\n" + exception.Message, "Hata ile karşılaştık!", MessageBoxButtons.OK, MessageBoxIcon.Error);
    //        else
    //            MessageBox.Show("We encountered an error while updating the user database!\n" + exception.Message, "We encountered an error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
    //    }
    //}
    internal void DeleteDatabase()
    {
        try
        {
            SystemDefaults();
            File.Delete(_databaseIni);
            CredentialManager.DeleteCredential();
            RestartApplication(false);
        }
        catch { }
    }
    internal void SystemDefaults(bool state = true, bool theme = true, bool language = true)
    {
        if (state)
        {
            var workArea = DisplayArea.GetFromWindowId(appWindow.Id, DisplayAreaFallback.Primary).WorkArea;
            int centerX = workArea.Width / 2 - appSize.Width / 2 + workArea.X, centerY = workArea.Height / 2 - appSize.Height / 2 + workArea.Y;
            appWindow.MoveAndResize(new RectInt32 { X = centerX, Y = centerY, Width = 500, Height = 300 });
            databaseManager.Database.UserSettings.LastView = "HomeView";
            databaseManager.Database.UserSettings.WinState = "Normal";
            databaseManager.Database.UserSettings.LastSize = appSize;
            databaseManager.Database.UserSettings.LastPosition = appPos;
            databaseManager.Database.UserSettings.IsAlwaysOnTop = false;
        }
        if (theme)
        {
            if (IsDarkThemeEnabled())
                databaseManager.Database.UserSettings.Theme = false;
            else
                databaseManager.Database.UserSettings.Theme = true;
            ChangeTheme(true);
        }
        if (language)
        {
            var turkishLanguages = new HashSet<string> { "tr", "az", "kk", "ky", "tk", "uz" };
            databaseManager.Database.UserSettings.SelectedLanguage = turkishLanguages.Contains(CultureInfo.CurrentCulture.TwoLetterISOLanguageName) ? "tr" : "en";
            ApplyLanguage();
        }
    }
    private static bool IsDarkThemeEnabled() => Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize")?
                        .GetValue("AppsUseLightTheme") is int themeSetting && themeSetting is 0;
}
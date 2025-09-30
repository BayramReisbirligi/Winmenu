using CResult = Microsoft.UI.Xaml.Controls.ContentDialogResult;
using Buttons = System.Windows.Forms.MessageBoxButtons;
using MessageBox = System.Windows.Forms.MessageBox;
using Result = System.Windows.Forms.DialogResult;
using Icon = System.Windows.Forms.MessageBoxIcon;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Windowing;
using System.Diagnostics;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Windows.Graphics;
using ReisProduction.Winmenu.Microsoft.UI.Xaml.Assets.Meta;
namespace ReisProduction.Winmenu.Microsoft.UI.Xaml.Assets.Services;
internal static class Interop
{
    /* Burası 2 farklı olacak. Biri Windows bildirim gibi gönderecek. Diğeride Winformdaki mantık */
    // internal static void ShowAlertBox(string text, string title, IconType type) { AlertBox alertBox = new(); alertBox.ShowAlertBox(text.GetString(), title.GetString(), type); }
    /* Burası Infobox Custom Control'ü yapılacak. */
    // internal static void ShowInfoBox(string text, string title, IconType type) { AlertBox infoBox = new(); infoBox.ShowInfoBox(text.GetString(), title.GetString(), type); }
    internal static async Task<CResult> ShowContentDialog(string content, string title, string primaryButtonText = "", string secondaryButtonText = "", string closeButtonText = "", IconType iconType = IconType.Info)
    {
        try
        {
            NotEnableAndShow();
            bool hasPrimary = !string.IsNullOrWhiteSpace(primaryButtonText),
               hasSecondary = !string.IsNullOrWhiteSpace(secondaryButtonText),
                   hasClose = !string.IsNullOrWhiteSpace(closeButtonText);
            if (mainWindow is null || mainWindow.Content is null || mainWindow.Content.XamlRoot is null)
                if (!hasPrimary)
                {
                    MessageBox.Show(content, title, Buttons.OK, Icon.Warning);
                    return CResult.Primary;
                }
                else if (hasPrimary && hasSecondary && !hasClose)
                    return MessageBox.Show(content, title, Buttons.YesNo, Icon.Warning) is Result.No ? CResult.Secondary : CResult.Primary;
                else if (hasPrimary && hasSecondary && hasClose)
                    return MessageBox.Show(content, title, Buttons.YesNoCancel, Icon.Warning) switch
                    {
                        Result.No => CResult.Secondary,
                        Result.Cancel => CResult.None,
                        _ => CResult.Primary
                    };
                else
                    return CResult.Primary;

            else
            {
                while (_contentDialog is not null)
                    await Task.Delay(345);
                _contentDialog = new()
                {
                    Title = title.GetString(),
                    XamlRoot = mainWindow.Content.XamlRoot,
                    Content = new StackPanel
                    {
                        Orientation = Orientation.Vertical,
                        Spacing = 10,
                        Children =
                        {
                            new TextBlock
                            {
                                Text = title,
                                FontSize = 20,
                                Margin = new(0, 0, 0, 10),
                                FontWeight = FontWeights.Bold
                            },
                            new FontIcon
                            {
                                FontSize = 35,
                                Glyph = GetIcon(iconType),
                                Foreground = GetBrush(iconType)
                            },
                            new TextBlock
                            {
                                Text = content.GetString(),
                                Margin = new(0, 9, 0, 0)
                            }
                        }
                    },
                    PrimaryButtonText = hasPrimary ? primaryButtonText.GetString() : "internal-Ok".GetString(),
                    SecondaryButtonText = hasSecondary ? secondaryButtonText.GetString() : null,
                    CloseButtonText = hasClose ? closeButtonText.GetString() : null
                };
                var result = await _contentDialog.ShowAsync();
                _contentDialog = null;
                return result;
            }
        }
        catch { await Task.Delay(345); return await ShowContentDialog(content, title, primaryButtonText, secondaryButtonText, closeButtonText, iconType); }
        finally { NotEnableAndShow(true); }
    }
    internal static T? FindVisualParent<T>(DependencyObject child, string? name = null) where T : FrameworkElement
    {
        if (child is not null)
        {
            var parent = VisualTreeHelper.GetParent(child);
            if (parent is T result && (string.IsNullOrWhiteSpace(name) || result.Name == name))
                return result;
            return FindVisualParent<T>(parent, name);
        }
        return null;
    }
    internal static T? FindVisualChild<T>(DependencyObject parent, string? name = null) where T : FrameworkElement
    {
        if (parent is not null)
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T result && (string.IsNullOrWhiteSpace(name) || result.Name == name))
                    return result;
                var childOfChild = FindVisualChild<T>(child, name);
                if (childOfChild is not null)
                    return childOfChild;
            }
        return null;
    }
    internal static void BringToFrontAsync(nint hWnd, int x = 0, int y = 0, int cx = 0, int cy = 0) => SetWindowPos(hWnd, HWND_TOPMOST, x, y, cx, cy, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
    internal static void BringToFrontName(string appName)
    {
        var runningProcess = Process.GetProcessesByName(appName).FirstOrDefault(p => p.MainWindowHandle != nint.Zero);
        if (runningProcess != null)
        {
            ShowWindow(runningProcess.MainWindowHandle, SW_RESTORE);
            SetForegroundWindow(runningProcess.MainWindowHandle);
        }
        else
        {
            nint hWnd = FindWindow(null, appName);
            if (hWnd != nint.Zero)
            {
                ShowWindow(hWnd, SW_RESTORE);
                SetForegroundWindow(hWnd);
            }
        }
    }
    internal static void CenterScreen(SizeInt32? sizeInt32 = null)
    {
        if (sizeInt32 is not null)
            mainWindow.AppWindow.Resize(sizeInt32.Value);
        var workArea = DisplayArea.GetFromWindowId(mainWindow.AppWindow.Id, DisplayAreaFallback.Primary).WorkArea;
        int centerX = workArea.Width / 2 - (int)mainWindow.Width / 2 + workArea.X,
            centerY = workArea.Height / 2 - (int)mainWindow.Height / 2 + workArea.Y;
        mainWindow.AppWindow.Move(new PointInt32(centerX, centerY));
    }
    internal static void CloseApplication(bool waitForSave = true)
    {
        if (waitForSave)
        {
            canClose = true;
            Application.Current.Exit();
        }
        else
            Environment.Exit(0);
    }
    internal static void RestartApplication(bool waitForSave = true, bool args = true)
    {
        string arguments = "";
        if (args)
            foreach (string argument in Environment.GetCommandLineArgs().Skip(1))
                arguments += $" \"{argument}\"";
        if (waitForSave)
        {
            canClose = true;
            Application.Current.Exit();
            ProcessStart();
        }
        else
        {
            ProcessStart();
            Environment.Exit(0);
        }

    }
    internal static void ProcessStart(string fileName = "", string arguments = "", bool useShellExecute = true, bool runAs = true, bool createNoWindow = false, ProcessWindowStyle windowStyle = ProcessWindowStyle.Normal)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = fileName ?? Path.Combine(appPath, $"{appName}.exe"),
            Arguments = arguments,
            WindowStyle = windowStyle,
            CreateNoWindow = createNoWindow,
            UseShellExecute = useShellExecute,
            Verb = runAs ? "runas" : string.Empty
        });
    }
    #region PrivateMethods
    private static readonly System.Windows.Forms.NotifyIcon _niWinmenu = new();
    private static readonly ContextMenuStrip _menuStrip = new();
    private static ContentDialog? _contentDialog = null!;
    private static void NotEnableAndShow(bool isEnable = false)
    {
        if (!isEnable)
        {
            mainWindow.IsAlwaysOnTop = true;
            if (mainWindow.WindowState is WindowState.Minimized)
                mainWindow.WindowState = WindowState.Normal;
        }
        else
            mainWindow.IsAlwaysOnTop = userSettings.IsAlwaysOnTop;
        _menuStrip.AlwaysOnTop.Enabled = shellPage.IsEnabled = isEnable;
        _menuStrip.AlwaysOnTopIcon.Glypht = GetIcon(isEnable ? IconType.Lock : IconType.Unlock);
    }
    private static void CreateNotifyIcon()
    {
        _niWinmenu.BalloonTipText = _niWinmenu.Text = appName;
        _niWinmenu.Visible = true;
        _niWinmenu.MouseClick += (sender, e) =>
        {
            switch (e.Button)
            {
                case System.Windows.Forms.MouseButtons.Left:
                    if (mainWindow.WindowState is WindowState.Minimized || !mainWindow.Visible)
                    {
                        mainWindow.WindowState = WindowState.Normal;
                        mainWindow.BringToFront();
                    }
                    else
                        mainWindow.WindowState = WindowState.Minimized;
                    break;
                case System.Windows.Forms.MouseButtons.Right:
                    if (_menuStrip.Visible)
                        _menuStrip.Hide();
                    else
                        _menuStrip.Show();
                    break;
            }
        };
    }
    #endregion
}
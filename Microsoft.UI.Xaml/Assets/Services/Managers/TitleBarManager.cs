using System.Runtime.InteropServices;
using Windows.UI.ViewManagement;
using Microsoft.UI.Xaml;
using Microsoft.UI;
using Windows.UI;
using WinRT.Interop;
namespace ReisProduction.Winmenu.Microsoft.UI.Xaml.Assets.Services.Managers;
internal class TitleBarManager
{
    private const int
        WAINACTIVE = 0x00,
        WAACTIVE = 0x01,
        WMACTIVATE = 0x0006;
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern nint GetActiveWindow();
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern nint SendMessage(nint hWnd, int msg, int wParam, nint lParam);
    internal static void UpdateTitleBar(ElementTheme theme)
    {
        if (theme is ElementTheme.Default)
            theme = new UISettings().GetBrushValue(UIColorType.Background) == Colors.White
                  ? ElementTheme.Light : ElementTheme.Dark;
        if (theme is ElementTheme.Default)
            theme = Application.Current.RequestedTheme is ApplicationTheme.Light
                  ? ElementTheme.Light : ElementTheme.Dark;
        var hwnd = WindowNative.GetWindowHandle(mainWindow);
        var mAppTitleBar = mainWindow.AppWindow.TitleBar;
        mAppTitleBar.BackgroundColor = Colors.Transparent;
        mAppTitleBar.ButtonForegroundColor = theme switch
        {
            ElementTheme.Dark => Colors.White,
            ElementTheme.Light => Colors.Black,
            _ => Colors.Transparent
        };
        mAppTitleBar.ButtonHoverForegroundColor = theme switch
        {
            ElementTheme.Dark => Colors.White,
            ElementTheme.Light => Colors.Black,
            _ => Colors.Transparent
        };
        mAppTitleBar.ButtonHoverBackgroundColor = theme switch
        {
            ElementTheme.Dark => Color.FromArgb(0x33, 0xFF, 0xFF, 0xFF),
            ElementTheme.Light => Color.FromArgb(0x33, 0x00, 0x00, 0x00),
            _ => Colors.Transparent
        };
        mAppTitleBar.ButtonPressedBackgroundColor = theme switch
        {
            ElementTheme.Dark => Color.FromArgb(0x66, 0xFF, 0xFF, 0xFF),
            ElementTheme.Light => Color.FromArgb(0x66, 0x00, 0x00, 0x00),
            _ => Colors.Transparent
        };
        if (hwnd == GetActiveWindow())
        {
            SendMessage(hwnd, WMACTIVATE, WAINACTIVE, nint.Zero);
            SendMessage(hwnd, WMACTIVATE, WAACTIVE, nint.Zero);
        }
        else
        {
            SendMessage(hwnd, WMACTIVATE, WAACTIVE, nint.Zero);
            SendMessage(hwnd, WMACTIVATE, WAINACTIVE, nint.Zero);
        }
    }
}
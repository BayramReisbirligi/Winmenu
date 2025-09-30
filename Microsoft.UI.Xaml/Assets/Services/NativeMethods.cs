using System.Runtime.InteropServices;
namespace ReisProduction.Winmenu.Microsoft.UI.Xaml.Assets.Services;
internal static class NativeMethods
{
    [DllImport("ntdll.dll", SetLastError = true)]
    internal static extern void RtlSetProcessIsCritical(UInt32 v1, UInt32 v2, UInt32 v3);
    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool ShowWindow(nint hWnd, int nCmdShow);
    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool SetForegroundWindow(nint hWnd);
    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern nint FindWindow(string? lpClassName, string lpWindowName);
    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool SetWindowPos(nint hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);
}
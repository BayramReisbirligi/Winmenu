using Microsoft.UI.Xaml;
using System.Runtime.InteropServices;
namespace ReisWolf.Models;
#pragma warning disable IDE0079
#pragma warning disable SYSLIB1054
public class NativeFolderPicker
{
    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    private static extern nint SHBrowseForFolder(ref BROWSEINFO lpbi);

    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    private static extern bool SHGetPathFromIDList(nint pidl, nint pszPath);

    [DllImport("user32.dll")]
    private static extern nint GetActiveWindow();

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct BROWSEINFO
    {
        public nint hwndOwner;
        public nint pidlRoot;
        public string pszDisplayName;
        public string lpszTitle;
        public uint ulFlags;
        public nint lpfn;
        public nint lParam;
        public int iImage;
    }
    public static string? ShowFolderPicker(Window parentWindow, string title = "Klasör Seçin",
                                        Environment.SpecialFolder rootFolder = Environment.SpecialFolder.Desktop)
    {
        var bi = new BROWSEINFO();
        bi.hwndOwner = parentWindow != null ?
                      WinRT.Interop.WindowNative.GetWindowHandle(parentWindow) :
                      GetActiveWindow();
        bi.lpszTitle = title;
        bi.ulFlags = 0x00000041; // BIF_NEWDIALOGSTYLE | BIF_RETURNONLYFSDIRS

        nint pidl = SHBrowseForFolder(ref bi);
        if (pidl != nint.Zero)
        {
            nint pathPtr = Marshal.AllocHGlobal(260);
            SHGetPathFromIDList(pidl, pathPtr);
            string? selectedPath = Marshal.PtrToStringAuto(pathPtr);
            Marshal.FreeHGlobal(pathPtr);
            Marshal.FreeCoTaskMem(pidl);
            return selectedPath;
        }

        return string.Empty;
    }
}
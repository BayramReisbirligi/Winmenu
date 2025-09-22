using Microsoft.UI.Xaml;
using System.Runtime.InteropServices;
namespace ReisWolf.Models;
public class NativeFilePicker
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct OpenFileName
    {
        public int lStructSize;
        public nint hwndOwner;
        public nint hInstance;
        public string lpstrFilter;
        public string lpstrCustomFilter;
        public int nMaxCustFilter;
        public int nFilterIndex;
        public string lpstrFile;
        public int nMaxFile;
        public string lpstrFileTitle;
        public int nMaxFileTitle;
        public string lpstrInitialDir;
        public string lpstrTitle;
        public int Flags;
        public short nFileOffset;
        public short nFileExtension;
        public string lpstrDefExt;
        public nint lCustData;
        public nint lpfnHook;
        public string lpTemplateName;
        public nint pvReserved;
        public int dwReserved;
        public int flagsEx;
    }

    [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool GetOpenFileName(ref OpenFileName ofn);

    [DllImport("user32.dll")]
    private static extern nint GetActiveWindow();

    public static string ShowOpenFileDialog(Window parentWindow, string title = "Dosya Seç",
                                         string initialDir = "", string filter = "Tüm Dosyalar\0*.*\0")
    {
        var ofn = new OpenFileName();
        ofn.lStructSize = Marshal.SizeOf(ofn);

        // Pencere tanıtıcısını al
        ofn.hwndOwner = parentWindow != null ?
                       WinRT.Interop.WindowNative.GetWindowHandle(parentWindow) :
                       GetActiveWindow();

        ofn.lpstrFilter = filter;
        ofn.lpstrFile = new string(new char[256]);
        ofn.nMaxFile = ofn.lpstrFile.Length;
        ofn.lpstrFileTitle = new string(new char[64]);
        ofn.nMaxFileTitle = ofn.lpstrFileTitle.Length;
        ofn.lpstrTitle = title;
        ofn.Flags = 0x00080000 | 0x00001000; // OFN_EXPLORER | OFN_FILEMUSTEXIST

        if (!string.IsNullOrEmpty(initialDir))
        {
            ofn.lpstrInitialDir = initialDir;
        }

        if (GetOpenFileName(ref ofn))
        {
            return ofn.lpstrFile;
        }

        return string.Empty;
    }
}
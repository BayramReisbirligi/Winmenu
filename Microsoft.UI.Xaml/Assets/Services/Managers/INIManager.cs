using System.Runtime.InteropServices;
using System.Text;
namespace ReisProduction.Winmenu.Microsoft.UI.Xaml.Assets.Models;
internal static class INIManager
{
    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    private static extern long WritePrivateProfileString(string section, string? key, string? val, string filePath);
    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    private static extern int GetPrivateProfileString(string section, string? key, string? def, StringBuilder retVal, int size, string filePath);
    private static readonly string INIPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Config.ini");
    static INIManager() => Directory.CreateDirectory(Path.GetDirectoryName(INIPath)!);
    internal static string Read(string section, string key, string defaultValue = "")
    {
        var retVal = new StringBuilder(1024);
        int len = GetPrivateProfileString(section, key, defaultValue, retVal, retVal.Capacity, INIPath);
        return len > 0 ? retVal.ToString(0, len) : defaultValue;
    }
    internal static void Write(string section, string? key, string? value) => WritePrivateProfileString(section, key, value, INIPath);
    internal static void DeleteKey(string section, string key) => Write(section, key, null);
    internal static void DeleteSection(string section) => Write(section, null, null);
    internal static List<string> ReadAllKeys(string section)
    {
        var keys = new List<string>();
        var lines = File.ReadLines(INIPath);
        var inside = false;
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith('[') && trimmed.EndsWith(']'))
            {
                inside = trimmed[1..^1].Equals(section, StringComparison.OrdinalIgnoreCase);
                continue;
            }
            if (inside && trimmed.Contains('='))
                keys.Add(trimmed.Split('=')[0].Trim());
        }
        return keys;
    }
}
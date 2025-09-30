using Windows.Graphics;
using Windows.UI;
namespace ReisProduction.Winmenu.Microsoft.UI.Xaml.Assets.Models;
internal class Database
{
    internal UserSettings UserSettings { get; set; } = new();
    // Other models
}
internal class UserSettings
{
    internal PointInt32 LastPosition { get; set; } = default;
    internal SizeInt32 LastSize { get; set; } = default;
    internal string NavigateToView { get; set; } = "GeneralView";
    internal string LastView { get; set; } = "HomeView";
    internal string SelectedLanguage { get; set; } = "";
    internal bool SystemDefaultLanguage { get; set; } = true;
    internal bool AlignSystemDefaults { get; set; } = false;
    internal bool IsTitleBarVisible { get; set; } = true;
    internal bool RuningBackground { get; set; } = true;
    internal bool IsAlwaysOnTop { get; set; } = false;
    internal bool IsPaneVisible { get; set; } = true;
    internal bool IsDarkTheme { get; set; } = false;
    internal bool Notification { get; set; } = true;
    internal bool MergeTheme { get; set; } = false;
    internal bool IsPaneOpen { get; set; } = true;
    internal bool Customize { get; set; } = false;
    internal bool UseSearch { get; set; } = true;
    internal bool Mute { get; set; } = false;
    internal byte Sound { get; set; } = 100;
    internal byte WindowState { get; set; } = 1;
    internal byte SelectedTheme { get; set; } = 1;
    internal string WinState { get; set; } = "Normal";
    internal Color BackgroundColor { get; set; } = Color.FromArgb(0, 0, 0, 0);
    internal Color[] IconsColor { get; set; } = [];
}
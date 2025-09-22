using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Controls;
using System.Reflection;
namespace ReisProduction.Winmenu.Microsoft.UI.Xaml.Assets.Views
{
    internal sealed partial class AboutView : Page
    {
        internal AboutView()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            VersionText.Text = "Version = " + Assembly.GetExecutingAssembly().GetName().Version?.ToString();
        }
    }
}
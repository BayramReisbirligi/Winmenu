using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
namespace ReisProduction.Winmenu.Microsoft.UI.Xaml.Assets.Views
{
    internal sealed partial class ExtraFeaturesView : Page
    {
        internal ExtraFeaturesView()
        {
            NavigationCacheMode = NavigationCacheMode.Required;
            if (string.IsNullOrWhiteSpace(userSettings.NavigateToView))
                InitializeComponent();
            else
            {
                // Return last view
                // shellPage.ContentFrame.Navigate(userSettings.NavigateToView);
            }
        }
        private void ItemButton_Click(object? __, RoutedEventArgs? ___)
        {
            // Navigate to the selected view
        }
    }
}

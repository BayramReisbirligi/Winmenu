using static ReisProduction.Winmenu.Microsoft.UI.Xaml.Assets.Services.Managers.ExceptionManager;
using Microsoft.UI.Xaml;
namespace ReisProduction.Winmenu;
internal partial class App : Application
{
    internal App() => InitializeComponent();
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        UnhandledException += async (__, e) =>
        {
            for (var exception = e.Exception; exception is not null; exception = exception.InnerException)
                await HandleException(exception);
            // CloseApplication(false); Optionally close the application
            e.Handled = true;
        };
        //switch (args)
        //{
        //    case "args":
        //        Launch args
        //        break;
        //    default:
        //        Default launch
                  mainWindow.LoadWinmenu();
        //        break;
        //}
    }
}
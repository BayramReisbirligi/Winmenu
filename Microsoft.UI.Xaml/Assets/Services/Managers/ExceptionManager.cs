using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;
namespace ReisProduction.Winmenu.Microsoft.UI.Xaml.Assets.Services.Managers;
internal static class ExceptionManager
{
    internal static async Task HandleException(Exception exception)
    {
        string exceptionDetails = GetExceptionDetails(exception);
        await CreateLogAsync(exception.GetType().Name, exceptionDetails);
        if (await ShowContentDialog("Would you like to notify us via E-Mail?\n\nPlease do not send too much. We will try to fix the error.", "internal-EncounteredAnError") is ContentDialogResult.Primary)
            await SendMail(exceptionDetails);
    }
    private static async Task CreateLogAsync(string exceptionName, string exceptionDetails)
    {
        try
        {
            string logFilePath = Path.Combine(GetLogFilePath(), $"{exceptionName}.log");
            FileMode fileMode = File.Exists(logFilePath)
                              ? FileMode.Append : FileMode.Create;
            StreamWriter writer = new(logFilePath, true);
            if (fileMode is FileMode.Append)
                writer.WriteLine("\n\n\nA different time:\n");
            writer.WriteLine($"{"exception-Time".GetString()}: {DateTime.Now}\n{exceptionName}");
        }
        catch (Exception exception2) { await ShowContentDialog($"{"exception-Creating".GetString()}: {exception2.Message}", "internal-EncounteredAnError"); }
    }
    private static async Task SendMail(string exceptionDetails)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = $"mailto:{Uri.EscapeDataString("support@reisproduction.com")}?" +
                           $"subject={Uri.EscapeDataString("I encountered an error in Winmenu.")}&" +
                           $"body={Uri.EscapeDataString(exceptionDetails +
                           "Before I encountered the error I was doing the following:\n\n" +
                           "This may be the reason for the error:"
            )}", UseShellExecute = true});
            await ICantMailTo(exceptionDetails);
        }
        catch (Exception exception3)
        {
            await ShowContentDialog(
                $"There was an error while sending the error via Email :(\n\n{exception3.Message}\n" +
                $"Error trying to send via e-mail: {exceptionDetails}",
                "internal-EncounteredAnError".GetString());
        }
    }
    private static async Task ICantMailTo(string exceptionDetails)
    {
        try
        {
            string pathChrome = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Google", "Chrome", "Application", "chrome.exe");
            if (File.Exists(pathChrome) && await ShowContentDialog("I can help you send it to you\n\nAre you want?", "Can't send email?") is ContentDialogResult.Primary)
            {
                string tempFile = Path.Combine(Path.GetTempPath(), "iCantMailTo.txt");
                if (!File.Exists(tempFile))
                    exceptionDetails = "If you are using Google Chrome as the MailTo application and your e-mail address is from the Gmail domain\n" +
                                       "Go to Settings -> Privacy and Security -> Site settings -> Additional permissions -> Handlers Enabled\n" +
                                       "Go to Gmail and the top right to allow this site to send Handlers.\n\n" +
                                       "We have opened these pages for you and the links are here:\n" +
                                       "mail.google.com/mail/u/0/#inbox\n" +
                                       "chrome://settings/handlers\n\n" + exceptionDetails;
                else
                    exceptionDetails = "\nA different time:\n" + exceptionDetails;
                Process.Start(pathChrome, "mail.google.com/mail/u/0/#inbox");
                File.AppendAllText(tempFile, exceptionDetails);
                Process.Start("notepad", tempFile);
            }
        }
        catch (Exception exception)
        {
            await ShowContentDialog(
                $"There was an error while help you send it :(\n\n{exception.Message}\n" +
                $"Error trying to send via e-mail: {exceptionDetails}",
                "internal-EncounteredAnError".GetString());
        }
    }
    private static string GetExceptionDetails(Exception ex) =>
        $"Exception details ({DateTime.Now}):\n\n" +
        $"Type: {ex.GetType().FullName}\n" +
        $"Message: {ex.Message}\n" +
        $"StackTrace: {ex.StackTrace}\n" +
        (ex.Data?.Count > 0 ? $"Data:\n{string.Join('\n', ex.Data.Keys.Cast<object>().Select(k => $"   {k}: {ex.Data[k]}"))}\n" : "") + "\n";
    private static string GetLogFilePath()
    {
        string logFilePath = Path.Combine(appPath, "Logs");
        if (!Directory.Exists(logFilePath))
            Directory.CreateDirectory(logFilePath);
        return logFilePath;
    }
}
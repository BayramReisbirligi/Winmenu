using System.Runtime.InteropServices;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Windowing;
using System.Diagnostics;
using Windows.Foundation;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using System.Reflection;
using Windows.Graphics;
using Windows.System;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Media.Animation;
namespace ReisProduction.Winmenu.Microsoft.UI.Xaml.Assets.Meta;
internal sealed partial class ShellPage : Page
{
    #region dllImport
    [DllImport("psapi.dll")]
    static extern int EmptyWorkingSet(nint hwProc);
    #endregion
    #region WindowEvents
    internal ShellPage() => LoadAllEvents();
    private void ShellPage_Loaded(object? __, RoutedEventArgs? ___)
    {
        var backButton = FindVisualChild<Button>(NavViewHorizontal);
        var topArea = FindVisualChild<StackPanel>(NavViewHorizontal);
        if (topArea is not null &&
            backButton is not null &&
            NavViewHorizontal.SettingsItem is NavigationViewItem settingsItem
            && NavViewVertical.SettingsItem is NavigationViewItem aboutItem)
        {
            topArea.MaxHeight = topArea.MinHeight = 33;
            appTitleBar = topArea;
            shellPage.Children.Remove(SearchBox);
            appTitleBar.Children.Add(SearchBox);
            aboutItem.Icon = new FontIcon { Glyph = "\uE946" };
            settingsItem.Margin = new Thickness(0, -17, 133, 0);
            backButton.Margin = new Thickness(0, -7, 0, 0);
            appTitleBar.Margin = new Thickness(-5, 0, 0, 0);
            mainWindow.SetTitleBar(appTitleBar);
            SetRegionsForCustomTitleBar();
            mainWindow.Show();
        }
        else throw new Exception(
            "Items not found!\n" +
            (topArea is null) + "\n" +
            (backButton is null) + "\n" +
            (NavViewHorizontal.SettingsItem is not NavigationViewItem) + "\n" +
            (NavViewVertical.SettingsItem is not NavigationViewItem));
    }
    private void LoadAllEvents()
    {
        InitializeComponent();
        LoadAnimationEvents();
    }
    internal void RefreshPage()
    {
        if (ContentFrame.CanGoBack)
        {
            ContentFrame.GoBack();
            ContentFrame.GoForward();
        }
    }
    internal void NavigateToView(string selectedItem)
    {
        if (ContentFrame.SourcePageType == null || ContentFrame.SourcePageType.FullName != $"Winmenu.Views.{selectedItem}")
        {
            if (selectedItem == "SettingsView" && NavViewHorizontal.SettingsItem is NavigationViewItem settingsItem)
                NavViewHorizontal.SelectedItem = settingsItem;
            else
                NavViewHorizontal.SelectedItem = NavViewHorizontal.MenuItems.OfType<NavigationViewItem>().FirstOrDefault(item => item.Tag.ToString() == selectedItem);
            var view = Assembly.GetExecutingAssembly().GetType($"Winmenu.Views.{selectedItem}");
            if (view != null)
            {
                ContentFrame.Navigate(view, null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
            }
            //else
            //    throw new Exception("Page is not found!\n" + "internal-Mistake".GetLocalized());
        }
        else
            NavView_BackRequested(NavViewHorizontal, null);
    }
    private void NavViewHorizontal_ItemInvoked(NavigationView? __, NavigationViewItemInvokedEventArgs args)
    {
        var item = args.InvokedItemContainer as NavigationViewItem;
        if (item == null)
            return;
        var ClickedView = item.Tag?.ToString();
        if (ClickedView == null)
            return;
        else if (ClickedView == "Settings")
            ClickedView += "View";
        if (!ItemInvokedNavigate(ClickedView))
            return;
    }
    private bool ItemInvokedNavigate(string ClickedView)
    {
        var view = Assembly.GetExecutingAssembly().GetType($"Winmenu.Views.{ClickedView}");
        if (string.IsNullOrWhiteSpace(ClickedView) || view == null)
            return false;
        if (ContentFrame.SourcePageType != null && view.FullName == ContentFrame.SourcePageType.FullName)
        {
            NavView_BackRequested(NavViewHorizontal, null);
            return false;
        }
        ContentFrame.Navigate(view, null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        return true;
    }
    private void ContentFrame_Navigated(object? __, NavigationEventArgs? ___)
    {
        var currentPageType = ContentFrame.Content?.GetType();
        if (currentPageType == typeof(SettingsView) && NavViewHorizontal.SettingsItem is NavigationViewItem settings)
            NavViewHorizontal.SelectedItem = settings;
        else if (currentPageType != null)
            foreach (var item in NavViewHorizontal.MenuItems)
                if (item is NavigationViewItem navItem && navItem.Tag is string tag && tag == currentPageType.Name)
                    NavViewHorizontal.SelectedItem = item;
        NavViewHorizontal.IsBackEnabled = ContentFrame.CanGoBack;
        ForwardButton.IsEnabled = ContentFrame.CanGoForward;
        ElementSoundPlayer.Play(ElementSoundKind.Invoke);
        GC.Collect();
        GC.WaitForPendingFinalizers();
        EmptyWorkingSet(Process.GetCurrentProcess().Handle);
    }
    #endregion
    #region Customize
    private void ShellPage_SizeChanged(object? __, SizeChangedEventArgs? ___) => SetRegionsForCustomTitleBar();
    private void SetRegionsForCustomTitleBar()
    {
        if (userSettings != null && NavViewVertical != null)
            ResponsiveSizeApply();
        if (appTitleBar != null)
        {
            var scaleAdjustment = appTitleBar.XamlRoot.RasterizationScale;
            GeneralTransform transform = SearchBox.TransformToVisual(null);
            Rect bounds = transform.TransformBounds(new Rect(0, 0, SearchBox.ActualWidth, SearchBox.ActualHeight));
            RectInt32 SearchBoxRect = GetRect(bounds, scaleAdjustment);
            transform = ForwardButton.TransformToVisual(null);
            bounds = transform.TransformBounds(new Rect(0, 0, ForwardButton.ActualWidth, ForwardButton.ActualHeight));
            RectInt32 ForwardButtonRect = GetRect(bounds, scaleAdjustment);
            transform = NavViewHorizontal.TransformToVisual(null);
            bounds = transform.TransformBounds(new Rect(0, 0, 40, 40));
            RectInt32 BackButtonRect = GetRect(bounds, scaleAdjustment), SettingsItemRect = BackButtonRect;
            if (NavViewHorizontal.SettingsItem is NavigationViewItem settingsItem)
            {
                transform = settingsItem.TransformToVisual(null);
                bounds = transform.TransformBounds(new Rect(0, 0, settingsItem.ActualWidth, settingsItem.ActualHeight));
                SettingsItemRect = GetRect(bounds, scaleAdjustment);
            }
            var rectArray = new RectInt32[] { BackButtonRect, ForwardButtonRect, SettingsItemRect, SearchBoxRect };
            InputNonClientPointerSource nonClientInputSrc = InputNonClientPointerSource.GetForWindowId(appWindow.Id);
            nonClientInputSrc.SetRegionRects(NonClientRegionKind.Passthrough, rectArray);
        }
    }
    internal void ResponsiveSizeApply()
    {
        bool maximized = mainWindow.WindowState is WindowState.Maximized;
        int size, top;
        if (userSettings.TitleBarVisible)
        {
            top = VERTICAL_MARGIN_TOP;
            size = appSize.Height - (maximized ? VERTICAL_MINSIZE_MAX : VERTICAL_MINSIZE);
        }
        else
        {
            top = maximized ? VERTICAL_MARGIN_TOP_MAX : VERTICAL_MARGIN_TOP_V;
            size = appSize.Height;
        }
        NavViewVertical.Margin = new Thickness(NavViewVertical.Margin.Left, top, NavViewVertical.Margin.Right, NavViewVertical.Margin.Bottom);
        NavViewVertical.MinHeight = size;
    }
    private static RectInt32 GetRect(Rect bounds, double scale)
    {
        return new RectInt32(
            _X: (int)Math.Round(bounds.X * scale),
            _Y: (int)Math.Round(bounds.Y * scale),
            _Width: (int)Math.Round(bounds.Width * scale),
            _Height: (int)Math.Round(bounds.Height * scale)
        );
    }
    private void LoadAnimationEvents()
    {
        ForwardButton.AddHandler(PointerPressedEvent, new PointerEventHandler(ForwardButton_PointerPressed), true);
        ForwardButton.AddHandler(PointerReleasedEvent, new PointerEventHandler(ForwardButton_PointerReleased), true);
    }
    #endregion
    #region ClickEvents
    private void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs? ___)
    {
        System.Windows.Forms.MessageBox.Show(SearchBox.Text, "Aratılan");
    }
    private void SearchBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs? ___)
    {

    }
    internal void NavView_BackRequested(object? __, NavigationViewBackRequestedEventArgs? ___)
    {
        if (ContentFrame.CanGoBack)
        {
            if (InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down))
                while (ContentFrame.CanGoBack)
                    ContentFrame.GoBack();
            else
                ContentFrame.GoBack();
            ElementSoundPlayer.Play(ElementSoundKind.GoBack);
        }
    }
    internal void ForwardButton_Click(object? __, RoutedEventArgs? ___)
    {
        if (ContentFrame.CanGoForward)
        {
            if (InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down))
                while (ContentFrame.CanGoForward)
                    ContentFrame.GoForward();
            else
                ContentFrame.GoForward();
            ElementSoundPlayer.Play(ElementSoundKind.MoveNext);
        }
    }
    private void HideButton_Click(object? __, RoutedEventArgs? ___) => ShowHide_Invoked(null, null);
    #endregion
    #region Animations
    bool isForwardAnimate;
    private async void ForwardButton_PointerPressed(object? __, PointerRoutedEventArgs e)
    {
        if (e.GetCurrentPoint(ForwardButton).Properties.IsLeftButtonPressed)
        {
            isForwardAnimate = true;
            var icon = ForwardButton.Icon;
            while (icon.Margin.Right <= 3)
            {
                icon.Margin = new Thickness(icon.Margin.Left, icon.Margin.Top, icon.Margin.Right + 1, icon.Margin.Bottom);
                await Task.Delay(12);
            }
            isForwardAnimate = false;
        }
    }
    private async void ForwardButton_PointerReleased(object? __, PointerRoutedEventArgs? ___)
    {
        var icon = ForwardButton.Icon;
        while (isForwardAnimate)
            await Task.Delay(12);
        while (icon.Margin.Right >= 0 && !isForwardAnimate)
        {
            icon.Margin = new Thickness(icon.Margin.Left, icon.Margin.Top, icon.Margin.Right - 1, icon.Margin.Bottom);
            await Task.Delay(12);
        }
    }
    #endregion
    #region GotAndLostFocus
    private void NavViewVertical_PaneChanged(NavigationView? __, object? ___)
    {
        var togglePaneButton = FindVisualChild<Button>(shellPage.NavViewVertical, "TogglePaneButton");
        if (togglePaneButton != null && shellPage.NavViewVertical.SettingsItem is NavigationViewItem about)
        {
            about.Content = "About";
            ToolTipService.SetToolTip(about, $"{about.Content} [F1]");
            ToolTipService.SetToolTip(togglePaneButton, (NavViewVertical.IsPaneOpen ? "Close" : "Open") + " [Back]");
        }
    }
    private void SearchBox_GotFocus(object? __, RoutedEventArgs? ___)
    {
        foreach (var accelerator in ContentFrame.KeyboardAccelerators)
            accelerator.IsEnabled = false;
        foreach (var accelerator in SearchBox.KeyboardAccelerators)
            accelerator.IsEnabled = false;
    }
    private void SearchBox_LostFocus(object? __, RoutedEventArgs? ___)
    {
        foreach (var accelerator in ContentFrame.KeyboardAccelerators)
            accelerator.IsEnabled = true;
        foreach (var accelerator in SearchBox.KeyboardAccelerators)
            accelerator.IsEnabled = true;
    }
    #endregion
    #region KeyboardAccelerators
    private void ShellPage_PointerPressed(object? __, PointerRoutedEventArgs e)
    {
        var properties = e.GetCurrentPoint(null).Properties;
        if (properties.IsMiddleButtonPressed)
            NavigateToView("SettingsView");
        else if (properties.IsXButton1Pressed)
            Back_Invoked(null, null);
        else if (properties.IsXButton2Pressed)
            Forward_Invoked(null, null);
    }
    private void ShellPage_PointerWheelChanged(object? __, PointerRoutedEventArgs e)
    {
        int delta = e.GetCurrentPoint(null).Properties.MouseWheelDelta, currentIndex = 0;
        var menuItems = NavViewHorizontal.MenuItems.OfType<NavigationViewItem>().ToList();
        var selectedItem = NavViewHorizontal.SelectedItem as NavigationViewItem;
        if (selectedItem != null)
            currentIndex = menuItems.IndexOf(selectedItem);
        if (delta > 0)
        {
            if (currentIndex < 1)
                currentIndex = menuItems.Count;
            NavViewHorizontal.SelectedItem = menuItems[currentIndex - 1];
        }
        else if (delta < 0)
        {
            if (currentIndex > menuItems.Count - 2)
                currentIndex = -1;
            NavViewHorizontal.SelectedItem = menuItems[currentIndex + 1];
        }
        NavigateToView(((NavigationViewItem)NavViewHorizontal.SelectedItem).Tag.ToString()!);
    }
    private void NavView_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        switch (sender.Key)
        {
            case VirtualKey.Escape:
                NavigateToView("SettingsView");
                break;
            case VirtualKey.Home:
            case VirtualKey.Number1:
            case VirtualKey.NumberPad1:
                NavigateToView("HomeView");
                break;
            case VirtualKey.Number2:
            case VirtualKey.NumberPad2:
                NavigateToView("GameModeView");
                break;
            case VirtualKey.Number3:
            case VirtualKey.NumberPad3:
                NavigateToView("WindowsView");
                break;
            case VirtualKey.Number4:
            case VirtualKey.NumberPad4:
                NavigateToView("PerformanceView");
                break;
            case VirtualKey.Number5:
            case VirtualKey.NumberPad5:
                NavigateToView("CleaningView");
                break;
            case VirtualKey.Number6:
            case VirtualKey.NumberPad6:
                NavigateToView("TroubleshootingView");
                break;
            case VirtualKey.Number7:
            case VirtualKey.NumberPad7:
                NavigateToView("UpdatesView");
                break;
            case VirtualKey.End:
            case VirtualKey.Number8:
            case VirtualKey.NumberPad8:
                NavigateToView("ExtraFeaturesView");
                break;
        }
    }
    private void SearchBox_Invoked(KeyboardAccelerator? __, KeyboardAcceleratorInvokedEventArgs args)
    {
        if (InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down))
            SearchBox.Text = "";
        args.Handled = true;
        SearchBox.Focus(FocusState.Programmatic);
    }
    private void Help_Invoked(KeyboardAccelerator? __, KeyboardAcceleratorInvokedEventArgs? ___)
    {

    }
    private void ShowHide_Invoked(KeyboardAccelerator? __, KeyboardAcceleratorInvokedEventArgs? ___)
    {
        if (FocusManager.GetFocusedElement() as FrameworkElement != NavViewVertical)
        {
            if (NavViewVertical.IsPaneVisible)
                NavViewVertical.IsPaneOpen = true;
            NavViewVertical.IsPaneVisible = !NavViewVertical.IsPaneVisible;
            NavViewVertical.IsPaneOpen = NavViewVertical.IsPaneVisible == true
                                       ? userSettings.IsPaneOpen
                                       : NavViewVertical.IsPaneOpen;
        }
    }
    private void PaneTop_Invoked(KeyboardAccelerator? __, KeyboardAcceleratorInvokedEventArgs? ___)
    {
        if (FocusManager.GetFocusedElement() as FrameworkElement != NavViewVertical)
        {
            appTitleBar.Visibility = appTitleBar.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            userSettings.IsTitleBarVisible = appTitleBar.Visibility == Visibility.Visible ? true : false;
            NavViewHorizontal.IsBackButtonVisible = appTitleBar.Visibility == Visibility.Visible
                                                  ? NavigationViewBackButtonVisible.Visible
                                                  : NavigationViewBackButtonVisible.Collapsed;
            NavViewVertical.Margin = appTitleBar.Visibility == Visibility.Visible
                                   ? new Thickness(NavViewVertical.Margin.Left, 28, NavViewVertical.Margin.Right, NavViewVertical.Margin.Bottom)
                                   : new Thickness(NavViewVertical.Margin.Left, -5, NavViewVertical.Margin.Right, NavViewVertical.Margin.Bottom);
            ShellPage_SizeChanged(this, null);
        }
    }
    private void Pane_Invoked(KeyboardAccelerator? __, KeyboardAcceleratorInvokedEventArgs args)
    {
        if (FocusManager.GetFocusedElement() as FrameworkElement != NavViewVertical && NavViewVertical.IsPaneVisible)
        {
            NavViewVertical.IsPaneOpen = userSettings.IsPaneOpen = !NavViewVertical.IsPaneOpen;
            args.Handled = true;
        }
    }
    private void Back_Invoked(KeyboardAccelerator? __, KeyboardAcceleratorInvokedEventArgs? ___) => NavView_BackRequested(NavViewHorizontal, null);
    private void Forward_Invoked(KeyboardAccelerator? __, KeyboardAcceleratorInvokedEventArgs? ___) => ForwardButton_Click(ForwardButton, null);
    internal async void Close_Invoked(KeyboardAccelerator? __, KeyboardAcceleratorInvokedEventArgs? ___) { if (ContentDialogResult.Primary == await ShowContentDialog("settings-CloseApplication", "internal-AreYouSure", false, "internal-Sure", "internal-Cancel")) CloseApplication(); }
    private void Refresh_Invoked(   KeyboardAccelerator? __, KeyboardAcceleratorInvokedEventArgs? ___) => RefreshPage();
    private void Theme_Invoked(KeyboardAccelerator? __, KeyboardAcceleratorInvokedEventArgs? ___) => databaseManager.ChangeTheme();
    private void WinState_Invoked(KeyboardAccelerator? __, KeyboardAcceleratorInvokedEventArgs? ___)
    {
        if (mainWindow.WindowState == WindowState.Maximized && mainWindow.AppWindow.Presenter.Kind != AppWindowPresenterKind.FullScreen)
        {
            mainWindow.WindowState = WindowState.Normal;
            NavViewVertical.Margin = new Thickness(-5, 0, 0, 0);
            mainWindow.AppWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
        }
        else
        {
            mainWindow.WindowState = mainWindow.WindowState == WindowState.Normal
                                   ? WindowState.Maximized : WindowState.Normal;
            mainWindow.AppWindow.SetPresenter(AppWindowPresenterKind.Default);
            NavViewVertical.Margin = new Thickness(-5, 30, 0, 0);
        }
    }
    #endregion
}
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sphere_Schedule_App.Services;
using Sphere_Schedule_App.Views.Controls;
using Sphere_Schedule_App.Views.Pages;

namespace Sphere_Schedule_App
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            // Initialize Navigation Service
            NavigationService.Instance.Initialize(MainFrame);

            // Configure pages
            ConfigureNavigation();

            // Register callback with AppNavigationService
            AppNavigationService.Instance.RegisterMainWindow(SwitchToMainApp);

            // Set initial page to Login
            MainFrame.Navigate(typeof(LoginPage));

            // Set window size and position
            SetWindowSize(1200, 800);
            CenterWindow();
        }

        private void ConfigureNavigation()
        {
            NavigationService.Instance.Configure("Dashboard", typeof(DashboardPage));
            NavigationService.Instance.Configure("Tasks", typeof(TasksPage));
            NavigationService.Instance.Configure("Appointments", typeof(AppointmentsPage));
            NavigationService.Instance.Configure("Meetings", typeof(MeetingsPage));
            NavigationService.Instance.Configure("Reports", typeof(ReportsPage));
            NavigationService.Instance.Configure("Settings", typeof(SettingsPage));
            NavigationService.Instance.Configure("Help", typeof(HelpPage));
        }

        // Method to switch to main app view after login/skip
        public void SwitchToMainApp()
        {
            // Create a new grid for main app layout
            var mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Title bar
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Content

            // Create and add title bar
            var titleBar = new CustomTitleBar();
            Grid.SetRow(titleBar, 0);
            mainGrid.Children.Add(titleBar);

            // Create and add navigation view
            var navigationView = new ModernNavigationView();
            Grid.SetRow(navigationView, 1);
            mainGrid.Children.Add(navigationView);

            // Replace the entire window content
            this.Content = mainGrid;

            // Clear any navigation history
            NavigationService.Instance.ClearHistory();
        }

        private Grid CreateMainLayout()
        {
            var mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Title bar
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Content

            // Custom Title Bar
            var titleBar = new CustomTitleBar();
            Grid.SetRow(titleBar, 0);
            mainGrid.Children.Add(titleBar);

            // Main Content with Navigation
            var contentGrid = new Grid();
            Grid.SetRow(contentGrid, 1);

            // Navigation View
            var navigationView = new ModernNavigationView();
            contentGrid.Children.Add(navigationView);

            mainGrid.Children.Add(contentGrid);

            return mainGrid;
        }

        private void SetWindowSize(int width, int height)
        {
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

            appWindow.Resize(new Windows.Graphics.SizeInt32(width, height));
        }

        private void CenterWindow()
        {
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

            var displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(windowId, Microsoft.UI.Windowing.DisplayAreaFallback.Primary);
            var centerX = (displayArea.WorkArea.Width - appWindow.Size.Width) / 2;
            var centerY = (displayArea.WorkArea.Height - appWindow.Size.Height) / 2;

            appWindow.Move(new Windows.Graphics.PointInt32(centerX, centerY));
        }
    }
}
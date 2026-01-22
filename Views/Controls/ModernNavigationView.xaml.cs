using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Sphere_Schedule_App.Services;
using Sphere_Schedule_App.Views.Pages;
using System;

namespace Sphere_Schedule_App.Views.Controls
{
    public sealed partial class ModernNavigationView : UserControl
    {
        public ModernNavigationView()
        {
            this.InitializeComponent();
            InitializeNavigation();
        }

        private void InitializeNavigation()
        {
            // Configure navigation service to use our ContentFrame
            NavigationService.Instance.Initialize(ContentFrame);

            // Configure all page routes
            NavigationService.Instance.Configure("Dashboard", typeof(DashboardPage));
            NavigationService.Instance.Configure("Tasks", typeof(TasksPage));
            NavigationService.Instance.Configure("Appointments", typeof(AppointmentsPage));
            NavigationService.Instance.Configure("Meetings", typeof(MeetingsPage));
            NavigationService.Instance.Configure("Reports", typeof(ReportsPage));
            NavigationService.Instance.Configure("Settings", typeof(SettingsPage));
            NavigationService.Instance.Configure("Help", typeof(HelpPage));

            // Set up navigation event handler
            MainNavigation.SelectionChanged += OnNavigationSelectionChanged;

            // Set default selection to Dashboard
            MainNavigation.SelectedItem = MainNavigation.MenuItems[0];

            // Navigate to Dashboard initially
            NavigationService.Instance.NavigateTo("Dashboard");
        }

        private void OnNavigationSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem item && item.Tag is string pageKey)
            {
                NavigationService.Instance.NavigateTo(pageKey);
            }
        }
    }
}
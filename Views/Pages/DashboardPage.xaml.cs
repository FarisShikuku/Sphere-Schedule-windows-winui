using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sphere_Schedule_App.ViewModels;
using Sphere_Schedule_App.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Sphere_Schedule_App.Views.Pages
{
    public sealed partial class DashboardPage : BasePage
    {
        private readonly DashboardViewModel _viewModel;

        public DashboardPage()
        {
            this.InitializeComponent();

            // Get the ViewModel from the service provider
            var serviceProvider = ServiceRegistration.ConfigureServices();
            var unitOfWork = serviceProvider.GetService<Core.Interfaces.IUnitOfWork>();

            _viewModel = new DashboardViewModel(unitOfWork);
            this.DataContext = _viewModel;
        }

        protected override async void OnLoaded(object sender, RoutedEventArgs e)
        {
            base.OnLoaded(sender, e);

            try
            {
                // Initialize database (this will create it if doesn't exist)
                await _viewModel.UnitOfWork.InitializeDatabaseAsync();

                // Load dashboard data
                await _viewModel.LoadDashboardDataAsync();
            }
            catch (Exception ex)
            {
                // Only show error if it's not a "no data" scenario
                Console.WriteLine($"Error loading dashboard: {ex.Message}");

                // Don't show error dialog for empty database
                // Just let the UI show empty states
            }
        }
    }
}
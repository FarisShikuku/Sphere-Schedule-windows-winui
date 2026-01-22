using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sphere_Schedule_App.Core.Interfaces;
using Sphere_Schedule_App.Data.LocalDb;
using Sphere_Schedule_App.Services;
using Sphere_Schedule_App.ViewModels;
using Sphere_Schedule_App.Views.Dialogs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sphere_Schedule_App.Views.Controls
{
    public sealed partial class TaskFilterPanel : UserControl
    {
        private TaskFilterPanelViewModel _viewModel;

        public TaskFilterPanel()
        {
            this.InitializeComponent();
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_viewModel == null)
                {
                    // Use the app's service provider instead of creating a new one
                    if (App.ServiceProvider != null)
                    {
                        var unitOfWork = App.ServiceProvider.GetRequiredService<Core.Interfaces.IUnitOfWork>();

                        if (unitOfWork != null)
                        {
                            _viewModel = new TaskFilterPanelViewModel(unitOfWork);
                            DataContext = _viewModel;

                            // Get or create user
                            var users = await unitOfWork.Users.GetAllAsync();
                            var user = users.FirstOrDefault();

                            if (user != null)
                            {
                                await _viewModel.InitializeAsync(user);
                            }
                            else
                            {
                                _viewModel.IsLoading = false;
                            }
                        }
                    }
                    else
                    {
                        // Fallback to old method if App.ServiceProvider is null
                        var serviceProvider = ServiceRegistration.ConfigureServices();
                        var unitOfWork = serviceProvider.GetService<Core.Interfaces.IUnitOfWork>();

                        if (unitOfWork != null)
                        {
                            _viewModel = new TaskFilterPanelViewModel(unitOfWork);
                            DataContext = _viewModel;

                            // Initialize database
                            await unitOfWork.InitializeDatabaseAsync();

                            // Get or create user
                            var users = await unitOfWork.Users.GetAllAsync();
                            var user = users.FirstOrDefault();

                            if (user != null)
                            {
                                await _viewModel.InitializeAsync(user);
                            }
                            else
                            {
                                _viewModel.IsLoading = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TaskFilterPanel load error: {ex.Message}");
                if (_viewModel != null)
                {
                    _viewModel.IsLoading = false;
                }
            }
        }

        
        private async void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check if database is ready
                if (!App.IsDatabaseReady)
                {
                    await ShowErrorAsync($"Database error: {App.DatabaseError}");
                    return;
                }

                // Get UnitOfWork from DI
                var unitOfWork = App.ServiceProvider.GetRequiredService<IUnitOfWork>();

                // Get current user
                var users = await unitOfWork.Users.GetAllAsync();
                var currentUser = users.FirstOrDefault();

                if (currentUser == null)
                {
                    await ShowErrorAsync("No user found. Please restart the application.");
                    return;
                }

                // Show dialog and get new task
                var task = await AddTaskDialog.ShowAsync(this.XamlRoot, unitOfWork, currentUser.UserID);

                if (task != null)
                {
                    // Refresh tasks list
                    if (_viewModel != null)
                    {
                        await _viewModel.LoadTasksAsync();
                    }
                    else
                    {
                        // Re-initialize view model if needed
                        await ReinitializeViewModel();
                    }

                    // Show success message
                    await ShowSuccessAsync("Task added successfully!");
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error adding task: {ex.Message}");
            }
        }

        private async Task ReinitializeViewModel()
        {
            try
            {
                if (App.ServiceProvider != null)
                {
                    var unitOfWork = App.ServiceProvider.GetRequiredService<Core.Interfaces.IUnitOfWork>();

                    if (unitOfWork != null)
                    {
                        _viewModel = new TaskFilterPanelViewModel(unitOfWork);
                        DataContext = _viewModel;

                        // Get or create user
                        var users = await unitOfWork.Users.GetAllAsync();
                        var user = users.FirstOrDefault();

                        if (user != null)
                        {
                            await _viewModel.InitializeAsync(user);
                        }
                        else
                        {
                            _viewModel.IsLoading = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ReinitializeViewModel error: {ex.Message}");
            }
        }

        private async Task ShowSuccessAsync(string message)
        {
            var dialog = new ContentDialog
            {
                Title = "Success",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
        }

        private async Task ShowErrorAsync(string message)
        {
            var dialog = new ContentDialog
            {
                Title = "Error",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
        }

        // In TaskFilterPanel.xaml.cs, add the click handler:
        private void ViewAllTasksButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to TasksPage
            NavigationService.Instance.NavigateTo("Tasks");
        }

        // Optional: Add a refresh button handler
        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                await _viewModel.LoadTasksAsync();
            }
        }

        // Optional: Handle filter changes
        private async void PeriodFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel != null && e.AddedItems.Count > 0)
            {
                var selectedPeriod = e.AddedItems[0] as string;
                if (!string.IsNullOrEmpty(selectedPeriod))
                {
                    _viewModel.SelectedPeriod = selectedPeriod;
                    await _viewModel.LoadTasksAsync();
                }
            }
        }

        private async void PriorityFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel != null && e.AddedItems.Count > 0)
            {
                var selectedPriority = e.AddedItems[0] as string;
                if (!string.IsNullOrEmpty(selectedPriority))
                {
                    _viewModel.SelectedPriority = selectedPriority;
                    await _viewModel.LoadTasksAsync();
                }
            }
        }

        private async void CategoryFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel != null && e.AddedItems.Count > 0)
            {
                var selectedCategory = e.AddedItems[0] as string;
                if (!string.IsNullOrEmpty(selectedCategory))
                {
                    _viewModel.SelectedCategory = selectedCategory;
                    await _viewModel.LoadTasksAsync();
                }
            }
        }
    }
}
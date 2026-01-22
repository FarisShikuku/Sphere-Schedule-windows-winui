using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sphere_Schedule_App.Core.Interfaces;
using Sphere_Schedule_App.Core.Models;
using Sphere_Schedule_App.ViewModels;
using Sphere_Schedule_App.Views.Dialogs;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;
namespace Sphere_Schedule_App.Views.Pages
{
    public sealed partial class TasksPage : Page
    {
        private TaskListViewModel _viewModel;

        public TasksPage()
        {
            this.InitializeComponent();
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await InitializeViewModelAsync();
        }

        private async Task InitializeViewModelAsync()
        {
            try
            {
                if (App.ServiceProvider != null)
                {
                    var unitOfWork = App.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    // Get current user
                    var users = await unitOfWork.Users.GetAllAsync();
                    var currentUser = users.FirstOrDefault();

                    if (currentUser != null)
                    {
                        _viewModel = new TaskListViewModel(unitOfWork, currentUser.UserID);
                        DataContext = _viewModel;
                        await _viewModel.LoadTasksAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TasksPage initialization error: {ex.Message}");
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                await _viewModel.LoadTasksAsync();
            }
        }

        private async void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (App.ServiceProvider != null && _viewModel != null)
                {
                    var unitOfWork = App.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    // Show dialog and get new task
                    var task = await AddTaskDialog.ShowAsync(this.XamlRoot, unitOfWork, _viewModel.CurrentUserId);

                    if (task != null)
                    {
                        // Refresh tasks list
                        await _viewModel.LoadTasksAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding task: {ex.Message}");
            }
        }

        private async void TaskCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.Tag is UserTask task && _viewModel != null)
            {
                await _viewModel.ToggleTaskCompletionAsync(task);
            }
        }

        private async void TaskCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.Tag is UserTask task && _viewModel != null)
            {
                await _viewModel.ToggleTaskCompletionAsync(task);
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is UserTask task && _viewModel != null)
            {
                // Show confirmation dialog
                var dialog = new ContentDialog
                {
                    Title = "Delete Task",
                    Content = $"Are you sure you want to delete '{task.Title}'?",
                    PrimaryButtonText = "Delete",
                    SecondaryButtonText = "Cancel",
                    DefaultButton = ContentDialogButton.Secondary,
                    XamlRoot = this.XamlRoot
                };

                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    await _viewModel.DeleteTaskAsync(task);
                }
            }
        }

        private async void RetryButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                await _viewModel.LoadTasksAsync();
            }
        }
    }

    // Simple converter for Priority to Color
    public class PriorityColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string priority)
            {
                return priority?.ToLower() switch
                {
                    "critical" => "#D32F2F",
                    "high" => "#F57C00",
                    "medium" => "#1976D2",
                    "low" => "#388E3C",
                    _ => "#757575"
                };
            }
            return "#757575";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

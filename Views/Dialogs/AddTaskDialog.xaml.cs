using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sphere_Schedule_App.Core.Interfaces;
using Sphere_Schedule_App.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sphere_Schedule_App.Views.Dialogs
{
    public sealed partial class AddTaskDialog : ContentDialog
    {
        public UserTask CreatedTask { get; private set; }
        private readonly IUnitOfWork _unitOfWork;
        private readonly Guid _currentUserId;
        private DateTimeOffset _defaultDueDate = DateTimeOffset.Now.AddDays(1);
        private bool _isLoading = true;

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                // Update UI
                LoadingOverlay.Visibility = _isLoading ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public AddTaskDialog(XamlRoot xamlRoot, IUnitOfWork unitOfWork, Guid currentUserId)
        {
            this.InitializeComponent();
            this.XamlRoot = xamlRoot;
            _unitOfWork = unitOfWork;
            _currentUserId = currentUserId;

            InitializeEvents();
            SetDefaultValues();
        }

        private void InitializeEvents()
        {
            DueTimeToggle.Checked += DueTimeToggle_Checked;
            DueTimeToggle.Unchecked += DueTimeToggle_Unchecked;
        }

        private void SetDefaultValues()
        {
            // Due date defaults to tomorrow
            DueDatePicker.Date = _defaultDueDate;

            // Default time to 5:00 PM if time is enabled
            DueTimePicker.Time = new TimeSpan(17, 0, 0);

            // Default priority is Medium
            MediumPriorityButton.IsChecked = true;

            // Default status is Pending
            StatusComboBox.SelectedIndex = 0;

            // Default duration is 1 hour
            HoursComboBox.SelectedIndex = 1; // 1h
            MinutesComboBox.SelectedIndex = 0; // 0m
        }

        private async void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("AddTaskDialog: Loaded event triggered");
            await LoadCategoriesAsync();
        }

        private async Task LoadCategoriesAsync()
        {
            System.Diagnostics.Debug.WriteLine("AddTaskDialog: Starting LoadCategoriesAsync");

            try
            {
                IsLoading = true;
                ErrorBorder.Visibility = Visibility.Collapsed;

                // Clear existing items
                CategoryComboBox.Items.Clear();
                System.Diagnostics.Debug.WriteLine("AddTaskDialog: Cleared combobox items");

                // Add hardcoded default categories first
                var defaultCategories = new List<string>
                {
                    "Work",
                    "Personal",
                    "Health",
                    "Education",
                    "Shopping",
                    "Finance",
                    "Entertainment",
                    "Other"
                };

                System.Diagnostics.Debug.WriteLine($"AddTaskDialog: Adding {defaultCategories.Count} default categories");

                foreach (var category in defaultCategories)
                {
                    CategoryComboBox.Items.Add(new ComboBoxItem { Content = category, Tag = category });
                }

                // Try to load categories from database for current user
                if (_unitOfWork != null && _currentUserId != Guid.Empty)
                {
                    System.Diagnostics.Debug.WriteLine($"AddTaskDialog: Attempting to load categories from database for user {_currentUserId}");

                    try
                    {
                        // Check if Categories repository is accessible
                        var categories = await _unitOfWork.Categories.GetAllAsync();
                        System.Diagnostics.Debug.WriteLine($"AddTaskDialog: Retrieved {categories?.Count() ?? 0} categories from database");

                        if (categories != null)
                        {
                            var userCategories = categories
                                .Where(c => c.UserID == _currentUserId && !c.IsDeleted)
                                .OrderBy(c => c.CategoryOrder)
                                .ThenBy(c => c.CategoryName)
                                .ToList();

                            System.Diagnostics.Debug.WriteLine($"AddTaskDialog: Found {userCategories.Count} categories for current user");

                            foreach (var category in userCategories)
                            {
                                CategoryComboBox.Items.Add(new ComboBoxItem
                                {
                                    Content = category.CategoryName,
                                    Tag = category.CategoryName
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"AddTaskDialog: Error loading categories from database: {ex.Message}");
                        System.Diagnostics.Debug.WriteLine($"AddTaskDialog: Stack trace: {ex.StackTrace}");
                        // Continue with default categories
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"AddTaskDialog: UnitOfWork is null: {_unitOfWork == null}, UserID is empty: {_currentUserId == Guid.Empty}");
                }

                // Select the first category (Work)
                if (CategoryComboBox.Items.Count > 0)
                {
                    CategoryComboBox.SelectedIndex = 0;
                    System.Diagnostics.Debug.WriteLine($"AddTaskDialog: Selected first category, total items: {CategoryComboBox.Items.Count}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("AddTaskDialog: No categories available to select");
                }

                IsLoading = false;
                System.Diagnostics.Debug.WriteLine("AddTaskDialog: LoadCategoriesAsync completed successfully");
            }
            catch (Exception ex)
            {
                IsLoading = false;
                System.Diagnostics.Debug.WriteLine($"AddTaskDialog: LoadCategoriesAsync failed with exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"AddTaskDialog: Stack trace: {ex.StackTrace}");
                ShowError($"Failed to load categories: {ex.Message}");
            }
        }

        private void DueTimeToggle_Checked(object sender, RoutedEventArgs e)
        {
            DueTimePanel.Visibility = Visibility.Visible;
        }

        private void DueTimeToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            DueTimePanel.Visibility = Visibility.Collapsed;
        }

        private void TitleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateTitle();
        }

        private bool ValidateTitle()
        {
            bool isValid = !string.IsNullOrWhiteSpace(TitleTextBox.Text);
            TitleErrorText.Visibility = isValid ? Visibility.Collapsed : Visibility.Visible;
            return isValid;
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var deferral = args.GetDeferral();

            try
            {
                System.Diagnostics.Debug.WriteLine("AddTaskDialog: Primary button clicked - starting validation");

                // Validate required fields
                if (!ValidateTitle())
                {
                    System.Diagnostics.Debug.WriteLine("AddTaskDialog: Validation failed - title is empty");
                    TitleTextBox.Focus(FocusState.Programmatic);
                    args.Cancel = true;
                    ShowError("Please enter a task title.");
                    return;
                }

                // Show loading state
                IsLoading = true;
                ErrorBorder.Visibility = Visibility.Collapsed; // Clear any previous errors
                System.Diagnostics.Debug.WriteLine("AddTaskDialog: Creating task object");

                // Create task with required fields
                CreatedTask = new UserTask
                {
                    TaskID = Guid.NewGuid(), // Generated in background, user doesn't see this
                    UserID = _currentUserId,
                    Title = TitleTextBox.Text.Trim(),
                    Status = "pending", // Default status
                    CompletionPercentage = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                System.Diagnostics.Debug.WriteLine($"AddTaskDialog: Created task with ID: {CreatedTask.TaskID}");

                // Set optional description
                if (!string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
                {
                    CreatedTask.Description = DescriptionTextBox.Text.Trim();
                }

                // Set priority (optional, defaults to medium)
                if (CriticalPriorityButton.IsChecked == true)
                    CreatedTask.PriorityLevel = "critical";
                else if (HighPriorityButton.IsChecked == true)
                    CreatedTask.PriorityLevel = "high";
                else if (MediumPriorityButton.IsChecked == true)
                    CreatedTask.PriorityLevel = "medium";
                else if (LowPriorityButton.IsChecked == true)
                    CreatedTask.PriorityLevel = "low";
                else
                    CreatedTask.PriorityLevel = "medium"; // Default

                // Set category (optional)
                if (CategoryComboBox.SelectedItem is ComboBoxItem categoryItem && categoryItem.Tag != null)
                {
                    CreatedTask.Category = categoryItem.Tag.ToString();
                    System.Diagnostics.Debug.WriteLine($"AddTaskDialog: Set category to: {CreatedTask.Category}");
                }
                else
                {
                    CreatedTask.Category = "Personal"; // Default category
                    System.Diagnostics.Debug.WriteLine("AddTaskDialog: Using default category: Personal");
                }

                // Set due date (defaults to tomorrow)
                if (DueDatePicker.Date != null)
                {
                    CreatedTask.DueDate = DueDatePicker.Date.DateTime;
                    System.Diagnostics.Debug.WriteLine($"AddTaskDialog: Set due date to: {CreatedTask.DueDate}");

                    // Set due time if enabled
                    if (DueTimeToggle.IsChecked == true)
                    {
                        var time = DueTimePicker.Time;
                        CreatedTask.DueTime = time;
                        System.Diagnostics.Debug.WriteLine($"AddTaskDialog: Set due time to: {time}");

                        // Combine date and time for DueDate
                        CreatedTask.DueDate = new DateTime(
                            CreatedTask.DueDate.Value.Year,
                            CreatedTask.DueDate.Value.Month,
                            CreatedTask.DueDate.Value.Day,
                            time.Hours,
                            time.Minutes,
                            0);
                    }
                    // If time is disabled, DueTime remains null (end of day assumed)
                }

                // Set status from combo box
                if (StatusComboBox.SelectedItem is ComboBoxItem statusItem && statusItem.Tag != null)
                {
                    CreatedTask.Status = statusItem.Tag.ToString();
                }

                // Set estimated duration (optional)
                int estimatedMinutes = 0;
                if (HoursComboBox.SelectedItem is ComboBoxItem hoursItem && hoursItem.Tag != null)
                {
                    estimatedMinutes += int.Parse(hoursItem.Tag.ToString());
                }
                if (MinutesComboBox.SelectedItem is ComboBoxItem minutesItem && minutesItem.Tag != null)
                {
                    estimatedMinutes += int.Parse(minutesItem.Tag.ToString());
                }
                if (estimatedMinutes > 0)
                {
                    CreatedTask.EstimatedDurationMinutes = estimatedMinutes;
                }

                // Save to database
                if (_unitOfWork != null && _currentUserId != Guid.Empty)
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine("AddTaskDialog: Attempting to save task to database");
                        await _unitOfWork.UserTasks.AddAsync(CreatedTask);
                        await _unitOfWork.CompleteAsync();
                        System.Diagnostics.Debug.WriteLine("AddTaskDialog: Task saved to database successfully");

                        // Show success by updating the dialog title
                        this.Title = "Task Saved Successfully ✓";
                        await Task.Delay(300); // Brief delay to show success

                        args.Cancel = false; // Close the dialog
                    }
                    catch (Exception dbEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"AddTaskDialog: Database error: {dbEx.Message}");
                        ShowError($"Failed to save task: {dbEx.Message}");
                        args.Cancel = true; // Keep dialog open
                        return;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("AddTaskDialog: UnitOfWork is null or user ID is empty");
                    ShowError("Cannot save task: Database connection issue.");
                    args.Cancel = true; // Keep dialog open
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AddTaskDialog: General error creating task: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"AddTaskDialog: Stack trace: {ex.StackTrace}");
                ShowError($"Error creating task: {ex.Message}");
                args.Cancel = true; // Keep dialog open
            }
            finally
            {
                IsLoading = false;
                deferral.Complete();
                System.Diagnostics.Debug.WriteLine("AddTaskDialog: Primary button click handling completed");
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Cancel operation
            CreatedTask = null;
            System.Diagnostics.Debug.WriteLine("AddTaskDialog: Dialog cancelled by user");
        }

        private async void RetryButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("AddTaskDialog: Retry button clicked");
            await LoadCategoriesAsync();
        }

        private void ShowError(string message)
        {
            ErrorMessageText.Text = message;
            ErrorBorder.Visibility = Visibility.Visible;
            IsLoading = false;
            System.Diagnostics.Debug.WriteLine($"AddTaskDialog: Showing error: {message}");
        }

        public static async Task<UserTask> ShowAsync(XamlRoot xamlRoot, IUnitOfWork unitOfWork, Guid currentUserId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"AddTaskDialog.ShowAsync called with UserID: {currentUserId}");
                var dialog = new AddTaskDialog(xamlRoot, unitOfWork, currentUserId);
                var result = await dialog.ShowAsync();

                return result == ContentDialogResult.Primary ? dialog.CreatedTask : null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AddTaskDialog.ShowAsync error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");

                // Just return null, don't try to show another dialog
                return null;
            }
        }
    }
}
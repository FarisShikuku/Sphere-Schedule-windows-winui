using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sphere_Schedule_App.Core.Interfaces;
using Sphere_Schedule_App.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Sphere_Schedule_App.ViewModels
{
    public partial class TaskFilterPanelViewModel : ObservableObject
    {
        private readonly IUnitOfWork _unitOfWork;
        private User _currentUser;
        // Add this property to the TaskFilterPanelViewModel class:
        public IUnitOfWork UnitOfWork => _unitOfWork;

        public TaskFilterPanelViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            InitializeCommands();
            InitializeCollections();
        }

        [ObservableProperty]
        private bool _isLoading = true;

        [ObservableProperty]
        private string _selectedPeriod = "today";

        [ObservableProperty]
        private string _selectedViewMode = "list";

        [ObservableProperty]
        private string _selectedPriority = "all";

        [ObservableProperty]
        private string _selectedCategory = "all";

        [ObservableProperty]
        private ObservableCollection<UserTask> _currentTasks = new();

        [ObservableProperty]
        private ObservableCollection<UserTask> _upcomingTasks = new();

        [ObservableProperty]
        private ObservableCollection<string> _availableCategories = new();

        [ObservableProperty]
        private int _totalCurrentTasks = 0;

        [ObservableProperty]
        private int _totalUpcomingTasks = 0;

        [ObservableProperty]
        private int _completedTodayCount = 0;

        public IAsyncRelayCommand LoadTasksCommand { get; private set; }
        public IRelayCommand<string> ChangePeriodCommand { get; private set; }
        public IRelayCommand<string> ChangePriorityCommand { get; private set; }
        public IRelayCommand<string> ChangeCategoryCommand { get; private set; }
        public IAsyncRelayCommand<UserTask> ToggleTaskCompletionCommand { get; private set; }

        private void InitializeCommands()
        {
            LoadTasksCommand = new AsyncRelayCommand(LoadTasksAsync);
            ChangePeriodCommand = new RelayCommand<string>(ChangePeriod);
            ChangePriorityCommand = new RelayCommand<string>(ChangePriority);
            ChangeCategoryCommand = new RelayCommand<string>(ChangeCategory);
            ToggleTaskCompletionCommand = new AsyncRelayCommand<UserTask>(ToggleTaskCompletionAsync);
        }

        private void InitializeCollections()
        {
            AvailableCategories.Add("all");
            AvailableCategories.Add("Work");
            AvailableCategories.Add("Personal");
            AvailableCategories.Add("Health");
            AvailableCategories.Add("Education");
            AvailableCategories.Add("Shopping");
            AvailableCategories.Add("Finance");
            AvailableCategories.Add("Entertainment");
            AvailableCategories.Add("Other");
        }

        public async Task InitializeAsync(User currentUser)
        {
            _currentUser = currentUser;

            // Don't load tasks if no user
            if (_currentUser == null)
            {
                IsLoading = false;
                return;
            }

            await LoadTasksAsync();
        }

        public async Task LoadTasksAsync()
        {
            IsLoading = true;

            try
            {
                if (_currentUser == null)
                {
                    // Load current user from database
                    var users = await _unitOfWork.Users.FindAsync(u => u.Email == "demo@sphereschedule.com");
                    _currentUser = users.FirstOrDefault();

                    if (_currentUser == null)
                        return;
                }

                // Get all non-deleted tasks for the user
                var allTasks = await _unitOfWork.UserTasks.FindAsync(t =>
                    t.UserID == _currentUser.UserID &&
                    t.IsDeleted == false);

                // Apply period filter
                var filteredTasks = ApplyPeriodFilter(allTasks);

                // Apply priority filter
                filteredTasks = ApplyPriorityFilter(filteredTasks);

                // Apply category filter
                filteredTasks = ApplyCategoryFilter(filteredTasks);

                // Separate current and upcoming tasks
                var today = DateTime.Today;
                var currentTasksList = filteredTasks
                    .Where(t => !t.DueDate.HasValue || t.DueDate.Value.Date <= today.AddDays(1))
                    .Where(t => t.Status != "completed")
                    .ToList();

                var upcomingTasksList = filteredTasks
                    .Where(t => t.DueDate.HasValue && t.DueDate.Value.Date > today.AddDays(1))
                    .Where(t => t.Status != "completed")
                    .ToList();

                // Count completed tasks today
                CompletedTodayCount = allTasks
                    .Count(t => t.Status == "completed" &&
                                t.CompletedAt.HasValue &&
                                t.CompletedAt.Value.Date == today);

                // Update collections
                CurrentTasks = new ObservableCollection<UserTask>(currentTasksList
                    .OrderBy(t => t.PriorityLevel switch
                    {
                        "critical" => 1,
                        "high" => 2,
                        "medium" => 3,
                        "low" => 4,
                        _ => 5
                    })
                    .ThenBy(t => t.DueDate)
                    .ThenBy(t => t.DueTime));

                UpcomingTasks = new ObservableCollection<UserTask>(upcomingTasksList
                    .OrderBy(t => t.DueDate)
                    .ThenBy(t => t.PriorityLevel switch
                    {
                        "critical" => 1,
                        "high" => 2,
                        "medium" => 3,
                        "low" => 4,
                        _ => 5
                    }));

                TotalCurrentTasks = currentTasksList.Count;
                TotalUpcomingTasks = upcomingTasksList.Count;
            }
            catch (Exception ex)
            {
                // Log error in production
                Console.WriteLine($"Error loading tasks: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private IEnumerable<UserTask> ApplyPeriodFilter(IEnumerable<UserTask> tasks)
        {
            var today = DateTime.Today;

            return SelectedPeriod switch
            {
                "today" => tasks.Where(t => t.DueDate.HasValue && t.DueDate.Value.Date == today),
                "week" => tasks.Where(t => t.DueDate.HasValue &&
                                          t.DueDate.Value.Date >= today &&
                                          t.DueDate.Value.Date <= today.AddDays(7)),
                "month" => tasks.Where(t => t.DueDate.HasValue &&
                                           t.DueDate.Value.Date >= today &&
                                           t.DueDate.Value.Date <= today.AddMonths(1)),
                "year" => tasks.Where(t => t.DueDate.HasValue &&
                                          t.DueDate.Value.Date >= today &&
                                          t.DueDate.Value.Date <= today.AddYears(1)),
                "all" => tasks,
                _ => tasks.Where(t => !t.DueDate.HasValue || t.DueDate.Value.Date <= today.AddDays(1))
            };
        }

        private IEnumerable<UserTask> ApplyPriorityFilter(IEnumerable<UserTask> tasks)
        {
            return SelectedPriority switch
            {
                "critical" => tasks.Where(t => t.PriorityLevel == "critical"),
                "high" => tasks.Where(t => t.PriorityLevel == "high"),
                "medium" => tasks.Where(t => t.PriorityLevel == "medium"),
                "low" => tasks.Where(t => t.PriorityLevel == "low"),
                _ => tasks // "all"
            };
        }

        private IEnumerable<UserTask> ApplyCategoryFilter(IEnumerable<UserTask> tasks)
        {
            return SelectedCategory.ToLower() switch
            {
                "work" => tasks.Where(t => t.Category == "Work"),
                "personal" => tasks.Where(t => t.Category == "Personal"),
                "health" => tasks.Where(t => t.Category == "Health"),
                "education" => tasks.Where(t => t.Category == "Education"),
                "shopping" => tasks.Where(t => t.Category == "Shopping"),
                "finance" => tasks.Where(t => t.Category == "Finance"),
                "entertainment" => tasks.Where(t => t.Category == "Entertainment"),
                "other" => tasks.Where(t => t.Category == "Other"),
                _ => tasks // "all" or any other
            };
        }

        private void ChangePeriod(string period)
        {
            if (!string.IsNullOrEmpty(period))
            {
                SelectedPeriod = period;
                _ = LoadTasksAsync();
            }
        }

        private void ChangePriority(string priority)
        {
            if (!string.IsNullOrEmpty(priority))
            {
                SelectedPriority = priority;
                _ = LoadTasksAsync();
            }
        }

        private void ChangeCategory(string category)
        {
            if (!string.IsNullOrEmpty(category))
            {
                SelectedCategory = category;
                _ = LoadTasksAsync();
            }
        }

        private async Task ToggleTaskCompletionAsync(UserTask task)
        {
            if (task == null) return;

            try
            {
                // Toggle completion status
                if (task.Status == "completed")
                {
                    task.Status = "pending";
                    task.CompletedAt = null;
                    task.CompletionPercentage = 0;
                }
                else
                {
                    task.Status = "completed";
                    task.CompletedAt = DateTime.UtcNow;
                    task.CompletionPercentage = 100;
                }

                task.UpdatedAt = DateTime.UtcNow;

                // Update in database using the correct repository method
                await _unitOfWork.UserTasks.UpdateAsync(task);
                await _unitOfWork.CompleteAsync(); // This saves all changes

                // Reload tasks to reflect changes
                await LoadTasksAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error toggling task completion: {ex.Message}");
            }
        }

        // Property for UI binding to show filter labels
        public bool ShowFilterLabels => false; // Will be controlled by hover state in XAML

        // Formatted properties for UI
        public string PeriodDisplayText => SelectedPeriod switch
        {
            "today" => "Today",
            "week" => "This Week",
            "month" => "This Month",
            "year" => "This Year",
            "all" => "All Time",
            _ => "Custom"
        };

        public string PriorityDisplayText => SelectedPriority switch
        {
            "critical" => "Critical",
            "high" => "High",
            "medium" => "Medium",
            "low" => "Low",
            _ => "All"
        };

        public string CategoryDisplayText => SelectedCategory switch
        {
            "work" => "Work",
            "personal" => "Personal",
            "health" => "Health",
            "education" => "Education",
            "shopping" => "Shopping",
            "finance" => "Finance",
            "entertainment" => "Entertainment",
            "other" => "Other",
            _ => "All"
        };

        public string ViewModeDisplayText => SelectedViewMode switch
        {
            "list" => "List",
            "grid" => "Grid",
            "calendar" => "Calendar",
            "timeline" => "Timeline",
            _ => "List"
        };
    }
}
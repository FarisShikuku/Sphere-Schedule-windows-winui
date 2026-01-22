using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sphere_Schedule_App.Core.Interfaces;
using Sphere_Schedule_App.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sphere_Schedule_App.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly IUnitOfWork _unitOfWork;
        private TaskFilterPanelViewModel _taskFilterPanelViewModel;

        public DashboardViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            LoadDashboardDataCommand = new AsyncRelayCommand(LoadDashboardDataAsync);
            CurrentDate = DateTime.Now;
        }

        [ObservableProperty]
        private bool _isLoading = true;

        [ObservableProperty]
        private User _currentUser;

        [ObservableProperty]
        private DateTime _currentDate;

        [ObservableProperty]
        private ObservableCollection<DashboardStatCard> _statCards = new();

        [ObservableProperty]
        private ObservableCollection<Appointment> _todayAppointments = new();

        [ObservableProperty]
        private ObservableCollection<UserTask> _recentCompletedTasks = new();

        [ObservableProperty]
        private DashboardSummary _summary = new();

        [ObservableProperty]
        private ObservableCollection<CategoryProgress> _categoryProgress = new();

        // Task Filter Panel ViewModel property
        public TaskFilterPanelViewModel TaskFilterPanelViewModel
        {
            get
            {
                if (_taskFilterPanelViewModel == null)
                {
                    _taskFilterPanelViewModel = new TaskFilterPanelViewModel(_unitOfWork);
                }
                return _taskFilterPanelViewModel;
            }
        }

        public IUnitOfWork UnitOfWork => _unitOfWork;
        public IAsyncRelayCommand LoadDashboardDataCommand { get; }

        public async Task LoadDashboardDataAsync()
        {
            IsLoading = true;

            try
            {
                // Get first available user (or create temp if none)
                var users = await _unitOfWork.Users.GetAllAsync();
                CurrentUser = users.FirstOrDefault();

                if (CurrentUser == null)
                {
                    // Create temporary user
                    CurrentUser = new User
                    {
                        UserID = Guid.NewGuid(),
                        Email = $"temp_{Guid.NewGuid():N}@temp.sphereschedule.com",
                        Username = "tempuser",
                        DisplayName = "Temporary User",
                        AccountType = "free",
                        IsActive = true,
                        Preferences = @"{""theme"":""light"",""timezone"":""UTC""}"
                    };

                    await _unitOfWork.Users.AddAsync(CurrentUser);
                    await _unitOfWork.CompleteAsync();
                }

                if (CurrentUser != null)
                {
                    // Initialize task filter panel with current user
                    await TaskFilterPanelViewModel.InitializeAsync(CurrentUser);

                    // Load other dashboard data
                    await LoadSummaryAsync();
                    await LoadTodayAppointmentsAsync();
                    await LoadRecentCompletedTasksAsync();
                    await LoadCategoryProgressAsync();
                    await LoadStatCardsAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading dashboard: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadSummaryAsync()
        {
            var today = DateTime.Today;
            var tasks = await _unitOfWork.UserTasks.FindAsync(t =>
                t.UserID == CurrentUser.UserID &&
                t.IsDeleted == false);

            var todayTasks = tasks.Where(t => t.DueDate?.Date == today);
            var completedTasks = tasks.Where(t => t.Status == "completed");
            var pendingTasks = tasks.Where(t => t.Status == "pending" || t.Status == "in_progress");
            var overdueTasks = tasks.Where(t =>
                t.DueDate < today &&
                (t.Status == "pending" || t.Status == "in_progress"));

            var appointments = await _unitOfWork.Appointments.FindAsync(a =>
                a.UserID == CurrentUser.UserID &&
                a.IsDeleted == false &&
                a.Status == "scheduled");

            var todayAppointments = appointments.Where(a => a.StartDateTime.Date == today);

            Summary = new DashboardSummary
            {
                TotalTasks = tasks.Count(),
                CompletedToday = todayTasks.Count(t => t.Status == "completed"),
                PendingTasks = pendingTasks.Count(),
                OverdueTasks = overdueTasks.Count(),
                TodayAppointments = todayAppointments.Count(),
                ProductivityScore = await CalculateProductivityScoreAsync(),
                CurrentStreak = await GetCurrentStreakAsync()
            };
        }

        private async Task LoadTodayAppointmentsAsync()
        {
            var today = DateTime.Today;
            var appointments = await _unitOfWork.Appointments.FindAsync(a =>
                a.UserID == CurrentUser.UserID &&
                a.IsDeleted == false &&
                a.Status == "scheduled" &&
                a.StartDateTime.Date == today);

            TodayAppointments = new ObservableCollection<Appointment>(appointments
                .OrderBy(a => a.StartDateTime)
                .Take(5));
        }

        private async Task LoadRecentCompletedTasksAsync()
        {
            var weekAgo = DateTime.Today.AddDays(-7);
            var tasks = await _unitOfWork.UserTasks.FindAsync(t =>
                t.UserID == CurrentUser.UserID &&
                t.IsDeleted == false &&
                t.Status == "completed" &&
                t.CompletedAt >= weekAgo);

            RecentCompletedTasks = new ObservableCollection<UserTask>(tasks
                .OrderByDescending(t => t.CompletedAt)
                .Take(5));
        }

        private async Task LoadCategoryProgressAsync()
        {
            var tasks = await _unitOfWork.UserTasks.FindAsync(t =>
                t.UserID == CurrentUser.UserID &&
                t.IsDeleted == false &&
                !string.IsNullOrEmpty(t.Category));

            var categories = await _unitOfWork.Categories.FindAsync(c =>
                c.UserID == CurrentUser.UserID &&
                c.IsDeleted == false);

            var categoryProgress = new List<CategoryProgress>();

            foreach (var category in categories)
            {
                var categoryTasks = tasks.Where(t => t.Category == category.CategoryName);
                var total = categoryTasks.Count();
                var completed = categoryTasks.Count(t => t.Status == "completed");
                var percentage = total > 0 ? (completed * 100.0 / total) : 0;

                categoryProgress.Add(new CategoryProgress
                {
                    CategoryName = category.CategoryName,
                    ColorCode = category.ColorCode,
                    IconName = category.IconName,
                    TotalTasks = total,
                    CompletedTasks = completed,
                    CompletionPercentage = Math.Round(percentage, 1),
                    CompletionPercentageFormatted = $"{Math.Round(percentage, 1)}%"
                });
            }

            CategoryProgress = new ObservableCollection<CategoryProgress>(
                categoryProgress.OrderByDescending(c => c.TotalTasks)
            );
        }

        private async Task LoadStatCardsAsync()
        {
            var today = DateTime.Today;
            var tasks = await _unitOfWork.UserTasks.FindAsync(t =>
                t.UserID == CurrentUser.UserID &&
                t.IsDeleted == false);

            var appointments = await _unitOfWork.Appointments.FindAsync(a =>
                a.UserID == CurrentUser.UserID &&
                a.IsDeleted == false);

            var productivityScore = await CalculateProductivityScoreAsync();

            var statCards = new ObservableCollection<DashboardStatCard>
            {
                new DashboardStatCard
                {
                    Title = "Total Tasks",
                    Value = tasks.Count().ToString(),
                    Subtitle = "All time",
                    Icon = "📋",
                    Color = "#2196F3",
                    ChangePercentage = "+12%"
                },
                new DashboardStatCard
                {
                    Title = "Completed Today",
                    Value = tasks.Count(t => t.Status == "completed" && t.CompletedAt?.Date == today).ToString(),
                    Subtitle = "Daily completion",
                    Icon = "✅",
                    Color = "#4CAF50",
                    ChangePercentage = "+8%"
                },
                new DashboardStatCard
                {
                    Title = "Upcoming Appointments",
                    Value = appointments.Count(a => a.StartDateTime > DateTime.Now && a.Status == "scheduled").ToString(),
                    Subtitle = "This week",
                    Icon = "📅",
                    Color = "#9C27B0",
                    ChangePercentage = "+5%"
                },
                new DashboardStatCard
                {
                    Title = "Productivity Score",
                    Value = $"{productivityScore}%",
                    Subtitle = "7-day average",
                    Icon = "📈",
                    Color = "#FF9800",
                    ChangePercentage = "+3%"
                },
                new DashboardStatCard
                {
                    Title = "Overdue Tasks",
                    Value = tasks.Count(t => t.DueDate < DateTime.Today && (t.Status == "pending" || t.Status == "in_progress")).ToString(),
                    Subtitle = "Needs attention",
                    Icon = "⏰",
                    Color = "#F44336",
                    ChangePercentage = "-2%"
                },
                new DashboardStatCard
                {
                    Title = "Work Hours",
                    Value = "45h",
                    Subtitle = "This week",
                    Icon = "⏱️",
                    Color = "#795548",
                    ChangePercentage = "+15%"
                }
            };

            StatCards = statCards;
        }

        private async Task<int> CalculateProductivityScoreAsync()
        {
            var weekAgo = DateTime.Today.AddDays(-7);
            var stats = await _unitOfWork.DailyStats.FindAsync(s =>
                s.UserID == CurrentUser.UserID &&
                s.StatDate >= weekAgo &&
                s.ProductivityScore.HasValue);

            return stats.Any() ? (int)stats.Average(s => s.ProductivityScore.Value) : 75;
        }

        private async Task<int> GetCurrentStreakAsync()
        {
            var today = DateTime.Today;
            var streak = 0;

            // Check last 30 days for streak
            for (int i = 0; i < 30; i++)
            {
                var date = today.AddDays(-i);
                var stat = await _unitOfWork.DailyStats.FirstOrDefaultAsync(s =>
                    s.UserID == CurrentUser.UserID &&
                    s.StatDate == date &&
                    s.CompletedTasks > 0);

                if (stat != null)
                {
                    streak++;
                }
                else if (i > 0) // Break if we found at least one day but current day has no completion
                {
                    break;
                }
            }

            return streak;
        }

        // Method to refresh task panel data
        public async Task RefreshTaskPanelAsync()
        {
            if (TaskFilterPanelViewModel != null)
            {
                await TaskFilterPanelViewModel.LoadTasksAsync();
            }
        }
    }

    // Supporting Classes
    public class DashboardSummary
    {
        public int TotalTasks { get; set; }
        public int CompletedToday { get; set; }
        public int PendingTasks { get; set; }
        public int OverdueTasks { get; set; }
        public int TodayAppointments { get; set; }
        public int ProductivityScore { get; set; }
        public int CurrentStreak { get; set; }

        // Formatted properties for display
        public string ProductivityScoreFormatted => $"{ProductivityScore}%";
        public string TotalTasksFormatted => TotalTasks.ToString("N0");
        public string CompletedTodayFormatted => CompletedToday.ToString("N0");
        public string TodayAppointmentsFormatted => TodayAppointments.ToString("N0");
    }

    public class DashboardStatCard
    {
        public string Title { get; set; }
        public string Value { get; set; }
        public string Subtitle { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public string ChangePercentage { get; set; }
    }

    public class CategoryProgress
    {
        public string CategoryName { get; set; }
        public string ColorCode { get; set; }
        public string IconName { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public double CompletionPercentage { get; set; }
        public string CompletionPercentageFormatted { get; set; }
    }
}
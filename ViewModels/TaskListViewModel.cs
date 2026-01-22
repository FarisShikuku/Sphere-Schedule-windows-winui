
using Sphere_Schedule_App.Core.Interfaces;
using Sphere_Schedule_App.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sphere_Schedule_App.ViewModels
{
    public class TaskListViewModel
    {
        private readonly IUnitOfWork _unitOfWork;
        public Guid CurrentUserId { get; private set; }

        public ObservableCollection<UserTask> AllTasks { get; set; } = new ObservableCollection<UserTask>();
        public bool IsLoading { get; set; }
        public string ErrorMessage { get; set; }

        public TaskListViewModel(IUnitOfWork unitOfWork, Guid currentUserId)
        {
            _unitOfWork = unitOfWork;
            CurrentUserId = currentUserId;  // Changed from _currentUserId
        }


        public async Task LoadTasksAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;
                AllTasks.Clear();

                if (_unitOfWork == null || CurrentUserId == Guid.Empty)
                {
                    ErrorMessage = "Database connection error";
                    return;
                }

                // Get all tasks for current user that are not deleted
                var tasks = await _unitOfWork.UserTasks.GetAllAsync();
                var userTasks = tasks
                    .Where(t => t.UserID == CurrentUserId && !t.IsDeleted)
                    .OrderByDescending(t => t.CreatedAt) // Newest first
                    .ToList();

                foreach (var task in userTasks)
                {
                    AllTasks.Add(task);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading tasks: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Helper methods for formatting
        public string FormatDueDateTime(UserTask task)
        {
            if (!task.DueDate.HasValue)
                return "No due date";

            if (task.DueTime.HasValue)
            {
                return $"{task.DueDate.Value:MMM dd, yyyy} at {task.DueTime.Value:hh\\:mm}";
            }

            return $"{task.DueDate.Value:MMM dd, yyyy}";
        }

        public string GetPriorityColor(string priority)
        {
            return priority?.ToLower() switch
            {
                "critical" => "#D32F2F", // Red
                "high" => "#F57C00",     // Orange
                "medium" => "#1976D2",   // Blue
                "low" => "#388E3C",      // Green
                _ => "#757575"           // Gray
            };
        }

        public async Task DeleteTaskAsync(UserTask task)
        {
            try
            {
                if (_unitOfWork == null)
                    return;

                // Soft delete
                task.IsDeleted = true;
                task.DeletedAt = DateTime.UtcNow;

                await _unitOfWork.UserTasks.UpdateAsync(task);
                await _unitOfWork.CompleteAsync();

                // Remove from list
                AllTasks.Remove(task);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error deleting task: {ex.Message}";
            }
        }

        public async Task ToggleTaskCompletionAsync(UserTask task)
        {
            try
            {
                if (_unitOfWork == null)
                    return;

                if (task.Status == "completed")
                {
                    task.Status = "pending";
                    task.CompletionPercentage = 0;
                    task.CompletedAt = null;
                }
                else
                {
                    task.Status = "completed";
                    task.CompletionPercentage = 100;
                    task.CompletedAt = DateTime.UtcNow;
                }

                task.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.UserTasks.UpdateAsync(task);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error updating task: {ex.Message}";
            }
        }
        // Add these properties to TaskListViewModel.cs
        public int TotalTasks => AllTasks.Count;
        public int CompletedTasks => AllTasks.Count(t => t.Status == "completed");
        public int PendingTasks => AllTasks.Count(t => t.Status != "completed");
        public bool HasTasks => AllTasks.Count > 0;
    }
}

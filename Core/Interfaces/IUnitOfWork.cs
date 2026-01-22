using Sphere_Schedule_App.Core.Models;
using System;
using System.Threading.Tasks;

namespace Sphere_Schedule_App.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> Users { get; }
        IRepository<UserTask> UserTasks { get; } // Changed from Tasks
        IRepository<Category> Categories { get; }
        IRepository<Subtask> Subtasks { get; }
        IRepository<Appointment> Appointments { get; }
        IRepository<Reminder> Reminders { get; }
        IRepository<Participant> Participants { get; }
        IRepository<DailyStat> DailyStats { get; }
        IRepository<SystemSetting> SystemSettings { get; }
        IRepository<UserSetting> UserSettings { get; }
        IRepository<ActivityLog> ActivityLogs { get; }

        Task<int> CompleteAsync();
        Task InitializeDatabaseAsync();
    }
}
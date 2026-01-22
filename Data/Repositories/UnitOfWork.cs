using Microsoft.Extensions.DependencyInjection;
using Sphere_Schedule_App.Core.Interfaces;
using Sphere_Schedule_App.Core.Models;
using Sphere_Schedule_App.Data.LocalDb;
using System;
using System.Threading.Tasks;

namespace Sphere_Schedule_App.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext _context;
        private readonly IServiceProvider _serviceProvider;

        public UnitOfWork(DatabaseContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
            System.Diagnostics.Debug.WriteLine("UnitOfWork created");
        }

        private IRepository<User> _users;
        public IRepository<User> Users => _users ??= _serviceProvider.GetService<IRepository<User>>();

        private IRepository<UserTask> _userTasks;
        public IRepository<UserTask> UserTasks => _userTasks ??= _serviceProvider.GetService<IRepository<UserTask>>();

        private IRepository<Category> _categories;
        public IRepository<Category> Categories => _categories ??= _serviceProvider.GetService<IRepository<Category>>();

        private IRepository<Subtask> _subtasks;
        public IRepository<Subtask> Subtasks => _subtasks ??= _serviceProvider.GetService<IRepository<Subtask>>();

        private IRepository<Appointment> _appointments;
        public IRepository<Appointment> Appointments => _appointments ??= _serviceProvider.GetService<IRepository<Appointment>>();

        private IRepository<Reminder> _reminders;
        public IRepository<Reminder> Reminders => _reminders ??= _serviceProvider.GetService<IRepository<Reminder>>();

        private IRepository<Participant> _participants;
        public IRepository<Participant> Participants => _participants ??= _serviceProvider.GetService<IRepository<Participant>>();

        private IRepository<DailyStat> _dailyStats;
        public IRepository<DailyStat> DailyStats => _dailyStats ??= _serviceProvider.GetService<IRepository<DailyStat>>();

        private IRepository<SystemSetting> _systemSettings;
        public IRepository<SystemSetting> SystemSettings => _systemSettings ??= _serviceProvider.GetService<IRepository<SystemSetting>>();

        private IRepository<UserSetting> _userSettings;
        public IRepository<UserSetting> UserSettings => _userSettings ??= _serviceProvider.GetService<IRepository<UserSetting>>();

        private IRepository<ActivityLog> _activityLogs;
        public IRepository<ActivityLog> ActivityLogs => _activityLogs ??= _serviceProvider.GetService<IRepository<ActivityLog>>();

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task InitializeDatabaseAsync()
        {
            await _context.InitializeDatabaseAsync();
        }

        public void Dispose()
        {
            _context?.Dispose();
            System.Diagnostics.Debug.WriteLine("UnitOfWork disposed");
        }
    }
}
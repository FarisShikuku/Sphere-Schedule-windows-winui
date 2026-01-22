using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sphere_Schedule_App.Core.Interfaces;
using Sphere_Schedule_App.Data.LocalDb;
using Sphere_Schedule_App.Data.Repositories;
using System;
using System.IO;

namespace Sphere_Schedule_App.Services
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services)
        {
            // Get the database path
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "SphereSchedule",
                "sphere_schedule.db");

            // Ensure directory exists
            var dbDirectory = Path.GetDirectoryName(dbPath);
            if (!Directory.Exists(dbDirectory))
            {
                Directory.CreateDirectory(dbDirectory);
                System.Diagnostics.Debug.WriteLine($"Created database directory: {dbDirectory}");
            }

            System.Diagnostics.Debug.WriteLine($"Database path: {dbPath}");

            // Register DatabaseContext WITH configuration
            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseSqlite($"Data Source={dbPath}");
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
                options.LogTo(message => System.Diagnostics.Debug.WriteLine($"EF Core: {message}"));
            });

            // Register generic repository
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Register UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        public static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Add data services
            services.AddDataServices();

            // Build and return service provider
            return services.BuildServiceProvider();
        }
    }
}
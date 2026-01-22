
﻿using Microsoft.EntityFrameworkCore;
using Sphere_Schedule_App.Core.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Sphere_Schedule_App.Data.LocalDb
{
    public class DatabaseContext : DbContext
    {
        // Constructor for DI
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            System.Diagnostics.Debug.WriteLine("DatabaseContext created via DI");
        }

        // Parameterless constructor for migrations/EnsureCreated
        public DatabaseContext()
        {
            System.Diagnostics.Debug.WriteLine("DatabaseContext created parameterless");
        }

        // All DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<UserTask> UserTasks { get; set; }
        public DbSet<Subtask> Subtasks { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<DailyStat> DailyStats { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<UserSetting> UserSettings { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }

        // Static path for reference
        public static string DatabasePath =>
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "SphereSchedule",
                "sphere_schedule.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            System.Diagnostics.Debug.WriteLine("DatabaseContext.OnModelCreating started");

            // User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserID);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Configure nullable string properties
                entity.Property(e => e.Username).IsRequired(false);
                entity.Property(e => e.PasswordHash).IsRequired(false);
                entity.Property(e => e.PasswordSalt).IsRequired(false);
                entity.Property(e => e.DisplayName).IsRequired(false);
                entity.Property(e => e.FirstName).IsRequired(false);
                entity.Property(e => e.LastName).IsRequired(false);
                entity.Property(e => e.PhoneNumber).IsRequired(false);
                entity.Property(e => e.AvatarURL).IsRequired(false);
                entity.Property(e => e.GoogleID).IsRequired(false);
                entity.Property(e => e.MicrosoftID).IsRequired(false);
                entity.Property(e => e.FacebookID).IsRequired(false);

                // Configure default values
                entity.Property(e => e.EmailVerified).HasDefaultValue(false);
                entity.Property(e => e.TwoFactorEnabled).HasDefaultValue(false);
                entity.Property(e => e.LockoutEnabled).HasDefaultValue(false);
                entity.Property(e => e.AccessFailedCount).HasDefaultValue(0);
                entity.Property(e => e.AccountType).HasDefaultValue("free");
                entity.Property(e => e.Preferences).HasDefaultValue(@"{
                    ""theme"": ""light"",
                    ""timezone"": ""UTC"",
                    ""language"": ""en"",
                    ""notificationSettings"": {
                        ""email"": true,
                        ""push"": true,
                        ""sms"": false
                    },
                    ""workHours"": {
                        ""start"": ""09:00"",
                        ""end"": ""17:00""
                    },
                    ""weekStartDay"": 1
                }");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            });

            // UserTask
            modelBuilder.Entity<UserTask>(entity =>
            {
                entity.HasKey(e => e.TaskID);
                entity.HasIndex(e => e.UserID);
                entity.HasIndex(e => e.DueDate);
                entity.HasIndex(e => e.Status);

                // Configure default values
                entity.Property(e => e.Status).HasDefaultValue("pending");
                entity.Property(e => e.PriorityLevel).HasDefaultValue("medium");
                entity.Property(e => e.Category).HasDefaultValue("unspecified");
                entity.Property(e => e.TaskType).HasDefaultValue("general");
                entity.Property(e => e.CompletionPercentage).HasDefaultValue(0);
                entity.Property(e => e.TimeSpentMinutes).HasDefaultValue(0);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.ExternalSyncStatus).HasDefaultValue("not_synced");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Configure nullable properties
                entity.Property(e => e.Description).IsRequired(false);
                entity.Property(e => e.LocationName).IsRequired(false);
                entity.Property(e => e.LocationAddress).IsRequired(false);
                entity.Property(e => e.RecurrenceRule).IsRequired(false);
                entity.Property(e => e.ExternalID).IsRequired(false);
                entity.Property(e => e.ExternalSource).IsRequired(false);
                entity.Property(e => e.Tags).IsRequired(false);
                entity.Property(e => e.Notes).IsRequired(false);

                // Configure nullable value types
                entity.Property(e => e.DueDate).IsRequired(false);
                entity.Property(e => e.StartDate).IsRequired(false);
                entity.Property(e => e.EndDate).IsRequired(false);
                entity.Property(e => e.CompletedAt).IsRequired(false);
                entity.Property(e => e.DeletedAt).IsRequired(false);
                entity.Property(e => e.Latitude).IsRequired(false);
                entity.Property(e => e.Longitude).IsRequired(false);
                entity.Property(e => e.EstimatedDurationMinutes).IsRequired(false);
                entity.Property(e => e.ActualDurationMinutes).IsRequired(false);
                entity.Property(e => e.ParentTaskID).IsRequired(false);

                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Category
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryID);
                entity.HasIndex(e => new { e.UserID, e.CategoryName }).IsUnique();

                // Configure default values
                entity.Property(e => e.CategoryType).HasDefaultValue("custom");
                entity.Property(e => e.ColorCode).HasDefaultValue("#4CAF50");
                entity.Property(e => e.CategoryOrder).HasDefaultValue(0);
                entity.Property(e => e.IsDefault).HasDefaultValue(false);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Configure nullable properties
                entity.Property(e => e.Description).IsRequired(false);
                entity.Property(e => e.IconName).IsRequired(false);

                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Appointment
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.AppointmentID);

                // Configure default values
                entity.Property(e => e.AppointmentType).HasDefaultValue("general");
                entity.Property(e => e.AllDayEvent).HasDefaultValue(false);
                entity.Property(e => e.IsVirtual).HasDefaultValue(false);
                entity.Property(e => e.Status).HasDefaultValue("scheduled");
                entity.Property(e => e.ReminderMinutesBefore).HasDefaultValue(15);
                entity.Property(e => e.IsRecurring).HasDefaultValue(false);
                entity.Property(e => e.CalendarColor).HasDefaultValue("#2196F3");
                entity.Property(e => e.ExternalSyncStatus).HasDefaultValue("not_synced");
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Configure nullable properties
                entity.Property(e => e.Description).IsRequired(false);
                entity.Property(e => e.Location).IsRequired(false);
                entity.Property(e => e.MeetingLink).IsRequired(false);
                entity.Property(e => e.MeetingPlatform).IsRequired(false);
                entity.Property(e => e.RecurrencePattern).IsRequired(false);
                entity.Property(e => e.ExternalEventID).IsRequired(false);
                entity.Property(e => e.Notes).IsRequired(false);
                entity.Property(e => e.DeletedAt).IsRequired(false);

                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Subtask
            modelBuilder.Entity<Subtask>(entity =>
            {
                entity.HasKey(e => e.SubtaskID);

                // Configure default values
                entity.Property(e => e.Status).HasDefaultValue("pending");
                entity.Property(e => e.Priority).HasDefaultValue("medium");
                entity.Property(e => e.SubtaskOrder).HasDefaultValue(0);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Configure nullable properties
                entity.Property(e => e.Description).IsRequired(false);

                // Configure nullable value types
                entity.Property(e => e.DueDate).IsRequired(false);
                entity.Property(e => e.CompletedAt).IsRequired(false);

                entity.HasOne(e => e.UserTask)
                      .WithMany()
                      .HasForeignKey(e => e.TaskID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Reminder
            modelBuilder.Entity<Reminder>(entity =>
            {
                entity.HasKey(e => e.ReminderID);

                // Configure default values
                entity.Property(e => e.NotifyViaEmail).HasDefaultValue(true);
                entity.Property(e => e.NotifyViaPush).HasDefaultValue(true);
                entity.Property(e => e.Status).HasDefaultValue("pending");
                entity.Property(e => e.IsRecurring).HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Configure nullable properties
                entity.Property(e => e.Message).IsRequired(false);
                entity.Property(e => e.SentAt).IsRequired(false);

                // Configure nullable foreign keys
                entity.Property(e => e.TaskID).IsRequired(false);
                entity.Property(e => e.AppointmentID).IsRequired(false);

                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.UserTask)
                      .WithMany()
                      .HasForeignKey(e => e.TaskID)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Appointment)
                      .WithMany()
                      .HasForeignKey(e => e.AppointmentID)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Participant
            modelBuilder.Entity<Participant>(entity =>
            {
                entity.HasKey(e => e.ParticipantID);

                // Configure default values
                entity.Property(e => e.InvitationStatus).HasDefaultValue("pending");
                entity.Property(e => e.ParticipantRole).HasDefaultValue("attendee");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Configure nullable properties
                entity.Property(e => e.FullName).IsRequired(false);
                entity.Property(e => e.ResponseReceivedAt).IsRequired(false);

                // Configure nullable foreign key
                entity.Property(e => e.UserID).IsRequired(false);

                entity.HasOne(e => e.Appointment)
                      .WithMany()
                      .HasForeignKey(e => e.AppointmentID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserID)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // DailyStat
            modelBuilder.Entity<DailyStat>(entity =>
            {
                entity.HasKey(e => e.StatID);

                // Configure default values
                entity.Property(e => e.TotalTasks).HasDefaultValue(0);
                entity.Property(e => e.CompletedTasks).HasDefaultValue(0);
                entity.Property(e => e.IncompleteTasks).HasDefaultValue(0);
                entity.Property(e => e.OverdueTasks).HasDefaultValue(0);
                entity.Property(e => e.PersonalTasks).HasDefaultValue(0);
                entity.Property(e => e.JobTasks).HasDefaultValue(0);
                entity.Property(e => e.UnspecifiedTasks).HasDefaultValue(0);
                entity.Property(e => e.AppointmentTasks).HasDefaultValue(0);
                entity.Property(e => e.TotalAppointments).HasDefaultValue(0);
                entity.Property(e => e.CompletedAppointments).HasDefaultValue(0);
                entity.Property(e => e.CancelledAppointments).HasDefaultValue(0);
                entity.Property(e => e.CurrentStreakDays).HasDefaultValue(0);
                entity.Property(e => e.CalculatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Configure nullable property
                entity.Property(e => e.ProductivityScore).IsRequired(false);

                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // SystemSetting
            modelBuilder.Entity<SystemSetting>(entity =>
            {
                entity.HasKey(e => e.SettingID);
                entity.HasIndex(e => e.SettingKey).IsUnique();

                // Configure default values
                entity.Property(e => e.SettingType).HasDefaultValue("string");
                entity.Property(e => e.IsEditable).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Configure nullable properties
                entity.Property(e => e.SettingValue).IsRequired(false);
                entity.Property(e => e.Category).IsRequired(false);
                entity.Property(e => e.Description).IsRequired(false);
            });

            // UserSetting
            modelBuilder.Entity<UserSetting>(entity =>
            {
                entity.HasKey(e => e.UserSettingID);

                // Configure default values
                entity.Property(e => e.SettingType).HasDefaultValue("string");
                entity.Property(e => e.LastModifiedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Configure nullable property
                entity.Property(e => e.SettingValue).IsRequired(false);

                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ActivityLog
            modelBuilder.Entity<ActivityLog>(entity =>
            {
                entity.HasKey(e => e.LogID);

                // Configure default values
                entity.Property(e => e.Status).HasDefaultValue("success");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Configure nullable properties
                entity.Property(e => e.EntityType).IsRequired(false);
                entity.Property(e => e.IPAddress).IsRequired(false);
                entity.Property(e => e.UserAgent).IsRequired(false);
                entity.Property(e => e.Details).IsRequired(false);

                // Configure nullable foreign key
                entity.Property(e => e.UserID).IsRequired(false);
                entity.Property(e => e.EntityID).IsRequired(false);

                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserID)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            System.Diagnostics.Debug.WriteLine("DatabaseContext.OnModelCreating completed");
        }

        public async Task InitializeDatabaseAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("InitializeDatabaseAsync started");

                // Check if database file exists
                bool databaseExists = File.Exists(DatabasePath);

                if (databaseExists)
                {
                    System.Diagnostics.Debug.WriteLine($"Database exists at: {DatabasePath}");

                    try
                    {
                        // Check if database can be opened and has the expected schema
                        await Database.OpenConnectionAsync();

                        // Check if Users table exists (as a proxy for schema existence)
                        var connection = Database.GetDbConnection();
                        var command = connection.CreateCommand();
                        command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='Users'";

                        if (connection.State != System.Data.ConnectionState.Open)
                            await connection.OpenAsync();

                        var result = await command.ExecuteScalarAsync();

                        if (result != null && result.ToString() == "Users")
                        {
                            System.Diagnostics.Debug.WriteLine("Database schema exists, skipping creation");

                            // Check if we need to seed
                            await SeedIfNeededAsync();
                            return;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Database exists but schema is invalid, will recreate");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error checking schema: {ex.Message}");
                        System.Diagnostics.Debug.WriteLine("Will recreate database");
                    }
                    finally
                    {
                        await Database.CloseConnectionAsync();
                    }

                    // Delete invalid database
                    try
                    {
                        File.Delete(DatabasePath);
                        System.Diagnostics.Debug.WriteLine("Deleted invalid database");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error deleting database: {ex.Message}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Database doesn't exist at: {DatabasePath}");
                }

                // Ensure directory exists
                var dbDirectory = Path.GetDirectoryName(DatabasePath);
                if (!Directory.Exists(dbDirectory))
                {
                    Directory.CreateDirectory(dbDirectory);
                    System.Diagnostics.Debug.WriteLine($"Created database directory: {dbDirectory}");
                }

                // Create database with correct schema
                await Database.EnsureCreatedAsync();
                System.Diagnostics.Debug.WriteLine("Database.EnsureCreatedAsync completed");

                // Seed initial data
                await SeedIfNeededAsync();

                System.Diagnostics.Debug.WriteLine("InitializeDatabaseAsync completed successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database initialization error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private async Task SeedIfNeededAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Checking if seeding is needed...");

                // Check if we need to seed system settings
                if (!await SystemSettings.AnyAsync())
                {
                    System.Diagnostics.Debug.WriteLine("Seeding system settings...");
                    await SeedSystemSettingsAsync();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("System settings already exist, skipping seed");
                }

                // Check if we need to seed a user
                if (!await Users.AnyAsync())
                {
                    System.Diagnostics.Debug.WriteLine("Seeding initial user...");
                    await SeedInitialUserAsync();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("User already exists, skipping user seed");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Seeding error: {ex.Message}");
                // Don't throw - seeding failures shouldn't break the app
            }
        }

        private async Task SeedSystemSettingsAsync()
        {
            try
            {
                var settings = new[]
                {
                    new SystemSetting
                    {
                        SettingID = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                        SettingKey = "app.name",
                        SettingValue = "Sphere Schedule",
                        SettingType = "string",
                        Category = "general",
                        Description = "Application name displayed throughout the app",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new SystemSetting
                    {
                        SettingID = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                        SettingKey = "app.version",
                        SettingValue = "2.0.0",
                        SettingType = "string",
                        Category = "general",
                        Description = "Current application version number",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };

                await SystemSettings.AddRangeAsync(settings);
                await SaveChangesAsync();
                System.Diagnostics.Debug.WriteLine("System settings seeded");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"System settings seeding error: {ex.Message}");
            }
        }

        private async Task SeedInitialUserAsync()
        {
            var user = new User
            {
                UserID = Guid.NewGuid(),
                Email = "user@sphereschedule.com",
                Username = "defaultuser",
                DisplayName = "Default User",
                AccountType = "free",
                IsActive = true,
                EmailVerified = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                // All nullable properties can remain null - they are properly configured as nullable
                // in both the model and the entity configuration above
            };

            await Users.AddAsync(user);
            await SaveChangesAsync();
            System.Diagnostics.Debug.WriteLine($"Seeded initial user: {user.Email}");
        }

        public async Task<bool> CheckDatabaseHealthAsync()
        {
            try
            {
                if (!File.Exists(DatabasePath))
                    return false;

                await Database.OpenConnectionAsync();
                var connection = Database.GetDbConnection();

                if (connection.State != System.Data.ConnectionState.Open)
                    await connection.OpenAsync();

                // Simple query to check if database is functional
                var command = connection.CreateCommand();
                command.CommandText = "SELECT 1";
                var result = await command.ExecuteScalarAsync();

                return result != null && result.ToString() == "1";
            }
            catch
            {
                return false;
            }
            finally
            {
                await Database.CloseConnectionAsync();
            }
        }
    }
}

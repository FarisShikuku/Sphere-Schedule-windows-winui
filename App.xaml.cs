
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// Added for database
using Sphere_Schedule_App.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Sphere_Schedule_App
{
    public partial class App : Application
    {
        private Window? _window;

        // Added: Static properties for database
        private static IServiceProvider? _serviceProvider;
        private static string? _databaseError;

        // Publicly accessible main window instance for WinUI3 desktop apps
        public static Window? MainWindowInstance { get; private set; }

        // Added: Public access to service provider and database status
        public static IServiceProvider? ServiceProvider => _serviceProvider;
        public static string? DatabaseError => _databaseError;
        public static bool IsDatabaseReady => string.IsNullOrEmpty(_databaseError);

        public App()
        {
            InitializeComponent();

            // Added: Configure services on app startup
            try
            {
                _serviceProvider = ServiceRegistration.ConfigureServices();
            }
            catch (Exception ex)
            {
                _databaseError = $"Service configuration failed: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Service configuration error: {ex.Message}");
            }
        }

        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            // Added: Initialize database first
            await InitializeDatabaseAsync();

            // Original code preserved exactly
            _window = new MainWindow();
            MainWindowInstance = _window; // <-- set static property so controls can access XamlRoot
            _window.Activate();
        }

        // Added: Database initialization method (new)
        private async Task InitializeDatabaseAsync()
        {
            try
            {
                if (_serviceProvider == null)
                {
                    _databaseError = "Service provider not configured";
                    return;
                }

                using (var scope = _serviceProvider.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<Core.Interfaces.IUnitOfWork>();

                    // Try to initialize with retry logic
                    bool success = false;
                    int retryCount = 0;
                    const int maxRetries = 3;

                    while (!success && retryCount < maxRetries)
                    {
                        try
                        {
                            await unitOfWork.InitializeDatabaseAsync();
                            success = true;
                            System.Diagnostics.Debug.WriteLine($"Database initialized successfully on attempt {retryCount + 1}");
                        }
                        catch (Exception ex)
                        {
                            retryCount++;
                            System.Diagnostics.Debug.WriteLine($"Database initialization attempt {retryCount} failed: {ex.Message}");

                            if (retryCount >= maxRetries)
                            {
                                _databaseError = $"Database initialization failed after {maxRetries} attempts: {ex.Message}";
                                // Don't throw - let the app start in offline mode
                                return;
                            }

                            // Wait before retrying
                            await Task.Delay(1000 * retryCount);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _databaseError = $"Database initialization failed: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Database initialization error: {ex.Message}");
                // Don't throw - let the app start without database
            }
        }

        // Added: Helper to get XamlRoot from anywhere (new)
        public static XamlRoot? GetAppXamlRoot()
        {
            return MainWindowInstance?.Content?.XamlRoot;
        }

        // Added: Helper to check if we can show dialogs (new)
        public static bool CanShowDialogs()
        {
            return MainWindowInstance != null && MainWindowInstance.Content?.XamlRoot != null;
        }
    }
}

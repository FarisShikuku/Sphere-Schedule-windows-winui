using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Sphere_Schedule_App.Services
{
    public class NavigationService
    {
        private static Frame _mainFrame;
        private static NavigationService _instance;
        private readonly Dictionary<string, Type> _pages = new();

        public event EventHandler<Type> Navigated;
        public event EventHandler<bool> IsPaneOpenChanged;

        public static NavigationService Instance => _instance ??= new NavigationService();

        private static MainWindow _mainWindow;

        public static void SetMainWindow(MainWindow window)
        {
            _mainWindow = window;
        }

        public void NavigateToMainApp()
        {
            _mainWindow?.SwitchToMainApp();
        }

        public void Initialize(Frame frame)
        {
            _mainFrame = frame;
            if (_mainFrame != null)
            {
                _mainFrame.Navigated += Frame_Navigated;
            }
        }

        public void Configure(string key, Type pageType)
        {
            if (_pages.ContainsKey(key))
                _pages[key] = pageType;
            else
                _pages.Add(key, pageType);
        }

        public void NavigateTo(string pageKey, object parameter = null)
        {
            if (_pages.TryGetValue(pageKey, out var pageType))
            {
                Navigate(pageType, parameter);
            }
            else
            {
                Debug.WriteLine($"NavigationService Error: Page key '{pageKey}' not found");
            }
        }

        private void Frame_Navigated(object sender, Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            Navigated?.Invoke(this, e.SourcePageType);
        }

        public bool CanGoBack => _mainFrame?.CanGoBack ?? false;

        public void GoBack()
        {
            if (CanGoBack)
            {
                _mainFrame.GoBack();
            }
        }

        public bool Navigate(Type pageType, object parameter = null)
        {
            try
            {
                if (_mainFrame == null)
                {
                    Debug.WriteLine("NavigationService Error: MainFrame is not initialized");
                    return false;
                }

                return _mainFrame.Navigate(pageType, parameter);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"NavigationService Error: {ex.Message}");
                return false;
            }
        }

        public bool Navigate<T>(object parameter = null) where T : Page
        {
            return Navigate(typeof(T), parameter);
        }

        public void ClearHistory()
        {
            _mainFrame?.BackStack.Clear();
        }

        public Type CurrentPageType => _mainFrame?.CurrentSourcePageType;

        public void ToggleNavigationPane()
        {
            IsPaneOpenChanged?.Invoke(this, !IsPaneOpen);
        }

        public bool IsPaneOpen { get; set; } = true;
    }
}
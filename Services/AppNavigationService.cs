using System;

namespace Sphere_Schedule_App.Services
{
    public class AppNavigationService
    {
        private static AppNavigationService _instance;
        private Action _switchToMainAppCallback;

        public static AppNavigationService Instance => _instance ??= new AppNavigationService();

        public void RegisterMainWindow(Action switchToMainAppCallback)
        {
            _switchToMainAppCallback = switchToMainAppCallback;
        }

        public void SwitchToMainApp()
        {
            _switchToMainAppCallback?.Invoke();
        }
    }
}
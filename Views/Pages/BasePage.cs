using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Sphere_Schedule_App.Views.Pages
{
    public class BasePage : Page
    {
        public BasePage()
        {
            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;
        }

        protected virtual void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Page loaded logic
        }

        protected virtual void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // Page unloaded logic
        }

        protected void ShowFeatureNotImplemented()
        {
            var dialog = new ContentDialog
            {
                Title = "Feature Coming Soon",
                Content = "This feature is not yet implemented. It will be available in a future update.",
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };
            _ = dialog.ShowAsync();
        }
    }
}
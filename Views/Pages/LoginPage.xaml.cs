using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Sphere_Schedule_App.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.


using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;

namespace Sphere_Schedule_App.Views.Pages
{
    public sealed partial class LoginPage : Page
    {
        private readonly Random _random = new Random();
        private DispatcherTimer? _particleTimer;
        private DispatcherTimer? _orbAnimationTimer;
        private double _orb1Angle = 0;
        private double _orb2Angle = Math.PI / 2;
        private double _orb3Angle = Math.PI;

        public LoginPage()
        {
            this.InitializeComponent();
            InitializeParticles();
            StartAnimations();
        }

        private void InitializeParticles()
        {
            if (ParticleCanvas == null) return;

            // Create floating particles
            for (int i = 0; i < 15; i++)
            {
                CreateParticle();
            }

            _particleTimer = new DispatcherTimer();
            _particleTimer.Interval = TimeSpan.FromMilliseconds(50);
            _particleTimer.Tick += (s, e) => AnimateParticles();
            _particleTimer.Start();
        }

        private void CreateParticle()
        {
            if (ParticleCanvas == null) return;

            var size = _random.Next(3, 8);
            var ellipse = new Ellipse
            {
                Width = size,
                Height = size,
                Fill = new SolidColorBrush(GetRandomColor()),
                Opacity = _random.NextDouble() * 0.3 + 0.1
            };

            // Set initial position
            Canvas.SetLeft(ellipse, _random.Next(0, (int)ParticleCanvas.ActualWidth));
            Canvas.SetTop(ellipse, _random.Next(0, (int)ParticleCanvas.ActualHeight));

            // Store velocity in Tag
            ellipse.Tag = new Vector2(
                (float)(_random.NextDouble() * 2 - 1),
                (float)(_random.NextDouble() * 2 - 1)
            );

            ParticleCanvas.Children.Add(ellipse);
        }

        private Color GetRandomColor()
        {
            var colors = new[]
            {
                Color.FromArgb(255, 67, 97, 238),    // Blue
                Color.FromArgb(255, 76, 201, 240),   // Green
                Color.FromArgb(255, 255, 209, 102),  // Yellow
                Color.FromArgb(255, 114, 9, 183),    // Purple
                Color.FromArgb(255, 247, 37, 133)    // Pink
            };
            return colors[_random.Next(colors.Length)];
        }

        private void AnimateParticles()
        {
            if (ParticleCanvas == null) return;

            foreach (var child in ParticleCanvas.Children)
            {
                if (child is Ellipse particle && particle.Tag is Vector2 velocity)
                {
                    // Update position
                    var left = Canvas.GetLeft(particle) + velocity.X;
                    var top = Canvas.GetTop(particle) + velocity.Y;

                    // Boundary check
                    if (left < 0 || left > ParticleCanvas.ActualWidth - particle.Width)
                        velocity.X *= -1;
                    if (top < 0 || top > ParticleCanvas.ActualHeight - particle.Height)
                        velocity.Y *= -1;

                    Canvas.SetLeft(particle, left);
                    Canvas.SetTop(particle, top);
                    particle.Tag = velocity;

                    // Pulsing opacity effect
                    particle.Opacity = 0.1 + Math.Sin(DateTime.Now.Ticks * 0.0001 + particle.Width) * 0.2;
                }
            }
        }

        private void StartAnimations()
        {
            _orbAnimationTimer = new DispatcherTimer();
            _orbAnimationTimer.Interval = TimeSpan.FromMilliseconds(30);
            _orbAnimationTimer.Tick += (s, e) => AnimateOrbs();
            _orbAnimationTimer.Start();
        }

        private void AnimateOrbs()
        {
            // Animate floating orbs
            _orb1Angle += 0.01;
            _orb2Angle += 0.015;
            _orb3Angle += 0.02;
        }

        #region Event Handlers

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            ShowLoading(true);

            // Simulate login process
            await Task.Delay(800);

            ShowLoading(false);

            if (!string.IsNullOrEmpty(EmailTextBox.Text) && !string.IsNullOrEmpty(PasswordBox.Password))
            {
                ShowNotification("Login feature not yet implemented", "Please use 'Skip for Now' to continue");
            }
            else
            {
                ShowNotification("Please enter credentials", "Email and password are required");
            }
        }

        private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            ShowNotification("Account Creation", "This feature is not yet implemented");
        }

        private void GoogleLoginButton_Click(object sender, RoutedEventArgs e)
        {
            ShowNotification("Google Login", "Social login features coming soon");
        }

        private void MicrosoftLoginButton_Click(object sender, RoutedEventArgs e)
        {
            ShowNotification("Microsoft Login", "Social login features coming soon");
        }

        private void AppleLoginButton_Click(object sender, RoutedEventArgs e)
        {
            ShowNotification("Apple Login", "Social login features coming soon");
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            // Get MainWindow from XamlRoot
            AppNavigationService.Instance.SwitchToMainApp();
        }

        private void ForgotPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            ShowNotification("Password Recovery", "Password reset feature coming soon");
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            ShowNotification("Help & Support", "Support documentation coming soon");
        }

        private void EmailTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                PasswordBox.Focus(FocusState.Programmatic);
            }
        }

        private void PasswordBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                LoginButton_Click(sender, e);
            }
        }

        #endregion

        #region Helper Methods

        private void ShowNotification(string title, string message)
        {
            NotImplementedTip.Title = title;
            NotImplementedTip.Subtitle = message;
            NotImplementedTip.IsOpen = true;
        }

        private void ShowLoading(bool show)
        {
            LoadingOverlay.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            LoginButton.IsEnabled = !show;
        }

        #endregion

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            _particleTimer?.Stop();
            _orbAnimationTimer?.Stop();
        }
    }
}
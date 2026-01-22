using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Sphere_Schedule_App.ViewModels;
using System;
using Microsoft.UI.Xaml.Media;
using Windows.UI; // ✅ Correct namespace for Color in WinUI 3

namespace Sphere_Schedule_App.Views.Pages;

public sealed partial class MeetingsPage : Page
{
    public MeetingsViewModel ViewModel { get; } = new();

    public MeetingsPage()
    {
        this.InitializeComponent();
        this.DataContext = ViewModel;
        ViewModel.LoadSampleData(); // Call it here instead
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        // Refresh data when navigating to the page
        // ViewModel.LoadSampleData();
    }
}

// Helper class for platform items
public class PlatformItem
{
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public Color Color { get; set; }   // ✅ Corrected type for WinUI 3
}

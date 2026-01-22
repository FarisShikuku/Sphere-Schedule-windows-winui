using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System; // Add this for DateTime
using System.Collections.Generic; // Add this for List<>

using System.Linq; // Add this for Concat


namespace Sphere_Schedule_App.ViewModels;

public partial class MeetingsViewModel : ObservableObject
{
    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private DateTime selectedDate = DateTime.Today;

    [ObservableProperty]
    private string selectedFilter = "All";

    [ObservableProperty]
    private bool isCompactView = false;

    public ObservableCollection<MeetingItem> Meetings { get; } = new();
    public ObservableCollection<MeetingItem> TodayMeetings { get; } = new();
    public ObservableCollection<MeetingItem> UpcomingMeetings { get; } = new();

    public List<string> FilterOptions { get; } = new()
    {
        "All",
        "Today",
        "Upcoming",
        "Past",
        "Critical",
        "In Progress"
    };

    public MeetingsViewModel()
    {
        LoadSampleData();
    }
    public string MeetingCountText => $"{TodayMeetings.Count} meetings";

    public void LoadSampleData()
    {
        // Clear collections first
        TodayMeetings.Clear();
        UpcomingMeetings.Clear();
        Meetings.Clear();

        // Today's meetings
        TodayMeetings.Add(new MeetingItem
        {
            Id = 1,
            Title = "Team Standup",
            Description = "Daily standup meeting with development team",
            StartTime = DateTime.Today.AddHours(9),
            EndTime = DateTime.Today.AddHours(9).AddMinutes(30),
            Status = "In Progress",
            Priority = "High",
            Platform = "Google Meet",
            Participants = 8,
            IsRecurring = true
        });

        TodayMeetings.Add(new MeetingItem
        {
            Id = 2,
            Title = "Client Presentation",
            Description = "Quarterly review with important client",
            StartTime = DateTime.Today.AddHours(14),
            EndTime = DateTime.Today.AddHours(15).AddMinutes(30),
            Status = "Scheduled",
            Priority = "Critical",
            Platform = "Zoom",
            Participants = 15,
            HasAttachments = true
        });

        // Upcoming meetings
        UpcomingMeetings.Clear();
        UpcomingMeetings.Add(new MeetingItem
        {
            Id = 3,
            Title = "Sprint Planning",
            Description = "Plan next sprint tasks and assignments",
            StartTime = DateTime.Today.AddDays(1).AddHours(10),
            EndTime = DateTime.Today.AddDays(1).AddHours(12),
            Status = "Scheduled",
            Priority = "High",
            Platform = "Microsoft Teams",
            Participants = 12,
            IsRecurring = true
        });

        UpcomingMeetings.Add(new MeetingItem
        {
            Id = 4,
            Title = "Product Review",
            Description = "Review new features and bug fixes",
            StartTime = DateTime.Today.AddDays(2).AddHours(11),
            EndTime = DateTime.Today.AddDays(2).AddHours(13),
            Status = "Scheduled",
            Priority = "Normal",
            Platform = "Google Meet",
            Participants = 6
        });

        // Combine all meetings
        Meetings.Clear();
        foreach (var meeting in TodayMeetings.Concat(UpcomingMeetings))
        {
            Meetings.Add(meeting);
        }
    }

    [RelayCommand]
    private void CreateMeeting()
    {
        // Will be implemented later
        System.Diagnostics.Debug.WriteLine("Create meeting clicked");
    }

    [RelayCommand]
    private void JoinMeeting(object parameter)
    {
        if (parameter is MeetingItem meeting)
        {
            // Will be implemented later
            System.Diagnostics.Debug.WriteLine($"Joining meeting: {meeting.Title}");
        }
    }

    [RelayCommand]
    private void EditMeeting(object parameter)
    {
        if (parameter is MeetingItem meeting)
        {
            // Will be implemented later
            System.Diagnostics.Debug.WriteLine($"Editing meeting: {meeting.Title}");
        }
    }

    [RelayCommand]
    private void DeleteMeeting(object parameter)
    {
        if (parameter is MeetingItem meeting)
        {
            // Will be implemented later
            System.Diagnostics.Debug.WriteLine($"Deleting meeting: {meeting.Title}");
        }
    }

    [RelayCommand]
    private void ToggleViewMode()
    {
        IsCompactView = !IsCompactView;
    }

    [RelayCommand]
    private void ConnectToPlatform(string platform)
    {
        // Will be implemented later
        System.Diagnostics.Debug.WriteLine($"Connecting to {platform}");
    }
}

public class MeetingItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = "Scheduled";
    public string Priority { get; set; } = "Normal";
    public string Platform { get; set; } = "Custom";
    public int Participants { get; set; }
    public bool IsRecurring { get; set; }
    public bool HasAttachments { get; set; }
    public string Duration => (EndTime - StartTime).TotalMinutes + " min";
    public string TimeRange => $"{StartTime:hh:mm tt} - {EndTime:hh:mm tt}";
    

}

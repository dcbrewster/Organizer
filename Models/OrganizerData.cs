using Organizer.Models;

/// <summary>
/// Represents the data structure for the organizer application, including events, contacts, tasks,
/// notes, anniversaries, and user preferences.
/// </summary>
public sealed class OrganizerData
{
    public List<CalendarEvent> Events { get; set; } = new List<CalendarEvent>();
    public List<Contact> Contacts { get; set; } = new List<Contact>();
    public List<OrganizerTask> Tasks { get; set; } = new List<OrganizerTask>();
    public List<Note> Notes { get; set; } = new List<Note>();
    public List<Anniversary> Anniversaries { get; set; } = new List<Anniversary>();
    public OrganizerPreferences Preferences { get; set; } = new();
}
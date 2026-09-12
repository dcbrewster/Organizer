namespace Organizer;

/// <summary>
/// Represents the data structure for the organizer application, including events, contacts, tasks,
/// notes, anniversaries, and user preferences.
/// </summary>
public sealed class OrganizerData
{
    public List<CalendarEvent> Events { get; set; } = [];
    public List<Contact> Contacts { get; set; } = [];
    public List<OrganizerTask> Tasks { get; set; } = [];
    public List<Note> Notes { get; set; } = [];
    public List<Anniversary> Anniversaries { get; set; } = [];
    public OrganizerPreferences Preferences { get; set; } = new();
}

/// <summary>
/// Represents the user preferences for the organizer application, including settings for web browser,
/// </summary>
public sealed class OrganizerPreferences
{
    public string WebBrowser { get; set; } = "System default";
    public bool UseFirewall { get; set; }
    public string ProxyServer { get; set; } = string.Empty;
    public int ProxyPort { get; set; } = 80;
    public string ProxyBypassDomains { get; set; } = string.Empty;
    public string FavoriteAlarmTune { get; set; } = "Default";
    public bool DisplayMissedAlarms { get; set; } = true;
    public string OrganizerFilesPath { get; set; } = string.Empty;
    public string PaperLayoutsPath { get; set; } = string.Empty;
    public string CustomSmartIconsPath { get; set; } = string.Empty;
    public string BackupsPath { get; set; } = string.Empty;
    public bool AnimatedPageTurn { get; set; } = true;
    public string MousePointer { get; set; } = "Color";
    public string WeekStartsOn { get; set; } = "Sunday";
    public bool MuteOrganizerSounds { get; set; }
    public bool AutoCompleteContactNames { get; set; } = true;
    public bool AutomaticallyOpen { get; set; } = true;
    public string AutomaticallyOpenPath { get; set; } = string.Empty;
    public bool AlwaysStartWithNewOrganizerFile { get; set; }
    public string BaseNewOrganizersOnPath { get; set; } = string.Empty;
    public bool CreateBackupWhenClosed { get; set; } = true;
    public string PrinterName { get; set; } = string.Empty;
    public string PaperSize { get; set; } = "Letter";
    public string PaperSource { get; set; } = "Automatically Select";
    public bool PrinterLandscape { get; set; }
    public int MarginLeft { get; set; } = 100;
    public int MarginRight { get; set; } = 100;
    public int MarginTop { get; set; } = 100;
    public int MarginBottom { get; set; } = 100;
}

/// <summary>
/// Represents a calendar event in the organizer application, including details such as title, start and end times,
/// </summary>
public sealed class CalendarEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public DateTime Start { get; set; } = DateTime.Today.AddHours(9);
    public DateTime End { get; set; } = DateTime.Today.AddHours(10);
    public string Location { get; set; } = string.Empty;
    public string Categories { get; set; } = string.Empty;
    public bool WarnOfConflicts { get; set; } = true;
    public bool PencilIn { get; set; }
    public bool Confidential { get; set; }
    public bool AlarmEnabled { get; set; }
    public int AlarmAmount { get; set; } = 15;
    public string AlarmUnit { get; set; } = "Minutes";
    public string AlarmTiming { get; set; } = "Before";
    public string AlarmTune { get; set; } = "Default";
    public DateTime AlarmDate { get; set; } = DateTime.Today;
    public DateTime AlarmTime { get; set; } = DateTime.Today.AddHours(9);
    public string AlarmMessage { get; set; } = string.Empty;
    public string AlarmRunCommand { get; set; } = string.Empty;
    public bool AlarmDisplayDialog { get; set; } = true;
    public string Notes { get; set; } = string.Empty;
}

/// <summary>
/// Represents a contact in the organizer application, including details such as name, company, email,
/// phone number, address, and notes.
/// </summary>
public sealed class Contact
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

/// <summary>
/// Represents a task in the organizer application, including details such as title, due date, completion status,
/// priority, repeat settings, and notes.
/// </summary>
public sealed class OrganizerTask
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public DateTime DueDate { get; set; } = DateTime.Today;
    public bool Completed { get; set; }
    public string Priority { get; set; } = "Low";
    public int RepeatEvery { get; set; }
    public string RepeatUnit { get; set; } = "None";
    public string Notes { get; set; } = string.Empty;
}

/// <summary>
/// Represents a note in the organizer application, including details such as title, body, update timestamp,
/// </summary>
public sealed class Note
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public DateTime Updated { get; set; } = DateTime.Now;
    public string Body { get; set; } = string.Empty;
    public bool IsChapterHeading { get; set; }
    public Guid? ParentHeadingId { get; set; }
    public int SortOrder { get; set; }
}

public sealed class Anniversary
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.Today;
    public string Type { get; set; } = "Anniversary";
    public string Categories { get; set; } = string.Empty;
    public bool AlarmEnabled { get; set; }
    public int AlarmDaysBefore { get; set; } = 0;
    public string Notes { get; set; } = string.Empty;
}
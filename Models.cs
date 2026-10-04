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
    // UI state persistence
    public string LastSection { get; set; } = "Calendar";
    public string LastCalendarView { get; set; } = "Day";

    // Mail & scheduling preferences
    public string MailProgram { get; set; } = "Microsoft Outlook";
    public string MailProtocol { get; set; } = "POP3";
    // Mail tab
    public string MailDefaultFrom { get; set; } = string.Empty;
    public string MailSignature { get; set; } = string.Empty;

    // Legacy scheduling & mail fields captured from org6.exe
    // Scheduling identification
    public bool UseCurrentOrganizerFileToReceiveMessages { get; set; }
    public string OrganizerFilePath { get; set; } = string.Empty;
    // Tracks recently used organizer file paths (most recent first)
    public List<string> RecentFiles { get; set; } = new();
    public string SchedulingName { get; set; } = string.Empty;
    public string SchedulingEmail { get; set; } = string.Empty;
    public List<string> SchedulingForwardingAddresses { get; set; } = [];

    // Connections tab
    public int CheckInboxEveryMinutes { get; set; } = 0;
    // FavoriteAlarmTune already exists above as FavoriteAlarmTune
    public DateTime LastMeetingNoticeDate { get; set; } = DateTime.MinValue;
    public bool RequestConfirmationBeforeProcessingNotices { get; set; }
    public bool DeleteNoticesFromInboxAfterRetrieval { get; set; }

    // Auto-process flags
    public bool AutoProcessChairAcceptances { get; set; }
    public bool AutoProcessChairDeclines { get; set; }
    public bool AutoProcessChairWithMessages { get; set; }
    public bool AutoProcessInviteeInvitations { get; set; }
    public bool AutoProcessInviteeCancellations { get; set; }
    public bool AutoProcessInviteeRescheduling { get; set; }
    public bool AutoProcessInviteeStatusUpdates { get; set; }
    public bool AutoProcessInviteeConfirmations { get; set; }

    // Scheduling tab
    public bool SendMeetingRequests { get; set; } = true;
    public int DefaultReminderMinutes { get; set; } = 15;
    public int DefaultMeetingLengthMinutes { get; set; } = 60;

    // Connections tab
    public string MailServer { get; set; } = string.Empty;
    public int MailServerPort { get; set; } = 110;
    public string MailUsername { get; set; } = string.Empty;
    public string MailPassword { get; set; } = string.Empty;
    public bool MailUseSsl { get; set; }

    // Auto-process tab
    public List<string> AutoProcessRules { get; set; } = [];

    // Busy time tab
    public bool PublishBusyTime { get; set; }
    public string BusyTimePublishUrl { get; set; } = string.Empty;
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
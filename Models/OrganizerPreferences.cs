namespace Organizer.Models;

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
    public List<string> RecentFiles { get; set; } = [];

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
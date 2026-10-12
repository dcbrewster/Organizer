using System;
using System.Collections.Generic;
using System.Text;

namespace Organizer.Models;

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

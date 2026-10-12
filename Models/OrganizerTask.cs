using System;
using System.Collections.Generic;
using System.Text;

namespace Organizer.Models;

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
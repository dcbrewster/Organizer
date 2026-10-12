using System;
using System.Collections.Generic;
using System.Text;

namespace Organizer.Models;

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
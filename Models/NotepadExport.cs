namespace Organizer.Models;

/// <summary>
/// Represents a notepad export with various properties related to notes and their metadata.
/// </summary>
internal class NotepadExport
{
    public bool Confidential { get; set; } = false;
    public string Categories { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public int PageNumber { get; set; }
    public DateTime CreationDateTime { get; set; }
    public string PageType { get; set; } = string.Empty;
    public DateTime UrlLastUpdatedDateTime { get; set; }
    public string UrlName { get; set; } = string.Empty;
}
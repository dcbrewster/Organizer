namespace Organizer.Models;

/// <summary>
/// Represents a calendar export with various properties related to calendar events and their metadata.
/// </summary>
internal class CalendarExport
{
    public bool Confidential { get; set; } = false;
    public string Categories { get; set; } = string.Empty;
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public string Description { get; set; } = string.Empty;
    public string CostCode { get; set; } = string.Empty;
    public string CustomerCode { get; set; } = string.Empty;
    public bool PencilIn { get; set; } = false;
}
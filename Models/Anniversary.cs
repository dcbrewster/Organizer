namespace Organizer.Models;

/// <summary>
/// Represents an anniversary in the organizer application, including details such as description, date, type, categories, alarm settings, and notes.
/// </summary>
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
namespace Organizer.Models;

/// <summary>
/// Represents a to-do export with various properties related to tasks and their metadata.
/// </summary>
internal class ToDoExport
{
    public bool Confidential { get; set; } = false;
    public string Categories { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime DueDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string CostCode { get; set; } = string.Empty;
    public string CustomerCode { get; set; } = string.Empty;
    public int Priority { get; set; }
    public DateTime CompletionDate { get; set; }
    public string Status { get; set; } = string.Empty;
}
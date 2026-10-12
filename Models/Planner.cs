using System;
using System.Collections.Generic;
using System.Text;

namespace Organizer.Models;

internal class Planner
{
    public bool Confidential { get; set; } = false;
    public string Categories { get; set; } = string.Empty;
    public DateTime FromDate { get; set; }
    public DateTime UntilDate { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string CostCode { get; set; } = string.Empty;
    public string CustomerCode { get; set; } = string.Empty;
    public int Row { get; set; }
    public Guid KeyId { get; set; }
}
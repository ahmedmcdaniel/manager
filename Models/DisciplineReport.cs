using System;
using System.Collections.Generic;

namespace SchoolManager.Models;

public partial class DisciplineReport
{
    public Guid Id { get; set; }

    public Guid? StudentId { get; set; }

    public Guid? TeacherId { get; set; }

    public DateOnly Date { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Student? Student { get; set; }

    public virtual User? Teacher { get; set; }
}

using Domain.Base;

namespace Domain.Models;

public class WorkSchedule : BaseEntity
{
    public Guid BusinessId { get; set; }
    public Business Business { get; set; } = default!;
    
    public int TermDurationMinutes { get; set; }
    public DateOnly LastGeneratedThroughDate { get; set; }

    public virtual ICollection<WorkingDay> WorkingDays { get; set; } = new List<WorkingDay>();
}
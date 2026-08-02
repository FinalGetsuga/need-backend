using Domain.Base;
using Domain.Enums;

namespace Domain.Models;

public class Term : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = default!;
    
    public Guid BusinessId { get; set; }
    public Business Business { get; set; } = default!;
    
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public TermStatus Status { get; set; } = TermStatus.Available;
    
    public Booking? Booking { get; set; }
}
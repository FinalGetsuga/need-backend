using Domain.Base;
using Domain.Enums;
using Domain.Identity;

namespace Domain.Models;

public class Booking : BaseEntity
{
    public Guid TermId { get; set; }
    public Term Term { get; set; } = default!;

    public string CustomerId { get; set; } = default!;
    public AppUser Customer { get; set; } = default!;

    public BookingStatus Status { get; set; } = BookingStatus.Confirmed;
    public string? Notes { get; set; }
    public DateTime BookedAt { get; set; } = DateTime.UtcNow;
    
    public Review? Review { get; set; }
}
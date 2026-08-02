using Domain.Base;

namespace Domain.Models;

public class Review : BaseEntity
{
    public Guid BookingId { get; set; }
    public Booking Booking { get; set; } = default!;
    
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
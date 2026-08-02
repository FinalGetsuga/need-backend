namespace Domain.DTO;

public class CreateBookingDto
{
    public Guid TermId { get; set; }
    public string? Notes { get; set; }
}
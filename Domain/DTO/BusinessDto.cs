namespace Domain.DTO;

public class BusinessDto
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    
    public string Address { get; set; } = default!;
    public string? ImageUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    
    public string? OwnerId { get; set; }
    
    public Guid CategoryId { get; set; }
    
    public Guid WorkScheduleId { get; set; }
}
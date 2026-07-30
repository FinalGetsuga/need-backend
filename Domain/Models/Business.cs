using Domain.Base;
using Domain.Identity;

namespace Domain.Models;

public class Business : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    
    public string Address { get; set; } = default!;
    public string? ImageUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    
    public string? OwnerId { get; set; }
    public AppUser? Owner { get; set; }
    
    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }
    
    public Guid WorkScheduleId { get; set; }
    public WorkSchedule? WorkSchedule { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
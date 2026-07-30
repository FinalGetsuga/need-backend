using Domain.Base;

namespace Domain.Models;

public class Category : BaseEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public virtual ICollection<Business> Businesses { get; set; } = new List<Business>();
}
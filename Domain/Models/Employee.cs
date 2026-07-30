using Domain.Base;
using Domain.Identity;

namespace Domain.Models;

public class Employee : BaseEntity
{
    public string? UserId { get; set; }
    public AppUser? User { get; set; }
    
    public Guid BusinessId { get; set; }
    public Business? Business { get; set; }
    
    public bool IsActive { get; set; }
    
    public virtual ICollection<Term> Terms { get; set; } = new List<Term>();
}
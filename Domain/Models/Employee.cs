using Domain.Base;
using Domain.Identity;

namespace Domain.Models;

public class Employee : BaseEntity
{
    public string UserId { get; set; } = default!;
    public AppUser User { get; set; } = default!;
    
    public Guid BusinessId { get; set; }
    public Business Business { get; set; } = default!;
    
    public bool IsActive { get; set; }
    
    public virtual ICollection<Term> Terms { get; set; } = new List<Term>();
}
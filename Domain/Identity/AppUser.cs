using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Domain.Identity;

public class AppUser : IdentityUser
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    
    public Guid BusinessId { get; set; }
    public Business? Business { get; set; }

    public bool IsOwner { get; set; } = false;
    public bool IsEmployee { get; set; } = false;
    public bool IsCustomer { get; set; } = true;
}
namespace Domain.DTO;

public class EmployeeDto
{
    public string? UserId { get; set; }
    
    public Guid BusinessId { get; set; }
    
    public bool IsActive { get; set; }
    
}
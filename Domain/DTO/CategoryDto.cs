namespace Domain.DTO;

public class CategoryDto
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}
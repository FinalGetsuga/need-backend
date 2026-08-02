namespace Domain.DTO;

public class WorkScheduleDto
{
    public Guid BusinessId { get; set; }
    public int TermDurationMinutes { get; set; }
    public List<WorkingDayDto> WorkingDays { get; set; } = new List<WorkingDayDto>();
}
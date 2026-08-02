using Domain.DTO;
using Domain.Models;

namespace Service.Interface;

public interface IWorkScheduleService
{
    Task<List<WorkSchedule>> GetAllAsync();
    Task<WorkSchedule> GetByBusinessIdAsync(Guid businessId);
    Task<WorkSchedule> CreateAsync(string userId, WorkScheduleDto dto);
    Task<WorkSchedule> UpdateAsync(string userId, Guid businessId, WorkScheduleDto dto);
}
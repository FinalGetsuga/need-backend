using Domain.DTO;
using Service.Interface;
using Web.Extensions;
using Web.Requests;
using Web.Responses;

namespace Web.Mappers;

public class WorkScheduleMapper
{
    private readonly IWorkScheduleService _service;

    public WorkScheduleMapper(IWorkScheduleService service)
    {
        _service = service;
    }

    public async Task<WorkScheduleResponse> GetByBusinessIdAsync(Guid businessId)
    {
        var result = await _service.GetByBusinessIdAsync(businessId);
        return result.ToWorkScheduleResponse();
    }

    public async Task<WorkScheduleResponse> CreateAsync(string userId, WorkScheduleRequest request)
    {
        var dto = new WorkScheduleDto
        {
            BusinessId = request.BusinessId,
            TermDurationMinutes = request.TermDurationMinutes,
            WorkingDays = request.WorkingDays.Select(d => new WorkingDayDto
            {
                DayOfWeek = d.DayOfWeek,
                StartTime = d.StartTime,
                EndTime = d.EndTime
            }).ToList()
        };

        var result = await _service.CreateAsync(userId, dto);
        return result.ToWorkScheduleResponse();
    }

    public async Task<WorkScheduleResponse> UpdateAsync(string userId, Guid businessId, WorkScheduleRequest request)
    {
        var dto = new WorkScheduleDto
        {
            BusinessId = request.BusinessId,
            TermDurationMinutes = request.TermDurationMinutes,
            WorkingDays = request.WorkingDays.Select(d => new WorkingDayDto
            {
                DayOfWeek = d.DayOfWeek,
                StartTime = d.StartTime,
                EndTime = d.EndTime
            }).ToList()
        };
        
        var result = await _service.UpdateAsync(userId, businessId, dto);
        return result.ToWorkScheduleResponse();
    }
}
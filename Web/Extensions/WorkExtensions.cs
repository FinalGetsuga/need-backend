using Domain.Models;
using Web.Responses;

namespace Web.Extensions;

public static class WorkExtensions
{
    public static WorkingDayResponse ToWorkingDayResponse(this WorkingDay day)
    {
        return new WorkingDayResponse(
            day.Id,
            day.WorkScheduleId,
            day.DayOfWeek,
            day.StartTime,
            day.EndTime
        );
    }

    public static WorkScheduleResponse ToWorkScheduleResponse(this WorkSchedule schedule)
    {
        return new WorkScheduleResponse(
            schedule.Id,
            schedule.BusinessId,
            schedule.TermDurationMinutes,
            schedule.LastGeneratedThroughDate,
            schedule.WorkingDays.Select(x => x.ToWorkingDayResponse()).ToList()
        );
    }
}
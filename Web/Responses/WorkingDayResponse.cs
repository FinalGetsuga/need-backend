namespace Web.Responses;

public record WorkingDayResponse(
    Guid Id,
    Guid WorkScheduleId,
    DayOfWeek DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime
    );
namespace Web.Responses;

public record WorkScheduleResponse(
    Guid Id,
    Guid BusinessId,
    int TermDurationMinutes,
    DateOnly LastGeneratedThroughDate,
    List<WorkingDayResponse> WorkingDays
    );
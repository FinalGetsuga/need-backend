namespace Web.Requests;

public record WorkScheduleRequest(
    Guid BusinessId,
    int TermDurationMinutes,
    List<WorkingDayRequest> WorkingDays
    );
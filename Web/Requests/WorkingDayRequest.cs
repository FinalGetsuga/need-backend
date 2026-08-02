namespace Web.Requests;

public record WorkingDayRequest(
    DayOfWeek DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime
    );
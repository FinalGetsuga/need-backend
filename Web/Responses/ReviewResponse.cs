namespace Web.Responses;

public record ReviewResponse(
    Guid Id,
    Guid BookingId,
    int Rating,
    string? Comment
    );
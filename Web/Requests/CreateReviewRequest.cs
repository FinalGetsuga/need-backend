namespace Web.Requests;

public record CreateReviewRequest(
    Guid BookingId,
    int Rating,
    string? Comment
    );
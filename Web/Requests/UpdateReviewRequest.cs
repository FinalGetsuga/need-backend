namespace Web.Requests;

public record UpdateReviewRequest(
    int Rating,
    string? Comment
    );
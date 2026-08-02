namespace Web.Requests;

public record BookingRequest(
    Guid TermId,
    string? Notes
    );
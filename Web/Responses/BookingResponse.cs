using Domain.Enums;

namespace Web.Responses;

public record BookingResponse(
    Guid Id,
    Guid TermId,
    string CustomerId,
    BookingStatus Status,
    string? Notes,
    DateTime BookedAt
    );
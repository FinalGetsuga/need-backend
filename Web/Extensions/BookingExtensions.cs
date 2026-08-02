using Domain.Models;
using Web.Responses;

namespace Web.Extensions;

public static class BookingExtensions
{
    public static BookingResponse ToBookingResponse(this Booking booking)
    {
        return new BookingResponse(
            booking.Id,
            booking.TermId,
            booking.CustomerId ?? "",
            booking.Status,
            booking.Notes,
            booking.BookedAt
        );
    }
}
using Domain.Models;
using Web.Responses;

namespace Web.Extensions;

public static class ReviewExtensions
{
    public static ReviewResponse ToReviewResponse(this Review review)
    {
        return new ReviewResponse(
            review.Id,
            review.BookingId,
            review.Rating,
            review.Comment
        );
    }
}
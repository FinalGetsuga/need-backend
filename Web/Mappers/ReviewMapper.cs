using Domain.DTO;
using Service.Interface;
using Web.Extensions;
using Web.Requests;
using Web.Responses;

namespace Web.Mappers;

public class ReviewMapper
{
    private readonly IReviewService _reviewService;

    public ReviewMapper(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    public async Task<ReviewResponse> GetByIdNotNullAsync(Guid id)
    {
        var result = await _reviewService.GetByIdNotNullAsync(id);
        return result.ToReviewResponse();
    }

    public async Task<List<ReviewResponse>> GetAllByBusinessAsync(Guid businessId)
    {
        var result = await _reviewService.GetAllByBusinessAsync(businessId);
        return result.Select(x => x.ToReviewResponse()).ToList();
    }
    
    public async Task<List<ReviewResponse>> GetAllByUserAsync(string userId)
    {
        var result = await _reviewService.GetAllByUserAsync(userId);
        return result.Select(x => x.ToReviewResponse()).ToList();
    }

    public async Task<ReviewResponse> CreateAsync(string userId, CreateReviewRequest request)
    {
        var dto = new CreateReviewDto
        {
            BookingId = request.BookingId,
            Rating = request.Rating,
            Comment = request.Comment
        };
        
        var result = await _reviewService.CreateAsync(userId, dto);
        return result.ToReviewResponse();
    }

    public async Task<ReviewResponse> UpdateAsync(string userId, Guid id, UpdateReviewRequest request)
    {
        var dto = new UpdateReviewDto
        {
            Rating = request.Rating,
            Comment = request.Comment
        };
        
        var result = await _reviewService.UpdateAsync(userId, id, dto);
        return result.ToReviewResponse();
    }

    public async Task<ReviewResponse> DeleteAsync(string userId, Guid id)
    {
        var result = await _reviewService.DeleteAsync(userId, id);
        return result.ToReviewResponse();
    }
    
}
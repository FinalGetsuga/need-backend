using Domain.DTO;
using Service.Interface;
using Web.Extensions;
using Web.Requests;
using Web.Responses;

namespace Web.Mappers;

public class BookingMapper
{
    private readonly IBookingService _bookingService;

    public BookingMapper(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    public async Task<BookingResponse> GetByIdNotNullAsync(Guid id)
    {
        var result = await _bookingService.GetByIdNotNullAsync(id);
        return result.ToBookingResponse();
    }

    public async Task<List<BookingResponse>> GetAllByUserAsync(string userId)
    {
        var result = await _bookingService.GetAllByUserAsync(userId);
        return result.Select(x => x.ToBookingResponse()).ToList();
    }

    public async Task<List<BookingResponse>> GetAllByBusinessAsync(string userId, Guid businessId)
    {
        var result = await _bookingService.GetAllByBusinessAsync(userId, businessId);
        return result.Select(x => x.ToBookingResponse()).ToList();
    }

    public async Task<BookingResponse> CreateAsync(string userId, BookingRequest request)
    {
        var dto = new CreateBookingDto
        {
            TermId = request.TermId,
            Notes = request.Notes
        };
        
        var result = await _bookingService.CreateAsync(userId, dto);
        return result.ToBookingResponse();
    }

    public async Task<BookingResponse> CancelAsync(string userId, Guid id)
    {
        var result = await _bookingService.CancelAsync(userId, id);
        return result.ToBookingResponse();
    }
    
    public async Task<BookingResponse> DeleteAsync(string userId, Guid id)
    {
        var result = await _bookingService.DeleteAsync(userId, id);
        return result.ToBookingResponse();
    }
}
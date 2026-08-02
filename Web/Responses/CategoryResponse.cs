namespace Web.Responses;

public record CategoryResponse(
    Guid Id,
    string Name,
    string Description,
    bool IsActive
    );
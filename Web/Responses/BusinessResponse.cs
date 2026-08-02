namespace Web.Responses;

public record BusinessResponse(
    Guid Id,
    string Name,
    string Description,
    string Address,
    string? ImageUrl,
    string? WebsiteUrl,
    Guid CategoryId
    );
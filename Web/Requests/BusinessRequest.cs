namespace Web.Requests;

public record BusinessRequest(
    string Name,
    string Description,
    string Address,
    string? ImageUrl,
    string? WebsiteUrl,
    Guid CategoryId
    );
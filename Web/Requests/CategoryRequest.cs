namespace Web.Requests;

public record CategoryRequest(
    string Name,
    string Description,
    bool IsActive
    );
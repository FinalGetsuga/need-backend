namespace Web.Responses;

public record EmployeeResponse(
    Guid Id,
    Guid BusinessId,
    string UserId,
    bool IsActive
    );
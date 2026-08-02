namespace Web.Requests;

public record EmployeeRequest(
    Guid BusinessId,
    string UserId,
    bool IsActive
    );
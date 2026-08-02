using Domain.Models;
using Web.Responses;

namespace Web.Extensions;

public static class EmployeeExtensions
{
    public static EmployeeResponse ToEmployeeResponse(this Employee employee)
    {
        return new EmployeeResponse(
            employee.Id,
            employee.BusinessId,
            employee.UserId,
            employee.IsActive
        );
    }
}
namespace Web.Auth;

public interface ICurrentUser
{
    string? UserId { get; }
}
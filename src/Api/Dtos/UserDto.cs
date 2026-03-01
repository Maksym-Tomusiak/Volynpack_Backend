using Domain.Users;

namespace Api.Dtos;

public record UserDto(
    Guid Id,
    string Username,
    string? Email,
    IReadOnlyList<string> Roles)
{
    public static UserDto FromDomainModelAndRoles(User user, IReadOnlyList<string> roles)
        => new(user.Id, user!.UserName, user.Email, roles);
    
    public static UserDto FromDomainModel(User user) => new(user.Id, user!.UserName, user.Email, Array.Empty<string>());
}

public record UserCreateDto(
    string Username,
    string Password,
    string? Email,
    Guid RoleId);

public record UserUpdateDto(
    Guid Id, 
    string? Username, 
    string? Password,
    string? Email,
    Guid RoleId);

public record LoginDto(string Username, string Password);

public record RefreshDto(string RefreshToken);
using System.Security.Claims;
using Application.Users.Exceptions;
using Domain.Roles;
using Domain.Users;
using LanguageExt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Commands;

public record CreateUserCommand(
    string Username, 
    string Password,
    string? Email,
    Guid RoleId);

public class CreateUserCommandHandler
{
    public static async Task<Either<UserException, User>> Handle(
        CreateUserCommand command,
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var sessionUserId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(sessionUserId))
        {
            return new UserIdNotFoundException();
        }
        
        var isSessionUserAdmin = (bool)httpContextAccessor.HttpContext?.User.IsInRole("Admin");
        if (!isSessionUserAdmin)
        {
            return new UserUnauthorizedAccessException("Only admins can create new users.");
        }
        
        var existingUserNameUser = await userManager.FindByNameAsync(command.Username);
        if (existingUserNameUser != null)
        {
            return new UserWithNameAlreadyExistsException(existingUserNameUser.Id);
        }
        
        var user = new User
        {
            UserName = command.Username,
            Email = command.Email,
            EmailConfirmed = false
        };

        var result = await userManager.CreateAsync(user, command.Password);
        if (!result.Succeeded)
        {
            return new InvalidCredentialsException();
        }
        
        var existingRole = await roleManager.FindByIdAsync(command.RoleId.ToString());
        if (existingRole == null)
        {
            return new UserRoleNotFoundException(command.RoleId);
        }
        
        await userManager.AddToRoleAsync(user, existingRole!.Name);

        await userManager.UpdateAsync(user);

        return user;
    }
}
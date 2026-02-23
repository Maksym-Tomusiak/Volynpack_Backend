using System.Security.Claims;
using Application.Users.Exceptions;
using Domain.Roles;
using Domain.Users;
using LanguageExt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Commands;

public record UpdateUserCommand(
    Guid Id,
    string? NewUsername,
    string? NewPassword,
    string? NewEmail,
    Guid RoleId);

public class UpdateUserCommandHandler
{
    public static async Task<Either<UserException, User>> Handle(
        UpdateUserCommand command,
        RoleManager<Role> roleManager,
        IHttpContextAccessor httpContextAccessor,
        UserManager<User> userManager
        , CancellationToken cancellationToken)
    {
        var sessionUserId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(sessionUserId))
        {
            return new UserIdNotFoundException();
        }
        
        var isSessionUserAdmin = (bool)httpContextAccessor.HttpContext?.User.IsInRole("Admin");
        var updatedUser = await userManager.FindByIdAsync(command.Id.ToString());
        if (updatedUser == null)
        {
            return new UserNotFoundException(command.Id);
        }
        var isUpdatedUserAdmin = await userManager.IsInRoleAsync(updatedUser, "Admin");
        if (isSessionUserAdmin && isUpdatedUserAdmin && new Guid(sessionUserId) != updatedUser.Id)
        {
            return new UserUnauthorizedAccessException("Admins cannot update other admin accounts.");
        }
        
        if (!isSessionUserAdmin && new Guid(sessionUserId) != updatedUser.Id)
        {
            return new UserUnauthorizedAccessException("Only admins can update other users.");
        }
        
        var role = await roleManager.FindByIdAsync(command.RoleId.ToString());
        if (role == null)
        {
            return new UserRoleNotFoundException(command.RoleId);
        }
        var isRoleChanged = !userManager.IsInRoleAsync(updatedUser, role!.Name).Result;
        if (isRoleChanged)
        {
            var userRoles = await userManager.GetRolesAsync(updatedUser);
            await userManager.RemoveFromRolesAsync(updatedUser, userRoles);
            await userManager.AddToRoleAsync(updatedUser, role.Name);
        }
        
        if (!string.IsNullOrEmpty(command.NewUsername) && updatedUser.UserName != command.NewUsername)
            updatedUser.UserName = command.NewUsername;
        
        if (!string.IsNullOrEmpty(command.NewEmail) && updatedUser.Email != command.NewEmail)
            updatedUser.Email = command.NewEmail;
        
        if (!string.IsNullOrEmpty(command.NewPassword))
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(updatedUser);
            await userManager.ResetPasswordAsync(updatedUser, token, command.NewPassword);
        }
        await userManager.UpdateAsync(updatedUser);
        
        updatedUser = await userManager.FindByIdAsync(command.Id.ToString());
        return updatedUser;
    }
}
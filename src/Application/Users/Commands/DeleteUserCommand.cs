using System.Security.Claims;
using Application.Users.Exceptions;
using Domain.Users;
using LanguageExt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Commands;

public record DeleteUserCommand(Guid UserId);

public class DeleteUserCommandHandler
{
    public static async Task<Either<UserException, string>> Handle(
        DeleteUserCommand command,
        UserManager<User> userManager,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var sessionUserId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(sessionUserId))
        {
            return new UserIdNotFoundException();
        }

        var userToDelete = await userManager.FindByIdAsync(command.UserId.ToString());
        if (userToDelete == null)
        {
            return new UserNotFoundException(command.UserId);
        }

        var sessionUserIsAdmin = (bool)httpContextAccessor.HttpContext?.User.IsInRole("Admin");
        var targetUserIsAdmin = await userManager.IsInRoleAsync(userToDelete, "Admin");
        
        
        if (sessionUserIsAdmin && targetUserIsAdmin)
        {
            return new UserUnauthorizedAccessException("Admins cannot delete other admin accounts.");
        }

        if (!sessionUserIsAdmin)
        {
            return new UserUnauthorizedAccessException("Only admins can delete other accounts.");
        }

        try
        {
            var result = await userManager.DeleteAsync(userToDelete);
            return !result.Succeeded ? "Could not delete user" : "User account deleted successfully.";
        }
        catch (Exception ex)
        {
            return new UserUnknownException(userToDelete.Id, ex);
        }
    }
}
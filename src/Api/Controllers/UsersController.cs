using Api.Dtos;
using Api.Modules;
using Api.Modules.Errors;
using Application.Common.Models;
using Application.Users.Commands;
using Application.Users.Exceptions;
using Application.Users.Queries;
using Domain.Subscriptions;
using Domain.Users;
using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Api.Controllers;

[ApiController]
public class UsersController(IMessageBus messageBus) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpGet("api/users")]
    public async Task<IResult> GetUsers(CancellationToken cancellationToken)
    {
        var input = new GetAllUsersQuery();
        var items = await messageBus.InvokeAsync<IEnumerable<(User, IReadOnlyList<string>)>>(input, cancellationToken);
        var dtos = items.Select(x => UserDto.FromDomainModelAndRoles(x.Item1, x.Item2));
        return Results.Ok(dtos);
    }
    
    
    
    [Authorize(Roles = "Admin")]
    [HttpGet("api/users/paginated")]
    public async Task<IResult> GetUsersPaginated(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        var input = new GetAllUsersPaginatedQuery(
            pageNumber,
            pageSize,
            searchTerm,
            sortBy,
            sortDescending);
        var result = await messageBus.InvokeAsync<PaginatedResult<(User, IReadOnlyList<string>)>>(input, cancellationToken);
        
        return Results.Ok(PaginatedResult<(User, IReadOnlyList<string>)>
            .MapFrom(result, x 
                => UserDto.FromDomainModelAndRoles(x.Item1, x.Item2)));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("api/users")]
    public async Task<IResult> CreateUser(UserCreateDto request, CancellationToken cancellationToken)
    {
        var cmd = new CreateUserCommand(
            request.Username, 
            request.Password,
            request.Email,
            request.RoleId);
        var res = await messageBus.InvokeAsync<Either<UserException, User>>(cmd, cancellationToken);

        return res.Match<IResult>(
            u => Results.Created($"/api/users/{u.Id}", UserDto.FromDomainModel(u)),
            ex => ex.ToIResult());
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("api/users/{id}")]
    public async Task<IResult> UpdateUser(Guid id, UserUpdateDto request, CancellationToken cancellationToken)
    {
        var cmd = new UpdateUserCommand(
            id, 
            request.Username, 
            request.Password);
        var res = await messageBus.InvokeAsync<Either<UserException, User>>(cmd, cancellationToken);

        return res.Match<IResult>(
            u => Results.Ok(UserDto.FromDomainModel(u)),
            ex => ex.ToIResult());
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("api/users/{id}")]
    public async Task<IResult> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        var cmd = new DeleteUserCommand(id);
        var res = await messageBus.InvokeAsync<Either<UserException, string>>(cmd, cancellationToken);

        return res.Match<IResult>(
            Results.Ok,
            ex => ex.ToIResult());
    }

    [HttpPost("api/users/login")]
    public async Task<IResult> Login(LoginDto request, CancellationToken cancellationToken)
    {
        var cmd = new LoginUserCommand(request.Username, request.Password);
        var res = await messageBus.InvokeAsync<Either<UserException, (string AccessToken, string RefreshToken)>>(cmd,
            cancellationToken);
        
        return res.Match<IResult>(
            tokens =>
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(7)
                };
            
                Response.Cookies.Append("RefreshToken", tokens.RefreshToken, cookieOptions);
                return Results.Ok((object?)tokens.ToTokenResponse());
            },

    ex => ex.ToIResult());
    }
    
    [HttpPost("api/users/refresh-token")]
    public async Task<IResult> Refresh(CancellationToken cancellationToken)
    {
        var cookies = Request.Cookies;
        var cmd = new RefreshUserCommand(cookies.FirstOrDefault(x => x.Key == "RefreshToken").Value);
        var res = await messageBus.InvokeAsync<Either<UserException, string>>(cmd,
            cancellationToken);
        
        return res.Match<IResult>(
            Results.Ok,
            ex => ex.ToIResult());
    }
    
    [HttpPost("api/users/logout")]
    public IResult Logout()
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None
        };

        Response.Cookies.Delete("RefreshToken", cookieOptions);

        return Results.Ok();
    }

    [HttpPost("/subscribe")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> SubscribePost(string email)
    {
        var cmd = new SubscribeEmailCommand(email);
        var result = await messageBus.InvokeAsync<Either<bool, Subscription>>(cmd);
        return result.Match<IActionResult>(
            s => Ok(),
            _ => Conflict());
    }

    [HttpPost("/unsubscribe/{token}")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> UnsubscribePost(Guid token)
    {
        var form = await Request.ReadFormAsync();
        if (form["List-Unsubscribe"] != "One-Click")
        {
            return BadRequest();
        }

        var command = new UnsubscribeEmailCommand(token);
        var result = await messageBus.InvokeAsync<Either<bool, Subscription>>(command);
        return result.Match<IActionResult>(
            s => Ok(),
            _ => BadRequest());
    }

}
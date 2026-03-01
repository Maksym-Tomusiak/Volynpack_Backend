using Api.Dtos;
using Application.Roles.Queries;
using Domain.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Api.Controllers;

[ApiController]
public class RolesController(IMessageBus messageBus) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpGet("api/roles")]
    public async Task<IResult> GetRoles(CancellationToken cancellationToken)
    {
        var input = new GetAllRolesQuery();
        var items = await messageBus.InvokeAsync<IEnumerable<Role>>(input, cancellationToken);
        var dtos = items.Select(RoleDto.FromDomainModel);
        return Results.Ok(dtos);
    }
}
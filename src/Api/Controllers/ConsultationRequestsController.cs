using Api.Dtos;
using Api.Hubs;
using Api.Modules.Errors;
using Application.Common.Models;
using Application.ConsultationRequests.Commands;
using Application.ConsultationRequests.Exceptions;
using Application.ConsultationRequests.Queries;
using Domain.ConsultationRequest;
using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;
using Wolverine;

namespace Api.Controllers;

[ApiController]
[Route("api/consultation-requests")]
public class ConsultationRequestsController(IMessageBus messageBus, IHubContext<NotificationHub> hubContext) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IResult> GetPaginated(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetConsultationRequestsPaginatedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending);
        var result = await messageBus.InvokeAsync<PaginatedResult<ConsultationRequest>>(query, cancellationToken);
        
        return Results.Ok(PaginatedResult<ConsultationRequest>.MapFrom(result, ConsultationRequestDto.FromDomainModel));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id:guid}")]
    public async Task<IResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetConsultationRequestByIdQuery(id);
        var result = await messageBus.InvokeAsync<Either<ConsultationRequestException, ConsultationRequest>>(query, cancellationToken);
        
        return result.Match<IResult>(
            request => Results.Ok(ConsultationRequestDto.FromDomainModel(request)),
            ex => ex.ToIResult());
    }

    [EnableRateLimiting("SensitiveActionPolicy")]
    [HttpPost]
    public async Task<IResult> Create([FromBody] ConsultationRequestCreateDto request, CancellationToken cancellationToken)
    {
        var cmd = new CreateConsultationRequestCommand(request.PhoneNumber);
        var result = await messageBus.InvokeAsync<Either<ConsultationRequestException, ConsultationRequest>>(cmd, cancellationToken);
        
        return await result.MatchAsync<IResult>(
            async req =>
            {
                await hubContext.Clients.All.SendAsync("ConsultationRequestCreated", ConsultationRequestDto.FromDomainModel(req), cancellationToken);
                return Results.Created($"/api/consultation-requests/{req.Id.Value}", ConsultationRequestDto.FromDomainModel(req));
            },
            ex => Task.FromResult(ex.ToIResult()));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    public async Task<IResult> Update(Guid id, [FromBody] ConsultationRequestUpdateDto request, CancellationToken cancellationToken)
    {
        var cmd = new UpdateConsultationRequestCommand(id, request.PhoneNumber, request.IsActive);
        var result = await messageBus.InvokeAsync<Either<ConsultationRequestException, ConsultationRequest>>(cmd, cancellationToken);
        
        return await result.MatchAsync<IResult>(
            async req =>
            {
                await hubContext.Clients.All.SendAsync("ConsultationRequestUpdated", ConsultationRequestDto.FromDomainModel(req), cancellationToken);
                return Results.Ok(ConsultationRequestDto.FromDomainModel(req));
            },
            ex => Task.FromResult(ex.ToIResult()));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var cmd = new DeleteConsultationRequestCommand(id);
        var result = await messageBus.InvokeAsync<Either<ConsultationRequestException, ConsultationRequest>>(cmd, cancellationToken);
        
        return result.Match<IResult>(
            req => Results.Ok(ConsultationRequestDto.FromDomainModel(req)),
            ex => ex.ToIResult());
    }
}
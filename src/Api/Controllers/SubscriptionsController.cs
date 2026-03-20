using Application.Subscriptions.Commands;
using Domain.Subscriptions;
using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController(IMessageBus messageBus) : ControllerBase
{
    [HttpPost("subscribe")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> Subscribe([FromForm] string email)
    {
        var cmd = new SubscribeEmailCommand(email);
        var result = await messageBus.InvokeAsync<Either<bool, Subscription>>(cmd);
        return result.Match<IActionResult>(
            s => Ok(),
            _ => Conflict());
    }

    [HttpGet("unsubscribe/{token}")]
    // Manual unsubscribe endpoint (handles browser GET requests)
    public async Task<IActionResult> Unsubscribe(Guid token, [FromServices] Microsoft.Extensions.Options.IOptions<Application.Common.Models.EmailSettings> emailSettings)
    {
        var command = new UnsubscribeEmailCommand(token);
        var result = await messageBus.InvokeAsync<Either<bool, Subscription>>(command);

        var settings = emailSettings.Value;
        var frontendUrl = settings.BaseFrontendUrl ?? "http://localhost:3000";

        return result.Match<IActionResult>(
            s => Redirect($"{frontendUrl}/unsubscribe"),
            _ => BadRequest("Посилання недійсне або термін дії закінчився."));
    }

    [HttpPost("unsubscribe/{token}")]
    [Consumes("application/x-www-form-urlencoded")]
    // RFC 8058 One-Click Unsubscribe (handles automated POST requests)
    public async Task<IActionResult> UnsubscribePost(Guid token)
    {
        // RFC 8058 specifically looks for this form value
        if (!Request.HasFormContentType || Request.Form["List-Unsubscribe"] != "One-Click")
        {
            return BadRequest();
        }

        var command = new UnsubscribeEmailCommand(token);
        var result = await messageBus.InvokeAsync<Either<bool, Subscription>>(command);
        return result.Match<IActionResult>(
            s => Ok(),
            _ => BadRequest());
    }


    [Authorize(Roles = "Admin")]
    [HttpPost("notify")]
    public async Task<IActionResult> Notify([FromBody] NotifySubscribersCommand command)
    {
        await messageBus.InvokeAsync(command);
        return Ok();
    }
}

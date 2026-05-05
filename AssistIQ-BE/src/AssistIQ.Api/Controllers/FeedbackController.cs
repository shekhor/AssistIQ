using AssistIQ.Api.Contracts;
using AssistIQ.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AssistIQ.Api.Controllers;

[ApiController]
[Route("api/feedback")]
public class FeedbackController : ControllerBase
{
    private readonly TicketService _ticketService;

    public FeedbackController(TicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpPost]
    public IActionResult SubmitFeedback([FromBody] FeedbackRequest request)
    {
        _ticketService.InsertHumanFeedback(
            request.ResponseId,
            request.VerifiedSolutionId,
            request.FeedbackType,
            request.VerifiedBy);

        return Ok(new { ResponseId = request.ResponseId });
    }
}



using AssistIQ.Api.Contracts;
using AssistIQ.Api.Models;
using AssistIQ.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace AssistIQ.Api.Controllers;

[ApiController]
[Route("api/tickets")]
public class TicketController : ControllerBase
{
    private readonly TicketService _ticketService;
    private readonly IAiService _aiService;

    public TicketController(TicketService ticketService, IAiService aiService)
    {
        _ticketService = ticketService;
        _aiService = aiService;
    }

    [HttpPost]
    public IActionResult CreateTicket([FromBody] CreateTicketRequest request)
    {
        var ticketId = _ticketService.CreateExternalTicket(
            request.ExternalSystem,
            request.ExternalTicketRef,
            request.TicketText);

        return Ok(new { TicketId = ticketId });
    }

    [HttpGet]
    public IActionResult GetAllTickets()
    {
        var tickets = _ticketService.GetAllExternalTickets();
        return Ok(tickets);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetTicketById(int id)
    {
        var ticket = _ticketService.GetExternalTicketById(id);
        return ticket is null ? NotFound() : Ok(ticket);
    }

    [HttpGet("{id:int}/ai-response")]
    public IActionResult GetAIResponse(int id)
    {
        var responses = _ticketService.GetAIResponsesByTicketId(id);
        return Ok(responses);
    }

    [HttpPost("{id:int}/ai-response")]
    public IActionResult CreateAIResponse(int id, [FromBody] CreateAIResponseRequest request)
    {
        var responseId = _ticketService.InsertAIResponse(
            id,
            request.ResponseText,
            request.ConfidenceScore);

        return Ok(new { TicketId = id, ResponseId = responseId });
    }

    [HttpPost("{id:int}/ai-generate")]
    public async Task<IActionResult> GenerateAIResponse(int id)
    {
        var ticket = _ticketService.GetExternalTicketById(id);
        if (ticket is null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(ticket.TicketText))
        {
            return BadRequest(new { Message = "Ticket text is required to generate an AI response." });
        }

        var ragResults = _ticketService.GetRelevantVerifiedSolutions(ticket.TicketText);
        var context = BuildRagContext(ragResults);
        var aiResult = await _aiService.GenerateSolutionAsync(ticket.TicketText, context);

        var responseId = _ticketService.InsertAIResponse(
            id,
            aiResult.ResponseText,
            aiResult.ConfidenceScore);

        return Ok(new
        {
            TicketId = id,
            ResponseId = responseId,
            aiResult.ResponseText,
            aiResult.ConfidenceScore
        });
    }

    [HttpGet("{id:int}/rag")]
    public IActionResult GetRelevantSolutions(int id)
    {
        var ticket = _ticketService.GetExternalTicketById(id);
        if (ticket is null)
        {
            return NotFound();
        }

        var results = _ticketService.GetRelevantVerifiedSolutions(ticket.TicketText);
        return Ok(results);
    }

    [HttpPost("{id:int}/verify")]
    public IActionResult SubmitVerification(int id, [FromBody] VerifySolutionRequest request)
    {
        var solutionId = _ticketService.InsertVerifiedSolution(
            id,
            request.VerifiedSolutionText,
            request.VerifiedBy,
            request.FeedbackType);

        return Ok(new { TicketId = id, SolutionId = solutionId });
    }

    private static string? BuildRagContext(IReadOnlyList<VerifiedSolutionRagResult> results)
    {
        if (results.Count == 0)
        {
            return null;
        }

        var builder = new StringBuilder();
        foreach (var result in results)
        {
            builder.Append("- ");
            builder.Append(result.VerifiedSolution);
            builder.Append(" (confidence: ");
            builder.Append(result.ConfidenceScore);
            builder.AppendLine(")");
        }

        return builder.ToString();
    }
}



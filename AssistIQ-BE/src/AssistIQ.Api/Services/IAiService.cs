using AssistIQ.Api.Models;

namespace AssistIQ.Api.Services;

public interface IAiService
{
    Task<AiResult> GenerateSolutionAsync(string ticketText, string? context = null);
}



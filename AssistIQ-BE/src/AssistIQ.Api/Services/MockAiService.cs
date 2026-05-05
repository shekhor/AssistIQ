using AssistIQ.Api.Models;

namespace AssistIQ.Api.Services;

public class MockAiService : IAiService
{
    public Task<AiResult> GenerateSolutionAsync(string ticketText, string? context = null)
    {
        return Task.FromResult(new AiResult
        {
            ResponseText = "Please restart the VPN and verify firewall rules.",
            ConfidenceScore = 0.75m
        });
    }
}



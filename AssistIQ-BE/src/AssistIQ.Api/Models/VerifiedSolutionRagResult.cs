namespace AssistIQ.Api.Models;

public class VerifiedSolutionRagResult
{
    public int SolutionId { get; set; }
    public int TicketId { get; set; }
    public string VerifiedSolution { get; set; } = string.Empty;
    public double ConfidenceScore { get; set; }
    public DateTime CreatedAt { get; set; }
    public string OriginalTicket { get; set; } = string.Empty;
}

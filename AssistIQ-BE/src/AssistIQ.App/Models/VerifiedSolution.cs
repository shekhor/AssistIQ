namespace AssistIQ.App.Models;

public class VerifiedSolution
{
    public int SolutionId { get; set; }
    public int TicketId { get; set; }
    public string VerifiedSolutionText { get; set; } = string.Empty;
    public string VerifiedBy { get; set; } = string.Empty;
    public string FeedbackType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}


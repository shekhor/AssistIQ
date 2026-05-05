namespace AssistIQ.App.Models;

public class HumanFeedback
{
    public int FeedbackId { get; set; }
    public int ResponseId { get; set; }
    public int? VerifiedSolutionId { get; set; }
    public string FeedbackType { get; set; } = string.Empty;
    public string VerifiedBy { get; set; } = string.Empty;
    public DateTime FeedbackAt { get; set; }
}


namespace AssistIQ.Api.Contracts;

public class FeedbackRequest
{
    public int ResponseId { get; set; }
    public int? VerifiedSolutionId { get; set; }
    public string FeedbackType { get; set; } = string.Empty;
    public string VerifiedBy { get; set; } = string.Empty;
}




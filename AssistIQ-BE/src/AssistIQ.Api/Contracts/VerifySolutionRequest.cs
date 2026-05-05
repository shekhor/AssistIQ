namespace AssistIQ.Api.Contracts;

public class VerifySolutionRequest
{
    public string VerifiedSolutionText { get; set; } = string.Empty;
    public string VerifiedBy { get; set; } = string.Empty;
    public string FeedbackType { get; set; } = string.Empty;
}




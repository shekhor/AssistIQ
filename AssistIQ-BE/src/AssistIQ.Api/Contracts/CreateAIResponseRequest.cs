namespace AssistIQ.Api.Contracts;

public class CreateAIResponseRequest
{
    public string ResponseText { get; set; } = string.Empty;
    public decimal? ConfidenceScore { get; set; }
}



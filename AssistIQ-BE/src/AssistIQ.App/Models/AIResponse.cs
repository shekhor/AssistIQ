namespace AssistIQ.App.Models;

public class AIResponse
{
    public int ResponseId { get; set; }
    public int TicketId { get; set; }
    public string ResponseText { get; set; } = string.Empty;
    public decimal? ConfidenceScore { get; set; }
    public DateTime CreatedAt { get; set; }
}


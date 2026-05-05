namespace AssistIQ.App.Models;

public class ExternalTicket
{
    public int TicketId { get; set; }
    public string ExternalSystem { get; set; } = string.Empty;
    public string ExternalTicketRef { get; set; } = string.Empty;
    public string TicketText { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}


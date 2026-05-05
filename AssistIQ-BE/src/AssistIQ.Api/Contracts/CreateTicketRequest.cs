namespace AssistIQ.Api.Contracts;

public class CreateTicketRequest
{
    public string ExternalSystem { get; set; } = string.Empty;
    public string ExternalTicketRef { get; set; } = string.Empty;
    public string TicketText { get; set; } = string.Empty;
}




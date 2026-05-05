CREATE OR ALTER PROCEDURE ai.GetAllExternalTickets
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        TicketId,
        ExternalSystem,
        ExternalTicketRef,
        TicketText,
        CreatedAt
    FROM ai.ExternalTicket
    ORDER BY TicketId;
END;
GO

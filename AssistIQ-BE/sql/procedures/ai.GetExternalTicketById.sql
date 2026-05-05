CREATE OR ALTER PROCEDURE ai.GetExternalTicketById
    @TicketId INT
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
    WHERE TicketId = @TicketId;
END;
GO

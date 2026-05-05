CREATE OR ALTER PROCEDURE ai.InsertExternalTicket
    @ExternalSystem NVARCHAR(50),
    @ExternalTicketRef NVARCHAR(100),
    @TicketText NVARCHAR(MAX),
    @TicketId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO ai.ExternalTicket (ExternalSystem, ExternalTicketRef, TicketText)
    VALUES (@ExternalSystem, @ExternalTicketRef, @TicketText);

    SET @TicketId = SCOPE_IDENTITY();
END;
GO

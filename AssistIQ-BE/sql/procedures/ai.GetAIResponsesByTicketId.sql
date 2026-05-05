CREATE OR ALTER PROCEDURE ai.GetAIResponsesByTicketId
    @TicketId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ResponseId,
        TicketId,
        ResponseText,
        ConfidenceScore,
        CreatedAt
    FROM ai.AIResponse
    WHERE TicketId = @TicketId
    ORDER BY CreatedAt DESC;
END;
GO

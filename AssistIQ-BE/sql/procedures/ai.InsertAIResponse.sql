CREATE OR ALTER PROCEDURE ai.InsertAIResponse
    @TicketId INT,
    @ResponseText NVARCHAR(MAX),
    @ConfidenceScore DECIMAL(9, 4) = NULL,
    @ResponseId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO ai.AIResponse (TicketId, ResponseText, ConfidenceScore)
    VALUES (@TicketId, @ResponseText, @ConfidenceScore);

    SET @ResponseId = SCOPE_IDENTITY();
END;
GO

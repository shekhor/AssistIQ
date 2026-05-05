CREATE OR ALTER PROCEDURE ai.GetRelevantVerifiedSolutions
    @TicketText NVARCHAR(4000),
    @MinConfidence FLOAT = 0.8,
    @TopN INT = 5
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (@TopN)
        vs.SolutionId,
        vs.TicketId,
        vs.VerifiedSolution,
        vs.ConfidenceScore,
        vs.CreatedAt,
        et.TicketText AS OriginalTicket
    FROM ai.VerifiedSolution vs
    JOIN ai.ExternalTicket et ON et.TicketId = vs.TicketId
    WHERE vs.ConfidenceScore >= @MinConfidence
      AND (CONTAINS(et.TicketText, @TicketText) OR CONTAINS(vs.VerifiedSolution, @TicketText))
    ORDER BY vs.CreatedAt DESC;
END;
GO

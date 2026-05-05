CREATE OR ALTER PROCEDURE ai.InsertVerifiedSolution
    @TicketId INT,
    @VerifiedSolutionText NVARCHAR(MAX),
    @VerifiedBy NVARCHAR(100),
    @FeedbackType NVARCHAR(50),
    @SolutionId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO ai.VerifiedSolution (TicketId, VerifiedSolution, VerifiedBy, FeedbackType)
    VALUES (@TicketId, @VerifiedSolutionText, @VerifiedBy, @FeedbackType);

    SET @SolutionId = SCOPE_IDENTITY();
END;
GO

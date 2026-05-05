CREATE OR ALTER PROCEDURE ai.InsertHumanFeedback
    @ResponseId INT,
    @VerifiedSolutionId INT = NULL,
    @FeedbackType NVARCHAR(50),
    @VerifiedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO ai.HumanFeedback (ResponseId, VerifiedSolutionId, FeedbackType, VerifiedBy, FeedbackAt)
    VALUES (@ResponseId, @VerifiedSolutionId, @FeedbackType, @VerifiedBy, SYSDATETIME());
END;
GO

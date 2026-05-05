using System.Data;
using AssistIQ.App.Data;
using AssistIQ.App.Models;
using AssistIQ.Api.Models;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AssistIQ.Api.Services;

public class TicketService
{
    private readonly AssistIQContext _context;

    public TicketService(AssistIQContext context)
    {
        _context = context;
    }

    public int CreateExternalTicket(string externalSystem, string externalTicketRef, string ticketText)
    {
        var ticketIdParam = new SqlParameter("@TicketId", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };

        _context.Database.ExecuteSqlRaw(
            "EXEC ai.InsertExternalTicket @ExternalSystem, @ExternalTicketRef, @TicketText, @TicketId OUTPUT",
            new SqlParameter("@ExternalSystem", externalSystem),
            new SqlParameter("@ExternalTicketRef", externalTicketRef),
            new SqlParameter("@TicketText", ticketText),
            ticketIdParam);

        return ticketIdParam.Value is int ticketId ? ticketId : 0;
    }

    public IReadOnlyList<AIResponse> GetAIResponsesByTicketId(int ticketId)
    {
        return _context.AIResponses
            .FromSqlRaw("EXEC ai.GetAIResponsesByTicketId @TicketId",
                new SqlParameter("@TicketId", ticketId))
            .AsNoTracking()
            .ToList();
    }

    public ExternalTicket? GetExternalTicketById(int ticketId)
    {
        return _context.ExternalTickets
            .FromSqlRaw("EXEC ai.GetExternalTicketById @TicketId",
                new SqlParameter("@TicketId", ticketId))
            .AsNoTracking()
            .AsEnumerable()
            .FirstOrDefault();
    }

    public IReadOnlyList<ExternalTicket> GetAllExternalTickets()
    {
        return _context.ExternalTickets
            .FromSqlRaw("EXEC ai.GetAllExternalTickets")
            .AsNoTracking()
            .ToList();
    }

    public IReadOnlyList<VerifiedSolutionRagResult> GetRelevantVerifiedSolutions(string ticketText, double minConfidence = 0.8, int topN = 5)
    {
        return _context.Set<VerifiedSolutionRagResult>()
            .FromSqlRaw(
                "EXEC ai.GetRelevantVerifiedSolutions @TicketText, @MinConfidence, @TopN",
                new SqlParameter("@TicketText", ticketText),
                new SqlParameter("@MinConfidence", minConfidence),
                new SqlParameter("@TopN", topN))
            .AsNoTracking()
            .ToList();
    }

    public int InsertVerifiedSolution(int ticketId, string verifiedSolutionText, string verifiedBy, string feedbackType)
    {
        var solutionIdParam = new SqlParameter("@SolutionId", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };

        _context.Database.ExecuteSqlRaw(
            "EXEC ai.InsertVerifiedSolution @TicketId, @VerifiedSolutionText, @VerifiedBy, @FeedbackType, @SolutionId OUTPUT",
            new SqlParameter("@TicketId", ticketId),
            new SqlParameter("@VerifiedSolutionText", verifiedSolutionText),
            new SqlParameter("@VerifiedBy", verifiedBy),
            new SqlParameter("@FeedbackType", feedbackType),
            solutionIdParam);

        return solutionIdParam.Value is int solutionId ? solutionId : 0;
    }

    public int InsertAIResponse(int ticketId, string responseText, decimal? confidenceScore)
    {
        var responseIdParam = new SqlParameter("@ResponseId", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };

        _context.Database.ExecuteSqlRaw(
            "EXEC ai.InsertAIResponse @TicketId, @ResponseText, @ConfidenceScore, @ResponseId OUTPUT",
            new SqlParameter("@TicketId", ticketId),
            new SqlParameter("@ResponseText", responseText),
            new SqlParameter("@ConfidenceScore", (object?)confidenceScore ?? DBNull.Value),
            responseIdParam);

        return responseIdParam.Value is int responseId ? responseId : 0;
    }

    public void InsertHumanFeedback(int responseId, int? verifiedSolutionId, string feedbackType, string verifiedBy)
    {
        _context.Database.ExecuteSqlRaw(
            "EXEC ai.InsertHumanFeedback @ResponseId, @VerifiedSolutionId, @FeedbackType, @VerifiedBy",
            new SqlParameter("@ResponseId", responseId),
            new SqlParameter("@VerifiedSolutionId", (object?)verifiedSolutionId ?? DBNull.Value),
            new SqlParameter("@FeedbackType", feedbackType),
            new SqlParameter("@VerifiedBy", verifiedBy));
    }
}



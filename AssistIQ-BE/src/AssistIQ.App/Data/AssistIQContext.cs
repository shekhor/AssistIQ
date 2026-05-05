using AssistIQ.App.Models;
using Microsoft.EntityFrameworkCore;

namespace AssistIQ.App.Data;

public class AssistIQContext : DbContext
{
    public AssistIQContext(DbContextOptions<AssistIQContext> options)
        : base(options)
    {
    }

    public DbSet<ExternalTicket> ExternalTickets { get; set; } = default!;
    public DbSet<AIResponse> AIResponses { get; set; } = default!;
    public DbSet<VerifiedSolution> VerifiedSolutions { get; set; } = default!;
    public DbSet<HumanFeedback> HumanFeedbacks { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExternalTicket>().HasKey(x => x.TicketId);
        modelBuilder.Entity<AIResponse>().HasKey(x => x.ResponseId);
        modelBuilder.Entity<VerifiedSolution>().HasKey(x => x.SolutionId);
        modelBuilder.Entity<HumanFeedback>().HasKey(x => x.FeedbackId);

        modelBuilder.HasDefaultSchema("ai");
        base.OnModelCreating(modelBuilder);
    }
}



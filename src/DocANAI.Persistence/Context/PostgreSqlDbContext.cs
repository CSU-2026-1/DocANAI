using DocANAI.Persistence.Entities;
using DocANAI.Persistence.Entities.AIModel;
using DocANAI.Persistence.Entities.Answer;
using DocANAI.Persistence.Entities.Format;
using DocANAI.Persistence.Entities.Priority;
using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.Entities.Question;
using DocANAI.Persistence.Entities.QuestionFile;
using DocANAI.Persistence.Entities.SourceDocument;
using DocANAI.Persistence.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace DocANAI.Persistence.Context;

public sealed class PostgreSqlDbContext(DbContextOptions<PostgreSqlDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<AIModel> AIModels => Set<AIModel>();
    public DbSet<Format> Formats => Set<Format>();
    public DbSet<PriorityLevel> PriorityLevels => Set<PriorityLevel>();
    public DbSet<Priority> Priorities => Set<Priority>();
    public DbSet<ProcessingTask> ProcessingTasks => Set<ProcessingTask>();
    public DbSet<SourceDocument> SourceDocuments => Set<SourceDocument>();
    public DbSet<QuestionFile> QuestionFiles => Set<QuestionFile>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Answer> Answers => Set<Answer>();
    public DbSet<Report> Reports => Set<Report>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
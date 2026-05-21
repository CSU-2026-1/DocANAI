using DocANAI.Persistence.Converters;
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
using DocANAI.Persistence.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DocANAI.Persistence.Context;

public sealed class PostgreSqlDbContext(DbContextOptions<PostgreSqlDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
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

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<IdOf<User>>().HaveConversion<IdOfValueConverter<User>>();
        configurationBuilder.Properties<IdOf<RefreshToken>>().HaveConversion<IdOfValueConverter<RefreshToken>>();
        configurationBuilder.Properties<IdOf<AIModel>>().HaveConversion<IdOfValueConverter<AIModel>>();
        configurationBuilder.Properties<IdOf<Priority>>().HaveConversion<IdOfValueConverter<Priority>>();
        configurationBuilder.Properties<IdOf<ProcessingTask>>().HaveConversion<IdOfValueConverter<ProcessingTask>>();
        configurationBuilder.Properties<IdOf<SourceDocument>>().HaveConversion<IdOfValueConverter<SourceDocument>>();
        configurationBuilder.Properties<IdOf<QuestionFile>>().HaveConversion<IdOfValueConverter<QuestionFile>>();
        configurationBuilder.Properties<IdOf<Question>>().HaveConversion<IdOfValueConverter<Question>>();
        configurationBuilder.Properties<IdOf<Answer>>().HaveConversion<IdOfValueConverter<Answer>>();
        configurationBuilder.Properties<IdOf<Report>>().HaveConversion<IdOfValueConverter<Report>>();
    }
}
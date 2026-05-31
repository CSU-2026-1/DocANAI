using DocANAI.Persistence.Entities;
using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocANAI.Persistence.Configurations;

internal sealed class QuestionEntityTypeConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable(nameof(Question).ToSnakeCase());
        
        builder.HasId();
        builder.HasAudits();

        builder.Property(x => x.Text)
            .IsRequired()
            .HasColumnName(nameof(Question.Text).ToSnakeCase());
        
        builder.Property(x => x.QuestionNumber)
            .IsRequired()
            .HasColumnName(nameof(Question.QuestionNumber).ToSnakeCase());

        builder.Property(x => x.TaskId)
            .IsRequired()
            .HasColumnName(nameof(Question.TaskId).ToSnakeCase());
        
        builder.Property(x => x.QuestionFileId)
            .IsRequired(false)
            .HasColumnName(nameof(Question.QuestionFileId).ToSnakeCase());
        
        builder.HasOne<QuestionFile>()
            .WithMany()
            .HasForeignKey(x => x.QuestionFileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ProcessingTask>()
            .WithMany()
            .HasForeignKey(x => x.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
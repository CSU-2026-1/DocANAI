using DocANAI.Persistence.Entities;
using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocANAI.Persistence.Configurations;

internal sealed class AnswerEntityTypeConfiguration : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.ToTable(nameof(Answer).ToSnakeCase());
        
        builder.HasId();
        builder.HasAudits();

        builder.Property(x => x.Text)
            .IsRequired()
            .HasColumnName(nameof(Answer.Text).ToSnakeCase());
        
        builder.Property(x => x.ModelAccuracy)
            .HasPrecision(4, 2)
            .IsRequired()
            .HasColumnName(nameof(Answer.ModelAccuracy).ToSnakeCase());
        
        builder.Property(x => x.QuestionId)
            .HasColumnName(nameof(Answer.QuestionId).ToSnakeCase());

        builder.HasOne<Question>()
            .WithMany()
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(x => x.TaskId)
            .HasColumnName(nameof(Answer.TaskId).ToSnakeCase());

        builder.HasOne<ProcessingTask>()
            .WithMany()
            .HasForeignKey(x => x.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
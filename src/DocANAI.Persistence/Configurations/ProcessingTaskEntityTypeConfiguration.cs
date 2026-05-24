using DocANAI.Persistence.Entities.AIModel;
using DocANAI.Persistence.Entities.Priority;
using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.Entities.User;
using DocANAI.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocANAI.Persistence.Configurations;

internal sealed class ProcessingTaskConfiguration : IEntityTypeConfiguration<ProcessingTask>
{
    public void Configure(EntityTypeBuilder<ProcessingTask> builder)
    {
        builder.ToTable(nameof(ProcessingTask).ToSnakeCase());
        
        builder.HasId();
        builder.HasAudits();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasColumnName(nameof(ProcessingTask.Status).ToSnakeCase());

        builder.Property(x => x.StartTime)
            .IsRequired()
            .HasColumnName(nameof(ProcessingTask.StartTime).ToSnakeCase());

        builder.Property(x => x.EndTime)
            .IsRequired(false)
            .HasColumnName(nameof(ProcessingTask.EndTime).ToSnakeCase());

        builder.Property(x => x.UserId)
            .HasColumnName(nameof(ProcessingTask.UserId).ToSnakeCase());

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .HasConstraintName("fk_tasks_user");
        
        builder.Property(x => x.ModelId)
            .HasColumnName(nameof(ProcessingTask.ModelId).ToSnakeCase());
        
        builder.HasOne<AIModel>()
            .WithMany()
            .HasForeignKey(t => t.ModelId)
            .HasConstraintName("fk_tasks_model");
        
        builder.Property(x => x.PriorityId)
            .HasColumnName(nameof(ProcessingTask.PriorityId).ToSnakeCase());
        
        builder.HasOne<Priority>()
            .WithMany()
            .HasForeignKey(t => t.PriorityId)
            .HasConstraintName("fk_tasks_priority");
    }
}
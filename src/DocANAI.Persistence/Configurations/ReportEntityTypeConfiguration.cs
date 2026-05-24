using DocANAI.Persistence.Entities;
using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocANAI.Persistence.Configurations;

internal sealed class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.ToTable(nameof(Report).ToSnakeCase());
        builder.HasId();
        builder.HasAudits();

        builder.Property(x => x.Extension)
            .HasMaxLength(20).IsRequired()
            .HasColumnName(nameof(Report.Extension).ToSnakeCase());
        
        builder.HasOne<Format>()
            .WithMany()
            .HasForeignKey(x => x.Extension)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(x => x.DeletionDate)
            .IsRequired(false)
            .HasColumnName(nameof(Report.DeletionDate).ToSnakeCase());

        builder.Property(x => x.TaskId)
            .HasColumnName(nameof(Report.TaskId).ToSnakeCase());
        
        builder.HasOne<ProcessingTask>()
            .WithMany()
            .HasForeignKey(x => x.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
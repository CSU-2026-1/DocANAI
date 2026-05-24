using DocANAI.Persistence.Entities;
using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.Entities.User;
using DocANAI.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocANAI.Persistence.Configurations;

internal sealed class SourceDocumentEntityTypeConfiguration : IEntityTypeConfiguration<SourceDocument>
{
    public void Configure(EntityTypeBuilder<SourceDocument> builder)
    {
        builder.ToTable(nameof(SourceDocument).ToSnakeCase());
        
        builder.HasId();
        builder.HasAudits();

        builder.Property(x => x.Filename)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName(nameof(SourceDocument.Filename).ToSnakeCase());
        
        builder.Property(x => x.Extension)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName(nameof(SourceDocument.Extension).ToSnakeCase());
        
        builder.HasOne<Format>()
            .WithMany()
            .HasForeignKey(x => x.Extension)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(x => x.Size)
            .IsRequired()
            .HasColumnName(nameof(SourceDocument.Size).ToSnakeCase());
        
        builder.Property(x => x.UserId)
            .IsRequired()
            .HasColumnName(nameof(SourceDocument.UserId).ToSnakeCase());

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(x => x.TaskId)
            .HasColumnName(nameof(SourceDocument.TaskId).ToSnakeCase());
        
        builder.HasOne<ProcessingTask>()
            .WithMany()
            .HasForeignKey(x => x.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
using DocANAI.Persistence.Entities.Format;
using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.Entities.QuestionFile;
using DocANAI.Persistence.Entities.User;
using DocANAI.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocANAI.Persistence.Configurations;

public class QuestionFileEntityTypeConfiguration : IEntityTypeConfiguration<QuestionFile>
{
    public void Configure(EntityTypeBuilder<QuestionFile> builder)
    {
        builder.ToTable(nameof(QuestionFile).ToSnakeCase());
        
        builder.HasId();
        builder.HasAudits();

        builder.Property(x => x.Filename)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName(nameof(QuestionFile.Filename).ToSnakeCase());
        
        builder.Property(x => x.Extension)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName(nameof(QuestionFile.Extension).ToSnakeCase());
        
        builder.HasOne<Format>()
            .WithMany()
            .HasForeignKey(x => x.Extension)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(x => x.UserId)
            .HasGuidConversion()
            .HasColumnName(nameof(QuestionFile.UserId).ToSnakeCase());

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(x => x.TaskId)
            .HasGuidConversion()
            .HasColumnName(nameof(QuestionFile.TaskId).ToSnakeCase());
        
        builder.HasOne<ProcessingTask>()
            .WithMany()
            .HasForeignKey(x => x.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
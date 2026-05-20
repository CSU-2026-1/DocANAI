using DocANAI.Persistence.Entities.Format;
using DocANAI.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocANAI.Persistence.Configurations;

public class FormatEntityTypeConfiguration : IEntityTypeConfiguration<Format>
{
    public void Configure(EntityTypeBuilder<Format> builder)
    {
        builder.ToTable(nameof(Format).ToSnakeCase());
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasMaxLength(20)
            .HasColumnName(nameof(Format.Extension).ToSnakeCase());

        builder.Property(x => x.MaxSize)
            .IsRequired()
            .HasColumnName(nameof(Format.MaxSize).ToSnakeCase());

        builder.Property(x => x.MimeType)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName(nameof(Format.MimeType).ToSnakeCase());
    }
}
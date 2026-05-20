using DocANAI.Persistence.Entities.Priority;
using DocANAI.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocANAI.Persistence.Configurations;

internal sealed class PriorityLevelEntityTypeConfiguration : IEntityTypeConfiguration<PriorityLevel>
{
    public void Configure(EntityTypeBuilder<PriorityLevel> builder)
    {
        builder.ToTable(nameof(PriorityLevel).ToSnakeCase());
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasMaxLength(50)
            .HasColumnName(nameof(PriorityLevel.Level).ToSnakeCase());

        builder.Property(x => x.Weight)
            .IsRequired()
            .HasColumnName(nameof(PriorityLevel.Weight).ToSnakeCase());
    }
}

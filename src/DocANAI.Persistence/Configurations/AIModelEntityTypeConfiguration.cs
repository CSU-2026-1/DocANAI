using DocANAI.Persistence.Entities.AIModel;
using DocANAI.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocANAI.Persistence.Configurations;

internal sealed class AIModelEntityTypeConfiguration : IEntityTypeConfiguration<AIModel>
{
    public void Configure(EntityTypeBuilder<AIModel> builder)
    {
        builder.ToTable(nameof(AIModel).ToSnakeCase());
        
        builder.HasId();
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName(nameof(AIModel.Name).ToSnakeCase());
        
        builder.Property(x => x.Version)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName(nameof(AIModel.Version).ToSnakeCase());
        
        builder.Property(x => x.IsActive)
            .HasDefaultValue(false)
            .HasColumnName(nameof(AIModel.IsActive).ToSnakeCase());
        
    }
}
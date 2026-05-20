using DocANAI.Persistence.Entities.Priority;
using DocANAI.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocANAI.Persistence.Configurations;

internal sealed class PriorityEntityTypeConfiguration : IEntityTypeConfiguration<Priority>
{
    public void Configure(EntityTypeBuilder<Priority> builder)
    {
        builder.ToTable(nameof(Priority).ToSnakeCase());
        
        builder.HasId(); 
        
        builder.HasOne(x => x.PriorityLevel)
            .WithMany()
            .HasForeignKey(nameof(Priority.PriorityLevel).ToSnakeCase()) 
            .OnDelete(DeleteBehavior.Restrict);
    }
}
using DocANAI.Persistence.Entities.User;
using DocANAI.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocANAI.Persistence.Configurations;

internal sealed class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(nameof(User).ToSnakeCase());

        builder.HasId();
        builder.HasAudits();

        builder.Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName(nameof(User.Username).ToSnakeCase());

        builder.Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName(nameof(User.PasswordHash).ToSnakeCase());

        builder.Property(x => x.UserType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired()
            .HasColumnName(nameof(User.UserType).ToSnakeCase());
    }
}
using DocANAI.Persistence.ValueObjects;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CSharpFunctionalExtensions;
namespace DocANAI.Persistence.Extensions;

public static class PropertyBuilderExtensions
{
    public static PropertyBuilder<IdOf<TEntity>> HasGuidConversion<TEntity>(
        this PropertyBuilder<IdOf<TEntity>> propertyBuilder)
        where TEntity : Entity<IdOf<TEntity>>
        => propertyBuilder.HasConversion<Guid>(
            id => id,
            value => IdOf<TEntity>.From(value)
        );
}
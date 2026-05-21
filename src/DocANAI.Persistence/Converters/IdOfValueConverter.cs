using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using DocANAI.Persistence.ValueObjects;
using CSharpFunctionalExtensions;

namespace DocANAI.Persistence.Converters;

public class IdOfValueConverter<T> : ValueConverter<IdOf<T>, Guid> where T : Entity<IdOf<T>>
{
    public IdOfValueConverter()
        : base (
            id => id,
            value => IdOf<T>.From(value)
        )
    {  }
}
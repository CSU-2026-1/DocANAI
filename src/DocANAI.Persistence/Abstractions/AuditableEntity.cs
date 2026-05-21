using CSharpFunctionalExtensions;

namespace DocANAI.Persistence.Abstractions;

public class AuditableEntity<TId> : Entity<TId>, IAuditableEntity where TId : IComparable<TId>
{
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; protected set; }

    protected AuditableEntity() {  }
    protected AuditableEntity(TId id) : base(id) {  }
}
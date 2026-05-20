using CSharpFunctionalExtensions;
using DocANAI.Persistence.ValueObjects;
using JetBrains.Annotations;

namespace DocANAI.Persistence.Entities.Priority;

public sealed class Priority : Entity<IdOf<Priority>> 
{

    public PriorityLevel PriorityLevel { get; private set; }
    
    private Priority(IdOf<Priority> id, PriorityLevel priorityLevel)
    {
        Id = id;
        PriorityLevel = priorityLevel;
    }
    
    public static Priority Create(IdOf<Priority> id, PriorityLevel priorityLevel)
    {
        ArgumentNullException.ThrowIfNull(priorityLevel);
        return new Priority(id, priorityLevel);
    }

    
    [UsedImplicitly]
    #pragma warning disable CS8618
    private Priority() { }
    #pragma warning restore CS8618
}
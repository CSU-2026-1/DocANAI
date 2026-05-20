using CSharpFunctionalExtensions;
using JetBrains.Annotations;

namespace DocANAI.Persistence.Entities.Priority;

public sealed class PriorityLevel : Entity<string>
{
    public string Level => Id;
    public int Weight { get; private set; }

    private PriorityLevel(string level, int weight)
    {
        Id = level;
        Weight = weight;
    }
    
    public static PriorityLevel Create(string level, int weight)
    {
        if (string.IsNullOrWhiteSpace(level)) throw new ArgumentException("Priority level shouldn't be  null or empty");
        if (weight < 0) throw new ArgumentException("Weight should be non negative");
        
        return new PriorityLevel(level, weight);
    }

    
    [UsedImplicitly]
    #pragma warning disable CS8618
    private PriorityLevel() { }
    #pragma warning restore CS8618
}
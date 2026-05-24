using CSharpFunctionalExtensions;
using DocANAI.Persistence.ValueObjects;
using JetBrains.Annotations;

namespace DocANAI.Persistence.Entities.AIModel;

public sealed class AIModel : Entity<IdOf<AIModel>>
{
    public string Name { get; private set; }
    public string Version { get; private set; }
    public bool IsAvailable { get; private set; }

    private AIModel(IdOf<AIModel> id, string name, string version, bool isAvailable)
    {
        Id = id;
        Name = name;
        Version = version;
        IsAvailable = isAvailable;
    }

    public static AIModel Create(IdOf<AIModel> id, string name, string version)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        ArgumentException.ThrowIfNullOrWhiteSpace(version, nameof(version));

        return new AIModel(id, name, version, true);
    }

    public void ToggleAvailabilityStatus(bool isAvailable) => IsAvailable = isAvailable;

    /// <summary>
    /// Для EF Core
    /// </summary>
    [UsedImplicitly]
    #pragma warning disable CS8618
    private AIModel() {}
    #pragma warning restore CS8618
}
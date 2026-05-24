using CSharpFunctionalExtensions;
using JetBrains.Annotations;

namespace DocANAI.Persistence.Entities;

public sealed class Format : Entity<string>
{
    public string Extension => Id;
    public int MaxSize { get; private set; }
    public string MimeType { get; private set; }
    
    private Format(string extension, int maxSize, string mimeType)
    {
        Id = extension;
        MaxSize = maxSize;
        MimeType = mimeType;
    }

    public static Format Create(string extension, int maxSize, string mimeType)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(extension);
        ArgumentException.ThrowIfNullOrWhiteSpace(mimeType);
        
        return new Format(extension, maxSize, mimeType);
    }
    
    /// <summary>
    /// Для EF Core
    /// </summary>
    [UsedImplicitly]
    #pragma warning disable CS8618
    private Format() {}
    #pragma warning restore CS8618
}
using DocANAI.Persistence.Abstractions;
using DocANAI.Persistence.ValueObjects;
using JetBrains.Annotations;

namespace DocANAI.Persistence.Entities.SourceDocument;

public sealed class SourceDocument : AuditableEntity<IdOf<SourceDocument>>
{
    public IdOf<User.User> UserId { get; private set; }
    public IdOf<ProcessingTask.ProcessingTask> TaskId { get; private set; }
    public string Extension { get; private set; }
    public string Filename { get; private set; }
    public int Size { get; private set; }

    private SourceDocument(
        IdOf<SourceDocument> id,
        IdOf<User.User> userId,
        IdOf<ProcessingTask.ProcessingTask> taskId,
        string extension,
        string filename,
        int size)
    {
        Id = id;
        UserId = userId;
        TaskId = taskId;
        Extension = extension;
        Filename = filename;
        Size = size;
    }

    public static SourceDocument Create(
        IdOf<SourceDocument> id,
        IdOf<User.User> userId,
        IdOf<ProcessingTask.ProcessingTask> taskId,
        string extension,
        string filename,
        int size)
        => new(id, userId, taskId, extension, filename, size);

    [UsedImplicitly]
    #pragma warning disable CS8618
    private SourceDocument() { }
    #pragma warning restore CS8618
}
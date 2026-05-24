using DocANAI.Persistence.Abstractions;
using DocANAI.Persistence.ValueObjects;
using JetBrains.Annotations;

namespace DocANAI.Persistence.Entities;

public sealed class QuestionFile : AuditableEntity<IdOf<QuestionFile>>
{
    public IdOf<User.User> UserId { get; private set; }
    public IdOf<ProcessingTask.ProcessingTask> TaskId { get; private set; }
    public string Extension { get; private set; }
    public string Filename { get; private set; }

    private QuestionFile(
        IdOf<QuestionFile> id,
        IdOf<User.User> userId,
        IdOf<ProcessingTask.ProcessingTask> taskId,
        string extension,
        string filename)
    {
        Id = id;
        UserId = userId;
        TaskId = taskId;
        Extension = extension;
        Filename = filename;
    }

    public static QuestionFile Create(
        IdOf<QuestionFile> id,
        IdOf<User.User> userId,
        IdOf<ProcessingTask.ProcessingTask> taskId,
        string extension,
        string filename)
        => new(id, userId, taskId, extension, filename);

    [UsedImplicitly]
    #pragma warning disable CS8618
    private QuestionFile() { }
    #pragma warning restore CS8618
}
using DocANAI.Persistence.Abstractions;
using DocANAI.Persistence.ValueObjects;
using JetBrains.Annotations;

namespace DocANAI.Persistence.Entities;

public sealed class Report : AuditableEntity<IdOf<Report>>
{
    public IdOf<ProcessingTask.ProcessingTask> TaskId { get; private set; }
    public string Extension { get; private set; }
    public DateTime? DeletionDate { get; private set; }

    private Report(
        IdOf<Report> id,
        IdOf<ProcessingTask.ProcessingTask> taskId,
        string extension,
        DateTime? deletionDate)
    {
        Id = id;
        TaskId = taskId;
        Extension = extension;
        DeletionDate = deletionDate;
    }

    public static Report Create(
        IdOf<Report> id,
        IdOf<ProcessingTask.ProcessingTask> taskId,
        string extension,
        DateTime? deletionDate = null)
        => new(id, taskId, extension, deletionDate);

    [UsedImplicitly]
    #pragma warning disable CS8618
    private Report() { }
    #pragma warning restore CS8618
}
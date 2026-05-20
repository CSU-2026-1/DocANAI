using DocANAI.Persistence.Abstractions;
using DocANAI.Persistence.ValueObjects;
using JetBrains.Annotations;

namespace DocANAI.Persistence.Entities.ProcessingTask;

public sealed class ProcessingTask : AuditableEntity<IdOf<ProcessingTask>>
{
    public IdOf<User.User> UserId { get; private set; }
    public IdOf<AIModel.AIModel> ModelId { get; private set; }
    public IdOf<Priority.Priority> PriorityId { get; private set; }
    public TaskStatus Status { get; private set; }
    public DateTime? StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }

    private ProcessingTask(
        IdOf<ProcessingTask> id,
        IdOf<User.User> userId,
        IdOf<AIModel.AIModel> modelId,
        IdOf<Priority.Priority> priorityId)
    {
        Id = id;
        UserId = userId;
        ModelId = modelId;
        PriorityId = priorityId;
        Status = TaskStatus.InQueue;
    }

    public static ProcessingTask Create(
        IdOf<ProcessingTask> id,
        IdOf<User.User> userId,
        IdOf<AIModel.AIModel> modelId,
        IdOf<Priority.Priority> priorityId)
        => new(id, userId, modelId, priorityId);

    public void Start()
    {
        Status = TaskStatus.Processing;
        StartTime = DateTime.UtcNow;
    }

    public void Complete()
    {
        Status = TaskStatus.Done;
        EndTime = DateTime.UtcNow;
    }

    public void Fail()
    {
        Status = TaskStatus.Failed;
        EndTime = DateTime.UtcNow;
    }

    [UsedImplicitly]
#pragma warning disable CS8618
    private ProcessingTask() { }
#pragma warning restore CS8618
}
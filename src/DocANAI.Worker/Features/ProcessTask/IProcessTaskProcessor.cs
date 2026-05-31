namespace DocANAI.Worker.Features.ProcessTask;

public interface IProcessTaskProcessor
{
    Task ProcessAsync(Guid taskId, CancellationToken ct = default);
}

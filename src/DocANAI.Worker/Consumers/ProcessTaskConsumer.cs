using DocANAI.Contracts.Messages;
using DocANAI.Worker.Features.ProcessTask;
using MassTransit;

namespace DocANAI.Worker.Consumers;

public sealed class ProcessTaskConsumer(
    IProcessTaskProcessor processTaskProcessor,
    ILogger<ProcessTaskConsumer> logger) : IConsumer<ProcessTaskMessage>
{
    public async Task Consume(ConsumeContext<ProcessTaskMessage> context)
    {
        logger.LogInformation(
            "Received ProcessTaskMessage for task {TaskId}",
            context.Message.TaskId);

        await processTaskProcessor.ProcessAsync(context.Message.TaskId, context.CancellationToken);
    }
}

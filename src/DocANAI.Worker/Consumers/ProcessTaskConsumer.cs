using DocANAI.Contracts.Messages;
using MassTransit;

namespace DocANAI.Worker.Consumers;

public sealed class ProcessTaskConsumer(ILogger<ProcessTaskConsumer> logger) : IConsumer<ProcessTaskMessage>
{
    public Task Consume(ConsumeContext<ProcessTaskMessage> context)
    {
        logger.LogInformation(
            "Received ProcessTaskMessage for task {TaskId}",
            context.Message.TaskId);

        return Task.CompletedTask;
    }
}

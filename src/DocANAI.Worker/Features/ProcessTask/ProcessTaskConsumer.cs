using DocANAI.Contracts.Messages;
using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.ValueObjects;
using MassTransit;
using MediatR;

namespace DocANAI.Worker.Features.ProcessTask;

public sealed class ProcessTaskConsumer(
    IMediator mediator,
    ILogger<ProcessTaskConsumer> logger) : IConsumer<ProcessTaskMessage>
{
    public async Task Consume(ConsumeContext<ProcessTaskMessage> context)
    {
        logger.LogInformation(
            "Received ProcessTaskMessage for task {TaskId}",
            context.Message.TaskId);

        var taskId = IdOf<ProcessingTask>.From(context.Message.TaskId);
        
        await mediator.Send(new ProcessTaskCommand(taskId), context.CancellationToken);
    }
}

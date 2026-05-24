using DocANAI.Persistence.Abstractions;
using DocANAI.Persistence.ValueObjects;
using JetBrains.Annotations;

namespace DocANAI.Persistence.Entities;

public sealed class Answer : AuditableEntity<IdOf<Answer>>
{
    public IdOf<Question> QuestionId { get; private set; }
    public IdOf<ProcessingTask.ProcessingTask> TaskId { get; private set; }
    public string Text { get; private set; }
    public decimal ModelAccuracy { get; private set; }

    private Answer(
        IdOf<Answer> id,
        IdOf<Question> questionId,
        IdOf<ProcessingTask.ProcessingTask> taskId,
        string text,
        decimal modelAccuracy)
    {
        Id = id;
        QuestionId = questionId;
        TaskId = taskId;
        Text = text;
        ModelAccuracy = modelAccuracy;
    }

    public static Answer Create(
        IdOf<Answer> id,
        IdOf<Question> questionId,
        IdOf<ProcessingTask.ProcessingTask> taskId,
        string text,
        decimal modelAccuracy)
        => new(id, questionId, taskId, text, modelAccuracy);

    [UsedImplicitly]
    #pragma warning disable CS8618
    private Answer() { }
    #pragma warning restore CS8618
}
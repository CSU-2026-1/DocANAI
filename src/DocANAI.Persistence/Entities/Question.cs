using DocANAI.Persistence.Abstractions;
using DocANAI.Persistence.ValueObjects;
using JetBrains.Annotations;

namespace DocANAI.Persistence.Entities;

public sealed class Question : AuditableEntity<IdOf<Question>>
{
    public IdOf<QuestionFile> QuestionFileId { get; private set; }
    public int QuestionNumber { get; private set; }
    public string Text { get; private set; }

    private Question(
        IdOf<Question> id,
        IdOf<QuestionFile> questionFileId,
        int questionNumber,
        string text)
    {
        Id = id;
        QuestionFileId = questionFileId;
        QuestionNumber = questionNumber;
        Text = text;
    }

    public static Question Create(
        IdOf<Question> id,
        IdOf<QuestionFile> questionFileId,
        int questionNumber,
        string text)
        => new(id, questionFileId, questionNumber, text);

    [UsedImplicitly]
    #pragma warning disable CS8618
    private Question() { }
    #pragma warning restore CS8618
}
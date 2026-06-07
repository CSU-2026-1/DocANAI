using DocANAI.Persistence.Entities;
using DocANAI.Persistence.Repositories.AIModels;
using DocANAI.Persistence.Repositories.Answers;
using DocANAI.Persistence.Repositories.ProcessingTasks;
using DocANAI.Persistence.Repositories.Questions;
using DocANAI.Persistence.ValueObjects;
using DocANAI.Worker.Infrastructure.Llm;
using MediatR;

namespace DocANAI.Worker.Features.RunAnalysis;

public class RunAnalysisCommandHandler(
    IProcessingTasksRepository processingTasksRepository,
    IQuestionsRepository questionsRepository,
    IAIModelsRepository aiModelsRepository,
    IAnswersRepository answersRepository,
    IAnswerGenerator answerGenerator)
    : IRequestHandler<RunAnalysisCommand>
{
    public async Task Handle(RunAnalysisCommand request, CancellationToken ct)
    {
        var maybeTask = await processingTasksRepository.GetByIdAsync(request.TaskId, ct);
        if (maybeTask.HasNoValue) throw new InvalidOperationException("Task not found");
        var task = maybeTask.Value;

        var maybeModel = await aiModelsRepository.GetByIdAsync(task.ModelId, ct);
        if (maybeModel.HasNoValue) throw new InvalidOperationException("Model not found");
        var modelName = ResolveOllamaModelName(maybeModel.Value);

        var questions = await questionsRepository.GetByTaskIdAsync(request.TaskId, ct);
        if (questions.Count == 0) throw new InvalidOperationException("No questions found");

        var answers = new List<Answer>(questions.Count);

        foreach (var question in questions.OrderBy(q => q.QuestionNumber))
        {
            var answerText = await answerGenerator.GenerateAnswerAsync(
                modelName,
                request.DocumentText,
                question,
                ct);

            answers.Add(Answer.Create(
                IdOf<Answer>.New(),
                question.Id,
                request.TaskId,
                answerText,
                modelAccuracy: 0m));
        }

        await answersRepository.AddRangeAsync(answers, ct);
    }

    private static string ResolveOllamaModelName(DocANAI.Persistence.Entities.AIModel.AIModel model) =>
        string.IsNullOrWhiteSpace(model.Version) || model.Version.Equals("latest", StringComparison.OrdinalIgnoreCase)
            ? model.Name
            : $"{model.Name}:{model.Version}";
}
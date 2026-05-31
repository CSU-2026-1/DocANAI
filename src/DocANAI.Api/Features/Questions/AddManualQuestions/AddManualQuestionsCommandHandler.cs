using CSharpFunctionalExtensions;
using DocANAI.Persistence.Entities;
using DocANAI.Persistence.Repositories.ProcessingTasks;
using DocANAI.Persistence.Repositories.Questions;
using DocANAI.Persistence.ValueObjects;
using MediatR;

namespace DocANAI.Api.Features.Questions.AddManualQuestions;

internal sealed class AddManualQuestionsCommandHandler(
    IProcessingTasksRepository processingTasksRepository,
    IQuestionsRepository questionsRepository)
    : IRequestHandler<AddManualQuestionsCommand, Result<AddManualQuestionsResponse, string>>
{
    public async Task<Result<AddManualQuestionsResponse, string>> Handle(AddManualQuestionsCommand command, CancellationToken ct)
    {
        var maybeTask = await processingTasksRepository.GetByIdAsync(command.TaskId, ct);
        if (maybeTask.HasNoValue)
            return Result.Failure<AddManualQuestionsResponse, string>("Task not found");

        if (maybeTask.Value.UserId != command.UserId)
            return Result.Failure<AddManualQuestionsResponse, string>("You do not have access to this task");

        if (command.Questions.Count == 0)
            return Result.Failure<AddManualQuestionsResponse, string>("At least one question is required");

        var duplicateQuestionNumbers = command.Questions
            .GroupBy(q => q.QuestionNumber)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToArray();
        if (duplicateQuestionNumbers.Length > 0)
        {
            return Result.Failure<AddManualQuestionsResponse, string>(
                $"Question numbers must be unique. Duplicates: {string.Join(", ", duplicateQuestionNumbers)}");
        }

        if (command.Questions.Any(q => q.QuestionNumber <= 0))
            return Result.Failure<AddManualQuestionsResponse, string>("Question numbers must be greater than zero");

        if (command.Questions.Any(q => string.IsNullOrWhiteSpace(q.Text)))
            return Result.Failure<AddManualQuestionsResponse, string>("Question text cannot be empty");

        var questions = command.Questions
            .Select(q => Question.Create(
                IdOf<Question>.New(),
                command.TaskId,
                questionFileId: null,
                q.QuestionNumber,
                q.Text.Trim()))
            .ToArray();

        await questionsRepository.AddRangeAsync(questions, ct);

        return Result.Success<AddManualQuestionsResponse, string>(
            new AddManualQuestionsResponse((Guid)command.TaskId, questions.Length));
    }
}

using CSharpFunctionalExtensions;
using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.Entities.User;
using DocANAI.Persistence.ValueObjects;
using MediatR;

namespace DocANAI.Api.Features.Questions.AddManualQuestions;

public sealed record AddManualQuestionsCommand(
    IdOf<ProcessingTask> TaskId,
    IdOf<User> UserId,
    IReadOnlyCollection<ManualQuestionItemRequest> Questions)
    : IRequest<Result<AddManualQuestionsResponse, string>>;

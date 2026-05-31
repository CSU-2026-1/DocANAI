namespace DocANAI.Api.Features.Questions.AddManualQuestions;

public sealed record ManualQuestionItemRequest(int QuestionNumber, string Text);

public sealed record AddManualQuestionsRequest(IReadOnlyCollection<ManualQuestionItemRequest> Questions);

public sealed record AddManualQuestionsResponse(Guid TaskId, int AddedCount);

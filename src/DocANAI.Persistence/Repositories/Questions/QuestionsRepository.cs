using DocANAI.Persistence.Abstractions;
using DocANAI.Persistence.Attributes;
using DocANAI.Persistence.Context;
using DocANAI.Persistence.Entities;
using DocANAI.Persistence.ValueObjects;

namespace DocANAI.Persistence.Repositories.Questions;

[Repository]
internal sealed class QuestionsRepository(PostgreSqlDbContext dbContext) : BaseRepository<Question, IdOf<Question>>(dbContext), IQuestionsRepository
{
    
}
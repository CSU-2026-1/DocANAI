using CSharpFunctionalExtensions;
using DocANAI.Persistence.Abstractions;
using DocANAI.Persistence.Attributes;
using DocANAI.Persistence.Context;
using DocANAI.Persistence.Entities.User;
using DocANAI.Persistence.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DocANAI.Persistence.Repositories.Users;

[Repository]
internal sealed class UsersRepository(PostgreSqlDbContext dbContext) : BaseRepository<User, IdOf<User>>(dbContext), IUsersRepository
{
    public async Task<Maybe<User>> GetByUsernameAsync(string username, CancellationToken ct = default)
    {
        var user = await DbContext.Users
            .FirstOrDefaultAsync(u => u.Username == username, ct);

        return user ?? Maybe<User>.None;
    }
}
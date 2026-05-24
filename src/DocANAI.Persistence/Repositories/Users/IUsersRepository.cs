using CSharpFunctionalExtensions;
using DocANAI.Persistence.Entities.User;
using DocANAI.Persistence.ValueObjects;

namespace DocANAI.Persistence.Repositories.Users;

public interface IUsersRepository
{
    Task<Maybe<User>> GetByIdAsync(IdOf<User> id, CancellationToken ct = default);
    Task<Maybe<User>> GetByUsernameAsync(string username, CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);
}
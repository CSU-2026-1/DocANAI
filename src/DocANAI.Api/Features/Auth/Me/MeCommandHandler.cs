using CSharpFunctionalExtensions;
using DocANAI.Contracts.DTOs.Auth;
using DocANAI.Persistence.Repositories.Users;
using DocANAI.Persistence.ValueObjects;
using MediatR;

namespace DocANAI.Api.Features.Auth.Me;

internal sealed class MeCommandHandler(IUsersRepository usersRepository)
    : IRequestHandler<MeCommand, Result<MeResponse, string>>
{
    public async Task<Result<MeResponse, string>> Handle(MeCommand command, CancellationToken ct)
    {
        var maybeUser = await usersRepository.GetByIdAsync(command.UserId, ct);
        if (maybeUser.HasNoValue)
            return Result.Failure<MeResponse, string>("User not found");
        
        var user = maybeUser.Value;
        var response = new MeResponse(user.Username, user.UserType.ToString());

        return Result.Success<MeResponse, string>(response);
    }
}
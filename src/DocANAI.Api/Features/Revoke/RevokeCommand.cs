using CSharpFunctionalExtensions;
using MediatR;

namespace DocANAI.Api.Features.Revoke;

public sealed record RevokeCommand(string RefreshToken, string IpAddress) 
    : IRequest<Result<bool, string>>;
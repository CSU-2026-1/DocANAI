using CSharpFunctionalExtensions;
using DocANAI.Contracts.DTOs.Auth;
using MediatR;

namespace DocANAI.Api.Features.Auth.Login;

public sealed record LoginCommand(LoginRequest Request, string IpAddress) 
    : IRequest<Result<AuthResponse, string>>;
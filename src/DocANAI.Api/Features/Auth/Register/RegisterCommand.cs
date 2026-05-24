using CSharpFunctionalExtensions;
using DocANAI.Contracts.DTOs.Auth;
using MediatR;

namespace DocANAI.Api.Features.Auth.Register;

public sealed record RegisterCommand(RegisterRequest Request, string IpAddress) 
    : IRequest<Result<AuthResponse, string>>;
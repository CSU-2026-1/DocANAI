using CSharpFunctionalExtensions;
using DocANAI.Contracts.DTOs.Auth;
using MediatR;

namespace DocANAI.Api.Features.Auth.Refresh;

public sealed record RefreshCommand(string RefreshToken, string IpAddress) 
    : IRequest<Result<AuthResponse, string>>;
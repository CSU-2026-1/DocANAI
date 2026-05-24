using CSharpFunctionalExtensions;
using DocANAI.Contracts.DTOs.FileStorage;
using MediatR;

namespace DocANAI.Api.Features.DeleteFile;

public sealed record DeleteFileCommand(string ObjectName) 
    : IRequest<Result<DeleteFileResponse, string>>;
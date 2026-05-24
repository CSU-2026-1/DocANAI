using CSharpFunctionalExtensions;
using MediatR;

namespace DocANAI.Api.Features.DownloadReport;

public sealed record DownloadReportQuery(string ObjectName) 
    : IRequest<Result<Stream, string>>;
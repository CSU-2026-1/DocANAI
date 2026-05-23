using System;
using DocANAI.Contracts.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace DocANAI.Api.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "Unhandled exception occurred");

        var response = context.Exception switch
        {
            FileNotFoundException => new ErrorResponse("File not found", context.Exception.Message),
            UnauthorizedAccessException => new ErrorResponse("Unauthorized"),
            ArgumentException => new ErrorResponse("Invalid request", context.Exception.Message),
            _ => new ErrorResponse("Internal server error", "An unexpected error occurred. Please try again later.")
        };

        int statusCode = context.Exception switch
        {
            FileNotFoundException => 404,
            UnauthorizedAccessException => 401,
            ArgumentException => 400,
            _ => 500
        };

        context.Result = new ObjectResult(response) { StatusCode = statusCode };
        context.ExceptionHandled = true;
    }
}
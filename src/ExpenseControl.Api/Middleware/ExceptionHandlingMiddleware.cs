using System.Net;
using System.Text.Json;
using ExpenseControl.Shared.Common;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseControl.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleAsync(context, ex);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        var (statusCode, problem) = exception switch
        {
            ValidationException ve => (HttpStatusCode.BadRequest, new ProblemDetails
            {
                Title = "Validation Error",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = string.Join("; ", ve.Errors.Select(e => e.ErrorMessage)),
                Extensions = { ["errors"] = ve.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }).ToArray() }
            }),
            DomainException de => (HttpStatusCode.BadRequest, new ProblemDetails
            {
                Title = de.Code,
                Status = (int)HttpStatusCode.BadRequest,
                Detail = de.Message
            }),
            _ => (HttpStatusCode.InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = "An unexpected error occurred."
            })
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }
}

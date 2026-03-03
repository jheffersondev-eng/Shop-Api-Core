using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Shop.Api.Middleware;

public sealed class ApiExceptionMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly RequestDelegate _next;

    public ApiExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            var (statusCode, title, detail) = MapException(exception);
            var problem = new ProblemDetails
            {
                Type = $"https://httpstatuses.com/{statusCode}",
                Title = title,
                Status = statusCode,
                Detail = detail,
                Instance = context.Request.Path
            };

            problem.Extensions["traceId"] = context.TraceIdentifier;

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(problem, JsonOptions));
        }
    }

    private static (int StatusCode, string Title, string Detail) MapException(Exception exception)
    {
        if (exception is ArgumentException argumentException)
        {
            return (StatusCodes.Status400BadRequest, "Invalid request", argumentException.Message);
        }

        if (IsDatabaseUnavailable(exception))
        {
            return (
                StatusCodes.Status503ServiceUnavailable,
                "Database unavailable",
                "PostgreSQL is not reachable at 127.0.0.1:5432. Start the database and try again.");
        }

        if (exception is InvalidOperationException invalidOperationException)
        {
            if (invalidOperationException.Message.Contains("AWS credentials are not configured for Cognito access."))
            {
                return (StatusCodes.Status503ServiceUnavailable, "Cognito credentials not configured", invalidOperationException.Message);
            }

            if (invalidOperationException.Message.Contains("Cognito request failed ("))
            {
                if (invalidOperationException.Message.Contains("Invalid phone number format."))
                {
                    return (
                        StatusCodes.Status400BadRequest,
                        "Invalid phone format",
                        "Phone must be in E.164 format, for example: +5511999999999.");
                }

                return (StatusCodes.Status502BadGateway, "Cognito request failed", invalidOperationException.Message);
            }

            if (invalidOperationException.Message.Contains("Email already exists."))
            {
                return (StatusCodes.Status409Conflict, "Duplicate email", "A customer with this email already exists.");
            }

            if (invalidOperationException.Message.Contains("CognitoSub already exists."))
            {
                return (StatusCodes.Status409Conflict, "Duplicate CognitoSub", "A customer with this CognitoSub already exists.");
            }
        }

        return (
            StatusCodes.Status500InternalServerError,
            "Unexpected error",
            "An unexpected error occurred while processing your request.");
    }

    private static bool IsDatabaseUnavailable(Exception exception)
    {
        if (exception is NpgsqlException)
        {
            return true;
        }

        if (exception.InnerException is null)
        {
            return false;
        }

        return IsDatabaseUnavailable(exception.InnerException);
    }
}
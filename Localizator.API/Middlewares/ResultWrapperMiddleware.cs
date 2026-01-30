using Localizator.Shared.Exceptions;
using Localizator.Shared.Extensions;
using Localizator.Shared.Helpers;
using Localizator.Shared.Providers;
using Localizator.Shared.Resources;
using Localizator.Shared.Result;
using Microsoft.AspNetCore.Mvc;
using Soenneker.Dtos.ProblemDetails;
using System.Diagnostics;
using System.Text.Json;

namespace Localizator.API.Middlewares;

public class ResultWrapperMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);

            await HandleSuccessfulResponse(context, originalBodyStream, responseBody);
        }
        catch (BaseException ex)
        {
            await HandleBaseException(context, originalBodyStream, ex);
        }
        catch (Exception ex)
        {
            await HandleException(context, originalBodyStream, ex);
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    private async Task HandleSuccessfulResponse(HttpContext context, Stream originalBodyStream, MemoryStream responseBody)
    {
        var meta = MetaProvider.GetFromContext(context);

        responseBody.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(responseBody).ReadToEndAsync();

        if (!IsJsonResponse(context) || responseText.IsNullOrWhitespace())
        {
            await CopyOriginalResponse(responseBody, originalBodyStream);
            return;
        }

        try
        {
            var responseObject = JsonSerializer.Deserialize<object>(responseText, GetJsonOptions());
            var finalResult = BuildResult(context, responseText, responseObject, meta);

            var json = JsonSerializer.Serialize(finalResult, GetJsonOptions());
            await WriteResponse(context, originalBodyStream, json);
        }
        catch (JsonException)
        {
            await CopyOriginalResponse(responseBody, originalBodyStream);
        }
    }

    private Result BuildResult(HttpContext context, string rawResponse, object? responseObject, Meta meta)
    {
        // If result is already Result type add meta
        if (IsResultObject(rawResponse))
        {
            var existing = JsonSerializer.Deserialize<Result>(rawResponse, GetJsonOptions());
            if (existing != null)
            {
                existing.Meta = meta;
                return existing;
            }
        }

        // Success
        if (context.Response.StatusCode is >= 200 and < 300)
        {
            return Result.Success(responseObject, meta: meta);
        }

        // Error
        context.Response.ContentType = "application/json";

        return Result.Failure(
            message: $"Error: {context.Response.StatusCode}",
            data: responseObject,
            meta: meta
        );
    }

    private async Task HandleBaseException(HttpContext context, Stream originalStream, BaseException exception)
    {
        var meta = MetaProvider.GetFromContext(context);

        if (!context.Response.HasStarted)
        {
            context.Response.StatusCode = exception.StatusCode;
            context.Response.ContentType = "application/json";

            var isDevelopment = context.RequestServices
                .GetService<IWebHostEnvironment>()?
                .IsDevelopment() ?? false;

            var problemDetails = exception.GetProblemDetails();

            if (isDevelopment)
            {
                problemDetails.Extensions!.TryAdd("exceptionType", exception?.GetType().FullName ?? string.Empty);
                problemDetails.Extensions!.TryAdd("stackTrace", exception?.StackTrace ?? string.Empty);
                problemDetails.Extensions!.TryAdd("innerException", exception?.InnerException?.Message ?? string.Empty);
            }

            var errorResult = Result.Failure(
                message: exception?.Message ?? string.Empty,
                problemDetails: problemDetails,
                meta: meta
            );

            var errorResponse = JsonSerializer.Serialize(errorResult, GetJsonOptions());
            await WriteResponse(context, originalStream, errorResponse);
        }
        else
        {

            throw exception;
        }
    }

    private async Task HandleException(HttpContext context, Stream originalStream, Exception exception)
    {
        if (context.Response.HasStarted)
            throw exception;

        var meta = MetaProvider.GetFromContext(context);

        var isDevelopment = context.RequestServices
            .GetService<IWebHostEnvironment>()?
            .IsDevelopment() ?? false;

        var (statusCode, title, code) = MapException(exception);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var problemDetails = new ProblemDetailsDto
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message,
            Type = HttpProblemTypeMapper.FromStatusCode(statusCode),
            Extensions = { }
        };

        problemDetails.Extensions!.TryAdd("code", code);

        if (isDevelopment)
        {
            problemDetails.Extensions!.TryAdd("exceptionType", exception?.GetType().FullName ?? string.Empty);
            problemDetails.Extensions!.TryAdd("stackTrace", exception?.StackTrace ?? string.Empty);
            problemDetails.Extensions!.TryAdd("innerException", exception?.InnerException?.Message ?? string.Empty);
        }

        var result = Result.Failure(
            message: exception?.Message ?? string.Empty,
            data: null,
            problemDetails: problemDetails,
            meta: meta
        );

        var json = JsonSerializer.Serialize(result, GetJsonOptions());
        await WriteResponse(context, originalStream, json);
    }

    private (int Status, string Title, string Code) MapException(Exception ex)
    {
        return ex switch
        {
            ArgumentNullException => (
                StatusCodes.Status400BadRequest,
                Errors.InvalidRequest,
                "ArgumentNull"
            ),

            ArgumentException => (
                StatusCodes.Status400BadRequest,
                Errors.InvalidRequest,
                "Argument"
            ),

            ValidationException => (
                StatusCodes.Status422UnprocessableEntity,
                Errors.ValidationFailed,
                "Validation"
            ),

            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                Errors.Unauthorized,
                "Unauthorized"
            ),

            TaskCanceledException or TimeoutException => (
                StatusCodes.Status408RequestTimeout,
                Errors.RequestTimeout,
                "Timeout"
            ),

            OperationCanceledException => (
                499, // Client Closed Request (nginx standard)
                Errors.RequestCancelled,
                "ClientCancelled"
            ),

            NotImplementedException => (
                StatusCodes.Status501NotImplemented,
                Errors.NotImplemented,
                "NotImplemented"
            ),

            HttpRequestException => (
                StatusCodes.Status502BadGateway,
                Errors.BadGateway,
                "BadGateway"
            ),

            NullReferenceException => (
                StatusCodes.Status500InternalServerError,
                Errors.InternalServerError,
                "NullReference"
            ),

            _ => (
                StatusCodes.Status500InternalServerError,
                Errors.AnErrorOccured,
                "UnhandledException"
            )
        };
    }

    private bool IsJsonResponse(HttpContext context)
    {
        return context.Response.ContentType?.Contains("application/json") == true;
    }

    private bool IsResultObject(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            return root.TryGetProperty("isSuccess", out _) ||
                   root.TryGetProperty("IsSuccess", out _);
        }
        catch
        {
            return false;
        }
    }

    private JsonSerializerOptions GetJsonOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            PropertyNameCaseInsensitive = true
        };
    }

    private async Task WriteResponse(HttpContext context, Stream originalStream, string content)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        context.Response.ContentLength = bytes.Length;
        context.Response.Body = originalStream;
        await context.Response.WriteAsync(content);
    }

    private async Task CopyOriginalResponse(MemoryStream responseBody, Stream originalStream)
    {
        responseBody.Seek(0, SeekOrigin.Begin);
        await responseBody.CopyToAsync(originalStream);
    }
}
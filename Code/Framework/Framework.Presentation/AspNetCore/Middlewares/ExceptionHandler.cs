using Framework.Core.Domain.Exceptions;
using Framework.Presentation.AspNetCore.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Framework.Presentation.AspNetCore.Middlewares;

public class ErrorHandlingMiddleware(
    RequestDelegate next,
    ILogger<ErrorHandlingMiddleware> logger,
    IConfiguration configuration)
{
    public async Task Invoke(HttpContext context)
    {
        context.Request.EnableBuffering();

        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            if (await HandleExceptionAsync(context, ex))
                throw;
        }
    }

    private async Task<bool> HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var requestBody = await GenerateRequestInformation(context);

        logger.LogError($"{ex.Message} , Request :{requestBody}");

        var code = 500;
        context.Response.ContentType = "application/json";
        object? result = null;
        switch (ex)
        {
            case BusinessException businessException:
                code = 400;
                result = new ApiResult{
                    Errors = new ErrorResult{Title = businessException.Message}
                };
                break;
            case NotFoundException:
                code = 404;
                break;
            default:
                if (bool.TryParse(configuration["UseDeveloperExceptionPage"], out bool useDeveloperExceptionPage) && useDeveloperExceptionPage)
                    return true;
                break;
        }

        context.Response.StatusCode = code;
        await context.Response.WriteAsJsonAsync(result);
        return false;
    }
    private async Task<string> GenerateRequestInformation(HttpContext httpContext)
    {
        return $"HTTP request information:\n" +
               $"\tMethod: {httpContext.Request.Method}\n" +
               $"\tPath: {httpContext.Request.Path}\n" +
               $"\tQueryString: {httpContext.Request.QueryString}\n" +
               $"\tHeaders: {FormatHeaders(httpContext.Request.Headers)}\n" +
               $"\tSchema: {httpContext.Request.Scheme}\n" +
               $"\tHost: {httpContext.Request.Host}\n" +
               $"\tBody: {await ReadBodyFromRequest(httpContext.Request)}";
    }
    private static string FormatHeaders(IHeaderDictionary headers) => string.Join(", ", headers.Select(kvp => $"{{{kvp.Key}: {string.Join(", ", kvp.Value!)}}}"));
    private static async Task<string> ReadBodyFromRequest(HttpRequest request)
    {
        request.Body.Seek(0, SeekOrigin.Begin);

        using var streamReader = new StreamReader(request.Body, encoding: System.Text.Encoding.UTF8);
        var requestBody = await streamReader.ReadToEndAsync();

        // Reset the request's body stream position for next middleware in the pipeline.
        return requestBody;
    }
}
using Microsoft.Extensions.Options;
using ApiSAPBridge.Models.Configuration;
using ApiSAPBridge.Models.Constants;
using System.Text.Json;

public class ApiKeyAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ApiKeyConfig _apiKeyConfig;
    private readonly ILogger<ApiKeyAuthenticationMiddleware> _logger;

    public ApiKeyAuthenticationMiddleware(
        RequestDelegate next,
        IOptions<ApiKeyConfig> apiKeyConfig,
        ILogger<ApiKeyAuthenticationMiddleware> logger)
    {
        _next = next;
        _apiKeyConfig = apiKeyConfig.Value;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Permitir endpoints GET sin autenticación
        if (context.Request.Method == HttpMethods.Get ||
            context.Request.Path.StartsWithSegments("/health") ||
            context.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(context);
            return;
        }

        // Verificar API Key para endpoints POST
        if (context.Request.Method == HttpMethods.Post)
        {
            if (!HasValidApiKey(context.Request))
            {
                await WriteUnauthorizedResponse(context, "API Key requerida o inválida");
                return;
            }

            // Verificar Auth Token para endpoints de tarifas
            if (context.Request.Path.StartsWithSegments("/api/tarifas"))
            {
                if (!HasValidAuthToken(context.Request))
                {
                    await WriteUnauthorizedResponse(context, "Auth Token requerido o inválido para endpoints de tarifas");
                    return;
                }
            }
        }

        await _next(context);
    }

    private bool HasValidApiKey(HttpRequest request)
    {
        if (!request.Headers.TryGetValue(ApiConstants.API_KEY_HEADER, out var extractedApiKey))
        {
            return false;
        }

        return string.Equals(extractedApiKey, _apiKeyConfig.SAPApiKey, StringComparison.OrdinalIgnoreCase);
    }

    private bool HasValidAuthToken(HttpRequest request)
    {
        if (!request.Headers.TryGetValue(ApiConstants.AUTH_TOKEN_HEADER, out var extractedToken))
        {
            return false;
        }

        return string.Equals(extractedToken, _apiKeyConfig.AuthToken, StringComparison.OrdinalIgnoreCase);
    }

    private async Task WriteUnauthorizedResponse(HttpContext context, string message)
    {
        _logger.LogWarning("Unauthorized access attempt from {RemoteIpAddress}: {Message}",
            context.Connection.RemoteIpAddress, message);

        context.Response.StatusCode = 401;
        context.Response.ContentType = "application/json";

        var response = new
        {
            Success = false,
            Message = message,
            Timestamp = DateTime.UtcNow
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
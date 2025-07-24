using ApiSAPBridge.API.Attributes;
using ApiSAPBridge.Models.Configuration;
using ApiSAPBridge.Models.Constants;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace ApiSAPBridge.API.Middleware
{
    public class ApiKeyAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiKeyAuthenticationMiddleware> _logger;
        private readonly ApiKeyConfig _apiKeyConfig;

        public ApiKeyAuthenticationMiddleware(
            RequestDelegate next,
            ILogger<ApiKeyAuthenticationMiddleware> logger,
            IOptions<ApiKeyConfig> apiKeyConfig)
        {
            _next = next;
            _logger = logger;
            _apiKeyConfig = apiKeyConfig.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Verificar si el endpoint requiere autenticación
            var endpoint = context.GetEndpoint();
            var requiresAuth = endpoint?.Metadata.GetMetadata<ApiKeyAuthAttribute>() != null;

            if (!requiresAuth)
            {
                await _next(context);
                return;
            }

            // Verificar API Key
            if (!context.Request.Headers.TryGetValue(ApiConstants.API_KEY_HEADER, out var apiKey))
            {
                _logger.LogWarning("API Key faltante en request a {Path}", context.Request.Path);
                await WriteUnauthorizedResponse(context, "API Key requerida");
                return;
            }

            if (!IsValidApiKey(apiKey))
            {
                _logger.LogWarning("API Key inválida: {ApiKey} para {Path}", apiKey, context.Request.Path);
                await WriteUnauthorizedResponse(context, "API Key inválida");
                return;
            }

            _logger.LogInformation("Autenticación exitosa para {Path}", context.Request.Path);
            await _next(context);
        }

        private bool IsValidApiKey(string apiKey)
        {
            // Verificar contra las API Keys configuradas
            return !string.IsNullOrEmpty(_apiKeyConfig.SapApiKey) &&
                   apiKey == _apiKeyConfig.SapApiKey;
        }

        private async Task WriteUnauthorizedResponse(HttpContext context, string message)
        {
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";

            var response = new
            {
                Success = false,
                Message = message,
                Timestamp = DateTime.UtcNow
            };

            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
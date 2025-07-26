using System.ComponentModel.DataAnnotations;

namespace ApiSAPBridge.Configuration.Models.DTOs
{
    /// <summary>
    /// DTO para test de conexión
    /// </summary>
    public class ConnectionTestResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public TimeSpan ConnectionTime { get; set; }
        public string? ErrorDetails { get; set; }
    }

    /// <summary>
    /// DTO para resultado de operación
    /// </summary>
    public class OperationResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ErrorDetails { get; set; }

        public static OperationResult Success(string message = "Operación exitosa")
            => new OperationResult { IsSuccess = true, Message = message };

        public static OperationResult Failure(string message, string? errorDetails = null)
            => new OperationResult { IsSuccess = false, Message = message, ErrorDetails = errorDetails };
    }

    /// <summary>
    /// DTO para autenticación
    /// </summary>
    public class AuthenticationRequest
    {
        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthenticationResult
    {
        public bool IsAuthenticated { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime? ExpiresAt { get; set; }
    }
}

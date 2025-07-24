namespace ApiSAPBridge.Models.DTOs
{
    public class ResponseDto<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Constructor privado para forzar uso de métodos estáticos
        private ResponseDto() { }

        public static ResponseDto<T> CreateSuccess(T data, string message = "Operación exitosa")
        {
            return new ResponseDto<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Timestamp = DateTime.UtcNow
            };
        }

        public static ResponseDto<T> CreateError(string message, List<string>? errors = null)
        {
            return new ResponseDto<T>
            {
                Success = false,
                Message = message,
                Errors = errors,
                Timestamp = DateTime.UtcNow
            };
        }

        public static ResponseDto<T> CreateError(string message, string error)
        {
            return CreateError(message, new List<string> { error });
        }
    }
}
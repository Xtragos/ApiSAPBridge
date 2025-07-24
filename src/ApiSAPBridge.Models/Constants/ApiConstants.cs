namespace ApiSAPBridge.Models.Constants
{
    public static class ApiConstants
    {
        public const string API_KEY_HEADER = "x-api-key";
        public const string AUTH_TOKEN_HEADER = "x-auth-token";

        public static class ResponseMessages
        {
            public const string SUCCESS_CREATED = "Registros creados exitosamente";
            public const string SUCCESS_UPDATED = "Registros actualizados exitosamente";
            public const string SUCCESS_RETRIEVED = "Datos obtenidos exitosamente";
            public const string SUCCESS_CREATE = "Recursos creados exitosamente";
            public const string SUCCESS_RETRIEVE = "Recursos obtenidos exitosamente";
            public const string SUCCESS_UPDATE = "Recursos actualizados exitosamente";
            public const string SUCCESS_DELETE = "Recursos eliminados exitosamente";

            public const string ERROR_VALIDATION = "Error de validación en los datos";
            public const string ERROR_DUPLICATE = "Ya existe un registro con esos datos";
            public const string ERROR_NOT_FOUND = "Registro no encontrado";
            public const string ERROR_DATABASE = "Error en la base de datos";
            public const string ERROR_UNAUTHORIZED = "No autorizado - API Key inválida";
            public const string ERROR_GENERIC = "Ha ocurrido un error interno del servidor";
            public const string ERROR_FORBIDDEN = "Acceso prohibido";

        }

        public static class ValidationMessages
        {
            public const string REQUIRED_FIELD = "El campo {0} es requerido";
            public const string MAX_LENGTH = "El campo {0} no puede exceder {1} caracteres";
            public const string INVALID_FORMAT = "El formato del campo {0} es inválido";
        }
    }

    public static class MapsterConfig
    {
        /// <summary>
        /// Configuraciones personalizadas para Mapster
        /// Se pueden agregar mapeos específicos aquí si son necesarios
        /// </summary>
        public static void Configure()
        {
            // Mapster realiza mapeo automático por convención de nombres
            // Solo agregar configuraciones especiales aquí si son necesarias

            // Ejemplo de configuración personalizada (comentado):
            // TypeAdapterConfig<Articulo, ArticuloDto>.NewConfig()
            //     .Map(dest => dest.NombreCompleto, src => $"{src.DESCRIPCION} - {src.DESCRIPADIC}");
        }
    }
}
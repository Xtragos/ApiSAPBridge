namespace ApiSAPBridge.Configuration.Models
{
    public static class ConfigurationConstants
    {
        public const string CONFIG_FILE_NAME = "appsettings.json";
        public const string SECURITY_CONFIG_FILE = "security.config";
        public const string APP_TITLE = "ApiSAPBridge - Configuración";
        public const string APP_VERSION = "1.0.0";

        public static class TabNames
        {
            public const string SQL_CONFIGURATION = "Configuración SQL";
            public const string METHODS_CONFIGURATION = "Métodos SAP";
            public const string SWAGGER_CONFIGURATION = "Swagger";
        }

        public static class Messages
        {
            public const string CONNECTION_SUCCESS = "Conexión exitosa a la base de datos";
            public const string CONNECTION_FAILED = "Error al conectar con la base de datos";
            public const string CONFIG_SAVED = "Configuración guardada exitosamente";
            public const string CONFIG_LOADED = "Configuración cargada exitosamente";
            public const string INVALID_PASSWORD = "Contraseña incorrecta";
            public const string ACCESS_DENIED = "Acceso denegado";
        }

        public static class DefaultValues
        {
            public const string DEFAULT_SERVER = "localhost";
            public const string DEFAULT_DATABASE = "ApiSAPBridge";
            public const int DEFAULT_SYNC_INTERVAL = 30;
            public const int DEFAULT_CONNECTION_TIMEOUT = 30;
        }
    }
}
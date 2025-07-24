namespace ApiSAPBridge.API.Attributes
{
    /// <summary>
    /// Atributo para marcar endpoints que requieren autenticación por API Key
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ApiKeyAuthAttribute : Attribute
    {
    }
}
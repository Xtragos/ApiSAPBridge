using ApiSAPBridge.Models.DTOs;

namespace ApiSAPBridge.Core.Interfaces
{
    /// <summary>
    /// Servicio para gestión completa de artículos con líneas y precios
    /// </summary>
    public interface IArticuloService
    {
        // Operaciones básicas de artículos
        Task<ResponseDto<List<ArticuloResponseDto>>> CreateArticulosAsync(List<ArticuloDto> articulos);
        Task<ResponseDto<List<ArticuloResponseDto>>> GetArticulosAsync(bool incluirLineas = false, bool incluirPrecios = false);
        Task<ResponseDto<ArticuloResponseDto?>> GetArticuloByIdAsync(int codigoArticulo, bool incluirLineas = false, bool incluirPrecios = false);

        // Operaciones complejas
        Task<ResponseDto<List<ArticuloResponseDto>>> CreateArticulosCompletosAsync(List<ArticuloCompletoRequest> articulosCompletos);
        Task<ResponseDto<ArticuloResponseDto?>> GetArticuloCompletoAsync(int codigoArticulo);

        // Búsquedas y filtros
        Task<ResponseDto<List<ArticuloResponseDto>>> SearchArticulosAsync(ArticuloSearchRequest request);
        Task<ResponseDto<List<ArticuloResponseDto>>> GetArticulosByDepartamentoAsync(int numDpto, bool incluirLineas = false);
        Task<ResponseDto<List<ArticuloResponseDto>>> GetArticulosByImpuestoAsync(int tipoImpuesto);

        // Validaciones
        Task<ResponseDto<bool>> ValidarIntegridadArticuloAsync(int codigoArticulo);
        Task<ResponseDto<object>> GetEstadisticasArticulosAsync();

        // Operaciones de líneas
        Task<ResponseDto<List<ArticuloLineaResponseDto>>> GetLineasByArticuloAsync(int codigoArticulo);
        Task<ResponseDto<List<ArticuloResponseDto>>> GetArticulosByCodBarrasAsync(string codigoBarras);
    }
}
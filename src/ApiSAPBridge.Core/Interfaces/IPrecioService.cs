using ApiSAPBridge.Models.DTOs;

namespace ApiSAPBridge.Core.Interfaces
{
    /// <summary>
    /// Servicio para gestión de precios con validaciones complejas
    /// </summary>
    public interface IPrecioService
    {
        // Operaciones básicas
        Task<ResponseDto<List<PrecioResponseDto>>> CreatePreciosAsync(List<PrecioDto> precios);
        Task<ResponseDto<List<PrecioResponseDto>>> GetPreciosAsync(bool incluirTarifa = false, bool incluirArticulo = false);
        Task<ResponseDto<PrecioResponseDto?>> GetPrecioAsync(int idTarifa, int codigoArticulo, string talla, string color);

        // Operaciones masivas
        Task<ResponseDto<List<PrecioResponseDto>>> CreatePreciosMasivosAsync(PrecioMasivoRequest request);
        Task<ResponseDto<List<PrecioResponseDto>>> ActualizarPreciosPorTarifaAsync(int idTarifa, decimal porcentajeIncremento);

        // Consultas especializadas
        Task<ResponseDto<List<PrecioResponseDto>>> GetPreciosByTarifaAsync(int idTarifa, bool incluirArticulo = false);
        Task<ResponseDto<List<PrecioResponseDto>>> GetPreciosByArticuloAsync(int codigoArticulo, bool incluirTarifa = false);
        Task<ResponseDto<List<PrecioResponseDto>>> SearchPreciosAsync(PrecioSearchRequest request);

        // Análisis y reportes
        Task<ResponseDto<object>> GetAnalisisPreciosAsync(int? idTarifa = null, int? codigoArticulo = null);
        Task<ResponseDto<object>> GetComparativaTarifasAsync(int codigoArticulo);
        Task<ResponseDto<object>> GetEstadisticasPreciosAsync();

        // Validaciones
        Task<ResponseDto<bool>> ValidarConsistenciaPrecioAsync(PrecioDto precio);
        Task<ResponseDto<List<string>>> ValidarPreciosTarifaAsync(int idTarifa);
    }
}
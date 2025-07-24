using ApiSAPBridge.Models.DTOs;

namespace ApiSAPBridge.Core.Interfaces
{
    /// <summary>
    /// Servicio para gestión de tarifas con validaciones de fechas
    /// </summary>
    public interface ITarifaService
    {
        /// <summary>
        /// Crear/actualizar tarifas con validaciones de fechas
        /// </summary>
        Task<ResponseDto<List<TarifaResponseDto>>> CreateTarifasAsync(List<TarifaDto> tarifas);

        /// <summary>
        /// Obtener todas las tarifas con información de estado
        /// </summary>
        Task<ResponseDto<List<TarifaResponseDto>>> GetTarifasAsync(bool incluirConteoPrecios = false);

        /// <summary>
        /// Obtener tarifa específica con información completa
        /// </summary>
        Task<ResponseDto<TarifaResponseDto?>> GetTarifaByIdAsync(int id, bool incluirConteoPrecios = false);

        /// <summary>
        /// Obtener tarifas activas en una fecha específica
        /// </summary>
        Task<ResponseDto<List<TarifaResponseDto>>> GetTarifasActivasAsync(DateTime? fecha = null);

        /// <summary>
        /// Obtener tarifas vigentes (activas hoy)
        /// </summary>
        Task<ResponseDto<List<TarifaResponseDto>>> GetTarifasVigentesAsync();

        /// <summary>
        /// Buscar tarifas con filtros avanzados
        /// </summary>
        Task<ResponseDto<List<TarifaResponseDto>>> SearchTarifasAsync(TarifaSearchRequest request);

        /// <summary>
        /// Validar solapamiento de fechas entre tarifas
        /// </summary>
        Task<ResponseDto<bool>> ValidarSolapamientoAsync(int idTarifa, DateTime fechaInicio, DateTime fechaFin);

        /// <summary>
        /// Obtener estadísticas de tarifas
        /// </summary>
        Task<ResponseDto<object>> GetEstadisticasTarifasAsync();
    }
}
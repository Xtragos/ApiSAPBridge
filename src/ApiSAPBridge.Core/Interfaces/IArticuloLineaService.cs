using ApiSAPBridge.Models.DTOs;

namespace ApiSAPBridge.Core.Interfaces
{
    /// <summary>
    /// Servicio para gestión de líneas de artículos (variantes)
    /// </summary>
    public interface IArticuloLineaService
    {
        Task<ResponseDto<List<ArticuloLineaResponseDto>>> CreateArticuloLineasAsync(List<ArticuloLineaDto> lineas);
        Task<ResponseDto<List<ArticuloLineaResponseDto>>> GetArticuloLineasAsync(bool incluirPrecios = false);
        Task<ResponseDto<ArticuloLineaResponseDto?>> GetArticuloLineaAsync(int codigoArticulo, string talla, string color);
        Task<ResponseDto<List<ArticuloLineaResponseDto>>> GetLineasByArticuloAsync(int codigoArticulo, bool incluirPrecios = false);
        Task<ResponseDto<List<ArticuloLineaResponseDto>>> SearchArticuloLineasAsync(ArticuloLineaSearchRequest request);
        Task<ResponseDto<ArticuloLineaResponseDto?>> GetLineaByCodigoBarrasAsync(string codigoBarras);
        Task<ResponseDto<bool>> ValidarCodigoBarrasUnicoAsync(string codigoBarras, int? excludeArticulo = null, string? excludeTalla = null, string? excludeColor = null);
        Task<ResponseDto<object>> GetEstadisticasLineasAsync();
    }
}
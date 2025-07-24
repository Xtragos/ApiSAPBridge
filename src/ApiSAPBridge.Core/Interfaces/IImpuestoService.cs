using ApiSAPBridge.Models.DTOs;

namespace ApiSAPBridge.Core.Interfaces
{
    /// <summary>
    /// Servicio para gestión de impuestos/IVA
    /// </summary>
    public interface IImpuestoService
    {
        Task<ResponseDto<List<ImpuestoResponseDto>>> CreateImpuestosAsync(List<ImpuestoDto> impuestos);
        Task<ResponseDto<List<ImpuestoResponseDto>>> GetImpuestosAsync();
        Task<ResponseDto<ImpuestoResponseDto?>> GetImpuestoByIdAsync(int tipoIva);
        Task<ResponseDto<bool>> DeleteImpuestoAsync(int tipoIva);
        Task<ResponseDto<ImpuestoStatsDto>> GetImpuestosStatsAsync();
    }
}
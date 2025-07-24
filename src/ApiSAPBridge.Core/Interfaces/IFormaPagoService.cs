using ApiSAPBridge.Models.DTOs;

namespace ApiSAPBridge.Core.Interfaces
{
    /// <summary>
    /// Servicio para gestión de formas de pago
    /// </summary>
    public interface IFormaPagoService
    {
        Task<ResponseDto<List<FormaPagoResponseDto>>> CreateFormasPagoAsync(List<FormaPagoDto> formasPago);
        Task<ResponseDto<List<FormaPagoResponseDto>>> GetFormasPagoAsync();
        Task<ResponseDto<FormaPagoResponseDto?>> GetFormaPagoByIdAsync(int codigo);
        Task<ResponseDto<bool>> DeleteFormaPagoAsync(int codigo);
        Task<ResponseDto<FormaPagoStatsDto>> GetFormasPagoStatsAsync();
        Task<ResponseDto<List<FormaPagoResponseDto>>> GetFormasPagoByTipoAsync(string tipo); // "contado" o "credito"
    }
}
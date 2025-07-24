using ApiSAPBridge.Models.DTOs;

namespace ApiSAPBridge.Core.Interfaces
{
    /// <summary>
    /// Servicio para gestión de vendedores
    /// </summary>
    public interface IVendedorService
    {
        Task<ResponseDto<List<VendedorResponseDto>>> CreateVendedoresAsync(List<VendedorDto> vendedores);
        Task<ResponseDto<List<VendedorResponseDto>>> GetVendedoresAsync();
        Task<ResponseDto<VendedorResponseDto?>> GetVendedorByIdAsync(int id);
        Task<ResponseDto<bool>> DeleteVendedorAsync(int id);
    }
}
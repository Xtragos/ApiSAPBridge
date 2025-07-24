using ApiSAPBridge.Models.DTOs;

namespace ApiSAPBridge.Core.Interfaces
{
    /// <summary>
    /// Servicio para gestión completa de clientes
    /// Incluye validaciones avanzadas y búsquedas
    /// </summary>
    public interface IClienteService
    {
        /// <summary>
        /// Crear/actualizar clientes con validaciones de negocio
        /// </summary>
        Task<ResponseDto<List<ClienteResponseDto>>> CreateClientesAsync(List<ClienteDto> clientes);

        /// <summary>
        /// Obtener todos los clientes con información adicional
        /// </summary>
        Task<ResponseDto<List<ClienteResponseDto>>> GetClientesAsync();

        /// <summary>
        /// Obtener cliente específico con información completa
        /// </summary>
        Task<ResponseDto<ClienteResponseDto?>> GetClienteByIdAsync(int id);

        /// <summary>
        /// Buscar clientes con filtros avanzados
        /// </summary>
        Task<ResponseDto<List<ClienteResponseDto>>> SearchClientesAsync(ClienteSearchRequest request);

        /// <summary>
        /// Obtener clientes por provincia
        /// </summary>
        Task<ResponseDto<List<ClienteResponseDto>>> GetClientesByProvinciaAsync(string provincia);

        /// <summary>
        /// Validar CIF de cliente
        /// </summary>
        Task<ResponseDto<bool>> ValidarCIFAsync(string cif);

        /// <summary>
        /// Obtener estadísticas de clientes
        /// </summary>
        Task<ResponseDto<object>> GetEstadisticasClientesAsync();
    }
}
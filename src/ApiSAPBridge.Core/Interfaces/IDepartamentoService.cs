using ApiSAPBridge.Models.DTOs;

namespace ApiSAPBridge.Core.Interfaces
{
    /// <summary>
    /// Servicio para gestión de departamentos
    /// Maneja la lógica de negocio para CRUD de departamentos
    /// </summary>
    public interface IDepartamentoService
    {
        /// <summary>
        /// Crea o actualiza múltiples departamentos (Upsert)
        /// Usado por el endpoint POST desde SAP
        /// </summary>
        /// <param name="departamentos">Lista de departamentos a procesar</param>
        /// <returns>Respuesta con lista de departamentos procesados</returns>
        Task<ResponseDto<List<DepartamentoResponseDto>>> CreateDepartamentosAsync(
            List<DepartamentoDto> departamentos);

        /// <summary>
        /// Obtiene todos los departamentos
        /// Endpoint público para consulta
        /// </summary>
        /// <returns>Lista completa de departamentos</returns>
        Task<ResponseDto<List<DepartamentoResponseDto>>> GetDepartamentosAsync();

        /// <summary>
        /// Obtiene un departamento específico por ID
        /// </summary>
        /// <param name="id">ID del departamento</param>
        /// <returns>Departamento encontrado o null</returns>
        Task<ResponseDto<DepartamentoResponseDto?>> GetDepartamentoByIdAsync(int id);
    }
}
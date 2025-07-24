using ApiSAPBridge.Models.DTOs;

namespace ApiSAPBridge.Core.Interfaces
{
    /// <summary>
    /// Servicio para gestión de secciones
    /// Maneja la lógica de negocio para CRUD de secciones
    /// </summary>
    public interface ISeccionService
    {
        /// <summary>
        /// Crea o actualiza múltiples secciones (Upsert)
        /// Valida que los departamentos existan antes de crear secciones
        /// </summary>
        Task<ResponseDto<List<SeccionResponseDto>>> CreateSeccionesAsync(
            List<SeccionDto> secciones);

        /// <summary>
        /// Obtiene todas las secciones con información de departamentos
        /// </summary>
        Task<ResponseDto<List<SeccionResponseDto>>> GetSeccionesAsync();

        /// <summary>
        /// Obtiene secciones por departamento
        /// </summary>
        Task<ResponseDto<List<SeccionResponseDto>>> GetSeccionesByDepartamentoAsync(int numDpto);

        /// <summary>
        /// Obtiene una sección específica
        /// </summary>
        Task<ResponseDto<SeccionResponseDto?>> GetSeccionAsync(int numDpto, int numSeccion);
    }
}
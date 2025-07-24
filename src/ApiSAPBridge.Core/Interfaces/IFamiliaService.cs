using ApiSAPBridge.Models.DTOs;

namespace ApiSAPBridge.Core.Interfaces
{
    /// <summary>
    /// Servicio para gestión de familias
    /// Maneja la lógica de negocio para CRUD de familias
    /// </summary>
    public interface IFamiliaService
    {
        /// <summary>
        /// Crea o actualiza múltiples familias (Upsert)
        /// Valida que las secciones existan antes de crear familias
        /// </summary>
        Task<ResponseDto<List<FamiliaResponseDto>>> CreateFamiliasAsync(
            List<FamiliaDto> familias);

        /// <summary>
        /// Obtiene todas las familias con información de secciones
        /// </summary>
        Task<ResponseDto<List<FamiliaResponseDto>>> GetFamiliasAsync();

        /// <summary>
        /// Obtiene familias por sección
        /// </summary>
        Task<ResponseDto<List<FamiliaResponseDto>>> GetFamiliasBySeccionAsync(
            int numDpto, int numSeccion);

        /// <summary>
        /// Obtiene una familia específica
        /// </summary>
        Task<ResponseDto<FamiliaResponseDto?>> GetFamiliaAsync(
            int numDpto, int numSeccion, int numFamilia);
    }
}
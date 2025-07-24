using ApiSAPBridge.Core.Interfaces;
using ApiSAPBridge.Data.UnitOfWork;
using ApiSAPBridge.Models;
using ApiSAPBridge.Models.DTOs;
using ApiSAPBridge.Models.Constants;
using Mapster;
using Microsoft.Extensions.Logging;

namespace ApiSAPBridge.Services.Implementations
{
    public class FamiliaService : IFamiliaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FamiliaService> _logger;

        public FamiliaService(IUnitOfWork unitOfWork, ILogger<FamiliaService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ResponseDto<List<FamiliaResponseDto>>> CreateFamiliasAsync(
            List<FamiliaDto> familias)
        {
            try
            {
                _logger.LogInformation("Iniciando creación de {Count} familias", familias.Count);

                var resultados = new List<FamiliaResponseDto>();

                foreach (var familiaDto in familias)
                {
                    // Validar que la sección existe
                    var seccionesRepo = _unitOfWork.Repository<Seccion>();
                    var seccion = (await seccionesRepo.FindAsync(s =>
                        s.NUMDPTO == familiaDto.NUMDPTO &&
                        s.NUMSECCION == familiaDto.NUMSECCION)).FirstOrDefault();

                    if (seccion == null)
                    {
                        _logger.LogWarning("Sección {DptoId}-{SeccionId} no encontrada para familia {FamiliaId}",
                            familiaDto.NUMDPTO, familiaDto.NUMSECCION, familiaDto.NUMFAMILIA);
                        continue; // Skip esta familia pero continúa con las demás
                    }

                    // Buscar familia existente
                    var familiasRepo = _unitOfWork.Repository<Familia>();
                    var existente = (await familiasRepo.FindAsync(f =>
                        f.NUMDPTO == familiaDto.NUMDPTO &&
                        f.NUMSECCION == familiaDto.NUMSECCION &&
                        f.NUMFAMILIA == familiaDto.NUMFAMILIA)).FirstOrDefault();

                    if (existente != null)
                    {
                        // Actualizar existente
                        existente.DESCRIPCION = familiaDto.DESCRIPCION;
                        existente.UpdatedAt = DateTime.UtcNow;

                        familiasRepo.Update(existente);
                        _logger.LogInformation("Familia {DptoId}-{SeccionId}-{FamiliaId} actualizada",
                            existente.NUMDPTO, existente.NUMSECCION, existente.NUMFAMILIA);

                        var responseExistente = await BuildFamiliaResponseDto(existente);
                        resultados.Add(responseExistente);
                    }
                    else
                    {
                        // Crear nueva familia
                        var nuevaFamilia = familiaDto.Adapt<Familia>();
                        nuevaFamilia.CreatedAt = DateTime.UtcNow;
                        nuevaFamilia.UpdatedAt = DateTime.UtcNow;

                        await familiasRepo.AddAsync(nuevaFamilia);
                        _logger.LogInformation("Familia {DptoId}-{SeccionId}-{FamiliaId} creada",
                            nuevaFamilia.NUMDPTO, nuevaFamilia.NUMSECCION, nuevaFamilia.NUMFAMILIA);

                        var responseNueva = await BuildFamiliaResponseDto(nuevaFamilia);
                        resultados.Add(responseNueva);
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Procesamiento completado: {Count} familias", resultados.Count);

                return ResponseDto<List<FamiliaResponseDto>>.CreateSuccess(
                    resultados,
                    ApiConstants.ResponseMessages.SUCCESS_CREATE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar familias");
                return ResponseDto<List<FamiliaResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC,
                    ex.Message);
            }
        }

        public async Task<ResponseDto<List<FamiliaResponseDto>>> GetFamiliasAsync()
        {
            try
            {
                var familiasRepo = _unitOfWork.Repository<Familia>();
                var familias = await familiasRepo.GetAllAsync();

                var response = new List<FamiliaResponseDto>();

                foreach (var familia in familias)
                {
                    var familiaDto = await BuildFamiliaResponseDto(familia);
                    response.Add(familiaDto);
                }

                _logger.LogInformation("Obtenidas {Count} familias", response.Count);

                return ResponseDto<List<FamiliaResponseDto>>.CreateSuccess(
                    response,
                    ApiConstants.ResponseMessages.SUCCESS_RETRIEVE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener familias");
                return ResponseDto<List<FamiliaResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC,
                    ex.Message);
            }
        }

        public async Task<ResponseDto<List<FamiliaResponseDto>>> GetFamiliasBySeccionAsync(
            int numDpto, int numSeccion)
        {
            try
            {
                var familiasRepo = _unitOfWork.Repository<Familia>();
                var familias = await familiasRepo.FindAsync(f =>
                    f.NUMDPTO == numDpto && f.NUMSECCION == numSeccion);

                var response = new List<FamiliaResponseDto>();

                foreach (var familia in familias)
                {
                    var familiaDto = await BuildFamiliaResponseDto(familia);
                    response.Add(familiaDto);
                }

                _logger.LogInformation("Obtenidas {Count} familias para sección {DptoId}-{SeccionId}",
                    response.Count, numDpto, numSeccion);

                return ResponseDto<List<FamiliaResponseDto>>.CreateSuccess(
                    response,
                    ApiConstants.ResponseMessages.SUCCESS_RETRIEVE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener familias de la sección {DptoId}-{SeccionId}", numDpto, numSeccion);
                return ResponseDto<List<FamiliaResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC,
                    ex.Message);
            }
        }

        public async Task<ResponseDto<FamiliaResponseDto?>> GetFamiliaAsync(
            int numDpto, int numSeccion, int numFamilia)
        {
            try
            {
                var familiasRepo = _unitOfWork.Repository<Familia>();
                var familia = (await familiasRepo.FindAsync(f =>
                    f.NUMDPTO == numDpto &&
                    f.NUMSECCION == numSeccion &&
                    f.NUMFAMILIA == numFamilia)).FirstOrDefault();

                if (familia == null)
                {
                    _logger.LogWarning("Familia {DptoId}-{SeccionId}-{FamiliaId} no encontrada",
                        numDpto, numSeccion, numFamilia);
                    return ResponseDto<FamiliaResponseDto?>.CreateError(
                        ApiConstants.ResponseMessages.ERROR_NOT_FOUND);
                }

                var response = await BuildFamiliaResponseDto(familia);

                return ResponseDto<FamiliaResponseDto?>.CreateSuccess(
                    response,
                    ApiConstants.ResponseMessages.SUCCESS_RETRIEVE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener familia {DptoId}-{SeccionId}-{FamiliaId}",
                    numDpto, numSeccion, numFamilia);
                return ResponseDto<FamiliaResponseDto?>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC,
                    ex.Message);
            }
        }

        /// <summary>
        /// Método auxiliar para construir FamiliaResponseDto con relaciones cargadas
        /// </summary>
        private async Task<FamiliaResponseDto> BuildFamiliaResponseDto(Familia familia)
        {
            var familiaDto = familia.Adapt<FamiliaResponseDto>();

            // Cargar sección con departamento
            var seccionesRepo = _unitOfWork.Repository<Seccion>();
            var seccion = (await seccionesRepo.FindAsync(s =>
                s.NUMDPTO == familia.NUMDPTO &&
                s.NUMSECCION == familia.NUMSECCION)).FirstOrDefault();

            if (seccion != null)
            {
                var seccionDto = seccion.Adapt<SeccionResponseDto>();

                // Cargar departamento
                var departamento = await _unitOfWork.Repository<Departamento>()
                    .GetByIdAsync(seccion.NUMDPTO);
                seccionDto.Departamento = departamento?.Adapt<DepartamentoResponseDto>();

                familiaDto.Seccion = seccionDto;
            }

            return familiaDto;
        }
    }
}
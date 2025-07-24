using ApiSAPBridge.Core.Interfaces;
using ApiSAPBridge.Data.UnitOfWork;
using ApiSAPBridge.Models;
using ApiSAPBridge.Models.DTOs;
using ApiSAPBridge.Models.Constants;
using Mapster;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace ApiSAPBridge.Services.Implementations
{
    public class SeccionService : ISeccionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SeccionService> _logger;

        public SeccionService(IUnitOfWork unitOfWork, ILogger<SeccionService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ResponseDto<List<SeccionResponseDto>>> CreateSeccionesAsync(
            List<SeccionDto> secciones)
        {
            try
            {
                _logger.LogInformation("Iniciando creación de {Count} secciones", secciones.Count);

                var resultados = new List<SeccionResponseDto>();

                foreach (var seccionDto in secciones)
                {
                    // Validar que el departamento existe
                    var departamento = await _unitOfWork.Repository<Departamento>()
                        .GetByIdAsync(seccionDto.NUMDPTO);

                    if (departamento == null)
                    {
                        _logger.LogWarning("Departamento {DptoId} no encontrado para sección {SeccionId}",
                            seccionDto.NUMDPTO, seccionDto.NUMSECCION);
                        continue; // Skip esta sección pero continúa con las demás
                    }

                    // Buscar sección existente
                    var seccionesRepo = _unitOfWork.Repository<Seccion>();
                    var existente = (await seccionesRepo.FindAsync(s =>
                        s.NUMDPTO == seccionDto.NUMDPTO &&
                        s.NUMSECCION == seccionDto.NUMSECCION)).FirstOrDefault();

                    if (existente != null)
                    {
                        // Actualizar existente
                        existente.DESCRIPCION = seccionDto.DESCRIPCION;
                        existente.UpdatedAt = DateTime.UtcNow;

                        seccionesRepo.Update(existente);
                        _logger.LogInformation("Sección {DptoId}-{SeccionId} actualizada",
                            existente.NUMDPTO, existente.NUMSECCION);

                        var responseExistente = existente.Adapt<SeccionResponseDto>();
                        responseExistente.Departamento = departamento.Adapt<DepartamentoResponseDto>();
                        resultados.Add(responseExistente);
                    }
                    else
                    {
                        // Crear nueva
                        var nuevaSeccion = seccionDto.Adapt<Seccion>();
                        nuevaSeccion.CreatedAt = DateTime.UtcNow;
                        nuevaSeccion.UpdatedAt = DateTime.UtcNow;

                        await seccionesRepo.AddAsync(nuevaSeccion);
                        _logger.LogInformation("Sección {DptoId}-{SeccionId} creada",
                            nuevaSeccion.NUMDPTO, nuevaSeccion.NUMSECCION);

                        var responseNueva = nuevaSeccion.Adapt<SeccionResponseDto>();
                        responseNueva.Departamento = departamento.Adapt<DepartamentoResponseDto>();
                        resultados.Add(responseNueva);
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Procesamiento completado: {Count} secciones", resultados.Count);

                return ResponseDto<List<SeccionResponseDto>>.CreateSuccess(
                    resultados,
                    ApiConstants.ResponseMessages.SUCCESS_CREATE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar secciones");
                return ResponseDto<List<SeccionResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC,
                    ex.Message);
            }
        }

        public async Task<ResponseDto<List<SeccionResponseDto>>> GetSeccionesAsync()
        {
            try
            {
                var seccionesRepo = _unitOfWork.Repository<Seccion>();
                var secciones = await seccionesRepo.GetAllAsync();

                var response = new List<SeccionResponseDto>();

                foreach (var seccion in secciones)
                {
                    var seccionDto = seccion.Adapt<SeccionResponseDto>();

                    // Cargar departamento
                    var departamento = await _unitOfWork.Repository<Departamento>()
                        .GetByIdAsync(seccion.NUMDPTO);
                    seccionDto.Departamento = departamento?.Adapt<DepartamentoResponseDto>();

                    response.Add(seccionDto);
                }

                return ResponseDto<List<SeccionResponseDto>>.CreateSuccess(
                    response,
                    ApiConstants.ResponseMessages.SUCCESS_RETRIEVE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener secciones");
                return ResponseDto<List<SeccionResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC,
                    ex.Message);
            }
        }

        public async Task<ResponseDto<List<SeccionResponseDto>>> GetSeccionesByDepartamentoAsync(int numDpto)
        {
            try
            {
                var seccionesRepo = _unitOfWork.Repository<Seccion>();
                var secciones = await seccionesRepo.FindAsync(s => s.NUMDPTO == numDpto);

                var response = new List<SeccionResponseDto>();

                foreach (var seccion in secciones)
                {
                    var seccionDto = seccion.Adapt<SeccionResponseDto>();

                    // Cargar departamento
                    var departamento = await _unitOfWork.Repository<Departamento>()
                        .GetByIdAsync(seccion.NUMDPTO);
                    seccionDto.Departamento = departamento?.Adapt<DepartamentoResponseDto>();

                    response.Add(seccionDto);
                }

                return ResponseDto<List<SeccionResponseDto>>.CreateSuccess(
                    response,
                    ApiConstants.ResponseMessages.SUCCESS_RETRIEVE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener secciones del departamento {DptoId}", numDpto);
                return ResponseDto<List<SeccionResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC,
                    ex.Message);
            }
        }

        public async Task<ResponseDto<SeccionResponseDto?>> GetSeccionAsync(int numDpto, int numSeccion)
        {
            try
            {
                var seccionesRepo = _unitOfWork.Repository<Seccion>();
                var seccion = (await seccionesRepo.FindAsync(s =>
                    s.NUMDPTO == numDpto && s.NUMSECCION == numSeccion)).FirstOrDefault();

                if (seccion == null)
                {
                    return ResponseDto<SeccionResponseDto?>.CreateError(
                        ApiConstants.ResponseMessages.ERROR_NOT_FOUND);
                }

                var response = seccion.Adapt<SeccionResponseDto>();

                // Cargar departamento
                var departamento = await _unitOfWork.Repository<Departamento>()
                    .GetByIdAsync(seccion.NUMDPTO);
                response.Departamento = departamento?.Adapt<DepartamentoResponseDto>();

                return ResponseDto<SeccionResponseDto?>.CreateSuccess(
                    response,
                    ApiConstants.ResponseMessages.SUCCESS_RETRIEVE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener sección {DptoId}-{SeccionId}", numDpto, numSeccion);
                return ResponseDto<SeccionResponseDto?>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC,
                    ex.Message);
            }
        }
    }
}
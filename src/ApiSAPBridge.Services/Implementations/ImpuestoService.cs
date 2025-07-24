using ApiSAPBridge.Core.Interfaces;
using ApiSAPBridge.Data.UnitOfWork;
using ApiSAPBridge.Models;
using ApiSAPBridge.Models.DTOs;
using ApiSAPBridge.Models.Constants;
using Mapster;
using Microsoft.Extensions.Logging;

namespace ApiSAPBridge.Services.Implementations
{
    public class ImpuestoService : IImpuestoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ImpuestoService> _logger;

        public ImpuestoService(IUnitOfWork unitOfWork, ILogger<ImpuestoService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ResponseDto<List<ImpuestoResponseDto>>> CreateImpuestosAsync(
            List<ImpuestoDto> impuestos)
        {
            try
            {
                _logger.LogInformation("Iniciando creación de {Count} impuestos", impuestos.Count);

                var resultados = new List<ImpuestoResponseDto>();

                foreach (var impuestoDto in impuestos)
                {
                    var impuestosRepo = _unitOfWork.Repository<Impuesto>();
                    var existente = await impuestosRepo.GetByIdAsync(impuestoDto.TIPOIVA);

                    if (existente != null)
                    {
                        // Actualizar existente
                        existente.DESCRIPCION = impuestoDto.DESCRIPCION;
                        existente.IVA = impuestoDto.IVA;
                        existente.UpdatedAt = DateTime.UtcNow;

                        impuestosRepo.Update(existente);
                        _logger.LogInformation("Impuesto {Id} actualizado - IVA: {Iva}%",
                            existente.TIPOIVA, existente.IVA);

                        var responseExistente = existente.Adapt<ImpuestoResponseDto>();
                        responseExistente.ArticulosAsociados = await GetArticulosCount(existente.TIPOIVA);
                        resultados.Add(responseExistente);
                    }
                    else
                    {
                        // Crear nuevo
                        var nuevoImpuesto = impuestoDto.Adapt<Impuesto>();
                        nuevoImpuesto.CreatedAt = DateTime.UtcNow;
                        nuevoImpuesto.UpdatedAt = DateTime.UtcNow;

                        await impuestosRepo.AddAsync(nuevoImpuesto);
                        _logger.LogInformation("Impuesto {Id} creado - IVA: {Iva}%",
                            nuevoImpuesto.TIPOIVA, nuevoImpuesto.IVA);

                        var responseNuevo = nuevoImpuesto.Adapt<ImpuestoResponseDto>();
                        responseNuevo.ArticulosAsociados = 0; // Recién creado
                        resultados.Add(responseNuevo);
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                return ResponseDto<List<ImpuestoResponseDto>>.CreateSuccess(
                    resultados,
                    ApiConstants.ResponseMessages.SUCCESS_CREATE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar impuestos");
                return ResponseDto<List<ImpuestoResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC,
                    ex.Message);
            }
        }

        public async Task<ResponseDto<List<ImpuestoResponseDto>>> GetImpuestosAsync()
        {
            try
            {
                var impuestos = await _unitOfWork.Repository<Impuesto>().GetAllAsync();
                var response = new List<ImpuestoResponseDto>();

                foreach (var impuesto in impuestos)
                {
                    var impuestoDto = impuesto.Adapt<ImpuestoResponseDto>();
                    impuestoDto.ArticulosAsociados = await GetArticulosCount(impuesto.TIPOIVA);
                    response.Add(impuestoDto);
                }

                _logger.LogInformation("Obtenidos {Count} impuestos", response.Count);

                return ResponseDto<List<ImpuestoResponseDto>>.CreateSuccess(
                    response,
                    ApiConstants.ResponseMessages.SUCCESS_RETRIEVE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener impuestos");
                return ResponseDto<List<ImpuestoResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC,
                    ex.Message);
            }
        }

        public async Task<ResponseDto<ImpuestoResponseDto?>> GetImpuestoByIdAsync(int tipoIva)
        {
            try
            {
                var impuesto = await _unitOfWork.Repository<Impuesto>().GetByIdAsync(tipoIva);

                if (impuesto == null)
                {
                    return ResponseDto<ImpuestoResponseDto?>.CreateError(
                        ApiConstants.ResponseMessages.ERROR_NOT_FOUND);
                }

                var response = impuesto.Adapt<ImpuestoResponseDto>();
                response.ArticulosAsociados = await GetArticulosCount(impuesto.TIPOIVA);

                return ResponseDto<ImpuestoResponseDto?>.CreateSuccess(
                    response,
                    ApiConstants.ResponseMessages.SUCCESS_RETRIEVE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener impuesto {Id}", tipoIva);
                return ResponseDto<ImpuestoResponseDto?>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC,
                    ex.Message);
            }
        }

        public async Task<ResponseDto<bool>> DeleteImpuestoAsync(int tipoIva)
        {
            try
            {
                // Verificar si hay artículos usando este impuesto
                var articulosCount = await GetArticulosCount(tipoIva);
                if (articulosCount > 0)
                {
                    return ResponseDto<bool>.CreateError(
                        $"No se puede eliminar el impuesto. Hay {articulosCount} artículos asociados.");
                }

                var impuestosRepo = _unitOfWork.Repository<Impuesto>();
                var impuesto = await impuestosRepo.GetByIdAsync(tipoIva);

                if (impuesto == null)
                {
                    return ResponseDto<bool>.CreateError(
                        ApiConstants.ResponseMessages.ERROR_NOT_FOUND);
                }

                impuestosRepo.Remove(impuesto);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Impuesto {Id} eliminado", tipoIva);

                return ResponseDto<bool>.CreateSuccess(
                    true,
                    "Impuesto eliminado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar impuesto {Id}", tipoIva);
                return ResponseDto<bool>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC,
                    ex.Message);
            }
        }

        public async Task<ResponseDto<ImpuestoStatsDto>> GetImpuestosStatsAsync()
        {
            try
            {
                var impuestos = await _unitOfWork.Repository<Impuesto>().GetAllAsync();
                var articulos = await _unitOfWork.Repository<Articulo>().GetAllAsync();

                var stats = new ImpuestoStatsDto
                {
                    TotalImpuestos = impuestos.Count(),
                    IvaMinimo = impuestos.Any() ? impuestos.Min(i => i.IVA) : 0,
                    IvaMaximo = impuestos.Any() ? impuestos.Max(i => i.IVA) : 0,
                    IvaPromedio = impuestos.Any() ? Math.Round(impuestos.Average(i => i.IVA), 2) : 0,
                    ArticulosTotales = articulos.Count()
                };

                return ResponseDto<ImpuestoStatsDto>.CreateSuccess(
                    stats,
                    "Estadísticas obtenidas exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de impuestos");
                return ResponseDto<ImpuestoStatsDto>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC,
                    ex.Message);
            }
        }

        /// <summary>
        /// Método auxiliar para contar artículos asociados a un impuesto
        /// </summary>
        private async Task<int> GetArticulosCount(int tipoIva)
        {
            try
            {
                var articulos = await _unitOfWork.Repository<Articulo>()
                    .FindAsync(a => a.TIPOIMPUESTO == tipoIva);
                return articulos.Count();
            }
            catch
            {
                return 0; // En caso de error, retornar 0
            }
        }
    }
}
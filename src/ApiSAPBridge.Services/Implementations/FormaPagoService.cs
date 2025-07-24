using ApiSAPBridge.Core.Interfaces;
using ApiSAPBridge.Data.UnitOfWork;
using ApiSAPBridge.Models;
using ApiSAPBridge.Models.DTOs;
using ApiSAPBridge.Models.Constants;
using Mapster;
using Microsoft.Extensions.Logging;

namespace ApiSAPBridge.Services.Implementations
{
    public class FormaPagoService : IFormaPagoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FormaPagoService> _logger;

        public FormaPagoService(IUnitOfWork unitOfWork, ILogger<FormaPagoService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ResponseDto<List<FormaPagoResponseDto>>> CreateFormasPagoAsync(
            List<FormaPagoDto> formasPago)
        {
            try
            {
                _logger.LogInformation("Iniciando creación de {Count} formas de pago", formasPago.Count);

                var resultados = new List<FormaPagoResponseDto>();

                foreach (var formaPagoDto in formasPago)
                {
                    var formasPagoRepo = _unitOfWork.Repository<FormaPago>();
                    var existente = await formasPagoRepo.GetByIdAsync(formaPagoDto.CODFORMAPAGO);

                    if (existente != null)
                    {
                        // Actualizar existente
                        existente.DESCRIPCION = formaPagoDto.DESCRIPCION;
                        existente.NUMVENCIMIENTOS = formaPagoDto.NUMVENCIMIENTOS;
                        existente.UpdatedAt = DateTime.UtcNow;

                        formasPagoRepo.Update(existente);
                        _logger.LogInformation("Forma de pago {Codigo} actualizada - {Vencimientos} vencimientos",
                            existente.CODFORMAPAGO, existente.NUMVENCIMIENTOS);

                        var responseExistente = existente.Adapt<FormaPagoResponseDto>();
                        responseExistente.ClientesAsociados = await GetClientesCount(existente.CODFORMAPAGO);
                        resultados.Add(responseExistente);
                    }
                    else
                    {
                        // Crear nueva
                        var nuevaFormaPago = formaPagoDto.Adapt<FormaPago>();
                        nuevaFormaPago.CreatedAt = DateTime.UtcNow;
                        nuevaFormaPago.UpdatedAt = DateTime.UtcNow;

                        await formasPagoRepo.AddAsync(nuevaFormaPago);
                        _logger.LogInformation("Forma de pago {Codigo} creada - {Vencimientos} vencimientos",
                            nuevaFormaPago.CODFORMAPAGO, nuevaFormaPago.NUMVENCIMIENTOS);

                        var responseNueva = nuevaFormaPago.Adapt<FormaPagoResponseDto>();
                        responseNueva.ClientesAsociados = 0; // Recién creada
                        resultados.Add(responseNueva);
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                return ResponseDto<List<FormaPagoResponseDto>>.CreateSuccess(
                    resultados,
                    ApiConstants.ResponseMessages.SUCCESS_CREATE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar formas de pago");
                return ResponseDto<List<FormaPagoResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC,
                    ex.Message);
            }
        }

        public async Task<ResponseDto<List<FormaPagoResponseDto>>> GetFormasPagoAsync()
        {
            try
            {
                var formasPago = await _unitOfWork.Repository<FormaPago>().GetAllAsync();
                var response = new List<FormaPagoResponseDto>();

                foreach (var formaPago in formasPago)
                {
                    var formaPagoDto = formaPago.Adapt<FormaPagoResponseDto>();
                    formaPagoDto.ClientesAsociados = await GetClientesCount(formaPago.CODFORMAPAGO);
                    response.Add(formaPagoDto);
                }

                _logger.LogInformation("Obtenidas {Count} formas de pago", response.Count);

                return ResponseDto<List<FormaPagoResponseDto>>.CreateSuccess(
                    response,
                    ApiConstants.ResponseMessages.SUCCESS_RETRIEVE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener formas de pago");
                return ResponseDto<List<FormaPagoResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC,
                    ex.Message);
            }
        }

        public async Task<ResponseDto<FormaPagoResponseDto?>> GetFormaPagoByIdAsync(int codigo)
        {
            try
            {
                var formaPago = await _unitOfWork.Repository<FormaPago>().GetByIdAsync(codigo);

                if (formaPago == null)
                {
                    return ResponseDto<FormaPagoResponseDto?>.CreateError(
                        ApiConstants.ResponseMessages.ERROR_NOT_FOUND);
                }

                var response = formaPago.Adapt<FormaPagoResponseDto>();
                response.ClientesAsociados = await GetClientesCount(formaPago.CODFORMAPAGO);

                return ResponseDto<FormaPagoResponseDto?>.CreateSuccess(
                    response,
                    ApiConstants.ResponseMessages.SUCCESS_RETRIEVE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener forma de pago {Codigo}", codigo);
                return ResponseDto<FormaPagoResponseDto?>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC,
                    ex.Message);
            }
        }

        public async Task<ResponseDto<bool>> DeleteFormaPagoAsync(int codigo)
        {
            try
            {
                // Verificar si hay clientes usando esta forma de pago
                var clientesCount = await GetClientesCount(codigo);
                if (clientesCount > 0)
                {
                    return ResponseDto<bool>.CreateError(
                        $"No se puede eliminar la forma de pago. Hay {clientesCount} clientes asociados.");
                }

                var formasPagoRepo = _unitOfWork.Repository<FormaPago>();
                var formaPago = await formasPagoRepo.GetByIdAsync(codigo);

                if (formaPago == null)
                {
                    return ResponseDto<bool>.CreateError(
                        ApiConstants.ResponseMessages.ERROR_NOT_FOUND);
                }

                formasPagoRepo.Remove(formaPago);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Forma de pago {Codigo} eliminada", codigo);

                return ResponseDto<bool>.CreateSuccess(
                    true,
                    "Forma de pago eliminada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar forma de pago {Codigo}", codigo);
                return ResponseDto<bool>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC,
                    ex.Message);
            }
        }

        public async Task<ResponseDto<FormaPagoStatsDto>> GetFormasPagoStatsAsync()
        {
            try
            {
                var formasPago = await _unitOfWork.Repository<FormaPago>().GetAllAsync();

                var stats = new FormaPagoStatsDto
                {
                    TotalFormasPago = formasPago.Count(),
                    FormasContado = formasPago.Count(f => f.NUMVENCIMIENTOS == 1),
                    FormasCredito = formasPago.Count(f => f.NUMVENCIMIENTOS > 1),
                    VencimientosMaximo = formasPago.Any() ? formasPago.Max(f => f.NUMVENCIMIENTOS) : 0,
                    VencimientosPromedio = formasPago.Any() ? Math.Round(formasPago.Average(f => f.NUMVENCIMIENTOS), 1) : 0
                };

                return ResponseDto<FormaPagoStatsDto>.CreateSuccess(
                    stats,
                    "Estadísticas obtenidas exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de formas de pago");
                return ResponseDto<FormaPagoStatsDto>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC,
                    ex.Message);
            }
        }

        public async Task<ResponseDto<List<FormaPagoResponseDto>>> GetFormasPagoByTipoAsync(string tipo)
        {
            try
            {
                var formasPagoRepo = _unitOfWork.Repository<FormaPago>();
                IEnumerable<FormaPago> formasPago;

                if (tipo.ToLower() == "contado")
                {
                    formasPago = await formasPagoRepo.FindAsync(f => f.NUMVENCIMIENTOS == 1);
                }
                else if (tipo.ToLower() == "credito")
                {
                    formasPago = await formasPagoRepo.FindAsync(f => f.NUMVENCIMIENTOS > 1);
                }
                else
                {
                    return ResponseDto<List<FormaPagoResponseDto>>.CreateError(
                        "Tipo debe ser 'contado' o 'credito'");
                }

                var response = new List<FormaPagoResponseDto>();

                foreach (var formaPago in formasPago)
                {
                    var formaPagoDto = formaPago.Adapt<FormaPagoResponseDto>();
                    formaPagoDto.ClientesAsociados = await GetClientesCount(formaPago.CODFORMAPAGO);
                    response.Add(formaPagoDto);
                }

                return ResponseDto<List<FormaPagoResponseDto>>.CreateSuccess(
                    response,
                    ApiConstants.ResponseMessages.SUCCESS_RETRIEVE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener formas de pago por tipo {Tipo}", tipo);
                return ResponseDto<List<FormaPagoResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC,
                    ex.Message);
            }
        }

        /// <summary>
        /// Método auxiliar para contar clientes asociados a una forma de pago
        /// </summary>
        private async Task<int> GetClientesCount(int codigoFormaPago)
        {
            try
            {
                // Nota: En el modelo Cliente no veo campo de forma de pago directamente
                // Ajustar según tu modelo real si existe relación
                // Por ahora retorno 0 como placeholder
                return 0;
            }
            catch
            {
                return 0;
            }
        }
    }
}
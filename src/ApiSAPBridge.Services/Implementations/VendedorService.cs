using ApiSAPBridge.Core.Interfaces;
using ApiSAPBridge.Data.UnitOfWork;
using ApiSAPBridge.Models;
using ApiSAPBridge.Models.DTOs;
using ApiSAPBridge.Models.Constants;
using Mapster;
using Microsoft.Extensions.Logging;

namespace ApiSAPBridge.Services.Implementations
{
    public class VendedorService : IVendedorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<VendedorService> _logger;

        public VendedorService(IUnitOfWork unitOfWork, ILogger<VendedorService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ResponseDto<List<VendedorResponseDto>>> CreateVendedoresAsync(
            List<VendedorDto> vendedores)
        {
            try
            {
                _logger.LogInformation("Iniciando creación de {Count} vendedores", vendedores.Count);

                var resultados = new List<VendedorResponseDto>();

                foreach (var vendedorDto in vendedores)
                {
                    var vendedoresRepo = _unitOfWork.Repository<Vendedor>();
                    var existente = await vendedoresRepo.GetByIdAsync(vendedorDto.CODVENDEDOR);

                    if (existente != null)
                    {
                        // Actualizar existente
                        existente.NOMBRE = vendedorDto.NOMBRE;
                        existente.UpdatedAt = DateTime.UtcNow;

                        vendedoresRepo.Update(existente);
                        _logger.LogInformation("Vendedor {Id} actualizado", existente.CODVENDEDOR);

                        resultados.Add(existente.Adapt<VendedorResponseDto>());
                    }
                    else
                    {
                        // Crear nuevo
                        var nuevoVendedor = vendedorDto.Adapt<Vendedor>();
                        nuevoVendedor.CreatedAt = DateTime.UtcNow;
                        nuevoVendedor.UpdatedAt = DateTime.UtcNow;

                        await vendedoresRepo.AddAsync(nuevoVendedor);
                        _logger.LogInformation("Vendedor {Id} creado", nuevoVendedor.CODVENDEDOR);

                        resultados.Add(nuevoVendedor.Adapt<VendedorResponseDto>());
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                return ResponseDto<List<VendedorResponseDto>>.CreateSuccess(
                    resultados,
                    ApiConstants.ResponseMessages.SUCCESS_CREATE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar vendedores");
                return ResponseDto<List<VendedorResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC,
                    ex.Message);
            }
        }

        public async Task<ResponseDto<List<VendedorResponseDto>>> GetVendedoresAsync()
        {
            try
            {
                var vendedores = await _unitOfWork.Repository<Vendedor>().GetAllAsync();
                var response = vendedores.Adapt<List<VendedorResponseDto>>();

                return ResponseDto<List<VendedorResponseDto>>.CreateSuccess(
                    response,
                    ApiConstants.ResponseMessages.SUCCESS_RETRIEVE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener vendedores");
                return ResponseDto<List<VendedorResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC,
                    ex.Message);
            }
        }

        public async Task<ResponseDto<VendedorResponseDto?>> GetVendedorByIdAsync(int id)
        {
            try
            {
                var vendedor = await _unitOfWork.Repository<Vendedor>().GetByIdAsync(id);

                if (vendedor == null)
                {
                    return ResponseDto<VendedorResponseDto?>.CreateError(
                        ApiConstants.ResponseMessages.ERROR_NOT_FOUND);
                }

                var response = vendedor.Adapt<VendedorResponseDto>();
                return ResponseDto<VendedorResponseDto?>.CreateSuccess(
                    response,
                    ApiConstants.ResponseMessages.SUCCESS_RETRIEVE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener vendedor {Id}", id);
                return ResponseDto<VendedorResponseDto?>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC,
                    ex.Message);
            }
        }

        public async Task<ResponseDto<bool>> DeleteVendedorAsync(int id)
        {
            try
            {
                var vendedoresRepo = _unitOfWork.Repository<Vendedor>();
                var vendedor = await vendedoresRepo.GetByIdAsync(id);

                if (vendedor == null)
                {
                    return ResponseDto<bool>.CreateError(
                        ApiConstants.ResponseMessages.ERROR_NOT_FOUND);
                }

                vendedoresRepo.Remove(vendedor);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Vendedor {Id} eliminado", id);

                return ResponseDto<bool>.CreateSuccess(
                    true,
                    "Vendedor eliminado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar vendedor {Id}", id);
                return ResponseDto<bool>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC,
                    ex.Message);
            }
        }
    }
}
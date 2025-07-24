using ApiSAPBridge.Core.Interfaces;
using ApiSAPBridge.Data.UnitOfWork;
using ApiSAPBridge.Models;
using ApiSAPBridge.Models.DTOs;
using ApiSAPBridge.Models.Constants;
using Mapster;
using Microsoft.Extensions.Logging;

namespace ApiSAPBridge.Services.Implementations
{
    public class DepartamentoService : IDepartamentoService
    {
        private readonly Core.Interfaces.IUnitOfWork _unitOfWork;
        private readonly ILogger<DepartamentoService> _logger;

        public DepartamentoService(Core.Interfaces.IUnitOfWork unitOfWork, ILogger<DepartamentoService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ResponseDto<List<DepartamentoResponseDto>>> CreateDepartamentosAsync(
            List<DepartamentoDto> departamentos)
        {
            try
            {
                _logger.LogInformation("Iniciando creación de {Count} departamentos", departamentos.Count);

                var entidades = departamentos.Adapt<List<Departamento>>();
                var resultados = new List<DepartamentoResponseDto>();

                foreach (var entidad in entidades)
                {
                    // Verificar si ya existe
                    var existente = await _unitOfWork.Repository<Departamento>()
                        .GetByIdAsync(entidad.NUMDPTO);

                    if (existente != null)
                    {
                        // Actualizar existente
                        existente.DESCRIPCION = entidad.DESCRIPCION;
                        existente.UpdatedAt = DateTime.UtcNow;

                        _unitOfWork.Repository<Departamento>().Update(existente);
                        _logger.LogInformation("Departamento {Id} actualizado", existente.NUMDPTO);

                        resultados.Add(existente.Adapt<DepartamentoResponseDto>());
                    }
                    else
                    {
                        // Crear nuevo
                        entidad.CreatedAt = DateTime.UtcNow;
                        entidad.UpdatedAt = DateTime.UtcNow;

                        await _unitOfWork.Repository<Departamento>().AddAsync(entidad);
                        _logger.LogInformation("Departamento {Id} creado", entidad.NUMDPTO);

                        resultados.Add(entidad.Adapt<DepartamentoResponseDto>());
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Procesamiento completado: {Count} departamentos", resultados.Count);

                return ResponseDto<List<DepartamentoResponseDto>>.CreateSuccess(
                    resultados,
                    ApiConstants.ResponseMessages.SUCCESS_CREATE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar departamentos");
                return ResponseDto<List<DepartamentoResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC,
                    ex.Message);
            }
        }

        public async Task<ResponseDto<List<DepartamentoResponseDto>>> GetDepartamentosAsync()
        {
            try
            {
                var departamentos = await _unitOfWork.Repository<Departamento>().GetAllAsync();
                var response = departamentos.Adapt<List<DepartamentoResponseDto>>();

                return ResponseDto<List<DepartamentoResponseDto>>.CreateSuccess(
                    response,
                    ApiConstants.ResponseMessages.SUCCESS_RETRIEVE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener departamentos");
                return ResponseDto<List<DepartamentoResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC,
                    ex.Message);
            }
        }

        public async Task<ResponseDto<DepartamentoResponseDto?>> GetDepartamentoByIdAsync(int id)
        {
            try
            {
                var departamento = await _unitOfWork.Repository<Departamento>().GetByIdAsync(id);

                if (departamento == null)
                {
                    return ResponseDto<DepartamentoResponseDto?>.CreateError(
                        ApiConstants.ResponseMessages.ERROR_NOT_FOUND);
                }

                var response = departamento.Adapt<DepartamentoResponseDto>();
                return ResponseDto<DepartamentoResponseDto?>.CreateSuccess(
                    response,
                    ApiConstants.ResponseMessages.SUCCESS_RETRIEVE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener departamento {Id}", id);
                return ResponseDto<DepartamentoResponseDto?>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC,
                    ex.Message);
            }
        }
    }
}
using ApiSAPBridge.Core.Interfaces;
using ApiSAPBridge.Data.UnitOfWork;
using ApiSAPBridge.Models;
using ApiSAPBridge.Models.DTOs;
using ApiSAPBridge.Models.Constants;
using Mapster;
using Microsoft.Extensions.Logging;

namespace ApiSAPBridge.Services.Implementations
{
    public class TarifaService : ITarifaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TarifaService> _logger;

        public TarifaService(IUnitOfWork unitOfWork, ILogger<TarifaService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ResponseDto<List<TarifaResponseDto>>> CreateTarifasAsync(List<TarifaDto> tarifas)
        {
            try
            {
                _logger.LogInformation("Iniciando creación de {Count} tarifas", tarifas.Count);

                var resultados = new List<TarifaResponseDto>();
                var errores = new List<string>();

                foreach (var tarifaDto in tarifas)
                {
                    // Validaciones de negocio específicas
                    var validationResult = await ValidateTarifaBusinessRules(tarifaDto);
                    if (!validationResult.IsValid)
                    {
                        errores.AddRange(validationResult.Errors);
                        _logger.LogWarning("Tarifa {Id} falló validaciones: {Errors}",
                            tarifaDto.IDTARIFAV, string.Join(", ", validationResult.Errors));
                        continue;
                    }

                    var tarifasRepo = _unitOfWork.Repository<Tarifa>();
                    var existente = await tarifasRepo.GetByIdAsync(tarifaDto.IDTARIFAV);

                    if (existente != null)
                    {
                        // Actualizar existente
                        await UpdateExistingTarifa(existente, tarifaDto);
                        tarifasRepo.Update(existente);

                        _logger.LogInformation("Tarifa {Id} actualizada", existente.IDTARIFAV);

                        var responseExistente = await BuildTarifaResponseDto(existente);
                        resultados.Add(responseExistente);
                    }
                    else
                    {
                        // Crear nueva
                        var nuevaTarifa = tarifaDto.Adapt<Tarifa>();
                        nuevaTarifa.CreatedAt = DateTime.UtcNow;
                        nuevaTarifa.UpdatedAt = DateTime.UtcNow;

                        await tarifasRepo.AddAsync(nuevaTarifa);
                        _logger.LogInformation("Tarifa {Id} creada", nuevaTarifa.IDTARIFAV);

                        var responseNueva = await BuildTarifaResponseDto(nuevaTarifa);
                        resultados.Add(responseNueva);
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                if (errores.Any() && !resultados.Any())
                {
                    return ResponseDto<List<TarifaResponseDto>>.CreateError(
                        "Todas las tarifas fallaron las validaciones", errores);
                }

                var message = errores.Any()
                    ? $"Procesadas {resultados.Count} tarifas, {errores.Count} con errores"
                    : ApiConstants.ResponseMessages.SUCCESS_CREATE;

                return ResponseDto<List<TarifaResponseDto>>.CreateSuccess(resultados, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar tarifas");
                return ResponseDto<List<TarifaResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<List<TarifaResponseDto>>> GetTarifasAsync(bool incluirConteoPrecios = false)
        {
            try
            {
                var tarifas = await _unitOfWork.Repository<Tarifa>().GetAllAsync();
                var response = new List<TarifaResponseDto>();

                foreach (var tarifa in tarifas)
                {
                    var tarifaDto = await BuildTarifaResponseDto(tarifa, incluirConteoPrecios);
                    response.Add(tarifaDto);
                }

                _logger.LogInformation("Obtenidas {Count} tarifas", response.Count);

                return ResponseDto<List<TarifaResponseDto>>.CreateSuccess(
                    response, ApiConstants.ResponseMessages.SUCCESS_RETRIEVE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tarifas");
                return ResponseDto<List<TarifaResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<List<TarifaResponseDto>>> GetTarifasActivasAsync(DateTime? fecha = null)
        {
            try
            {
                var fechaConsulta = fecha ?? DateTime.Today;
                var tarifasRepo = _unitOfWork.Repository<Tarifa>();

                var tarifasActivas = await tarifasRepo.FindAsync(t =>
                    t.FECHAINI <= fechaConsulta && t.FECHAFIN >= fechaConsulta);

                var response = new List<TarifaResponseDto>();
                foreach (var tarifa in tarifasActivas)
                {
                    var tarifaDto = await BuildTarifaResponseDto(tarifa);
                    response.Add(tarifaDto);
                }

                return ResponseDto<List<TarifaResponseDto>>.CreateSuccess(
                    response, $"Encontradas {response.Count} tarifas activas para {fechaConsulta:yyyy-MM-dd}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tarifas activas");
                return ResponseDto<List<TarifaResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<List<TarifaResponseDto>>> GetTarifasVigentesAsync()
        {
            return await GetTarifasActivasAsync(DateTime.Today);
        }
        public async Task<ResponseDto<TarifaResponseDto?>> GetTarifaByIdAsync(int id, bool incluirConteoPrecios = false)
        {
            try
            {
                var tarifa = await _unitOfWork.Repository<Tarifa>().GetByIdAsync(id);

                if (tarifa == null)
                {
                    _logger.LogWarning("Tarifa {Id} no encontrada", id);
                    return ResponseDto<TarifaResponseDto?>.CreateError(
                        ApiConstants.ResponseMessages.ERROR_NOT_FOUND);
                }

                var response = await BuildTarifaResponseDto(tarifa, incluirConteoPrecios);

                _logger.LogInformation("Tarifa {Id} obtenida exitosamente", id);
                return ResponseDto<TarifaResponseDto?>.CreateSuccess(
                    response, ApiConstants.ResponseMessages.SUCCESS_RETRIEVE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tarifa {Id}", id);
                return ResponseDto<TarifaResponseDto?>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<List<TarifaResponseDto>>> SearchTarifasAsync(TarifaSearchRequest request)
        {
            try
            {
                _logger.LogInformation("Iniciando búsqueda de tarifas con filtros");

                var tarifasRepo = _unitOfWork.Repository<Tarifa>();
                var todasLasTarifas = await tarifasRepo.GetAllAsync();

                // Aplicar filtros
                var tarifasFiltradas = todasLasTarifas.AsQueryable();

                // Filtro por término de búsqueda
                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var searchTerm = request.SearchTerm.ToLower();
                    tarifasFiltradas = tarifasFiltradas.Where(t =>
                        t.DESCRIPCION.ToLower().Contains(searchTerm) ||
                        t.IDTARIFAV.ToString().Contains(searchTerm));
                }

                // Filtro por rango de fechas
                if (request.FechaDesde.HasValue)
                {
                    tarifasFiltradas = tarifasFiltradas.Where(t => t.FECHAINI >= request.FechaDesde.Value);
                }

                if (request.FechaHasta.HasValue)
                {
                    tarifasFiltradas = tarifasFiltradas.Where(t => t.FECHAFIN <= request.FechaHasta.Value);
                }

                // Filtro solo activas
                if (request.SoloActivas.HasValue && request.SoloActivas.Value)
                {
                    var hoy = DateTime.Today;
                    tarifasFiltradas = tarifasFiltradas.Where(t =>
                        t.FECHAINI <= hoy && t.FECHAFIN >= hoy);
                }

                // Filtro solo vigentes (mismo que activas para este caso)
                if (request.SoloVigentes.HasValue && request.SoloVigentes.Value)
                {
                    var hoy = DateTime.Today;
                    tarifasFiltradas = tarifasFiltradas.Where(t =>
                        t.FECHAINI <= hoy && t.FECHAFIN >= hoy);
                }

                // Contar total antes de paginación
                var totalCount = tarifasFiltradas.Count();

                // Aplicar paginación
                var tarifasPaginadas = tarifasFiltradas
                    .OrderBy(t => t.FECHAINI) // Ordenar por fecha de inicio
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                // Construir respuesta
                var response = new List<TarifaResponseDto>();
                foreach (var tarifa in tarifasPaginadas)
                {
                    var tarifaDto = await BuildTarifaResponseDto(tarifa, request.IncluirConteoPrecios ?? false);
                    response.Add(tarifaDto);
                }

                var message = $"Búsqueda completada: {totalCount} tarifas encontradas, mostrando página {request.Page} de {Math.Ceiling((double)totalCount / request.PageSize)}";

                _logger.LogInformation("Búsqueda de tarifas completada: {Total} encontradas, {Returned} devueltas",
                    totalCount, response.Count);

                return ResponseDto<List<TarifaResponseDto>>.CreateSuccess(response, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en búsqueda de tarifas");
                return ResponseDto<List<TarifaResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<object>> GetEstadisticasTarifasAsync()
        {
            try
            {
                _logger.LogInformation("Generando estadísticas de tarifas");

                var tarifas = await _unitOfWork.Repository<Tarifa>().GetAllAsync();
                var hoy = DateTime.Today;

                // Calcular estadísticas
                var tarifasActivas = tarifas.Where(t => t.FECHAINI <= hoy && t.FECHAFIN >= hoy).ToList();
                var tarifasVencidas = tarifas.Where(t => t.FECHAFIN < hoy).ToList();
                var tarifasPendientes = tarifas.Where(t => t.FECHAINI > hoy).ToList();

                // Obtener información de precios si es necesario
                var preciosRepo = _unitOfWork.Repository<Precio>();
                var todosLosPrecios = await preciosRepo.GetAllAsync();

                var stats = new
                {
                    // Conteos básicos
                    TotalTarifas = tarifas.Count(),
                    TarifasActivas = tarifasActivas.Count,
                    TarifasVencidas = tarifasVencidas.Count,
                    TarifasPendientes = tarifasPendientes.Count,

                    // Porcentajes
                    PorcentajeActivas = tarifas.Count() > 0 ? Math.Round((double)tarifasActivas.Count / tarifas.Count() * 100, 2) : 0,
                    PorcentajeVencidas = tarifas.Count() > 0 ? Math.Round((double)tarifasVencidas.Count / tarifas.Count() * 100, 2) : 0,
                    PorcentajePendientes = tarifas.Count() > 0 ? Math.Round((double)tarifasPendientes.Count / tarifas.Count() * 100, 2) : 0,

                    // Información temporal
                    TarifaMasAntigua = tarifas.Any() ? tarifas.Min(t => t.FECHAINI) : (DateTime?)null,
                    TarifaMasReciente = tarifas.Any() ? tarifas.Max(t => t.FECHAFIN) : (DateTime?)null,
                    DuracionPromedioDias = tarifas.Any() ? Math.Round(tarifas.Average(t => (t.FECHAFIN - t.FECHAINI).TotalDays), 0) : 0,

                    // Información de precios relacionados
                    TotalPreciosAsociados = todosLosPrecios.Count(),
                    TarifasConPrecios = todosLosPrecios.GroupBy(p => p.IDTARIFAV).Count(),
                    PromedioPrecionsPorTarifa = tarifas.Count() > 0 ? Math.Round((double)todosLosPrecios.Count() / tarifas.Count(), 2) : 0,

                    // Próximos vencimientos (próximos 30 días)
                    ProximosVencimientos = tarifasActivas
                        .Where(t => (t.FECHAFIN - hoy).TotalDays <= 30)
                        .Select(t => new {
                            Id = t.IDTARIFAV,
                            Descripcion = t.DESCRIPCION,
                            FechaVencimiento = t.FECHAFIN,
                            DiasRestantes = (t.FECHAFIN - hoy).Days
                        })
                        .OrderBy(t => t.FechaVencimiento)
                        .ToList(),

                    // Próximas activaciones (próximos 30 días)
                    ProximasActivaciones = tarifasPendientes
                        .Where(t => (t.FECHAINI - hoy).TotalDays <= 30)
                        .Select(t => new {
                            Id = t.IDTARIFAV,
                            Descripcion = t.DESCRIPCION,
                            FechaInicio = t.FECHAINI,
                            DiasParaInicio = (t.FECHAINI - hoy).Days
                        })
                        .OrderBy(t => t.FechaInicio)
                        .ToList(),

                    // Distribución por año
                    DistribucionPorAno = tarifas
                        .GroupBy(t => t.FECHAINI.Year)
                        .Select(g => new {
                            Ano = g.Key,
                            Cantidad = g.Count()
                        })
                        .OrderBy(x => x.Ano)
                        .ToList(),

                    // Metadatos
                    FechaGeneracion = DateTime.UtcNow,
                    VersionEstadisticas = "1.0"
                };

                _logger.LogInformation("Estadísticas de tarifas generadas exitosamente");

                return ResponseDto<object>.CreateSuccess(stats, "Estadísticas de tarifas generadas exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar estadísticas de tarifas");
                return ResponseDto<object>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }
        public async Task<ResponseDto<bool>> ValidarSolapamientoAsync(int idTarifa, DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                var tarifasRepo = _unitOfWork.Repository<Tarifa>();
                var tarifasSolapadas = await tarifasRepo.FindAsync(t =>
                    t.IDTARIFAV != idTarifa &&
                    ((t.FECHAINI <= fechaInicio && t.FECHAFIN >= fechaInicio) ||
                     (t.FECHAINI <= fechaFin && t.FECHAFIN >= fechaFin) ||
                     (t.FECHAINI >= fechaInicio && t.FECHAFIN <= fechaFin)));

                var haySolapamiento = tarifasSolapadas.Any();
                var message = haySolapamiento
                    ? $"Existe solapamiento con {tarifasSolapadas.Count()} tarifa(s)"
                    : "No hay solapamiento de fechas";

                return ResponseDto<bool>.CreateSuccess(!haySolapamiento, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar solapamiento de tarifa {Id}", idTarifa);
                return ResponseDto<bool>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        #region Métodos Auxiliares

        private async Task<TarifaResponseDto> BuildTarifaResponseDto(Tarifa tarifa, bool incluirConteoPrecios = false)
        {
            var tarifaDto = tarifa.Adapt<TarifaResponseDto>();

            // Calcular información de estado
            var hoy = DateTime.Today;
            tarifaDto.EstadoInfo = new TarifaEstadoInfo
            {
                EstaActiva = tarifa.FECHAINI <= hoy && tarifa.FECHAFIN >= hoy,
                EstaVigente = tarifa.FECHAINI <= hoy && tarifa.FECHAFIN >= hoy,
                EstaVencida = tarifa.FECHAFIN < hoy,
                EsPendiente = tarifa.FECHAINI > hoy,
                DiasParaInicio = (tarifa.FECHAINI - hoy).Days,
                DiasParaVencimiento = (tarifa.FECHAFIN - hoy).Days
            };

            // Determinar descripción del estado
            if (tarifaDto.EstadoInfo.EstaVencida)
                tarifaDto.EstadoInfo.EstadoDescripcion = "Vencida";
            else if (tarifaDto.EstadoInfo.EsPendiente)
                tarifaDto.EstadoInfo.EstadoDescripcion = "Pendiente";
            else if (tarifaDto.EstadoInfo.EstaActiva)
                tarifaDto.EstadoInfo.EstadoDescripcion = "Activa";
            else
                tarifaDto.EstadoInfo.EstadoDescripcion = "Inactiva";

            // Incluir conteo de precios si se solicita
            if (incluirConteoPrecios)
            {
                var preciosRepo = _unitOfWork.Repository<Precio>();
                var precios = await preciosRepo.FindAsync(p => p.IDTARIFAV == tarifa.IDTARIFAV);
                tarifaDto.TotalPrecios = precios.Count();
            }

            return tarifaDto;
        }

        private async Task<(bool IsValid, List<string> Errors)> ValidateTarifaBusinessRules(TarifaDto tarifa)
        {
            var errors = new List<string>();

            // Validar fechas
            if (tarifa.FECHAFIN <= tarifa.FECHAINI)
            {
                errors.Add("La fecha de fin debe ser posterior a la fecha de inicio");
            }

            if (tarifa.FECHAINI < DateTime.Today.AddYears(-10))
            {
                errors.Add("La fecha de inicio no puede ser anterior a 10 años");
            }

            if (tarifa.FECHAFIN > DateTime.Today.AddYears(10))
            {
                errors.Add("La fecha de fin no puede ser posterior a 10 años");
            }

            // Validar solapamiento (opcional - se puede desactivar si se permiten)
            var solapamientoResult = await ValidarSolapamientoAsync(tarifa.IDTARIFAV, tarifa.FECHAINI, tarifa.FECHAFIN);
            if (solapamientoResult.Success && !solapamientoResult.Data)
            {
                // Solo advertencia, no error crítico
                _logger.LogWarning("Tarifa {Id} tiene solapamiento de fechas", tarifa.IDTARIFAV);
            }

            return (!errors.Any(), errors);
        }

        private async Task UpdateExistingTarifa(Tarifa existente, TarifaDto tarifaDto)
        {
            existente.DESCRIPCION = tarifaDto.DESCRIPCION;
            existente.FECHAINI = tarifaDto.FECHAINI;
            existente.FECHAFIN = tarifaDto.FECHAFIN;
            existente.UpdatedAt = DateTime.UtcNow;
        }

        #endregion
    }
}
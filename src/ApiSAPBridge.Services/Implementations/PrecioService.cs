using ApiSAPBridge.Core.Interfaces;
using ApiSAPBridge.Data.UnitOfWork;
using ApiSAPBridge.Models;
using ApiSAPBridge.Models.DTOs;
using ApiSAPBridge.Models.Constants;
using Mapster;
using Microsoft.Extensions.Logging;

namespace ApiSAPBridge.Services.Implementations
{
    public class PrecioService : IPrecioService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PrecioService> _logger;

        public PrecioService(IUnitOfWork unitOfWork, ILogger<PrecioService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ResponseDto<List<PrecioResponseDto>>> CreatePreciosAsync(List<PrecioDto> precios)
        {
            try
            {
                _logger.LogInformation("Iniciando creación de {Count} precios", precios.Count);

                var resultados = new List<PrecioResponseDto>();
                var errores = new List<string>();

                await _unitOfWork.BeginTransactionAsync();

                foreach (var precioDto in precios)
                {
                    // Validaciones de negocio
                    var validationResult = await ValidatePrecioBusinessRules(precioDto);
                    if (!validationResult.IsValid)
                    {
                        errores.AddRange(validationResult.Errors);
                        _logger.LogWarning("Precio {Tarifa}-{Articulo}-{Talla}-{Color} falló validaciones: {Errors}",
                            precioDto.IDTARIFAV, precioDto.CODARTICULO, precioDto.TALLA, precioDto.COLOR,
                            string.Join(", ", validationResult.Errors));
                        continue;
                    }

                    var preciosRepo = _unitOfWork.Repository<Precio>();
                    var existente = (await preciosRepo.FindAsync(p =>
                        p.IDTARIFAV == precioDto.IDTARIFAV &&
                        p.CODARTICULO == precioDto.CODARTICULO &&
                        p.TALLA == precioDto.TALLA &&
                        p.COLOR == precioDto.COLOR)).FirstOrDefault();

                    if (existente != null)
                    {
                        // Actualizar existente
                        await UpdateExistingPrecio(existente, precioDto);
                        preciosRepo.Update(existente);

                        _logger.LogInformation("Precio {Tarifa}-{Articulo}-{Talla}-{Color} actualizado",
                            existente.IDTARIFAV, existente.CODARTICULO, existente.TALLA, existente.COLOR);

                        var responseExistente = await BuildPrecioResponseDto(existente);
                        resultados.Add(responseExistente);
                    }
                    else
                    {
                        // Crear nuevo
                        var nuevoPrecio = precioDto.Adapt<Precio>();
                        nuevoPrecio.CreatedAt = DateTime.UtcNow;
                        nuevoPrecio.UpdatedAt = DateTime.UtcNow;

                        await preciosRepo.AddAsync(nuevoPrecio);
                        _logger.LogInformation("Precio {Tarifa}-{Articulo}-{Talla}-{Color} creado",
                            nuevoPrecio.IDTARIFAV, nuevoPrecio.CODARTICULO, nuevoPrecio.TALLA, nuevoPrecio.COLOR);

                        var responseNuevo = await BuildPrecioResponseDto(nuevoPrecio);
                        resultados.Add(responseNuevo);
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                if (errores.Any() && !resultados.Any())
                {
                    return ResponseDto<List<PrecioResponseDto>>.CreateError(
                        "Todos los precios fallaron las validaciones", errores);
                }

                var message = errores.Any()
                    ? $"Procesados {resultados.Count} precios, {errores.Count} con errores"
                    : ApiConstants.ResponseMessages.SUCCESS_CREATE;

                return ResponseDto<List<PrecioResponseDto>>.CreateSuccess(resultados, message);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error al procesar precios");
                return ResponseDto<List<PrecioResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<List<PrecioResponseDto>>> CreatePreciosMasivosAsync(PrecioMasivoRequest request)
        {
            try
            {
                _logger.LogInformation("Iniciando creación masiva de {Count} precios", request.Precios.Count);

                var resultados = new List<PrecioResponseDto>();
                var errores = new List<string>();

                await _unitOfWork.BeginTransactionAsync();

                // Validaciones previas si se solicitan
                if (request.ValidarTarifasActivas)
                {
                    var idsUnicosTarifas = request.Precios.Select(p => p.IDTARIFAV).Distinct();
                    foreach (var idTarifa in idsUnicosTarifas)
                    {
                        var tarifa = await _unitOfWork.Repository<Tarifa>().GetByIdAsync(idTarifa);
                        if (tarifa == null)
                        {
                            errores.Add($"La tarifa {idTarifa} no existe");
                            continue;
                        }

                        var hoy = DateTime.Today;
                        if (tarifa.FECHAINI > hoy || tarifa.FECHAFIN < hoy)
                        {
                            _logger.LogWarning("Tarifa {IdTarifa} no está activa (vigencia: {Inicio} - {Fin})",
                                idTarifa, tarifa.FECHAINI, tarifa.FECHAFIN);
                        }
                    }
                }

                if (request.ValidarArticulosExistentes)
                {
                    var codigosUnicosArticulos = request.Precios.Select(p => p.CODARTICULO).Distinct();
                    foreach (var codigoArticulo in codigosUnicosArticulos)
                    {
                        var articulo = await _unitOfWork.Repository<Articulo>().GetByIdAsync(codigoArticulo);
                        if (articulo == null)
                        {
                            errores.Add($"El artículo {codigoArticulo} no existe");
                        }
                    }
                }

                if (request.ValidarLineasExistentes)
                {
                    foreach (var precio in request.Precios)
                    {
                        var lineasRepo = _unitOfWork.Repository<ArticuloLinea>();
                        var linea = (await lineasRepo.FindAsync(l =>
                            l.CODARTICULO == precio.CODARTICULO &&
                            l.TALLA == precio.TALLA &&
                            l.COLOR == precio.COLOR)).FirstOrDefault();

                        if (linea == null)
                        {
                            errores.Add($"La línea {precio.CODARTICULO}-{precio.TALLA}-{precio.COLOR} no existe");
                        }
                    }
                }

                // Si hay errores críticos, detener
                if (errores.Any() && !request.SobreescribirExistentes)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ResponseDto<List<PrecioResponseDto>>.CreateError(
                        "Errores en validaciones previas", errores);
                }

                // Procesar precios
                foreach (var precioDto in request.Precios)
                {
                    // Calcular precio neto automáticamente si se solicita
                    if (request.CalcularPrecioNetoAutomatico)
                    {
                        precioDto.PNETO = precioDto.PBRUTO * (1 - precioDto.DTO / 100);
                    }

                    var preciosRepo = _unitOfWork.Repository<Precio>();
                    var existente = (await preciosRepo.FindAsync(p =>
                        p.IDTARIFAV == precioDto.IDTARIFAV &&
                        p.CODARTICULO == precioDto.CODARTICULO &&
                        p.TALLA == precioDto.TALLA &&
                        p.COLOR == precioDto.COLOR)).FirstOrDefault();

                    if (existente != null && !request.SobreescribirExistentes)
                    {
                        errores.Add($"El precio {precioDto.IDTARIFAV}-{precioDto.CODARTICULO}-{precioDto.TALLA}-{precioDto.COLOR} ya existe");
                        continue;
                    }

                    if (existente != null)
                    {
                        await UpdateExistingPrecio(existente, precioDto);
                        preciosRepo.Update(existente);
                        resultados.Add(await BuildPrecioResponseDto(existente));
                    }
                    else
                    {
                        var nuevoPrecio = precioDto.Adapt<Precio>();
                        nuevoPrecio.CreatedAt = DateTime.UtcNow;
                        nuevoPrecio.UpdatedAt = DateTime.UtcNow;
                        await preciosRepo.AddAsync(nuevoPrecio);
                        resultados.Add(await BuildPrecioResponseDto(nuevoPrecio));
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                var message = $"Procesamiento masivo completado: {resultados.Count} precios, {errores.Count} errores";
                return ResponseDto<List<PrecioResponseDto>>.CreateSuccess(resultados, message);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error en procesamiento masivo de precios");
                return ResponseDto<List<PrecioResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<List<PrecioResponseDto>>> ActualizarPreciosPorTarifaAsync(
            int idTarifa, decimal porcentajeIncremento)
        {
            try
            {
                _logger.LogInformation("Actualizando precios de tarifa {IdTarifa} con incremento de {Porcentaje}%",
                    idTarifa, porcentajeIncremento);

                await _unitOfWork.BeginTransactionAsync();

                var preciosRepo = _unitOfWork.Repository<Precio>();
                var precios = await preciosRepo.FindAsync(p => p.IDTARIFAV == idTarifa);

                if (!precios.Any())
                {
                    return ResponseDto<List<PrecioResponseDto>>.CreateError(
                        $"No se encontraron precios para la tarifa {idTarifa}");
                }

                var resultados = new List<PrecioResponseDto>();
                var factor = 1 + (porcentajeIncremento / 100);

                foreach (var precio in precios)
                {
                    var precioOriginal = precio.PBRUTO;
                    precio.PBRUTO = Math.Round(precio.PBRUTO * factor, 2);
                    precio.PNETO = Math.Round(precio.PBRUTO * (1 - precio.DTO / 100), 2);
                    precio.UpdatedAt = DateTime.UtcNow;

                    preciosRepo.Update(precio);

                    _logger.LogDebug("Precio {Tarifa}-{Articulo}-{Talla}-{Color}: {PrecioOriginal} -> {PrecioNuevo}",
                        precio.IDTARIFAV, precio.CODARTICULO, precio.TALLA, precio.COLOR,
                        precioOriginal, precio.PBRUTO);

                    resultados.Add(await BuildPrecioResponseDto(precio));
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return ResponseDto<List<PrecioResponseDto>>.CreateSuccess(
                    resultados,
                    $"Actualizados {resultados.Count} precios con incremento de {porcentajeIncremento}%");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error al actualizar precios por tarifa {IdTarifa}", idTarifa);
                return ResponseDto<List<PrecioResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<object>> GetAnalisisPreciosAsync(int? idTarifa = null, int? codigoArticulo = null)
        {
            try
            {
                var preciosRepo = _unitOfWork.Repository<Precio>();
                var precios = await preciosRepo.GetAllAsync();

                // Aplicar filtros si se especifican
                if (idTarifa.HasValue)
                {
                    precios = precios.Where(p => p.IDTARIFAV == idTarifa.Value);
                }

                if (codigoArticulo.HasValue)
                {
                    precios = precios.Where(p => p.CODARTICULO == codigoArticulo.Value);
                }

                var preciosList = precios.ToList();

                var analisis = new
                {
                    // Filtros aplicados
                    Filtros = new { IdTarifa = idTarifa, CodigoArticulo = codigoArticulo },

                    // Conteos básicos
                    TotalPrecios = preciosList.Count,
                    PreciosConDescuento = preciosList.Count(p => p.DTO > 0),
                    PreciosSinDescuento = preciosList.Count(p => p.DTO == 0),

                    // Análisis de precios
                    PrecioNetoMinimo = preciosList.Any() ? preciosList.Min(p => p.PNETO) : 0,
                    PrecioNetoMaximo = preciosList.Any() ? preciosList.Max(p => p.PNETO) : 0,
                    PrecioNetoPromedio = preciosList.Any() ? Math.Round(preciosList.Average(p => p.PNETO), 2) : 0,

                    PrecioBrutoMinimo = preciosList.Any() ? preciosList.Min(p => p.PBRUTO) : 0,
                    PrecioBrutoMaximo = preciosList.Any() ? preciosList.Max(p => p.PBRUTO) : 0,
                    PrecioBrutoPromedio = preciosList.Any() ? Math.Round(preciosList.Average(p => p.PBRUTO), 2) : 0,

                    // Análisis de descuentos
                    DescuentoMinimo = preciosList.Any() ? preciosList.Min(p => p.DTO) : 0,
                    DescuentoMaximo = preciosList.Any() ? preciosList.Max(p => p.DTO) : 0,
                    DescuentoPromedio = preciosList.Any() ? Math.Round(preciosList.Average(p => p.DTO), 2) : 0,

                    // Distribución de descuentos
                    DistribucionDescuentos = preciosList
                        .GroupBy(p => Math.Floor(p.DTO / 5) * 5) // Agrupa en rangos de 5%
                        .Select(g => new {
                            RangoDescuento = $"{g.Key}%-{g.Key + 5}%",
                            Cantidad = g.Count()
                        })
                        .OrderBy(x => x.RangoDescuento)
                        .ToList(),

                    // Distribución de precios
                    DistribucionPrecios = preciosList
                        .GroupBy(p => Math.Floor(p.PNETO / 10) * 10) // Agrupa en rangos de 10
                        .Select(g => new {
                            RangoPrecio = $"{g.Key}-{g.Key + 10}",
                            Cantidad = g.Count()
                        })
                        .OrderBy(x => x.RangoPrecio)
                        .Take(20) // Top 20 rangos
                        .ToList(),

                    // Artículos con más variaciones de precio
                    ArticulosConMasPrecios = idTarifa.HasValue ? null : preciosList
                        .GroupBy(p => p.CODARTICULO)
                        .Select(g => new {
                            CodigoArticulo = g.Key,
                            NumPrecios = g.Count(),
                            PrecioMinimo = g.Min(p => p.PNETO),
                            PrecioMaximo = g.Max(p => p.PNETO),
                            Diferencia = g.Max(p => p.PNETO) - g.Min(p => p.PNETO)
                        })
                        .OrderByDescending(x => x.NumPrecios)
                        .Take(10)
                        .ToList(),

                    Timestamp = DateTime.UtcNow
                };

                return ResponseDto<object>.CreateSuccess(analisis, "Análisis de precios generado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar análisis de precios");
                return ResponseDto<object>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<object>> GetComparativaTarifasAsync(int codigoArticulo)
        {
            try
            {
                var precios = await _unitOfWork.Repository<Precio>()
                    .FindAsync(p => p.CODARTICULO == codigoArticulo);

                if (!precios.Any())
                {
                    return ResponseDto<object>.CreateError(
                        $"No se encontraron precios para el artículo {codigoArticulo}");
                }

                var tarifasRepo = _unitOfWork.Repository<Tarifa>();
                var articuloRepo = _unitOfWork.Repository<Articulo>();

                var articulo = await articuloRepo.GetByIdAsync(codigoArticulo);
                var preciosPorTarifa = new List<object>();

                foreach (var grupo in precios.GroupBy(p => p.IDTARIFAV))
                {
                    var tarifa = await tarifasRepo.GetByIdAsync(grupo.Key);
                    var preciosTarifa = grupo.ToList();

                    preciosPorTarifa.Add(new
                    {
                        IdTarifa = grupo.Key,
                        NombreTarifa = tarifa?.DESCRIPCION ?? "Tarifa desconocida",
                        FechaInicio = tarifa?.FECHAINI,
                        FechaFin = tarifa?.FECHAFIN,
                        TarifaActiva = tarifa != null && tarifa.FECHAINI <= DateTime.Today && tarifa.FECHAFIN >= DateTime.Today,
                        NumLineasConPrecio = preciosTarifa.Count,
                        PrecioMinimo = preciosTarifa.Min(p => p.PNETO),
                        PrecioMaximo = preciosTarifa.Max(p => p.PNETO),
                        PrecioPromedio = Math.Round(preciosTarifa.Average(p => p.PNETO), 2),
                        DescuentoPromedio = Math.Round(preciosTarifa.Average(p => p.DTO), 2),
                        Lineas = preciosTarifa.Select(p => new
                        {
                            Talla = p.TALLA,
                            Color = p.COLOR,
                            PrecioBruto = p.PBRUTO,
                            Descuento = p.DTO,
                            PrecioNeto = p.PNETO
                        }).ToList()
                    });
                }

                var comparativa = new
                {
                    CodigoArticulo = codigoArticulo,
                    NombreArticulo = articulo?.DESCRIPCION ?? "Artículo desconocido",
                    TotalTarifas = preciosPorTarifa.Count,
                    TotalLineasConPrecio = precios.Count(),

                    ResumenGeneral = new
                    {
                        PrecioMinimoGlobal = precios.Min(p => p.PNETO),
                        PrecioMaximoGlobal = precios.Max(p => p.PNETO),
                        DiferenciaMaxima = precios.Max(p => p.PNETO) - precios.Min(p => p.PNETO),
                        PorcentajeDiferencia = precios.Min(p => p.PNETO) > 0
                            ? Math.Round(((precios.Max(p => p.PNETO) - precios.Min(p => p.PNETO)) / precios.Min(p => p.PNETO)) * 100, 2)
                            : 0
                    },

                    DetallesPorTarifa = preciosPorTarifa,

                    Timestamp = DateTime.UtcNow
                };

                return ResponseDto<object>.CreateSuccess(comparativa,
                    $"Comparativa de tarifas generada para artículo {codigoArticulo}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar comparativa de tarifas para artículo {Articulo}", codigoArticulo);
                return ResponseDto<object>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        // Implementar métodos restantes...
        public async Task<ResponseDto<List<PrecioResponseDto>>> GetPreciosAsync(bool incluirTarifa = false, bool incluirArticulo = false)
        {
            try
            {
                var precios = await _unitOfWork.Repository<Precio>().GetAllAsync();
                var response = new List<PrecioResponseDto>();

                foreach (var precio in precios)
                {
                    var precioDto = await BuildPrecioResponseDto(precio, incluirTarifa, incluirArticulo);
                    response.Add(precioDto);
                }

                return ResponseDto<List<PrecioResponseDto>>.CreateSuccess(
                    response, ApiConstants.ResponseMessages.SUCCESS_RETRIEVE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener precios");
                return ResponseDto<List<PrecioResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }
        public async Task<ResponseDto<PrecioResponseDto?>> GetPrecioAsync(
    int idTarifa, int codigoArticulo, string talla, string color)
        {
            try
            {
                var preciosRepo = _unitOfWork.Repository<Precio>();
                var precio = (await preciosRepo.FindAsync(p =>
                    p.IDTARIFAV == idTarifa &&
                    p.CODARTICULO == codigoArticulo &&
                    p.TALLA == talla &&
                    p.COLOR == color)).FirstOrDefault();

                if (precio == null)
                {
                    _logger.LogWarning("Precio {Tarifa}-{Articulo}-{Talla}-{Color} no encontrado",
                        idTarifa, codigoArticulo, talla, color);
                    return ResponseDto<PrecioResponseDto?>.CreateError(
                        ApiConstants.ResponseMessages.ERROR_NOT_FOUND);
                }

                var response = await BuildPrecioResponseDto(precio, incluirTarifa: true, incluirArticulo: true);

                return ResponseDto<PrecioResponseDto?>.CreateSuccess(
                    response, ApiConstants.ResponseMessages.SUCCESS_RETRIEVE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener precio {Tarifa}-{Articulo}-{Talla}-{Color}",
                    idTarifa, codigoArticulo, talla, color);
                return ResponseDto<PrecioResponseDto?>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<List<PrecioResponseDto>>> GetPreciosByTarifaAsync(
            int idTarifa, bool incluirArticulo = false)
        {
            try
            {
                var precios = await _unitOfWork.Repository<Precio>()
                    .FindAsync(p => p.IDTARIFAV == idTarifa);

                var response = new List<PrecioResponseDto>();
                foreach (var precio in precios)
                {
                    var precioDto = await BuildPrecioResponseDto(precio, incluirTarifa: false, incluirArticulo);
                    response.Add(precioDto);
                }

                return ResponseDto<List<PrecioResponseDto>>.CreateSuccess(
                    response, $"Encontrados {response.Count} precios para tarifa {idTarifa}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener precios de tarifa {IdTarifa}", idTarifa);
                return ResponseDto<List<PrecioResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<List<PrecioResponseDto>>> GetPreciosByArticuloAsync(
            int codigoArticulo, bool incluirTarifa = false)
        {
            try
            {
                var precios = await _unitOfWork.Repository<Precio>()
                    .FindAsync(p => p.CODARTICULO == codigoArticulo);

                var response = new List<PrecioResponseDto>();
                foreach (var precio in precios)
                {
                    var precioDto = await BuildPrecioResponseDto(precio, incluirTarifa, incluirArticulo: false);
                    response.Add(precioDto);
                }

                return ResponseDto<List<PrecioResponseDto>>.CreateSuccess(
                    response, $"Encontrados {response.Count} precios para artículo {codigoArticulo}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener precios de artículo {CodigoArticulo}", codigoArticulo);
                return ResponseDto<List<PrecioResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<List<PrecioResponseDto>>> SearchPreciosAsync(PrecioSearchRequest request)
        {
            try
            {
                _logger.LogInformation("Iniciando búsqueda de precios con filtros");

                var preciosRepo = _unitOfWork.Repository<Precio>();
                var todosLosPrecios = await preciosRepo.GetAllAsync();

                var preciosFiltrados = todosLosPrecios.AsQueryable();

                // Aplicar filtros
                if (request.IdTarifa.HasValue)
                {
                    preciosFiltrados = preciosFiltrados.Where(p => p.IDTARIFAV == request.IdTarifa.Value);
                }

                if (request.CodigoArticulo.HasValue)
                {
                    preciosFiltrados = preciosFiltrados.Where(p => p.CODARTICULO == request.CodigoArticulo.Value);
                }

                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var searchTerm = request.SearchTerm.ToLower();
                    preciosFiltrados = preciosFiltrados.Where(p =>
                        p.CODARTICULO.ToString().Contains(searchTerm) ||
                        p.IDTARIFAV.ToString().Contains(searchTerm) ||
                        p.TALLA.ToLower().Contains(searchTerm) ||
                        p.COLOR.ToLower().Contains(searchTerm) ||
                        (p.CODBARRAS != null && p.CODBARRAS.ToLower().Contains(searchTerm)));
                }

                if (request.PrecioMinimo.HasValue)
                {
                    preciosFiltrados = preciosFiltrados.Where(p => p.PNETO >= request.PrecioMinimo.Value);
                }

                if (request.PrecioMaximo.HasValue)
                {
                    preciosFiltrados = preciosFiltrados.Where(p => p.PNETO <= request.PrecioMaximo.Value);
                }

                if (request.DescuentoMinimo.HasValue)
                {
                    preciosFiltrados = preciosFiltrados.Where(p => p.DTO >= request.DescuentoMinimo.Value);
                }

                if (request.DescuentoMaximo.HasValue)
                {
                    preciosFiltrados = preciosFiltrados.Where(p => p.DTO <= request.DescuentoMaximo.Value);
                }

                if (request.SoloTarifasActivas)
                {
                    var hoy = DateTime.Today;
                    var tarifasActivas = await _unitOfWork.Repository<Tarifa>()
                        .FindAsync(t => t.FECHAINI <= hoy && t.FECHAFIN >= hoy);
                    var idsActivas = tarifasActivas.Select(t => t.IDTARIFAV).ToList();
                    preciosFiltrados = preciosFiltrados.Where(p => idsActivas.Contains(p.IDTARIFAV));
                }

                if (request.SoloConDescuento)
                {
                    preciosFiltrados = preciosFiltrados.Where(p => p.DTO > 0);
                }

                // Contar total antes de paginación
                var totalCount = preciosFiltrados.Count();

                // Aplicar paginación
                var preciosPaginados = preciosFiltrados
                    .OrderBy(p => p.IDTARIFAV)
                    .ThenBy(p => p.CODARTICULO)
                    .ThenBy(p => p.TALLA)
                    .ThenBy(p => p.COLOR)
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                var response = new List<PrecioResponseDto>();
                foreach (var precio in preciosPaginados)
                {
                    var precioDto = await BuildPrecioResponseDto(precio, request.IncluirTarifa, request.IncluirArticulo);
                    response.Add(precioDto);
                }

                var message = $"Búsqueda completada: {totalCount} precios encontrados, mostrando página {request.Page} de {Math.Ceiling((double)totalCount / request.PageSize)}";

                _logger.LogInformation("Búsqueda de precios completada: {Total} encontrados, {Returned} devueltos",
                    totalCount, response.Count);

                return ResponseDto<List<PrecioResponseDto>>.CreateSuccess(response, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en búsqueda de precios");
                return ResponseDto<List<PrecioResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<object>> GetEstadisticasPreciosAsync()
        {
            try
            {
                _logger.LogInformation("Generando estadísticas generales de precios");

                var precios = await _unitOfWork.Repository<Precio>().GetAllAsync();
                var tarifas = await _unitOfWork.Repository<Tarifa>().GetAllAsync();
                var articulos = await _unitOfWork.Repository<Articulo>().GetAllAsync();

                var hoy = DateTime.Today;
                var tarifasActivas = tarifas.Where(t => t.FECHAINI <= hoy && t.FECHAFIN >= hoy).ToList();
                var preciosEnTarifasActivas = precios.Where(p => tarifasActivas.Any(t => t.IDTARIFAV == p.IDTARIFAV)).ToList();

                var stats = new
                {
                    // Conteos generales
                    TotalPrecios = precios.Count(),
                    TotalTarifas = tarifas.Count(),
                    TotalArticulosConPrecios = precios.GroupBy(p => p.CODARTICULO).Count(),
                    TarifasActivas = tarifasActivas.Count,
                    PreciosEnTarifasActivas = preciosEnTarifasActivas.Count,

                    // Análisis de precios
                    PrecioNettoMinimo = precios.Any() ? precios.Min(p => p.PNETO) : 0,
                    PrecioNettoMaximo = precios.Any() ? precios.Max(p => p.PNETO) : 0,
                    PrecioNettoPromedio = precios.Any() ? Math.Round(precios.Average(p => p.PNETO), 2) : 0,
                    PrecioNettoMediano = precios.Any() ? CalcularMediana(precios.Select(p => p.PNETO).ToList()) : 0,

                    PrecioBrutoMinimo = precios.Any() ? precios.Min(p => p.PBRUTO) : 0,
                    PrecioBrutoMaximo = precios.Any() ? precios.Max(p => p.PBRUTO) : 0,
                    PrecioBrutoPromedio = precios.Any() ? Math.Round(precios.Average(p => p.PBRUTO), 2) : 0,

                    // Análisis de descuentos
                    PreciosConDescuento = precios.Count(p => p.DTO > 0),
                    PorcentajePreciosConDescuento = precios.Count() > 0 ? Math.Round((double)precios.Count(p => p.DTO > 0) / precios.Count() * 100, 2) : 0,
                    DescuentoPromedio = precios.Where(p => p.DTO > 0).Any() ? Math.Round(precios.Where(p => p.DTO > 0).Average(p => p.DTO), 2) : 0,
                    DescuentoMaximo = precios.Any() ? precios.Max(p => p.DTO) : 0,

                    // Distribución por rangos de precio
                    DistribucionPrecios = precios
                        .GroupBy(p => GetRangoPrecio(p.PNETO))
                        .Select(g => new { Rango = g.Key, Cantidad = g.Count() })
                        .OrderBy(x => x.Rango)
                        .ToList(),

                    // Distribución por rangos de descuento
                    DistribucionDescuentos = precios
                        .Where(p => p.DTO > 0)
                        .GroupBy(p => GetRangoDescuento(p.DTO))
                        .Select(g => new { Rango = g.Key, Cantidad = g.Count() })
                        .OrderBy(x => x.Rango)
                        .ToList(),

                    // Top artículos con más precios
                    ArticulosConMasPrecios = precios
                        .GroupBy(p => p.CODARTICULO)
                        .Select(g => new {
                            CodigoArticulo = g.Key,
                            NumPrecios = g.Count(),
                            PrecioMinimo = g.Min(p => p.PNETO),
                            PrecioMaximo = g.Max(p => p.PNETO),
                            DiferenciaPrecio = g.Max(p => p.PNETO) - g.Min(p => p.PNETO)
                        })
                        .OrderByDescending(x => x.NumPrecios)
                        .Take(10)
                        .ToList(),

                    // Top tarifas con más precios
                    TarifasConMasPrecios = precios
                        .GroupBy(p => p.IDTARIFAV)
                        .Select(g => new {
                            IdTarifa = g.Key,
                            NumPrecios = g.Count(),
                            PrecioPromedio = Math.Round(g.Average(p => p.PNETO), 2)
                        })
                        .OrderByDescending(x => x.NumPrecios)
                        .Take(10)
                        .ToList(),

                    // Análisis de variantes más comunes
                    TallasMasComunes = precios
                        .GroupBy(p => p.TALLA)
                        .Select(g => new { Talla = g.Key, Count = g.Count() })
                        .OrderByDescending(x => x.Count)
                        .Take(10)
                        .ToList(),

                    ColoresMasComunes = precios
                        .GroupBy(p => p.COLOR)
                        .Select(g => new { Color = g.Key, Count = g.Count() })
                        .OrderByDescending(x => x.Count)
                        .Take(10)
                        .ToList(),

                    // Análisis temporal
                    PreciosPorAnoTarifa = precios
                        .Join(tarifas, p => p.IDTARIFAV, t => t.IDTARIFAV, (p, t) => new { p, t })
                        .GroupBy(x => x.t.FECHAINI.Year)
                        .Select(g => new { Ano = g.Key, NumPrecios = g.Count() })
                        .OrderBy(x => x.Ano)
                        .ToList(),

                    Timestamp = DateTime.UtcNow
                };

                return ResponseDto<object>.CreateSuccess(stats, "Estadísticas de precios generadas exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar estadísticas de precios");
                return ResponseDto<object>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<List<string>>> ValidarPreciosTarifaAsync(int idTarifa)
        {
            try
            {
                _logger.LogInformation("Validando precios de tarifa {IdTarifa}", idTarifa);

                var errores = new List<string>();

                // Verificar que la tarifa existe
                var tarifaRepo = _unitOfWork.Repository<Tarifa>();
                var tarifaObj = await tarifaRepo.GetByIdAsync(idTarifa);
                if (tarifaObj == null)
                {
                    errores.Add($"La tarifa {idTarifa} no existe");
                    return ResponseDto<List<string>>.CreateError(
                        "Tarifa no encontrada", errores);
                }

                // Obtener precios de la tarifa
                var precios = await _unitOfWork.Repository<Precio>()
                    .FindAsync(p => p.IDTARIFAV == idTarifa);

                if (!precios.Any())
                {
                    errores.Add($"La tarifa {idTarifa} no tiene precios definidos");
                    return ResponseDto<List<string>>.CreateSuccess(errores,
                        $"Validación completada: {errores.Count} advertencias encontradas");
                }

                // Validar cada precio
                foreach (var precio in precios)
                {
                    // Validar coherencia matemática
                    var precioNetoCalculado = precio.PBRUTO * (1 - precio.DTO / 100);
                    if (Math.Abs(precio.PNETO - precioNetoCalculado) > 0.01m)
                    {
                        errores.Add($"Precio {precio.CODARTICULO}-{precio.TALLA}-{precio.COLOR}: precio neto inconsistente ({precio.PNETO} vs {precioNetoCalculado:F2} calculado)");
                    }

                    // Validar que el artículo existe
                    var articulo = await _unitOfWork.Repository<Articulo>().GetByIdAsync(precio.CODARTICULO);
                    if (articulo == null)
                    {
                        errores.Add($"Precio {precio.CODARTICULO}-{precio.TALLA}-{precio.COLOR}: artículo {precio.CODARTICULO} no existe");
                    }

                    // Validar que la línea del artículo existe
                    var lineasRepo = _unitOfWork.Repository<ArticuloLinea>();
                    var linea = (await lineasRepo.FindAsync(l =>
                        l.CODARTICULO == precio.CODARTICULO &&
                        l.TALLA == precio.TALLA &&
                        l.COLOR == precio.COLOR)).FirstOrDefault();

                    if (linea == null)
                    {
                        errores.Add($"Precio {precio.CODARTICULO}-{precio.TALLA}-{precio.COLOR}: línea no existe en el artículo");
                    }

                    // Validar rangos de precios razonables
                    if (precio.PBRUTO <= 0)
                    {
                        errores.Add($"Precio {precio.CODARTICULO}-{precio.TALLA}-{precio.COLOR}: precio bruto debe ser mayor a 0");
                    }

                    if (precio.PNETO <= 0)
                    {
                        errores.Add($"Precio {precio.CODARTICULO}-{precio.TALLA}-{precio.COLOR}: precio neto debe ser mayor a 0");
                    }

                    if (precio.DTO < 0 || precio.DTO > 100)
                    {
                        errores.Add($"Precio {precio.CODARTICULO}-{precio.TALLA}-{precio.COLOR}: descuento debe estar entre 0 y 100");
                    }

                    // Advertencia para descuentos muy altos
                    if (precio.DTO > 80)
                    {
                        errores.Add($"Precio {precio.CODARTICULO}-{precio.TALLA}-{precio.COLOR}: descuento muy alto ({precio.DTO}%)");
                    }
                }

                var message = errores.Any()
                    ? $"Validación completada: {errores.Count} errores/advertencias encontrados en {precios.Count()} precios"
                    : $"Validación exitosa: todos los {precios.Count()} precios son correctos";

                return ResponseDto<List<string>>.CreateSuccess(errores, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar precios de tarifa {IdTarifa}", idTarifa);
                return ResponseDto<List<string>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        #region Métodos Auxiliares Adicionales

        private decimal CalcularMediana(List<decimal> valores)
        {
            if (!valores.Any()) return 0;

            var ordenados = valores.OrderBy(x => x).ToList();
            int n = ordenados.Count;

            if (n % 2 == 0)
            {
                return (ordenados[n / 2 - 1] + ordenados[n / 2]) / 2;
            }
            else
            {
                return ordenados[n / 2];
            }
        }

        private string GetRangoPrecio(decimal precio)
        {
            if (precio < 10) return "0-9.99";
            if (precio < 25) return "10-24.99";
            if (precio < 50) return "25-49.99";
            if (precio < 100) return "50-99.99";
            if (precio < 250) return "100-249.99";
            if (precio < 500) return "250-499.99";
            if (precio < 1000) return "500-999.99";
            return "1000+";
        }

        private string GetRangoDescuento(decimal descuento)
        {
            if (descuento < 5) return "0-4.99%";
            if (descuento < 10) return "5-9.99%";
            if (descuento < 20) return "10-19.99%";
            if (descuento < 30) return "20-29.99%";
            if (descuento < 50) return "30-49.99%";
            return "50%+";
        }

        #endregion
        #region Métodos Auxiliares

        private async Task<PrecioResponseDto> BuildPrecioResponseDto(
            Precio precio, bool incluirTarifa = false, bool incluirArticulo = false)
        {
            var precioDto = precio.Adapt<PrecioResponseDto>();

            // Cargar tarifa si se solicita
            if (incluirTarifa)
            {
                var tarifaDetalle = await _unitOfWork.Repository<Tarifa>().GetByIdAsync(precio.IDTARIFAV);
                precioDto.Tarifa = tarifaDetalle?.Adapt<TarifaResponseDto>();
            }

            // Cargar artículo si se solicita
            if (incluirArticulo)
            {
                var articulo = await _unitOfWork.Repository<Articulo>().GetByIdAsync(precio.CODARTICULO);
                precioDto.Articulo = articulo?.Adapt<ArticuloResponseDto>();
            }

            // Calcular estadísticas del precio
            var tarifa = await _unitOfWork.Repository<Tarifa>().GetByIdAsync(precio.IDTARIFAV);
            var hoy = DateTime.Today;

            precioDto.Estadisticas = new PrecioEstadisticas
            {
                AhorroAbsoluto = precio.PBRUTO - precio.PNETO,
                PorcentajeAhorro = precio.DTO,
                TieneDescuento = precio.DTO > 0,
                EsPrecioEspecial = precio.DTO > 20, // Más del 20% de descuento se considera especial
                TarifaVigente = tarifa?.DESCRIPCION ?? "Desconocida",
                TarifaActiva = tarifa != null && tarifa.FECHAINI <= hoy && tarifa.FECHAFIN >= hoy
            };

            // Calcular precio con impuesto si tenemos información del artículo
            if (incluirArticulo && precioDto.Articulo?.Impuesto != null)
            {
                var ivaDecimal = precioDto.Articulo.Impuesto.IVA / 100;
                precioDto.Estadisticas.PrecioConImpuesto = Math.Round(precio.PNETO * (1 + ivaDecimal), 2);
            }

            return precioDto;
        }

        private async Task<(bool IsValid, List<string> Errors)> ValidatePrecioBusinessRules(PrecioDto precio)
        {
            var errors = new List<string>();

            // Validar que la tarifa existe
            var tarifa = await _unitOfWork.Repository<Tarifa>().GetByIdAsync(precio.IDTARIFAV);
            if (tarifa == null)
            {
                errors.Add($"La tarifa {precio.IDTARIFAV} no existe");
            }

            // Validar que el artículo existe
            var articulo = await _unitOfWork.Repository<Articulo>().GetByIdAsync(precio.CODARTICULO);
            if (articulo == null)
            {
                errors.Add($"El artículo {precio.CODARTICULO} no existe");
            }

            // Validar que la línea del artículo existe
            var lineasRepo = _unitOfWork.Repository<ArticuloLinea>();
            var linea = (await lineasRepo.FindAsync(l =>
                l.CODARTICULO == precio.CODARTICULO &&
                l.TALLA == precio.TALLA &&
                l.COLOR == precio.COLOR)).FirstOrDefault();

            if (linea == null)
            {
                errors.Add($"La línea {precio.CODARTICULO}-{precio.TALLA}-{precio.COLOR} no existe");
            }

            // Validar coherencia del precio neto
            var precioNetoCalculado = precio.PBRUTO * (1 - precio.DTO / 100);
            if (Math.Abs(precio.PNETO - precioNetoCalculado) > 0.01m)
            {
                errors.Add($"El precio neto ({precio.PNETO}) no coincide con el calculado ({precioNetoCalculado:F2})");
            }

            return (!errors.Any(), errors);
        }

        private async Task UpdateExistingPrecio(Precio existente, PrecioDto precioDto)
        {
            existente.CODBARRAS = precioDto.CODBARRAS;
            existente.PBRUTO = precioDto.PBRUTO;
            existente.DTO = precioDto.DTO;
            existente.PNETO = precioDto.PNETO;
            existente.UpdatedAt = DateTime.UtcNow;
        }

        // Implementar métodos restantes de la interfaz...
        public async Task<ResponseDto<bool>> ValidarConsistenciaPrecioAsync(PrecioDto precio)
        {
            var validation = await ValidatePrecioBusinessRules(precio);
            return ResponseDto<bool>.CreateSuccess(validation.IsValid,
                validation.IsValid ? "Precio válido" : string.Join(", ", validation.Errors));
        }

        // ... otros métodos de la interfaz
        #endregion
    }
}
using ApiSAPBridge.Core.Interfaces;
using ApiSAPBridge.Data.UnitOfWork;
using ApiSAPBridge.Models;
using ApiSAPBridge.Models.DTOs;
using ApiSAPBridge.Models.Constants;
using Mapster;
using Microsoft.Extensions.Logging;

namespace ApiSAPBridge.Services.Implementations
{
    public class ArticuloLineaService : IArticuloLineaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ArticuloLineaService> _logger;

        public ArticuloLineaService(IUnitOfWork unitOfWork, ILogger<ArticuloLineaService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ResponseDto<List<ArticuloLineaResponseDto>>> CreateArticuloLineasAsync(
            List<ArticuloLineaDto> lineas)
        {
            try
            {
                _logger.LogInformation("Iniciando creación de {Count} líneas de artículos", lineas.Count);

                var resultados = new List<ArticuloLineaResponseDto>();
                var errores = new List<string>();

                await _unitOfWork.BeginTransactionAsync();

                foreach (var lineaDto in lineas)
                {
                    // Validaciones de negocio
                    var validationResult = await ValidateArticuloLineaBusinessRules(lineaDto);
                    if (!validationResult.IsValid)
                    {
                        errores.AddRange(validationResult.Errors);
                        _logger.LogWarning("Línea {Articulo}-{Talla}-{Color} falló validaciones: {Errors}",
                            lineaDto.CODARTICULO, lineaDto.TALLA, lineaDto.COLOR, string.Join(", ", validationResult.Errors));
                        continue;
                    }

                    var lineasRepo = _unitOfWork.Repository<ArticuloLinea>();
                    var existente = (await lineasRepo.FindAsync(l =>
                        l.CODARTICULO == lineaDto.CODARTICULO &&
                        l.TALLA == lineaDto.TALLA &&
                        l.COLOR == lineaDto.COLOR)).FirstOrDefault();

                    if (existente != null)
                    {
                        // Actualizar existente
                        await UpdateExistingArticuloLinea(existente, lineaDto);
                        lineasRepo.Update(existente);

                        _logger.LogInformation("Línea {Articulo}-{Talla}-{Color} actualizada",
                            existente.CODARTICULO, existente.TALLA, existente.COLOR);

                        var responseExistente = await BuildArticuloLineaResponseDto(existente);
                        resultados.Add(responseExistente);
                    }
                    else
                    {
                        // Crear nueva
                        var nuevaLinea = lineaDto.Adapt<ArticuloLinea>();
                        nuevaLinea.CreatedAt = DateTime.UtcNow;
                        nuevaLinea.UpdatedAt = DateTime.UtcNow;

                        await lineasRepo.AddAsync(nuevaLinea);
                        _logger.LogInformation("Línea {Articulo}-{Talla}-{Color} creada",
                            nuevaLinea.CODARTICULO, nuevaLinea.TALLA, nuevaLinea.COLOR);

                        var responseNueva = await BuildArticuloLineaResponseDto(nuevaLinea);
                        resultados.Add(responseNueva);
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                if (errores.Any() && !resultados.Any())
                {
                    return ResponseDto<List<ArticuloLineaResponseDto>>.CreateError(
                        "Todas las líneas fallaron las validaciones", errores);
                }

                var message = errores.Any()
                    ? $"Procesadas {resultados.Count} líneas, {errores.Count} con errores"
                    : ApiConstants.ResponseMessages.SUCCESS_CREATE;

                return ResponseDto<List<ArticuloLineaResponseDto>>.CreateSuccess(resultados, message);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error al procesar líneas de artículos");
                return ResponseDto<List<ArticuloLineaResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<List<ArticuloLineaResponseDto>>> GetArticuloLineasAsync(bool incluirPrecios = false)
        {
            try
            {
                var lineas = await _unitOfWork.Repository<ArticuloLinea>().GetAllAsync();
                var response = new List<ArticuloLineaResponseDto>();

                foreach (var linea in lineas)
                {
                    var lineaDto = await BuildArticuloLineaResponseDto(linea, incluirPrecios);
                    response.Add(lineaDto);
                }

                _logger.LogInformation("Obtenidas {Count} líneas de artículos", response.Count);

                return ResponseDto<List<ArticuloLineaResponseDto>>.CreateSuccess(
                    response, ApiConstants.ResponseMessages.SUCCESS_RETRIEVE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener líneas de artículos");
                return ResponseDto<List<ArticuloLineaResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<ArticuloLineaResponseDto?>> GetArticuloLineaAsync(
            int codigoArticulo, string talla, string color)
        {
            try
            {
                var lineasRepo = _unitOfWork.Repository<ArticuloLinea>();
                var linea = (await lineasRepo.FindAsync(l =>
                    l.CODARTICULO == codigoArticulo &&
                    l.TALLA == talla &&
                    l.COLOR == color)).FirstOrDefault();

                if (linea == null)
                {
                    _logger.LogWarning("Línea {Articulo}-{Talla}-{Color} no encontrada",
                        codigoArticulo, talla, color);
                    return ResponseDto<ArticuloLineaResponseDto?>.CreateError(
                        ApiConstants.ResponseMessages.ERROR_NOT_FOUND);
                }

                var response = await BuildArticuloLineaResponseDto(linea, incluirPrecios: true);

                return ResponseDto<ArticuloLineaResponseDto?>.CreateSuccess(
                    response, ApiConstants.ResponseMessages.SUCCESS_RETRIEVE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener línea {Articulo}-{Talla}-{Color}",
                    codigoArticulo, talla, color);
                return ResponseDto<ArticuloLineaResponseDto?>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<List<ArticuloLineaResponseDto>>> GetLineasByArticuloAsync(
            int codigoArticulo, bool incluirPrecios = false)
        {
            try
            {
                var lineas = await _unitOfWork.Repository<ArticuloLinea>()
                    .FindAsync(l => l.CODARTICULO == codigoArticulo);

                var response = new List<ArticuloLineaResponseDto>();
                foreach (var linea in lineas)
                {
                    var lineaDto = await BuildArticuloLineaResponseDto(linea, incluirPrecios);
                    response.Add(lineaDto);
                }

                return ResponseDto<List<ArticuloLineaResponseDto>>.CreateSuccess(
                    response, $"Encontradas {response.Count} líneas para artículo {codigoArticulo}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener líneas del artículo {Articulo}", codigoArticulo);
                return ResponseDto<List<ArticuloLineaResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<List<ArticuloLineaResponseDto>>> SearchArticuloLineasAsync(
            ArticuloLineaSearchRequest request)
        {
            try
            {
                var lineasRepo = _unitOfWork.Repository<ArticuloLinea>();
                var todasLasLineas = await lineasRepo.GetAllAsync();

                var lineasFiltradas = todasLasLineas.AsQueryable();

                // Aplicar filtros
                if (request.CodigoArticulo.HasValue)
                {
                    lineasFiltradas = lineasFiltradas.Where(l => l.CODARTICULO == request.CodigoArticulo.Value);
                }

                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var searchTerm = request.SearchTerm.ToLower();
                    lineasFiltradas = lineasFiltradas.Where(l =>
                        l.TALLA.ToLower().Contains(searchTerm) ||
                        l.COLOR.ToLower().Contains(searchTerm) ||
                        (l.CODBARRAS != null && l.CODBARRAS.ToLower().Contains(searchTerm)) ||
                        (l.CODBARRAS2 != null && l.CODBARRAS2.ToLower().Contains(searchTerm)) ||
                        (l.CODBARRAS3 != null && l.CODBARRAS3.ToLower().Contains(searchTerm)));
                }

                if (!string.IsNullOrWhiteSpace(request.CodigoBarras))
                {
                    lineasFiltradas = lineasFiltradas.Where(l =>
                        l.CODBARRAS == request.CodigoBarras ||
                        l.CODBARRAS2 == request.CodigoBarras ||
                        l.CODBARRAS3 == request.CodigoBarras);
                }

                if (request.SoloConCostes)
                {
                    lineasFiltradas = lineasFiltradas.Where(l =>
                        l.COSTEMEDIO.HasValue && l.COSTEMEDIO > 0 ||
                        l.COSTESTOCK.HasValue && l.COSTESTOCK > 0 ||
                        l.ULTIMOCOSTE.HasValue && l.ULTIMOCOSTE > 0);
                }

                if (request.SoloActivas)
                {
                    lineasFiltradas = lineasFiltradas.Where(l => l.DESCATALOGADO != "S");
                }

                // Paginación
                var totalCount = lineasFiltradas.Count();
                var lineasPaginadas = lineasFiltradas
                    .OrderBy(l => l.CODARTICULO)
                    .ThenBy(l => l.TALLA)
                    .ThenBy(l => l.COLOR)
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                var response = new List<ArticuloLineaResponseDto>();
                foreach (var linea in lineasPaginadas)
                {
                    var lineaDto = await BuildArticuloLineaResponseDto(linea, request.IncluirPrecios);
                    response.Add(lineaDto);
                }

                var message = $"Encontradas {totalCount} líneas, mostrando página {request.Page}";
                return ResponseDto<List<ArticuloLineaResponseDto>>.CreateSuccess(response, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en búsqueda de líneas de artículos");
                return ResponseDto<List<ArticuloLineaResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<ArticuloLineaResponseDto?>> GetLineaByCodigoBarrasAsync(string codigoBarras)
        {
            try
            {
                var lineasRepo = _unitOfWork.Repository<ArticuloLinea>();
                var linea = (await lineasRepo.FindAsync(l =>
                    l.CODBARRAS == codigoBarras ||
                    l.CODBARRAS2 == codigoBarras ||
                    l.CODBARRAS3 == codigoBarras)).FirstOrDefault();

                if (linea == null)
                {
                    return ResponseDto<ArticuloLineaResponseDto?>.CreateError(
                        $"No se encontró línea con código de barras {codigoBarras}");
                }

                var response = await BuildArticuloLineaResponseDto(linea, incluirPrecios: true);

                return ResponseDto<ArticuloLineaResponseDto?>.CreateSuccess(
                    response, $"Línea encontrada por código de barras {codigoBarras}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar línea por código de barras {CodBarras}", codigoBarras);
                return ResponseDto<ArticuloLineaResponseDto?>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<bool>> ValidarCodigoBarrasUnicoAsync(
            string codigoBarras, int? excludeArticulo = null, string? excludeTalla = null, string? excludeColor = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(codigoBarras))
                {
                    return ResponseDto<bool>.CreateSuccess(true, "Código de barras vacío es válido");
                }

                var lineasRepo = _unitOfWork.Repository<ArticuloLinea>();
                var lineasConCodigo = await lineasRepo.FindAsync(l =>
                    l.CODBARRAS == codigoBarras ||
                    l.CODBARRAS2 == codigoBarras ||
                    l.CODBARRAS3 == codigoBarras);

                // Excluir la línea actual si se especifica
                if (excludeArticulo.HasValue && !string.IsNullOrWhiteSpace(excludeTalla) && !string.IsNullOrWhiteSpace(excludeColor))
                {
                    lineasConCodigo = lineasConCodigo.Where(l =>
                        !(l.CODARTICULO == excludeArticulo.Value &&
                          l.TALLA == excludeTalla &&
                          l.COLOR == excludeColor));
                }

                var esUnico = !lineasConCodigo.Any();
                var message = esUnico
                    ? "Código de barras único"
                    : $"Código de barras {codigoBarras} ya existe en {lineasConCodigo.Count()} línea(s)";

                return ResponseDto<bool>.CreateSuccess(esUnico, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar unicidad del código de barras {CodBarras}", codigoBarras);
                return ResponseDto<bool>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<object>> GetEstadisticasLineasAsync()
        {
            try
            {
                var lineas = await _unitOfWork.Repository<ArticuloLinea>().GetAllAsync();
                var precios = await _unitOfWork.Repository<Precio>().GetAllAsync();

                var stats = new
                {
                    // Conteos básicos
                    TotalLineas = lineas.Count(),
                    LineasActivas = lineas.Count(l => l.DESCATALOGADO != "S"),
                    LineasDescatalogadas = lineas.Count(l => l.DESCATALOGADO == "S"),

                    // Códigos de barras
                    LineasConCodBarras = lineas.Count(l => !string.IsNullOrWhiteSpace(l.CODBARRAS)),
                    LineasConCodBarras2 = lineas.Count(l => !string.IsNullOrWhiteSpace(l.CODBARRAS2)),
                    LineasConCodBarras3 = lineas.Count(l => !string.IsNullOrWhiteSpace(l.CODBARRAS3)),
                    TotalCodigosBarras = lineas.Count(l => !string.IsNullOrWhiteSpace(l.CODBARRAS)) +
                                        lineas.Count(l => !string.IsNullOrWhiteSpace(l.CODBARRAS2)) +
                                        lineas.Count(l => !string.IsNullOrWhiteSpace(l.CODBARRAS3)),

                    // Costes
                    LineasConCosteMedio = lineas.Count(l => l.COSTEMEDIO.HasValue && l.COSTEMEDIO > 0),
                    LineasConCosteStock = lineas.Count(l => l.COSTESTOCK.HasValue && l.COSTESTOCK > 0),
                    LineasConUltimoCoste = lineas.Count(l => l.ULTIMOCOSTE.HasValue && l.ULTIMOCOSTE > 0),

                    // Estadísticas de costes
                    CosteMedioPromedio = lineas.Where(l => l.COSTEMEDIO.HasValue && l.COSTEMEDIO > 0)
                        .Any() ? Math.Round(lineas.Where(l => l.COSTEMEDIO.HasValue && l.COSTEMEDIO > 0)
                        .Average(l => l.COSTEMEDIO!.Value), 2) : 0,

                    CosteStockPromedio = lineas.Where(l => l.COSTESTOCK.HasValue && l.COSTESTOCK > 0)
                        .Any() ? Math.Round(lineas.Where(l => l.COSTESTOCK.HasValue && l.COSTESTOCK > 0)
                        .Average(l => l.COSTESTOCK!.Value), 2) : 0,

                    // Distribución por artículos
                    ArticulosConMasLineas = lineas
                        .GroupBy(l => l.CODARTICULO)
                        .Select(g => new { CodigoArticulo = g.Key, NumLineas = g.Count() })
                        .OrderByDescending(x => x.NumLineas)
                        .Take(10)
                        .ToList(),

                    // Tallas más comunes
                    TallasMasComunes = lineas
                        .Where(l => !string.IsNullOrWhiteSpace(l.TALLA))
                        .GroupBy(l => l.TALLA)
                        .Select(g => new { Talla = g.Key, Count = g.Count() })
                        .OrderByDescending(x => x.Count)
                        .Take(10)
                        .ToList(),

                    // Colores más comunes
                    ColoresMasComunes = lineas
                        .Where(l => !string.IsNullOrWhiteSpace(l.COLOR))
                        .GroupBy(l => l.COLOR)
                        .Select(g => new { Color = g.Key, Count = g.Count() })
                        .OrderByDescending(x => x.Count)
                        .Take(10)
                        .ToList(),

                    // Relación con precios
                    LineasConPrecios = lineas.Count(l => precios.Any(p =>
                        p.CODARTICULO == l.CODARTICULO &&
                        p.TALLA == l.TALLA &&
                        p.COLOR == l.COLOR)),

                    Timestamp = DateTime.UtcNow
                };

                return ResponseDto<object>.CreateSuccess(stats, "Estadísticas de líneas generadas exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar estadísticas de líneas");
                return ResponseDto<object>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        #region Métodos Auxiliares

        private async Task<ArticuloLineaResponseDto> BuildArticuloLineaResponseDto(
            ArticuloLinea linea, bool incluirPrecios = false)
        {
            var lineaDto = linea.Adapt<ArticuloLineaResponseDto>();

            // Cargar información del artículo padre
            var articulo = await _unitOfWork.Repository<Articulo>().GetByIdAsync(linea.CODARTICULO);
            if (articulo != null)
            {
                lineaDto.Articulo = articulo.Adapt<ArticuloResponseDto>();
            }

            // Cargar precios si se solicita
            if (incluirPrecios)
            {
                var precios = await _unitOfWork.Repository<Precio>().FindAsync(p =>
                    p.CODARTICULO == linea.CODARTICULO &&
                    p.TALLA == linea.TALLA &&
                    p.COLOR == linea.COLOR);
                lineaDto.Precios = precios.Adapt<List<PrecioResponseDto>>();
            }

            // Calcular estadísticas
            lineaDto.Estadisticas = new LineaEstadisticas
            {
                TieneCodBarras = !string.IsNullOrWhiteSpace(linea.CODBARRAS) ||
                                !string.IsNullOrWhiteSpace(linea.CODBARRAS2) ||
                                !string.IsNullOrWhiteSpace(linea.CODBARRAS3),
                TieneCostesDefinidos = linea.COSTEMEDIO.HasValue && linea.COSTEMEDIO > 0 ||
                                      linea.COSTESTOCK.HasValue && linea.COSTESTOCK > 0 ||
                                      linea.ULTIMOCOSTE.HasValue && linea.ULTIMOCOSTE > 0,
                EstaDescatalogada = linea.DESCATALOGADO == "S",
                TotalPrecios = incluirPrecios ? lineaDto.Precios.Count : 0,
                PrecioMinimo = incluirPrecios && lineaDto.Precios.Any() ? lineaDto.Precios.Min(p => p.PNETO) : null,
                PrecioMaximo = incluirPrecios && lineaDto.Precios.Any() ? lineaDto.Precios.Max(p => p.PNETO) : null,
                CodigoBarrasPrincipal = !string.IsNullOrWhiteSpace(linea.CODBARRAS) ? linea.CODBARRAS :
                                       !string.IsNullOrWhiteSpace(linea.CODBARRAS2) ? linea.CODBARRAS2 :
                                       !string.IsNullOrWhiteSpace(linea.CODBARRAS3) ? linea.CODBARRAS3 : string.Empty
            };

            return lineaDto;
        }

        private async Task<(bool IsValid, List<string> Errors)> ValidateArticuloLineaBusinessRules(ArticuloLineaDto linea)
        {
            var errors = new List<string>();

            // Validar que el artículo existe
            var articulo = await _unitOfWork.Repository<Articulo>().GetByIdAsync(linea.CODARTICULO);
            if (articulo == null)
            {
                errors.Add($"El artículo {linea.CODARTICULO} no existe");
            }

            // Validar unicidad de códigos de barras
            if (!string.IsNullOrWhiteSpace(linea.CODBARRAS))
            {
                var unicidadResult = await ValidarCodigoBarrasUnicoAsync(
                    linea.CODBARRAS, linea.CODARTICULO, linea.TALLA, linea.COLOR);
                if (unicidadResult.Success && !unicidadResult.Data)
                {
                    errors.Add($"El código de barras {linea.CODBARRAS} ya existe");
                }
            }

            if (!string.IsNullOrWhiteSpace(linea.CODBARRAS2))
            {
                var unicidadResult = await ValidarCodigoBarrasUnicoAsync(
                    linea.CODBARRAS2, linea.CODARTICULO, linea.TALLA, linea.COLOR);
                if (unicidadResult.Success && !unicidadResult.Data)
                {
                    errors.Add($"El código de barras 2 {linea.CODBARRAS2} ya existe");
                }
            }

            if (!string.IsNullOrWhiteSpace(linea.CODBARRAS3))
            {
                var unicidadResult = await ValidarCodigoBarrasUnicoAsync(
                    linea.CODBARRAS3, linea.CODARTICULO, linea.TALLA, linea.COLOR);
                if (unicidadResult.Success && !unicidadResult.Data)
                {
                    errors.Add($"El código de barras 3 {linea.CODBARRAS3} ya existe");
                }
            }

            // Validar coherencia de costes
            if (linea.COSTEMEDIO.HasValue && linea.COSTESTOCK.HasValue &&
                linea.ULTIMOCOSTE.HasValue && linea.COSTEMEDIO > 0 &&
                linea.COSTESTOCK > 0 && linea.ULTIMOCOSTE > 0)
            {
                var costeMedio = linea.COSTEMEDIO.Value;
                var costeStock = linea.COSTESTOCK.Value;
                var ultimoCoste = linea.ULTIMOCOSTE.Value;

                // Advertencia si hay mucha diferencia entre costes (más del 50%)
                if (Math.Abs(costeMedio - ultimoCoste) / costeMedio > 0.5m)
                {
                    _logger.LogWarning("Gran diferencia entre coste medio ({CosteMedio}) y último coste ({UltimoCoste}) para línea {Articulo}-{Talla}-{Color}",
                        costeMedio, ultimoCoste, linea.CODARTICULO, linea.TALLA, linea.COLOR);
                }
            }

            return (!errors.Any(), errors);
        }

        private async Task UpdateExistingArticuloLinea(ArticuloLinea existente, ArticuloLineaDto lineaDto)
        {
            existente.CODBARRAS = lineaDto.CODBARRAS;
            existente.COSTEMEDIO = lineaDto.COSTEMEDIO;
            existente.COSTESTOCK = lineaDto.COSTESTOCK;
            existente.ULTIMOCOSTE = lineaDto.ULTIMOCOSTE;
            existente.CODBARRAS2 = lineaDto.CODBARRAS2;
            existente.CODBARRAS3 = lineaDto.CODBARRAS3;
            existente.DESCATALOGADO = lineaDto.DESCATALOGADO;
            existente.UpdatedAt = DateTime.UtcNow;
        }

        #endregion
    }
}
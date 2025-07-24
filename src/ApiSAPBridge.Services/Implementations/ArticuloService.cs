using ApiSAPBridge.Core.Interfaces;
using ApiSAPBridge.Data.UnitOfWork;
using ApiSAPBridge.Models;
using ApiSAPBridge.Models.DTOs;
using ApiSAPBridge.Models.Constants;
using Mapster;
using Microsoft.Extensions.Logging;

namespace ApiSAPBridge.Services.Implementations
{
    public class ArticuloService : IArticuloService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ArticuloService> _logger;

        public ArticuloService(IUnitOfWork unitOfWork, ILogger<ArticuloService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ResponseDto<List<ArticuloResponseDto>>> CreateArticulosAsync(List<ArticuloDto> articulos)
        {
            try
            {
                _logger.LogInformation("Iniciando creación de {Count} artículos", articulos.Count);

                var resultados = new List<ArticuloResponseDto>();
                var errores = new List<string>();

                await _unitOfWork.BeginTransactionAsync();

                foreach (var articuloDto in articulos)
                {
                    var validationResult = await ValidateArticuloBusinessRules(articuloDto);
                    if (!validationResult.IsValid)
                    {
                        errores.AddRange(validationResult.Errors);
                        continue;
                    }

                    var articulosRepo = _unitOfWork.Repository<Articulo>();
                    var existente = await articulosRepo.GetByIdAsync(articuloDto.CODARTICULO);

                    if (existente != null)
                    {
                        await UpdateExistingArticulo(existente, articuloDto);
                        articulosRepo.Update(existente);
                        _logger.LogInformation("Artículo {Id} actualizado", existente.CODARTICULO);

                        var responseExistente = await BuildArticuloResponseDto(existente);
                        resultados.Add(responseExistente);
                    }
                    else
                    {
                        var nuevoArticulo = articuloDto.Adapt<Articulo>();
                        nuevoArticulo.CreatedAt = DateTime.UtcNow;
                        nuevoArticulo.UpdatedAt = DateTime.UtcNow;

                        await articulosRepo.AddAsync(nuevoArticulo);
                        _logger.LogInformation("Artículo {Id} creado", nuevoArticulo.CODARTICULO);

                        var responseNuevo = await BuildArticuloResponseDto(nuevoArticulo);
                        resultados.Add(responseNuevo);
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                if (errores.Any() && !resultados.Any())
                {
                    return ResponseDto<List<ArticuloResponseDto>>.CreateError(
                        "Todos los artículos fallaron las validaciones", errores);
                }

                var message = errores.Any()
                    ? $"Procesados {resultados.Count} artículos, {errores.Count} con errores"
                    : ApiConstants.ResponseMessages.SUCCESS_CREATE;

                return ResponseDto<List<ArticuloResponseDto>>.CreateSuccess(resultados, message);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error al procesar artículos");
                return ResponseDto<List<ArticuloResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<List<ArticuloResponseDto>>> CreateArticulosCompletosAsync(
            List<ArticuloCompletoRequest> articulosCompletos)
        {
            try
            {
                _logger.LogInformation("Iniciando creación de {Count} artículos completos", articulosCompletos.Count);

                var resultados = new List<ArticuloResponseDto>();
                var errores = new List<string>();

                await _unitOfWork.BeginTransactionAsync();

                foreach (var request in articulosCompletos)
                {
                    // Crear o actualizar artículo principal
                    var articuloResult = await CreateArticulosAsync(new List<ArticuloDto> { request.Articulo });
                    if (!articuloResult.Success || !articuloResult.Data?.Any() == true)
                    {
                        errores.Add($"Error al crear artículo {request.Articulo.CODARTICULO}");
                        continue;
                    }

                    var articuloCreado = articuloResult.Data.First();

                    // Crear líneas si se especificaron
                    if (request.Lineas.Any())
                    {
                        foreach (var linea in request.Lineas)
                        {
                            linea.CODARTICULO = request.Articulo.CODARTICULO;
                        }

                        var lineasRepo = _unitOfWork.Repository<ArticuloLinea>();
                        var lineasExistentes = await lineasRepo.FindAsync(l => l.CODARTICULO == request.Articulo.CODARTICULO);

                        // Eliminar líneas existentes si se van a reemplazar
                        if (lineasExistentes.Any())
                        {
                            lineasRepo.RemoveRange(lineasExistentes);
                        }

                        foreach (var lineaDto in request.Lineas)
                        {
                            var nuevaLinea = lineaDto.Adapt<ArticuloLinea>();
                            nuevaLinea.CreatedAt = DateTime.UtcNow;
                            nuevaLinea.UpdatedAt = DateTime.UtcNow;
                            await lineasRepo.AddAsync(nuevaLinea);
                        }
                    }

                    // Crear precios si se especificaron
                    if (request.Precios.Any())
                    {
                        foreach (var precio in request.Precios)
                        {
                            precio.CODARTICULO = request.Articulo.CODARTICULO;
                        }

                        var preciosRepo = _unitOfWork.Repository<Precio>();
                        var preciosExistentes = await preciosRepo.FindAsync(p => p.CODARTICULO == request.Articulo.CODARTICULO);

                        // Eliminar precios existentes si se van a reemplazar
                        if (preciosExistentes.Any())
                        {
                            preciosRepo.RemoveRange(preciosExistentes);
                        }

                        foreach (var precioDto in request.Precios)
                        {
                            var nuevoPrecio = precioDto.Adapt<Precio>();
                            nuevoPrecio.CreatedAt = DateTime.UtcNow;
                            nuevoPrecio.UpdatedAt = DateTime.UtcNow;
                            await preciosRepo.AddAsync(nuevoPrecio);
                        }
                    }

                    resultados.Add(articuloCreado);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return ResponseDto<List<ArticuloResponseDto>>.CreateSuccess(
                    resultados, $"Artículos completos procesados: {resultados.Count}");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error al procesar artículos completos");
                return ResponseDto<List<ArticuloResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<ArticuloResponseDto?>> GetArticuloCompletoAsync(int codigoArticulo)
        {
            try
            {
                var articulo = await _unitOfWork.Repository<Articulo>().GetByIdAsync(codigoArticulo);

                if (articulo == null)
                {
                    return ResponseDto<ArticuloResponseDto?>.CreateError(
                        ApiConstants.ResponseMessages.ERROR_NOT_FOUND);
                }

                var response = await BuildArticuloResponseDto(articulo, incluirLineas: true, incluirPrecios: true, incluirEstadisticas: true);

                return ResponseDto<ArticuloResponseDto?>.CreateSuccess(
                    response, ApiConstants.ResponseMessages.SUCCESS_RETRIEVE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener artículo completo {Id}", codigoArticulo);
                return ResponseDto<ArticuloResponseDto?>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<List<ArticuloResponseDto>>> GetArticulosAsync(
            bool incluirLineas = false, bool incluirPrecios = false)
        {
            try
            {
                var articulos = await _unitOfWork.Repository<Articulo>().GetAllAsync();
                var response = new List<ArticuloResponseDto>();

                foreach (var articulo in articulos)
                {
                    var articuloDto = await BuildArticuloResponseDto(articulo, incluirLineas, incluirPrecios);
                    response.Add(articuloDto);
                }

                return ResponseDto<List<ArticuloResponseDto>>.CreateSuccess(
                    response, ApiConstants.ResponseMessages.SUCCESS_RETRIEVE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener artículos");
                return ResponseDto<List<ArticuloResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<ArticuloResponseDto?>> GetArticuloByIdAsync(
            int codigoArticulo, bool incluirLineas = false, bool incluirPrecios = false)
        {
            try
            {
                var articulo = await _unitOfWork.Repository<Articulo>().GetByIdAsync(codigoArticulo);

                if (articulo == null)
                {
                    return ResponseDto<ArticuloResponseDto?>.CreateError(
                        ApiConstants.ResponseMessages.ERROR_NOT_FOUND);
                }

                var response = await BuildArticuloResponseDto(articulo, incluirLineas, incluirPrecios);
                return ResponseDto<ArticuloResponseDto?>.CreateSuccess(
                    response, ApiConstants.ResponseMessages.SUCCESS_RETRIEVE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener artículo {Id}", codigoArticulo);
                return ResponseDto<ArticuloResponseDto?>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        #region Métodos de Búsqueda y Análisis

        public async Task<ResponseDto<List<ArticuloResponseDto>>> SearchArticulosAsync(ArticuloSearchRequest request)
        {
            try
            {
                var articulosRepo = _unitOfWork.Repository<Articulo>();
                var todosLosArticulos = await articulosRepo.GetAllAsync();

                var articulosFiltrados = todosLosArticulos.AsQueryable();

                // Aplicar filtros
                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var searchTerm = request.SearchTerm.ToLower();
                    articulosFiltrados = articulosFiltrados.Where(a =>
                        a.DESCRIPCION.ToLower().Contains(searchTerm) ||
                        (a.DESCRIPADIC != null && a.DESCRIPADIC.ToLower().Contains(searchTerm)) ||
                        a.CODARTICULO.ToString().Contains(searchTerm) ||
                        (a.REFPROVEEDOR != null && a.REFPROVEEDOR.ToLower().Contains(searchTerm)));
                }

                if (request.Departamento.HasValue)
                    articulosFiltrados = articulosFiltrados.Where(a => a.DPTO == request.Departamento.Value);

                if (request.Seccion.HasValue)
                    articulosFiltrados = articulosFiltrados.Where(a => a.SECCION == request.Seccion.Value);

                if (request.Familia.HasValue)
                    articulosFiltrados = articulosFiltrados.Where(a => a.FAMILIA == request.Familia.Value);

                if (request.TipoImpuesto.HasValue)
                    articulosFiltrados = articulosFiltrados.Where(a => a.TIPOIMPUESTO == request.TipoImpuesto.Value);

                if (!string.IsNullOrWhiteSpace(request.UsaStocks))
                    articulosFiltrados = articulosFiltrados.Where(a => a.USASTOCKS == request.UsaStocks);

                if (!string.IsNullOrWhiteSpace(request.Descatalogado))
                    articulosFiltrados = articulosFiltrados.Where(a => a.DESCATALOGADO == request.Descatalogado);

                // Paginación
                var totalCount = articulosFiltrados.Count();
                var articulosPaginados = articulosFiltrados
                    .OrderBy(a => a.DESCRIPCION)
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                var response = new List<ArticuloResponseDto>();
                foreach (var articulo in articulosPaginados)
                {
                    var articuloDto = await BuildArticuloResponseDto(
                        articulo,
                        request.IncluirLineas,
                        request.IncluirPrecios,
                        request.IncluirEstadisticas);
                    response.Add(articuloDto);
                }

                var message = $"Encontrados {totalCount} artículos, mostrando página {request.Page}";
                return ResponseDto<List<ArticuloResponseDto>>.CreateSuccess(response, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en búsqueda de artículos");
                return ResponseDto<List<ArticuloResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<object>> GetEstadisticasArticulosAsync()
        {
            try
            {
                var articulos = await _unitOfWork.Repository<Articulo>().GetAllAsync();
                var lineas = await _unitOfWork.Repository<ArticuloLinea>().GetAllAsync();
                var precios = await _unitOfWork.Repository<Precio>().GetAllAsync();

                var stats = new
                {
                    // Conteos básicos
                    TotalArticulos = articulos.Count(),
                    TotalLineas = lineas.Count(),
                    TotalPrecios = precios.Count(),

                    // Distribuciones
                    ArticulosPorDepartamento = articulos
                        .Where(a => a.DPTO.HasValue)
                        .GroupBy(a => a.DPTO)
                        .Select(g => new { Departamento = g.Key, Count = g.Count() })
                        .OrderByDescending(x => x.Count)
                        .ToList(),

                    ArticulosPorTipoImpuesto = articulos
                        .GroupBy(a => a.TIPOIMPUESTO)
                        .Select(g => new { TipoImpuesto = g.Key, Count = g.Count() })
                        .OrderByDescending(x => x.Count)
                        .ToList(),

                    // Estados
                    ArticulosDescatalogados = articulos.Count(a => a.DESCATALOGADO == "S"),
                    ArticulosConStock = articulos.Count(a => a.USASTOCKS == "S"),
                    ArticulosConLineas = articulos.Count(a => lineas.Any(l => l.CODARTICULO == a.CODARTICULO)),
                    ArticulosConPrecios = articulos.Count(a => precios.Any(p => p.CODARTICULO == a.CODARTICULO)),

                    // Promedios
                    PromedioLineasPorArticulo = articulos.Count() > 0 ? Math.Round((double)lineas.Count() / articulos.Count(), 2) : 0,
                    PromedioPreciosPorArticulo = articulos.Count() > 0 ? Math.Round((double)precios.Count() / articulos.Count(), 2) : 0,

                    Timestamp = DateTime.UtcNow
                };

                return ResponseDto<object>.CreateSuccess(stats, "Estadísticas generadas exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar estadísticas de artículos");
                return ResponseDto<object>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        #endregion

        #region Métodos Auxiliares

        private async Task<ArticuloResponseDto> BuildArticuloResponseDto(
            Articulo articulo,
            bool incluirLineas = false,
            bool incluirPrecios = false,
            bool incluirEstadisticas = false)
        {
            var articuloDto = articulo.Adapt<ArticuloResponseDto>();

            // Cargar impuesto
            if (articulo.TIPOIMPUESTO > 0)
            {
                var impuesto = await _unitOfWork.Repository<Impuesto>().GetByIdAsync(articulo.TIPOIMPUESTO);
                articuloDto.Impuesto = impuesto?.Adapt<ImpuestoResponseDto>();
            }

            // Cargar departamento
            if (articulo.DPTO.HasValue)
            {
                var departamento = await _unitOfWork.Repository<Departamento>().GetByIdAsync(articulo.DPTO.Value);
                articuloDto.Departamento = departamento?.Adapt<DepartamentoResponseDto>();
            }

            // Cargar líneas
            if (incluirLineas)
            {
                var lineas = await _unitOfWork.Repository<ArticuloLinea>()
                    .FindAsync(l => l.CODARTICULO == articulo.CODARTICULO);
                articuloDto.Lineas = lineas.Adapt<List<ArticuloLineaResponseDto>>();
            }

            // Cargar precios
            if (incluirPrecios)
            {
                var precios = await _unitOfWork.Repository<Precio>()
                    .FindAsync(p => p.CODARTICULO == articulo.CODARTICULO);
                articuloDto.Precios = precios.Adapt<List<PrecioResponseDto>>();
            }

            // Calcular estadísticas
            if (incluirEstadisticas)
            {
                var lineasCount = await _unitOfWork.Repository<ArticuloLinea>()
                    .CountAsync(l => l.CODARTICULO == articulo.CODARTICULO);
                var preciosData = await _unitOfWork.Repository<Precio>()
                    .FindAsync(p => p.CODARTICULO == articulo.CODARTICULO);

                articuloDto.Estadisticas = new ArticuloEstadisticas
                {
                    TotalLineas = lineasCount,
                    TotalPrecios = preciosData.Count(),
                    TotalTarifas = preciosData.GroupBy(p => p.IDTARIFAV).Count(),
                    PrecioMinimo = preciosData.Any() ? preciosData.Min(p => p.PNETO) : null,
                    PrecioMaximo = preciosData.Any() ? preciosData.Max(p => p.PNETO) : null,
                    PrecioPromedio = preciosData.Any() ? Math.Round(preciosData.Average(p => p.PNETO), 2) : null,
                    EstaDescatalogado = articulo.DESCATALOGADO == "S",
                    UsaStocks = articulo.USASTOCKS == "S",
                    TieneCostesDefinidos = articulo.UNID1C.HasValue && articulo.UNID1C > 0
                };
            }

            return articuloDto;
        }

        private async Task<(bool IsValid, List<string> Errors)> ValidateArticuloBusinessRules(ArticuloDto articulo)
        {
            var errors = new List<string>();

            // Validar que el impuesto existe
            var impuesto = await _unitOfWork.Repository<Impuesto>().GetByIdAsync(articulo.TIPOIMPUESTO);
            if (impuesto == null)
            {
                errors.Add($"El tipo de impuesto {articulo.TIPOIMPUESTO} no existe");
            }

            // Validar departamento, sección, familia si están especificados
            if (articulo.DPTO.HasValue)
            {
                var departamento = await _unitOfWork.Repository<Departamento>().GetByIdAsync(articulo.DPTO.Value);
                if (departamento == null)
                {
                    errors.Add($"El departamento {articulo.DPTO} no existe");
                }
                else if (articulo.SECCION.HasValue)
                {
                    var seccionRepo = _unitOfWork.Repository<Seccion>();
                    var seccion = (await seccionRepo.FindAsync(s =>
                        s.NUMDPTO == articulo.DPTO.Value &&
                        s.NUMSECCION == articulo.SECCION.Value)).FirstOrDefault();

                    if (seccion == null)
                    {
                        errors.Add($"La sección {articulo.SECCION} no existe en el departamento {articulo.DPTO}");
                    }
                    else if (articulo.FAMILIA.HasValue)
                    {
                        var familiaRepo = _unitOfWork.Repository<Familia>();
                        var familia = (await familiaRepo.FindAsync(f =>
                            f.NUMDPTO == articulo.DPTO.Value &&
                            f.NUMSECCION == articulo.SECCION.Value &&
                            f.NUMFAMILIA == articulo.FAMILIA.Value)).FirstOrDefault();

                        if (familia == null)
                        {
                            errors.Add($"La familia {articulo.FAMILIA} no existe en la sección {articulo.DPTO}-{articulo.SECCION}");
                        }
                    }
                }
            }

            return (!errors.Any(), errors);
        }

        private async Task UpdateExistingArticulo(Articulo existente, ArticuloDto articuloDto)
        {
            existente.DESCRIPCION = articuloDto.DESCRIPCION;
            existente.DESCRIPADIC = articuloDto.DESCRIPADIC;
            existente.TIPOIMPUESTO = articuloDto.TIPOIMPUESTO;
            existente.DPTO = articuloDto.DPTO;
            existente.SECCION = articuloDto.SECCION;
            existente.FAMILIA = articuloDto.FAMILIA;
            existente.UNID1C = articuloDto.UNID1C;
            existente.UNID1V = articuloDto.UNID1V;
            existente.REFPROVEEDOR = articuloDto.REFPROVEEDOR;
            existente.USASTOCKS = articuloDto.USASTOCKS;
            existente.IMPUESTOCOMPRA = articuloDto.IMPUESTOCOMPRA;
            existente.DESCATALOGADO = articuloDto.DESCATALOGADO;
            existente.UDSTRASPASO = articuloDto.UDSTRASPASO;
            existente.TIPOARTICULO = articuloDto.TIPOARTICULO;
            existente.UpdatedAt = DateTime.UtcNow;
        }

        // Implementar métodos restantes de la interfaz...
        public async Task<ResponseDto<List<ArticuloResponseDto>>> GetArticulosByDepartamentoAsync(int numDpto, bool incluirLineas = false)
        {
            try
            {
                var articulos = await _unitOfWork.Repository<Articulo>().FindAsync(a => a.DPTO == numDpto);
                var response = new List<ArticuloResponseDto>();

                foreach (var articulo in articulos)
                {
                    var articuloDto = await BuildArticuloResponseDto(articulo, incluirLineas);
                    response.Add(articuloDto);
                }

                return ResponseDto<List<ArticuloResponseDto>>.CreateSuccess(
                    response, $"Encontrados {response.Count} artículos en departamento {numDpto}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener artículos del departamento {Dpto}", numDpto);
                return ResponseDto<List<ArticuloResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<List<ArticuloResponseDto>>> GetArticulosByImpuestoAsync(int tipoImpuesto)
        {
            try
            {
                var articulos = await _unitOfWork.Repository<Articulo>().FindAsync(a => a.TIPOIMPUESTO == tipoImpuesto);
                var response = new List<ArticuloResponseDto>();

                foreach (var articulo in articulos)
                {
                    var articuloDto = await BuildArticuloResponseDto(articulo);
                    response.Add(articuloDto);
                }

                return ResponseDto<List<ArticuloResponseDto>>.CreateSuccess(
                    response, $"Encontrados {response.Count} artículos con impuesto {tipoImpuesto}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener artículos por impuesto {Impuesto}", tipoImpuesto);
                return ResponseDto<List<ArticuloResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<bool>> ValidarIntegridadArticuloAsync(int codigoArticulo)
        {
            try
            {
                var articulo = await _unitOfWork.Repository<Articulo>().GetByIdAsync(codigoArticulo);
                if (articulo == null)
                {
                    return ResponseDto<bool>.CreateError("Artículo no encontrado");
                }

                var validation = await ValidateArticuloBusinessRules(articulo.Adapt<ArticuloDto>());
                return ResponseDto<bool>.CreateSuccess(validation.IsValid,
                    validation.IsValid ? "Artículo válido" : string.Join(", ", validation.Errors));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar integridad del artículo {Id}", codigoArticulo);
                return ResponseDto<bool>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<List<ArticuloLineaResponseDto>>> GetLineasByArticuloAsync(int codigoArticulo)
        {
            try
            {
                var lineas = await _unitOfWork.Repository<ArticuloLinea>()
                    .FindAsync(l => l.CODARTICULO == codigoArticulo);

                var response = lineas.Adapt<List<ArticuloLineaResponseDto>>();

                return ResponseDto<List<ArticuloLineaResponseDto>>.CreateSuccess(
                    response, $"Encontradas {response.Count} líneas para artículo {codigoArticulo}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener líneas del artículo {Id}", codigoArticulo);
                return ResponseDto<List<ArticuloLineaResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<List<ArticuloResponseDto>>> GetArticulosByCodBarrasAsync(string codigoBarras)
        {
            try
            {
                var lineasConCodBarras = await _unitOfWork.Repository<ArticuloLinea>()
                    .FindAsync(l => l.CODBARRAS == codigoBarras ||
                                   l.CODBARRAS2 == codigoBarras ||
                                   l.CODBARRAS3 == codigoBarras);

                var codigosArticulos = lineasConCodBarras.Select(l => l.CODARTICULO).Distinct();
                var articulos = new List<Articulo>();

                foreach (var codigo in codigosArticulos)
                {
                    var articulo = await _unitOfWork.Repository<Articulo>().GetByIdAsync(codigo);
                    if (articulo != null)
                    {
                        articulos.Add(articulo);
                    }
                }

                var response = new List<ArticuloResponseDto>();
                foreach (var articulo in articulos)
                {
                    var articuloDto = await BuildArticuloResponseDto(articulo, incluirLineas: true);
                    response.Add(articuloDto);
                }

                return ResponseDto<List<ArticuloResponseDto>>.CreateSuccess(
                    response, $"Encontrados {response.Count} artículos con código de barras {codigoBarras}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar artículos por código de barras {CodBarras}", codigoBarras);
                return ResponseDto<List<ArticuloResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        #endregion
    }
}
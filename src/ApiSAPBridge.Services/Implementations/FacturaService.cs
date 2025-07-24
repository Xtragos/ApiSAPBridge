using ApiSAPBridge.Core.Interfaces;
using ApiSAPBridge.Data.UnitOfWork;
using ApiSAPBridge.Models;
using ApiSAPBridge.Models.DTOs;
using ApiSAPBridge.Models.Constants;
using Mapster;
using Microsoft.Extensions.Logging;

namespace ApiSAPBridge.Services.Implementations
{
    public class FacturaService : IFacturaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FacturaService> _logger;

        public FacturaService(IUnitOfWork unitOfWork, ILogger<FacturaService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ResponseDto<List<FacturaResponseDto>>> CreateFacturasAsync(
            List<FacturaCompletaRequest> facturas)
        {
            try
            {
                _logger.LogInformation("Iniciando creación de {Count} facturas", facturas.Count);

                var resultados = new List<FacturaResponseDto>();
                var errores = new List<string>();

                await _unitOfWork.BeginTransactionAsync();

                foreach (var facturaRequest in facturas)
                {
                    // Validaciones de negocio
                    var validationResult = await ValidateFacturaBusinessRules(facturaRequest);
                    if (!validationResult.IsValid)
                    {
                        errores.AddRange(validationResult.Errors);
                        continue;
                    }

                    // Calcular totales automáticamente si se solicita
                    if (facturaRequest.CalcularTotalesAutomatico)
                    {
                        CalcularTotalesFactura(facturaRequest);
                    }

                    // Crear factura principal
                    var factura = facturaRequest.Factura.Adapt<Factura>();
                    factura.CreatedAt = DateTime.UtcNow;
                    factura.UpdatedAt = DateTime.UtcNow;

                    var facturasRepo = _unitOfWork.Repository<Factura>();
                    await facturasRepo.AddAsync(factura);

                    // Crear detalles
                    var detallesRepo = _unitOfWork.Repository<FacturaDetalle>();
                    foreach (var detalleDto in facturaRequest.Detalles)
                    {
                        var detalle = detalleDto.Adapt<FacturaDetalle>();
                        detalle.CreatedAt = DateTime.UtcNow;
                        detalle.UpdatedAt = DateTime.UtcNow;
                        await detallesRepo.AddAsync(detalle);
                    }

                    // Crear pagos
                    var pagosRepo = _unitOfWork.Repository<FacturaPago>();
                    foreach (var pagoDto in facturaRequest.Pagos)
                    {
                        var pago = pagoDto.Adapt<FacturaPago>();
                        pago.CreatedAt = DateTime.UtcNow;
                        pago.UpdatedAt = DateTime.UtcNow;
                        await pagosRepo.AddAsync(pago);
                    }

                    _logger.LogInformation("Factura {Serie}-{Numero}-{N} creada exitosamente",
                        factura.NUMSERIE, factura.NUMFACTURA, factura.N);

                    var response = await BuildFacturaResponseDto(factura);
                    resultados.Add(response);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                if (errores.Any() && !resultados.Any())
                {
                    return ResponseDto<List<FacturaResponseDto>>.CreateError(
                        "Todas las facturas fallaron las validaciones", errores);
                }

                var message = errores.Any()
                    ? $"Procesadas {resultados.Count} facturas, {errores.Count} con errores"
                    : ApiConstants.ResponseMessages.SUCCESS_CREATE;

                return ResponseDto<List<FacturaResponseDto>>.CreateSuccess(resultados, message);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error al procesar facturas");
                return ResponseDto<List<FacturaResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<FacturacionSAPResponse>> GetFacturasSAPAsync(FacturaSearchRequest request)
        {
            try
            {
                _logger.LogInformation("Obteniendo facturas para SAP con filtros");

                var facturasRepo = _unitOfWork.Repository<Factura>();
                var todasLasFacturas = await facturasRepo.GetAllAsync();

                var facturasFiltradas = todasLasFacturas.AsQueryable();

                // Aplicar filtros
                if (request.FechaInicio.HasValue)
                {
                    facturasFiltradas = facturasFiltradas.Where(f => f.FECHA >= request.FechaInicio.Value);
                }

                if (request.FechaFin.HasValue)
                {
                    facturasFiltradas = facturasFiltradas.Where(f => f.FECHA <= request.FechaFin.Value);
                }

                if (!string.IsNullOrWhiteSpace(request.NumSerie))
                {
                    facturasFiltradas = facturasFiltradas.Where(f => f.NUMSERIE == request.NumSerie);
                }

                if (request.NumFactura.HasValue)
                {
                    facturasFiltradas = facturasFiltradas.Where(f => f.NUMFACTURA == request.NumFactura.Value);
                }

                if (request.CodCliente.HasValue)
                {
                    facturasFiltradas = facturasFiltradas.Where(f => f.CODCLIENTE == request.CodCliente.Value);
                }

                if (request.CodVendedor.HasValue)
                {
                    facturasFiltradas = facturasFiltradas.Where(f => f.CODVENDEDOR == request.CodVendedor);
                }

                if (!string.IsNullOrWhiteSpace(request.TipoDoc))
                {
                    facturasFiltradas = facturasFiltradas.Where(f => f.TIPODOC == request.TipoDoc);
                }

                // Contar total antes de paginación
                var totalCount = facturasFiltradas.Count();

                // Aplicar paginación
                var facturasPaginadas = facturasFiltradas
                    .OrderByDescending(f => f.FECHA)
                    .ThenBy(f => f.NUMSERIE)
                    .ThenBy(f => f.NUMFACTURA)
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                var facturasSAP = new List<FacturaSAPDto>();

                foreach (var factura in facturasPaginadas)
                {
                    var facturaSAP = new FacturaSAPDto
                    {
                        NUMSERIE = factura.NUMSERIE,
                        NUMFACTURA = factura.NUMFACTURA,
                        N = factura.N,
                        FECHA = factura.FECHA.ToString("yyyy-MM-dd"),
                        CODCLIENTE = factura.CODCLIENTE,
                        CODVENDEDOR = factura.CODVENDEDOR,
                        TOTALBRUTO = factura.TOTALBRUTO,
                        TOTALIMPUESTOS = factura.TOTALIMPUESTOS,
                        TOTDTOCOMERCIAL = factura.TOTDTOCOMERCIAL,
                        TOTALNETO = factura.TOTALNETO,
                        TIPODOC = factura.TIPODOC,
                        FECHACREADO = factura.FECHACREADO.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        FECHAMODIFICADO = factura.FECHAMODIFICADO.ToString("yyyy-MM-ddTHH:mm:ssZ")
                    };

                    // Cargar detalles si se solicita
                    if (request.IncluirDetalles)
                    {
                        var detalles = await _unitOfWork.Repository<FacturaDetalle>()
                            .FindAsync(d => d.SERIE == factura.NUMSERIE &&
                                           d.NUMERO == factura.NUMFACTURA &&
                                           d.N == factura.N);

                        facturaSAP.Detalles = detalles.Select(d => new DetalleSAPDto
                        {
                            SERIE = d.SERIE,
                            NUMERO = d.NUMERO,
                            N = d.N,
                            LINEA = d.LINEA,
                            CODARTICULO = d.CODARTICULO,
                            REFERENCIA = d.REFERENCIA,
                            DESCRIPCION = d.DESCRIPCION,
                            TALLA = d.TALLA,
                            COLOR = d.COLOR,
                            TIPOIMPUESTO = d.TIPOIMPUESTO,
                            UNIDADESTOTAL = d.UNIDADESTOTAL,
                            PRECIO = d.PRECIO,
                            DTO = d.DTO,
                            TOTAL = d.TOTAL
                        }).ToList();
                    }

                    // Cargar pagos si se solicita
                    if (request.IncluirPagos)
                    {
                        var pagos = await _unitOfWork.Repository<FacturaPago>()
                            .FindAsync(p => p.SERIE == factura.NUMSERIE &&
                                           p.NUMERO == factura.NUMFACTURA &&
                                           p.N == factura.N);

                        facturaSAP.Pagos = pagos.Select(p => new PagoSAPDto
                        {
                            SERIE = p.SERIE,
                            NUMERO = p.NUMERO,
                            N = p.N,
                            POSICION = p.POSICION,
                            CODTIPOPAGO = p.CODTIPOPAGO,
                            IMPORTE = p.IMPORTE,
                            DESCRIPCION = p.DESCRIPCION
                        }).ToList();
                    }

                    facturasSAP.Add(facturaSAP);
                }

                var response = new FacturacionSAPResponse
                {
                    Facturas = facturasSAP,
                    Paginacion = new PaginacionDto
                    {
                        PaginaActual = request.Page,
                        PorPagina = request.PageSize,
                        TotalFacturas = totalCount,
                        TotalPaginas = (int)Math.Ceiling((double)totalCount / request.PageSize)
                    }
                };

                return ResponseDto<FacturacionSAPResponse>.CreateSuccess(
                    response, $"Obtenidas {facturasSAP.Count} facturas de {totalCount} totales");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener facturas para SAP");
                return ResponseDto<FacturacionSAPResponse>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        // Implementar métodos específicos para SAP
        public async Task<ResponseDto<FacturacionSAPResponse>> GetFacturasByFechaRangoAsync(
            DateTime fechaInicio, DateTime fechaFin, int pageSize = 50, int page = 1)
        {
            var request = new FacturaSearchRequest
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                PageSize = pageSize,
                Page = page,
                IncluirDetalles = true,
                IncluirPagos = true
            };

            return await GetFacturasSAPAsync(request);
        }

        public async Task<ResponseDto<FacturacionSAPResponse>> GetFacturasByFechaYSerieAsync(
            DateTime fechaInicio, DateTime fechaFin, string numSerie, int pageSize = 50, int page = 1)
        {
            var request = new FacturaSearchRequest
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                NumSerie = numSerie,
                PageSize = pageSize,
                Page = page,
                IncluirDetalles = true,
                IncluirPagos = true
            };

            return await GetFacturasSAPAsync(request);
        }

        public async Task<ResponseDto<FacturacionSAPResponse>> GetFacturasBySerieAsync(
            string numSerie, int pageSize = 50, int page = 1)
        {
            var request = new FacturaSearchRequest
            {
                NumSerie = numSerie,
                PageSize = pageSize,
                Page = page,
                IncluirDetalles = true,
                IncluirPagos = true
            };

            return await GetFacturasSAPAsync(request);
        }

        #region Métodos Auxiliares

        private void CalcularTotalesFactura(FacturaCompletaRequest facturaRequest)
        {
            decimal totalBruto = 0;
            decimal totalDescuentos = 0;

            foreach (var detalle in facturaRequest.Detalles)
            {
                var subtotal = detalle.PRECIO * detalle.UNIDADESTOTAL;
                var descuento = subtotal * (detalle.DTO / 100);
                var totalLinea = subtotal - descuento;

                detalle.TOTAL = totalLinea;
                totalBruto += subtotal;
                totalDescuentos += descuento;
            }

            facturaRequest.Factura.TOTALBRUTO = totalBruto;
            facturaRequest.Factura.TOTDTOCOMERCIAL = totalDescuentos;

            // Calcular impuestos (simplificado - en realidad debería ser más complejo)
            var totalSinDescuentos = totalBruto - totalDescuentos;
            facturaRequest.Factura.TOTALIMPUESTOS = totalSinDescuentos * 0.07m; // 7% ITBMS ejemplo
            facturaRequest.Factura.TOTALNETO = totalSinDescuentos + facturaRequest.Factura.TOTALIMPUESTOS;
        }

        private async Task<(bool IsValid, List<string> Errors)> ValidateFacturaBusinessRules(
            FacturaCompletaRequest factura)
        {
            var errors = new List<string>();

            // Validar que el cliente existe
            var cliente = await _unitOfWork.Repository<Cliente>().GetByIdAsync(factura.Factura.CODCLIENTE);
            if (cliente == null)
            {
                errors.Add($"El cliente {factura.Factura.CODCLIENTE} no existe");
            }

            // Validar que el vendedor existe
            var vendedor = await _unitOfWork.Repository<Vendedor>().GetByIdAsync(factura.Factura.CODVENDEDOR);
            if (vendedor == null)
            {
                errors.Add($"El vendedor {factura.Factura.CODVENDEDOR} no existe");
            }

            // Validar que no existe la factura
            var facturaExistente = await _unitOfWork.Repository<Factura>()
                .FindAsync(f => f.NUMSERIE == factura.Factura.NUMSERIE &&
                               f.NUMFACTURA == factura.Factura.NUMFACTURA &&
                               f.N == factura.Factura.N);

            if (facturaExistente.Any())
            {
                errors.Add($"La factura {factura.Factura.NUMSERIE}-{factura.Factura.NUMFACTURA}-{factura.Factura.N} ya existe");
            }

            // Validar detalles
            if (!factura.Detalles.Any())
            {
                errors.Add("La factura debe tener al menos un detalle");
            }

            // Validar pagos
            if (!factura.Pagos.Any())
            {
                errors.Add("La factura debe tener al menos una forma de pago");
            }

            // Validar totales si se solicita
            if (factura.ValidarTotales)
            {
                var totalPagos = factura.Pagos.Sum(p => p.IMPORTE);
                if (Math.Abs(totalPagos - factura.Factura.TOTALNETO) > 0.01m)
                {
                    errors.Add($"El total de pagos ({totalPagos:F2}) no coincide con el total neto ({factura.Factura.TOTALNETO:F2})");
                }
            }

            return (!errors.Any(), errors);
        }

        private async Task<FacturaResponseDto> BuildFacturaResponseDto(Factura factura)
        {
            var facturaDto = factura.Adapt<FacturaResponseDto>();

            // Cargar relaciones si es necesario
            // ... implementar según necesidades

            return facturaDto;
        }

        // Implementar métodos restantes...
        public async Task<ResponseDto<List<FacturaResponseDto>>> GetFacturasAsync(bool incluirDetalles = true, bool incluirPagos = true)
        {
            // Implementación básica
            throw new NotImplementedException();
        }

        public async Task<ResponseDto<FacturaResponseDto?>> GetFacturaAsync(string numSerie, int numFactura, int n)
        {
            // Implementación básica  
            throw new NotImplementedException();
        }

        public async Task<ResponseDto<object>> GetEstadisticasFacturacionAsync(DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            // Implementación de estadísticas
            throw new NotImplementedException();
        }

        public async Task<ResponseDto<object>> GetResumenVentasPorClienteAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            // Implementación de resumen por cliente
            throw new NotImplementedException();
        }

        public async Task<ResponseDto<object>> GetResumenVentasPorVendedorAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            // Implementación de resumen por vendedor
            throw new NotImplementedException();
        }

        public async Task<ResponseDto<bool>> ValidarFacturaAsync(FacturaCompletaRequest factura)
        {
            var validation = await ValidateFacturaBusinessRules(factura);
            return ResponseDto<bool>.CreateSuccess(validation.IsValid,
                validation.IsValid ? "Factura válida" : string.Join(", ", validation.Errors));
        }

        #endregion
    }
}
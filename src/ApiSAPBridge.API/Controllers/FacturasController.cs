using ApiSAPBridge.API.Attributes;
using ApiSAPBridge.Core.Interfaces;
using ApiSAPBridge.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ApiSAPBridge.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacturasController : ControllerBase
    {
        private readonly IFacturaService _facturaService;
        private readonly ILogger<FacturasController> _logger;

        public FacturasController(IFacturaService facturaService, ILogger<FacturasController> logger)
        {
            _facturaService = facturaService;
            _logger = logger;
        }

        /// <summary>
        /// Crear facturas completas (endpoint para SAP)
        /// </summary>
        [HttpPost]
        [ApiKeyAuth]
        public async Task<ActionResult<ResponseDto<List<FacturaResponseDto>>>> CreateFacturas(
            [FromBody] List<FacturaCompletaRequest> facturas)
        {
            var result = await _facturaService.CreateFacturasAsync(facturas);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Obtener todas las facturas para SAP con filtros opcionales
        /// </summary>
        [HttpPost("search")]
        public async Task<ActionResult<ResponseDto<FacturacionSAPResponse>>> GetFacturasSAP(
            [FromBody] FacturaSearchRequest request)
        {
            var result = await _facturaService.GetFacturasSAPAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Obtener facturas por rango de fechas
        /// </summary>
        [HttpGet("fechas")]
        public async Task<ActionResult<ResponseDto<FacturacionSAPResponse>>> GetFacturasByFecha(
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin,
            [FromQuery] int pageSize = 50,
            [FromQuery] int page = 1)
        {
            var result = await _facturaService.GetFacturasByFechaRangoAsync(fechaInicio, fechaFin, pageSize, page);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Obtener facturas por rango de fechas y serie específica
        /// </summary>
        [HttpGet("fechas-serie")]
        public async Task<ActionResult<ResponseDto<FacturacionSAPResponse>>> GetFacturasByFechaYSerie(
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin,
            [FromQuery] string numSerie,
            [FromQuery] int pageSize = 50,
            [FromQuery] int page = 1)
        {
            var result = await _facturaService.GetFacturasByFechaYSerieAsync(fechaInicio, fechaFin, numSerie, pageSize, page);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Obtener facturas por serie específica
        /// </summary>
        [HttpGet("serie/{numSerie}")]
        public async Task<ActionResult<ResponseDto<FacturacionSAPResponse>>> GetFacturasBySerie(
            string numSerie,
            [FromQuery] int pageSize = 50,
            [FromQuery] int page = 1)
        {
            var result = await _facturaService.GetFacturasBySerieAsync(numSerie, pageSize, page);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Validar estructura de factura antes de crear
        /// </summary>
        [HttpPost("validar")]
        public async Task<ActionResult<ResponseDto<bool>>> ValidarFactura(
            [FromBody] FacturaCompletaRequest factura)
        {
            var result = await _facturaService.ValidarFacturaAsync(factura);
            return Ok(result);
        }

        /// <summary>
        /// Obtener estadísticas de facturación
        /// </summary>
        [HttpGet("estadisticas")]
        public async Task<ActionResult<ResponseDto<object>>> GetEstadisticas(
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null)
        {
            var result = await _facturaService.GetEstadisticasFacturacionAsync(fechaInicio, fechaFin);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
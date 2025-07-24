using ApiSAPBridge.API.Attributes;
using ApiSAPBridge.Core.Interfaces;
using ApiSAPBridge.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ApiSAPBridge.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PreciosController : ControllerBase
    {
        private readonly IPrecioService _precioService;
        private readonly ILogger<PreciosController> _logger;

        public PreciosController(IPrecioService precioService, ILogger<PreciosController> logger)
        {
            _precioService = precioService;
            _logger = logger;
        }

        /// <summary>
        /// Crear/actualizar precios (endpoint para SAP)
        /// </summary>
        [HttpPost]
        [ApiKeyAuth]
        public async Task<ActionResult<ResponseDto<List<PrecioResponseDto>>>> CreatePrecios(
            [FromBody] List<PrecioDto> precios)
        {
            var result = await _precioService.CreatePreciosAsync(precios);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Crear precios masivos con validaciones avanzadas
        /// </summary>
        [HttpPost("masivos")]
        [ApiKeyAuth]
        public async Task<ActionResult<ResponseDto<List<PrecioResponseDto>>>> CreatePreciosMasivos(
            [FromBody] PrecioMasivoRequest request)
        {
            var result = await _precioService.CreatePreciosMasivosAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Actualizar precios por tarifa con porcentaje de incremento
        /// </summary>
        [HttpPut("tarifa/{idTarifa}/incrementar")]
        [ApiKeyAuth]
        public async Task<ActionResult<ResponseDto<List<PrecioResponseDto>>>> ActualizarPreciosPorTarifa(
            int idTarifa,
            [FromQuery] decimal porcentajeIncremento)
        {
            var result = await _precioService.ActualizarPreciosPorTarifaAsync(idTarifa, porcentajeIncremento);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Obtener todos los precios
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ResponseDto<List<PrecioResponseDto>>>> GetPrecios(
            [FromQuery] bool incluirTarifa = false,
            [FromQuery] bool incluirArticulo = false)
        {
            var result = await _precioService.GetPreciosAsync(incluirTarifa, incluirArticulo);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Obtener precios por tarifa
        /// </summary>
        [HttpGet("tarifa/{idTarifa}")]
        public async Task<ActionResult<ResponseDto<List<PrecioResponseDto>>>> GetPreciosByTarifa(
            int idTarifa,
            [FromQuery] bool incluirArticulo = false)
        {
            var result = await _precioService.GetPreciosByTarifaAsync(idTarifa, incluirArticulo);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Obtener precios por artículo
        /// </summary>
        [HttpGet("articulo/{codigoArticulo}")]
        public async Task<ActionResult<ResponseDto<List<PrecioResponseDto>>>> GetPreciosByArticulo(
            int codigoArticulo,
            [FromQuery] bool incluirTarifa = false)
        {
            var result = await _precioService.GetPreciosByArticuloAsync(codigoArticulo, incluirTarifa);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Búsqueda avanzada de precios
        /// </summary>
        [HttpPost("search")]
        public async Task<ActionResult<ResponseDto<List<PrecioResponseDto>>>> SearchPrecios(
            [FromBody] PrecioSearchRequest request)
        {
            var result = await _precioService.SearchPreciosAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Análisis de precios
        /// </summary>
        [HttpGet("analisis")]
        public async Task<ActionResult<ResponseDto<object>>> GetAnalisisPrecios(
            [FromQuery] int? idTarifa = null,
            [FromQuery] int? codigoArticulo = null)
        {
            var result = await _precioService.GetAnalisisPreciosAsync(idTarifa, codigoArticulo);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Comparativa de tarifas para un artículo
        /// </summary>
        [HttpGet("comparativa/{codigoArticulo}")]
        public async Task<ActionResult<ResponseDto<object>>> GetComparativaTarifas(int codigoArticulo)
        {
            var result = await _precioService.GetComparativaTarifasAsync(codigoArticulo);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Estadísticas generales de precios
        /// </summary>
        [HttpGet("estadisticas")]
        public async Task<ActionResult<ResponseDto<object>>> GetEstadisticas()
        {
            var result = await _precioService.GetEstadisticasPreciosAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Validar consistencia de precio
        /// </summary>
        [HttpPost("validar")]
        public async Task<ActionResult<ResponseDto<bool>>> ValidarConsistencia(
            [FromBody] PrecioDto precio)
        {
            var result = await _precioService.ValidarConsistenciaPrecioAsync(precio);
            return Ok(result);
        }
    }
}
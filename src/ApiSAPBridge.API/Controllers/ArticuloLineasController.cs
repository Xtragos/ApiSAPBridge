using ApiSAPBridge.API.Attributes;
using ApiSAPBridge.Core.Interfaces;
using ApiSAPBridge.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ApiSAPBridge.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticuloLineasController : ControllerBase
    {
        private readonly IArticuloLineaService _articuloLineaService;
        private readonly ILogger<ArticuloLineasController> _logger;

        public ArticuloLineasController(
            IArticuloLineaService articuloLineaService,
            ILogger<ArticuloLineasController> logger)
        {
            _articuloLineaService = articuloLineaService;
            _logger = logger;
        }

        /// <summary>
        /// Crear/actualizar líneas de artículos (endpoint para SAP)
        /// </summary>
        [HttpPost]
        [ApiKeyAuth]
        public async Task<ActionResult<ResponseDto<List<ArticuloLineaResponseDto>>>> CreateArticuloLineas(
            [FromBody] List<ArticuloLineaDto> lineas)
        {
            var result = await _articuloLineaService.CreateArticuloLineasAsync(lineas);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Obtener todas las líneas de artículos
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ResponseDto<List<ArticuloLineaResponseDto>>>> GetArticuloLineas(
            [FromQuery] bool incluirPrecios = false)
        {
            var result = await _articuloLineaService.GetArticuloLineasAsync(incluirPrecios);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Obtener línea específica
        /// </summary>
        [HttpGet("{codigoArticulo}/{talla}/{color}")]
        public async Task<ActionResult<ResponseDto<ArticuloLineaResponseDto?>>> GetArticuloLinea(
            int codigoArticulo, string talla, string color)
        {
            var result = await _articuloLineaService.GetArticuloLineaAsync(codigoArticulo, talla, color);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Obtener líneas por artículo
        /// </summary>
        [HttpGet("articulo/{codigoArticulo}")]
        public async Task<ActionResult<ResponseDto<List<ArticuloLineaResponseDto>>>> GetLineasByArticulo(
            int codigoArticulo,
            [FromQuery] bool incluirPrecios = false)
        {
            var result = await _articuloLineaService.GetLineasByArticuloAsync(codigoArticulo, incluirPrecios);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Búsqueda avanzada de líneas
        /// </summary>
        [HttpPost("search")]
        public async Task<ActionResult<ResponseDto<List<ArticuloLineaResponseDto>>>> SearchArticuloLineas(
            [FromBody] ArticuloLineaSearchRequest request)
        {
            var result = await _articuloLineaService.SearchArticuloLineasAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Buscar línea por código de barras
        /// </summary>
        [HttpGet("codigo-barras/{codigoBarras}")]
        public async Task<ActionResult<ResponseDto<ArticuloLineaResponseDto?>>> GetLineaByCodigoBarras(string codigoBarras)
        {
            var result = await _articuloLineaService.GetLineaByCodigoBarrasAsync(codigoBarras);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Validar unicidad de código de barras
        /// </summary>
        [HttpGet("validar-codigo-barras/{codigoBarras}")]
        public async Task<ActionResult<ResponseDto<bool>>> ValidarCodigoBarrasUnico(
            string codigoBarras,
            [FromQuery] int? excludeArticulo = null,
            [FromQuery] string? excludeTalla = null,
            [FromQuery] string? excludeColor = null)
        {
            var result = await _articuloLineaService.ValidarCodigoBarrasUnicoAsync(
                codigoBarras, excludeArticulo, excludeTalla, excludeColor);
            return Ok(result);
        }

        /// <summary>
        /// Obtener estadísticas de líneas
        /// </summary>
        [HttpGet("estadisticas")]
        public async Task<ActionResult<ResponseDto<object>>> GetEstadisticas()
        {
            var result = await _articuloLineaService.GetEstadisticasLineasAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
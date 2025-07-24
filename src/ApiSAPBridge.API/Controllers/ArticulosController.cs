using ApiSAPBridge.API.Attributes;
using ApiSAPBridge.Core.Interfaces;
using ApiSAPBridge.Models.DTOs;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace ApiSAPBridge.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticulosController : ControllerBase
    {
        private readonly IArticuloService _articuloService;
        private readonly ILogger<ArticulosController> _logger;

        public ArticulosController(IArticuloService articuloService, ILogger<ArticulosController> logger)
        {
            _articuloService = articuloService;
            _logger = logger;
        }

        /// <summary>
        /// Crear/actualizar artículos básicos (endpoint para SAP)
        /// </summary>
        [HttpPost]
        [ApiKeyAuth]
        public async Task<ActionResult<ResponseDto<List<ArticuloResponseDto>>>> CreateArticulos(
            [FromBody] List<ArticuloDto> articulos)
        {
            var result = await _articuloService.CreateArticulosAsync(articulos);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Crear artículos completos con líneas y precios
        /// </summary>
        [HttpPost("completos")]
        [ApiKeyAuth]
        public async Task<ActionResult<ResponseDto<List<ArticuloResponseDto>>>> CreateArticulosCompletos(
            [FromBody] List<ArticuloCompletoRequest> articulosCompletos)
        {
            var result = await _articuloService.CreateArticulosCompletosAsync(articulosCompletos);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Obtener todos los artículos
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ResponseDto<List<ArticuloResponseDto>>>> GetArticulos(
            [FromQuery] bool incluirLineas = false,
            [FromQuery] bool incluirPrecios = false)
        {
            var result = await _articuloService.GetArticulosAsync(incluirLineas, incluirPrecios);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Obtener artículo específico
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto<ArticuloResponseDto?>>> GetArticulo(
            int id,
            [FromQuery] bool incluirLineas = false,
            [FromQuery] bool incluirPrecios = false)
        {
            var result = await _articuloService.GetArticuloByIdAsync(id, incluirLineas, incluirPrecios);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Obtener artículo completo con toda la información
        /// </summary>
        [HttpGet("{id}/completo")]
        public async Task<ActionResult<ResponseDto<ArticuloResponseDto?>>> GetArticuloCompleto(int id)
        {
            var result = await _articuloService.GetArticuloCompletoAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Búsqueda avanzada de artículos
        /// </summary>
        [HttpPost("search")]
        public async Task<ActionResult<ResponseDto<List<ArticuloResponseDto>>>> SearchArticulos(
            [FromBody] ArticuloSearchRequest request)
        {
            var result = await _articuloService.SearchArticulosAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Obtener artículos por departamento
        /// </summary>
        [HttpGet("departamento/{numDpto}")]
        public async Task<ActionResult<ResponseDto<List<ArticuloResponseDto>>>> GetArticulosByDepartamento(
            int numDpto,
            [FromQuery] bool incluirLineas = false)
        {
            var result = await _articuloService.GetArticulosByDepartamentoAsync(numDpto, incluirLineas);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Obtener artículos por tipo de impuesto
        /// </summary>
        [HttpGet("impuesto/{tipoImpuesto}")]
        public async Task<ActionResult<ResponseDto<List<ArticuloResponseDto>>>> GetArticulosByImpuesto(int tipoImpuesto)
        {
            var result = await _articuloService.GetArticulosByImpuestoAsync(tipoImpuesto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Buscar artículos por código de barras
        /// </summary>
        [HttpGet("codigo-barras/{codigoBarras}")]
        public async Task<ActionResult<ResponseDto<List<ArticuloResponseDto>>>> GetArticulosByCodigoBarras(string codigoBarras)
        {
            var result = await _articuloService.GetArticulosByCodBarrasAsync(codigoBarras);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Validar integridad de artículo
        /// </summary>
        [HttpGet("{id}/validar")]
        public async Task<ActionResult<ResponseDto<bool>>> ValidarIntegridad(int id)
        {
            var result = await _articuloService.ValidarIntegridadArticuloAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Obtener estadísticas de artículos
        /// </summary>
        [HttpGet("estadisticas")]
        public async Task<ActionResult<ResponseDto<object>>> GetEstadisticas()
        {
            var result = await _articuloService.GetEstadisticasArticulosAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Obtener líneas de un artículo específico
        /// </summary>
        [HttpGet("{id}/lineas")]
        public async Task<ActionResult<ResponseDto<List<ArticuloLineaResponseDto>>>> GetLineasByArticulo(int id)
        {
            var result = await _articuloService.GetLineasByArticuloAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        /// <summary>
        /// Crear artículos completos con líneas integradas (formato SAP específico)
        /// </summary>
        [HttpPost("completo")]
        [ApiKeyAuth]
        public async Task<ActionResult<ResponseDto<List<ArticuloResponseDto>>>> CreateArticulosCompletoSAP(
            [FromBody] List<ArticuloCompletoSAPRequest> articulosCompletos)
        {
            try
            {
                var articulosParaProcesar = new List<ArticuloCompletoRequest>();

                foreach (var articuloSAP in articulosCompletos)
                {
                    var articuloCompleto = new ArticuloCompletoRequest
                    {
                        Articulo = articuloSAP.Adapt<ArticuloDto>(),
                        Lineas = articuloSAP.Lineas ?? new List<ArticuloLineaDto>(),
                        Precios = new List<PrecioDto>(), // Se pueden agregar después
                        ValidarIntegridad = true,
                        CrearLineasAutomaticas = false
                    };

                    articulosParaProcesar.Add(articuloCompleto);
                }

                var result = await _articuloService.CreateArticulosCompletosAsync(articulosParaProcesar);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar artículos completos SAP");
                return BadRequest(ResponseDto<List<ArticuloResponseDto>>.CreateError(
                    "Error al procesar artículos completos", ex.Message));
            }
        }
    }
}
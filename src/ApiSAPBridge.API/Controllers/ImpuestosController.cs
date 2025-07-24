using ApiSAPBridge.API.Attributes;
using ApiSAPBridge.Core.Interfaces;
using ApiSAPBridge.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ApiSAPBridge.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImpuestosController : ControllerBase
    {
        private readonly IImpuestoService _impuestoService;
        private readonly ILogger<ImpuestosController> _logger;

        public ImpuestosController(
            IImpuestoService impuestoService,
            ILogger<ImpuestosController> logger)
        {
            _impuestoService = impuestoService;
            _logger = logger;
        }

        /// <summary>
        /// Crear/actualizar impuestos (endpoint para SAP)
        /// </summary>
        [HttpPost]
        [ApiKeyAuth]
        [ProducesResponseType(typeof(ResponseDto<List<ImpuestoResponseDto>>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<ResponseDto<List<ImpuestoResponseDto>>>> CreateImpuestos(
            [FromBody] List<ImpuestoDto> impuestos)
        {
            _logger.LogInformation("POST /api/impuestos - Recibidos {Count} impuestos", impuestos.Count);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _impuestoService.CreateImpuestosAsync(impuestos);

            if (!result.Success)
            {
                _logger.LogWarning("Error al crear impuestos: {Message}", result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("Impuestos procesados exitosamente: {Count}", result.Data?.Count ?? 0);
            return Ok(result);
        }

        /// <summary>
        /// Obtener todos los impuestos
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseDto<List<ImpuestoResponseDto>>), 200)]
        public async Task<ActionResult<ResponseDto<List<ImpuestoResponseDto>>>> GetImpuestos()
        {
            _logger.LogInformation("GET /api/impuestos");

            var result = await _impuestoService.GetImpuestosAsync();

            if (!result.Success)
            {
                _logger.LogWarning("Error al obtener impuestos: {Message}", result.Message);
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Obtener impuesto por ID
        /// </summary>
        [HttpGet("{tipoIva}")]
        [ProducesResponseType(typeof(ResponseDto<ImpuestoResponseDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseDto<ImpuestoResponseDto?>>> GetImpuesto(int tipoIva)
        {
            _logger.LogInformation("GET /api/impuestos/{TipoIva}", tipoIva);

            var result = await _impuestoService.GetImpuestoByIdAsync(tipoIva);

            if (!result.Success)
            {
                if (result.Message?.Contains("not found") == true)
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Eliminar impuesto (valida que no tenga artículos asociados)
        /// </summary>
        [HttpDelete("{tipoIva}")]
        [ApiKeyAuth]
        [ProducesResponseType(typeof(ResponseDto<bool>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<ResponseDto<bool>>> DeleteImpuesto(int tipoIva)
        {
            _logger.LogInformation("DELETE /api/impuestos/{TipoIva}", tipoIva);

            var result = await _impuestoService.DeleteImpuestoAsync(tipoIva);

            if (!result.Success)
            {
                if (result.Message?.Contains("not found") == true)
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Obtener estadísticas de impuestos
        /// </summary>
        [HttpGet("stats")]
        [ProducesResponseType(typeof(ResponseDto<ImpuestoStatsDto>), 200)]
        public async Task<ActionResult<ResponseDto<ImpuestoStatsDto>>> GetImpuestosStats()
        {
            _logger.LogInformation("GET /api/impuestos/stats");

            var result = await _impuestoService.GetImpuestosStatsAsync();

            if (!result.Success)
            {
                _logger.LogWarning("Error al obtener estadísticas de impuestos: {Message}", result.Message);
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
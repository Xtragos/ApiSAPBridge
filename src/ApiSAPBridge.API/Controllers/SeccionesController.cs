using ApiSAPBridge.API.Attributes;
using ApiSAPBridge.Core.Interfaces;
using ApiSAPBridge.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ApiSAPBridge.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeccionesController : ControllerBase
    {
        private readonly ISeccionService _seccionService;
        private readonly ILogger<SeccionesController> _logger;

        public SeccionesController(
            ISeccionService seccionService,
            ILogger<SeccionesController> logger)
        {
            _seccionService = seccionService;
            _logger = logger;
        }

        /// <summary>
        /// Crear/actualizar secciones (endpoint para SAP)
        /// </summary>
        [HttpPost]
        [ApiKeyAuth]
        [ProducesResponseType(typeof(ResponseDto<List<SeccionResponseDto>>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<ResponseDto<List<SeccionResponseDto>>>> CreateSecciones(
            [FromBody] List<SeccionDto> secciones)
        {
            _logger.LogInformation("POST /api/secciones - Recibidas {Count} secciones", secciones.Count);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _seccionService.CreateSeccionesAsync(secciones);

            if (!result.Success)
            {
                _logger.LogWarning("Error al crear secciones: {Message}", result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("Secciones procesadas exitosamente: {Count}", result.Data?.Count ?? 0);
            return Ok(result);
        }

        /// <summary>
        /// Obtener todas las secciones
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseDto<List<SeccionResponseDto>>), 200)]
        public async Task<ActionResult<ResponseDto<List<SeccionResponseDto>>>> GetSecciones()
        {
            _logger.LogInformation("GET /api/secciones");

            var result = await _seccionService.GetSeccionesAsync();

            if (!result.Success)
            {
                _logger.LogWarning("Error al obtener secciones: {Message}", result.Message);
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Obtener secciones por departamento
        /// </summary>
        [HttpGet("departamento/{numDpto}")]
        [ProducesResponseType(typeof(ResponseDto<List<SeccionResponseDto>>), 200)]
        public async Task<ActionResult<ResponseDto<List<SeccionResponseDto>>>> GetSeccionesByDepartamento(int numDpto)
        {
            _logger.LogInformation("GET /api/secciones/departamento/{NumDpto}", numDpto);

            var result = await _seccionService.GetSeccionesByDepartamentoAsync(numDpto);

            if (!result.Success)
            {
                _logger.LogWarning("Error al obtener secciones del departamento {NumDpto}: {Message}", numDpto, result.Message);
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Obtener sección específica
        /// </summary>
        [HttpGet("{numDpto}/{numSeccion}")]
        [ProducesResponseType(typeof(ResponseDto<SeccionResponseDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseDto<SeccionResponseDto?>>> GetSeccion(int numDpto, int numSeccion)
        {
            _logger.LogInformation("GET /api/secciones/{NumDpto}/{NumSeccion}", numDpto, numSeccion);

            var result = await _seccionService.GetSeccionAsync(numDpto, numSeccion);

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
    }
}
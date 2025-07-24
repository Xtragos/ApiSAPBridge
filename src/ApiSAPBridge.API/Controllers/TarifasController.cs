using ApiSAPBridge.API.Attributes;
using ApiSAPBridge.Core.Interfaces;
using ApiSAPBridge.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ApiSAPBridge.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TarifasController : ControllerBase
    {
        private readonly ITarifaService _tarifaService;
        private readonly ILogger<TarifasController> _logger;

        public TarifasController(ITarifaService tarifaService, ILogger<TarifasController> logger)
        {
            _tarifaService = tarifaService;
            _logger = logger;
        }

        [HttpPost]
        [ApiKeyAuth]
        public async Task<ActionResult<ResponseDto<List<TarifaResponseDto>>>> CreateTarifas(
            [FromBody] List<TarifaDto> tarifas)
        {
            var result = await _tarifaService.CreateTarifasAsync(tarifas);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto<List<TarifaResponseDto>>>> GetTarifas(
            [FromQuery] bool incluirConteoPrecios = false)
        {
            var result = await _tarifaService.GetTarifasAsync(incluirConteoPrecios);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("activas")]
        public async Task<ActionResult<ResponseDto<List<TarifaResponseDto>>>> GetTarifasActivas(
            [FromQuery] DateTime? fecha = null)
        {
            var result = await _tarifaService.GetTarifasActivasAsync(fecha);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("vigentes")]
        public async Task<ActionResult<ResponseDto<List<TarifaResponseDto>>>> GetTarifasVigentes()
        {
            var result = await _tarifaService.GetTarifasVigentesAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("validar-solapamiento/{id}")]
        public async Task<ActionResult<ResponseDto<bool>>> ValidarSolapamiento(
            int id, [FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            var result = await _tarifaService.ValidarSolapamientoAsync(id, fechaInicio, fechaFin);
            return Ok(result);
        }

        /// <summary>
        /// Obtener tarifa específica por ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseDto<TarifaResponseDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseDto<TarifaResponseDto?>>> GetTarifa(
            int id,
            [FromQuery] bool incluirConteoPrecios = false)
        {
            _logger.LogInformation("GET /api/tarifas/{Id}", id);

            var result = await _tarifaService.GetTarifaByIdAsync(id, incluirConteoPrecios);

            if (!result.Success)
            {
                if (result.Message?.Contains("not found") == true ||
                    result.Message?.Contains("no encontrada") == true)
                {
                    _logger.LogInformation("Tarifa {Id} no encontrada", id);
                    return NotFound(result);
                }

                _logger.LogWarning("Error al obtener tarifa {Id}: {Message}", id, result.Message);
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Buscar tarifas con filtros avanzados
        /// </summary>
        [HttpPost("search")]
        [ProducesResponseType(typeof(ResponseDto<List<TarifaResponseDto>>), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseDto<List<TarifaResponseDto>>>> SearchTarifas(
            [FromBody] TarifaSearchRequest request)
        {
            _logger.LogInformation("POST /api/tarifas/search - Búsqueda con filtros");

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToList();

                _logger.LogWarning("Errores de validación en búsqueda: {Errors}", string.Join(", ", errors));
                return BadRequest(ModelState);
            }

            var result = await _tarifaService.SearchTarifasAsync(request);

            if (!result.Success)
            {
                _logger.LogWarning("Error en búsqueda de tarifas: {Message}", result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("Búsqueda de tarifas completada: {Count} resultados", result.Data?.Count ?? 0);
            return Ok(result);
        }

        /// <summary>
        /// Obtener estadísticas completas de tarifas
        /// </summary>
        [HttpGet("estadisticas")]
        [ProducesResponseType(typeof(ResponseDto<object>), 200)]
        public async Task<ActionResult<ResponseDto<object>>> GetEstadisticas()
        {
            _logger.LogInformation("GET /api/tarifas/estadisticas");

            var result = await _tarifaService.GetEstadisticasTarifasAsync();

            if (!result.Success)
            {
                _logger.LogWarning("Error al obtener estadísticas de tarifas: {Message}", result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("Estadísticas de tarifas generadas exitosamente");
            return Ok(result);
        }
    }
}
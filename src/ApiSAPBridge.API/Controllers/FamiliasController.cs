using ApiSAPBridge.API.Attributes;
using ApiSAPBridge.Core.Interfaces;
using ApiSAPBridge.Models.Constants;
using ApiSAPBridge.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ApiSAPBridge.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FamiliasController : ControllerBase
    {
        private readonly IFamiliaService _familiaService;
        private readonly ILogger<FamiliasController> _logger;

        public FamiliasController(
            IFamiliaService familiaService,
            ILogger<FamiliasController> logger)
        {
            _familiaService = familiaService;
            _logger = logger;
        }

        /// <summary>
        /// Crear/actualizar familias (endpoint para SAP)
        /// </summary>
        [HttpPost]
        [ApiKeyAuth]
        [ProducesResponseType(typeof(ResponseDto<List<FamiliaResponseDto>>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<ResponseDto<List<FamiliaResponseDto>>>> CreateFamilias(
            [FromBody] List<FamiliaDto> familias)
        {
            _logger.LogInformation("POST /api/familias - Recibidas {Count} familias", familias.Count);

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToList();

                _logger.LogWarning("Errores de validación: {Errors}", string.Join(", ", errors));
                return BadRequest(ModelState);
            }

            var result = await _familiaService.CreateFamiliasAsync(familias);

            if (!result.Success)
            {
                _logger.LogWarning("Error al crear familias: {Message}", result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("Familias procesadas exitosamente: {Count}", result.Data?.Count ?? 0);
            return Ok(result);
        }

        /// <summary>
        /// Obtener todas las familias con información completa
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseDto<List<FamiliaResponseDto>>), 200)]
        public async Task<ActionResult<ResponseDto<List<FamiliaResponseDto>>>> GetFamilias()
        {
            _logger.LogInformation("GET /api/familias");

            var result = await _familiaService.GetFamiliasAsync();

            if (!result.Success)
            {
                _logger.LogWarning("Error al obtener familias: {Message}", result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("Obtenidas {Count} familias", result.Data?.Count ?? 0);
            return Ok(result);
        }

        /// <summary>
        /// Obtener familias por sección específica
        /// </summary>
        [HttpGet("seccion/{numDpto}/{numSeccion}")]
        [ProducesResponseType(typeof(ResponseDto<List<FamiliaResponseDto>>), 200)]
        public async Task<ActionResult<ResponseDto<List<FamiliaResponseDto>>>> GetFamiliasBySeccion(
            int numDpto, int numSeccion)
        {
            _logger.LogInformation("GET /api/familias/seccion/{NumDpto}/{NumSeccion}", numDpto, numSeccion);

            var result = await _familiaService.GetFamiliasBySeccionAsync(numDpto, numSeccion);

            if (!result.Success)
            {
                _logger.LogWarning("Error al obtener familias de la sección {NumDpto}-{NumSeccion}: {Message}",
                    numDpto, numSeccion, result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("Obtenidas {Count} familias para sección {NumDpto}-{NumSeccion}",
                result.Data?.Count ?? 0, numDpto, numSeccion);
            return Ok(result);
        }

        /// <summary>
        /// Obtener familia específica por su clave compuesta
        /// </summary>
        [HttpGet("{numDpto}/{numSeccion}/{numFamilia}")]
        [ProducesResponseType(typeof(ResponseDto<FamiliaResponseDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseDto<FamiliaResponseDto?>>> GetFamilia(
            int numDpto, int numSeccion, int numFamilia)
        {
            _logger.LogInformation("GET /api/familias/{NumDpto}/{NumSeccion}/{NumFamilia}",
                numDpto, numSeccion, numFamilia);

            var result = await _familiaService.GetFamiliaAsync(numDpto, numSeccion, numFamilia);

            if (!result.Success)
            {
                if (result.Message?.Contains("not found") == true ||
                    result.Message?.Contains("no encontrada") == true)
                {
                    _logger.LogInformation("Familia {NumDpto}-{NumSeccion}-{NumFamilia} no encontrada",
                        numDpto, numSeccion, numFamilia);
                    return NotFound(result);
                }

                _logger.LogWarning("Error al obtener familia {NumDpto}-{NumSeccion}-{NumFamilia}: {Message}",
                    numDpto, numSeccion, numFamilia, result.Message);
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Obtener estadísticas de familias por departamento
        /// </summary>
        [HttpGet("stats/departamento/{numDpto}")]
        [ProducesResponseType(typeof(ResponseDto<object>), 200)]
        public async Task<ActionResult<ResponseDto<object>>> GetFamiliaStatsByDepartamento(int numDpto)
        {
            _logger.LogInformation("GET /api/familias/stats/departamento/{NumDpto}", numDpto);

            try
            {
                // Obtener todas las secciones del departamento
                var seccionesResult = await GetSeccionesByDepartamento(numDpto);

                if (!seccionesResult.Success)
                {
                    return BadRequest(seccionesResult);
                }

                var stats = new
                {
                    DepartamentoId = numDpto,
                    TotalSecciones = seccionesResult.Data?.Count ?? 0,
                    SeccionesConFamilias = 0,
                    TotalFamilias = 0,
                    PromedioFamiliasPorSeccion = 0.0,
                    Timestamp = DateTime.UtcNow
                };

                // Aquí podrías agregar más lógica para calcular estadísticas reales
                // Por ahora retornamos estructura básica

                return Ok(ResponseDto<object>.CreateSuccess(stats, "Estadísticas obtenidas exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas del departamento {NumDpto}", numDpto);
                return BadRequest(ResponseDto<object>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message));
            }
        }

        /// <summary>
        /// Método auxiliar para obtener secciones (podrías inyectar ISeccionService si prefieres)
        /// </summary>
        private async Task<ResponseDto<List<SeccionResponseDto>>> GetSeccionesByDepartamento(int numDpto)
        {
            // Implementación simplificada - en producción podrías inyectar ISeccionService
            return ResponseDto<List<SeccionResponseDto>>.CreateSuccess(
                new List<SeccionResponseDto>(),
                "Método auxiliar");
        }
    }
}
using ApiSAPBridge.API.Attributes;
using ApiSAPBridge.Core.Interfaces;
using ApiSAPBridge.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ApiSAPBridge.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartamentosController : ControllerBase
    {
        private readonly IDepartamentoService _departamentoService;
        private readonly ILogger<DepartamentosController> _logger;

        public DepartamentosController(
            IDepartamentoService departamentoService,
            ILogger<DepartamentosController> logger)
        {
            _departamentoService = departamentoService;
            _logger = logger;
        }

        /// <summary>
        /// Crear departamentos (endpoint para SAP)
        /// </summary>
        [HttpPost]
        [ApiKeyAuth]
        [ProducesResponseType(typeof(ResponseDto<List<DepartamentoResponseDto>>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<ResponseDto<List<DepartamentoResponseDto>>>> CreateDepartamentos(
            [FromBody] List<DepartamentoDto> departamentos)
        {
            _logger.LogInformation("POST /api/departamentos - Recibidos {Count} departamentos", departamentos.Count);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _departamentoService.CreateDepartamentosAsync(departamentos);

            if (!result.Success)
            {
                _logger.LogWarning("Error al crear departamentos: {Message}", result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("Departamentos creados exitosamente: {Count}", result.Data?.Count ?? 0);
            return Ok(result);
        }

        /// <summary>
        /// Obtener todos los departamentos (endpoint público)
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseDto<List<DepartamentoResponseDto>>), 200)]
        public async Task<ActionResult<ResponseDto<List<DepartamentoResponseDto>>>> GetDepartamentos()
        {
            _logger.LogInformation("GET /api/departamentos");

            var result = await _departamentoService.GetDepartamentosAsync();

            if (!result.Success)
            {
                _logger.LogWarning("Error al obtener departamentos: {Message}", result.Message);
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Obtener departamento por ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseDto<DepartamentoResponseDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseDto<DepartamentoResponseDto?>>> GetDepartamento(int id)
        {
            _logger.LogInformation("GET /api/departamentos/{Id}", id);

            var result = await _departamentoService.GetDepartamentoByIdAsync(id);

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
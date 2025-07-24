using ApiSAPBridge.API.Attributes;
using ApiSAPBridge.Core.Interfaces;
using ApiSAPBridge.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ApiSAPBridge.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormasPagoController : ControllerBase
    {
        private readonly IFormaPagoService _formaPagoService;
        private readonly ILogger<FormasPagoController> _logger;

        public FormasPagoController(
            IFormaPagoService formaPagoService,
            ILogger<FormasPagoController> logger)
        {
            _formaPagoService = formaPagoService;
            _logger = logger;
        }

        /// <summary>
        /// Crear/actualizar formas de pago (endpoint para SAP)
        /// </summary>
        [HttpPost]
        [ApiKeyAuth]
        [ProducesResponseType(typeof(ResponseDto<List<FormaPagoResponseDto>>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<ResponseDto<List<FormaPagoResponseDto>>>> CreateFormasPago(
            [FromBody] List<FormaPagoDto> formasPago)
        {
            _logger.LogInformation("POST /api/formaspago - Recibidas {Count} formas de pago", formasPago.Count);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _formaPagoService.CreateFormasPagoAsync(formasPago);

            if (!result.Success)
            {
                _logger.LogWarning("Error al crear formas de pago: {Message}", result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("Formas de pago procesadas exitosamente: {Count}", result.Data?.Count ?? 0);
            return Ok(result);
        }

        /// <summary>
        /// Obtener todas las formas de pago
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseDto<List<FormaPagoResponseDto>>), 200)]
        public async Task<ActionResult<ResponseDto<List<FormaPagoResponseDto>>>> GetFormasPago()
        {
            _logger.LogInformation("GET /api/formaspago");

            var result = await _formaPagoService.GetFormasPagoAsync();

            if (!result.Success)
            {
                _logger.LogWarning("Error al obtener formas de pago: {Message}", result.Message);
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Obtener formas de pago por tipo
        /// </summary>
        [HttpGet("tipo/{tipo}")]
        [ProducesResponseType(typeof(ResponseDto<List<FormaPagoResponseDto>>), 200)]
        public async Task<ActionResult<ResponseDto<List<FormaPagoResponseDto>>>> GetFormasPagoByTipo(string tipo)
        {
            _logger.LogInformation("GET /api/formaspago/tipo/{Tipo}", tipo);

            var result = await _formaPagoService.GetFormasPagoByTipoAsync(tipo);

            if (!result.Success)
            {
                _logger.LogWarning("Error al obtener formas de pago por tipo {Tipo}: {Message}", tipo, result.Message);
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Obtener forma de pago por código
        /// </summary>
        [HttpGet("{codigo}")]
        [ProducesResponseType(typeof(ResponseDto<FormaPagoResponseDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseDto<FormaPagoResponseDto?>>> GetFormaPago(int codigo)
        {
            _logger.LogInformation("GET /api/formaspago/{Codigo}", codigo);

            var result = await _formaPagoService.GetFormaPagoByIdAsync(codigo);

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
        /// Eliminar forma de pago (valida que no tenga clientes asociados)
        /// </summary>
        [HttpDelete("{codigo}")]
        [ApiKeyAuth]
        [ProducesResponseType(typeof(ResponseDto<bool>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<ResponseDto<bool>>> DeleteFormaPago(int codigo)
        {
            _logger.LogInformation("DELETE /api/formaspago/{Codigo}", codigo);

            var result = await _formaPagoService.DeleteFormaPagoAsync(codigo);

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
        /// Obtener estadísticas de formas de pago
        /// </summary>
        [HttpGet("stats")]
        [ProducesResponseType(typeof(ResponseDto<FormaPagoStatsDto>), 200)]
        public async Task<ActionResult<ResponseDto<FormaPagoStatsDto>>> GetFormasPagoStats()
        {
            _logger.LogInformation("GET /api/formaspago/stats");

            var result = await _formaPagoService.GetFormasPagoStatsAsync();

            if (!result.Success)
            {
                _logger.LogWarning("Error al obtener estadísticas de formas de pago: {Message}", result.Message);
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
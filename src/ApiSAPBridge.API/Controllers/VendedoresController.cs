using ApiSAPBridge.API.Attributes;
using ApiSAPBridge.Core.Interfaces;
using ApiSAPBridge.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ApiSAPBridge.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendedoresController : ControllerBase
    {
        private readonly IVendedorService _vendedorService;
        private readonly ILogger<VendedoresController> _logger;

        public VendedoresController(
            IVendedorService vendedorService,
            ILogger<VendedoresController> logger)
        {
            _vendedorService = vendedorService;
            _logger = logger;
        }

        /// <summary>
        /// Crear/actualizar vendedores (endpoint para SAP)
        /// </summary>
        [HttpPost]
        [ApiKeyAuth]
        [ProducesResponseType(typeof(ResponseDto<List<VendedorResponseDto>>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<ResponseDto<List<VendedorResponseDto>>>> CreateVendedores(
            [FromBody] List<VendedorDto> vendedores)
        {
            _logger.LogInformation("POST /api/vendedores - Recibidos {Count} vendedores", vendedores.Count);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _vendedorService.CreateVendedoresAsync(vendedores);

            if (!result.Success)
            {
                _logger.LogWarning("Error al crear vendedores: {Message}", result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("Vendedores procesados exitosamente: {Count}", result.Data?.Count ?? 0);
            return Ok(result);
        }

        /// <summary>
        /// Obtener todos los vendedores
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseDto<List<VendedorResponseDto>>), 200)]
        public async Task<ActionResult<ResponseDto<List<VendedorResponseDto>>>> GetVendedores()
        {
            _logger.LogInformation("GET /api/vendedores");

            var result = await _vendedorService.GetVendedoresAsync();

            if (!result.Success)
            {
                _logger.LogWarning("Error al obtener vendedores: {Message}", result.Message);
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Obtener vendedor por ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseDto<VendedorResponseDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ResponseDto<VendedorResponseDto?>>> GetVendedor(int id)
        {
            _logger.LogInformation("GET /api/vendedores/{Id}", id);

            var result = await _vendedorService.GetVendedorByIdAsync(id);

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
        /// Eliminar vendedor
        /// </summary>
        [HttpDelete("{id}")]
        [ApiKeyAuth]
        [ProducesResponseType(typeof(ResponseDto<bool>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<ResponseDto<bool>>> DeleteVendedor(int id)
        {
            _logger.LogInformation("DELETE /api/vendedores/{Id}", id);

            var result = await _vendedorService.DeleteVendedorAsync(id);

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
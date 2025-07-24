using ApiSAPBridge.API.Attributes;
using ApiSAPBridge.Core.Interfaces;
using ApiSAPBridge.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ApiSAPBridge.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _clienteService;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(IClienteService clienteService, ILogger<ClientesController> logger)
        {
            _clienteService = clienteService;
            _logger = logger;
        }

        [HttpPost]
        [ApiKeyAuth]
        public async Task<ActionResult<ResponseDto<List<ClienteResponseDto>>>> CreateClientes(
            [FromBody] List<ClienteDto> clientes)
        {
            var result = await _clienteService.CreateClientesAsync(clientes);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto<List<ClienteResponseDto>>>> GetClientes()
        {
            var result = await _clienteService.GetClientesAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto<ClienteResponseDto?>>> GetCliente(int id)
        {
            var result = await _clienteService.GetClienteByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost("search")]
        public async Task<ActionResult<ResponseDto<List<ClienteResponseDto>>>> SearchClientes(
            [FromBody] ClienteSearchRequest request)
        {
            var result = await _clienteService.SearchClientesAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("provincia/{provincia}")]
        public async Task<ActionResult<ResponseDto<List<ClienteResponseDto>>>> GetClientesByProvincia(string provincia)
        {
            var result = await _clienteService.GetClientesByProvinciaAsync(provincia);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("validar-cif/{cif}")]
        public async Task<ActionResult<ResponseDto<bool>>> ValidarCIF(string cif)
        {
            var result = await _clienteService.ValidarCIFAsync(cif);
            return Ok(result);
        }

        [HttpGet("estadisticas")]
        public async Task<ActionResult<ResponseDto<object>>> GetEstadisticas()
        {
            var result = await _clienteService.GetEstadisticasClientesAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
using ApiSAPBridge.Core.Interfaces;
using ApiSAPBridge.Data.UnitOfWork;
using ApiSAPBridge.Models;
using ApiSAPBridge.Models.DTOs;
using ApiSAPBridge.Models.Constants;
using Mapster;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace ApiSAPBridge.Services.Implementations
{
    public class ClienteService : IClienteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ClienteService> _logger;

        public ClienteService(IUnitOfWork unitOfWork, ILogger<ClienteService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ResponseDto<List<ClienteResponseDto>>> CreateClientesAsync(List<ClienteDto> clientes)
        {
            try
            {
                _logger.LogInformation("Iniciando creación de {Count} clientes", clientes.Count);

                var resultados = new List<ClienteResponseDto>();
                var errores = new List<string>();

                foreach (var clienteDto in clientes)
                {
                    // Validaciones de negocio adicionales
                    var validationResult = await ValidateClienteBusinessRules(clienteDto);
                    if (!validationResult.IsValid)
                    {
                        errores.AddRange(validationResult.Errors);
                        _logger.LogWarning("Cliente {Id} falló validaciones: {Errors}",
                            clienteDto.CODCLIENTE, string.Join(", ", validationResult.Errors));
                        continue;
                    }

                    var clientesRepo = _unitOfWork.Repository<Cliente>();
                    var existente = await clientesRepo.GetByIdAsync(clienteDto.CODCLIENTE);

                    if (existente != null)
                    {
                        // Actualizar existente
                        await UpdateExistingCliente(existente, clienteDto);
                        clientesRepo.Update(existente);

                        _logger.LogInformation("Cliente {Id} actualizado", existente.CODCLIENTE);

                        var responseExistente = await BuildClienteResponseDto(existente);
                        resultados.Add(responseExistente);
                    }
                    else
                    {
                        // Crear nuevo
                        var nuevoCliente = clienteDto.Adapt<Cliente>();
                        nuevoCliente.CreatedAt = DateTime.UtcNow;
                        nuevoCliente.UpdatedAt = DateTime.UtcNow;

                        await clientesRepo.AddAsync(nuevoCliente);
                        _logger.LogInformation("Cliente {Id} creado", nuevoCliente.CODCLIENTE);

                        var responseNuevo = await BuildClienteResponseDto(nuevoCliente);
                        resultados.Add(responseNuevo);
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                if (errores.Any() && !resultados.Any())
                {
                    return ResponseDto<List<ClienteResponseDto>>.CreateError(
                        "Todos los clientes fallaron las validaciones", errores);
                }

                var message = errores.Any()
                    ? $"Procesados {resultados.Count} clientes, {errores.Count} con errores"
                    : ApiConstants.ResponseMessages.SUCCESS_CREATE;

                return ResponseDto<List<ClienteResponseDto>>.CreateSuccess(resultados, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar clientes");
                return ResponseDto<List<ClienteResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<List<ClienteResponseDto>>> GetClientesAsync()
        {
            try
            {
                var clientes = await _unitOfWork.Repository<Cliente>().GetAllAsync();
                var response = new List<ClienteResponseDto>();

                foreach (var cliente in clientes)
                {
                    var clienteDto = await BuildClienteResponseDto(cliente);
                    response.Add(clienteDto);
                }

                _logger.LogInformation("Obtenidos {Count} clientes", response.Count);

                return ResponseDto<List<ClienteResponseDto>>.CreateSuccess(
                    response, ApiConstants.ResponseMessages.SUCCESS_RETRIEVE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener clientes");
                return ResponseDto<List<ClienteResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<ClienteResponseDto?>> GetClienteByIdAsync(int id)
        {
            try
            {
                var cliente = await _unitOfWork.Repository<Cliente>().GetByIdAsync(id);

                if (cliente == null)
                {
                    return ResponseDto<ClienteResponseDto?>.CreateError(
                        ApiConstants.ResponseMessages.ERROR_NOT_FOUND);
                }

                var response = await BuildClienteResponseDto(cliente);
                return ResponseDto<ClienteResponseDto?>.CreateSuccess(
                    response, ApiConstants.ResponseMessages.SUCCESS_RETRIEVE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cliente {Id}", id);
                return ResponseDto<ClienteResponseDto?>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<List<ClienteResponseDto>>> SearchClientesAsync(ClienteSearchRequest request)
        {
            try
            {
                var clientesRepo = _unitOfWork.Repository<Cliente>();
                var clientes = await clientesRepo.GetAllAsync();

                // Aplicar filtros
                var filteredClientes = clientes.AsQueryable();

                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var searchTerm = request.SearchTerm.ToLower();
                    filteredClientes = filteredClientes.Where(c =>
                        c.NOMBRECLIENTE.ToLower().Contains(searchTerm) ||
                        (c.NOMBRECOMERCIAL != null && c.NOMBRECOMERCIAL.ToLower().Contains(searchTerm)) ||
                        (c.CIF != null && c.CIF.ToLower().Contains(searchTerm)) ||
                        (c.E_MAIL != null && c.E_MAIL.ToLower().Contains(searchTerm)));
                }

                if (!string.IsNullOrWhiteSpace(request.Provincia))
                {
                    filteredClientes = filteredClientes.Where(c =>
                        c.PROVINCIA != null && c.PROVINCIA.Equals(request.Provincia, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrWhiteSpace(request.Pais))
                {
                    filteredClientes = filteredClientes.Where(c =>
                        c.PAIS != null && c.PAIS.Equals(request.Pais, StringComparison.OrdinalIgnoreCase));
                }

                if (request.RiesgoMinimo.HasValue)
                {
                    filteredClientes = filteredClientes.Where(c => c.RIESGOCONCEDIDO >= request.RiesgoMinimo.Value);
                }

                if (request.RiesgoMaximo.HasValue)
                {
                    filteredClientes = filteredClientes.Where(c => c.RIESGOCONCEDIDO <= request.RiesgoMaximo.Value);
                }

                // Paginación
                var totalCount = filteredClientes.Count();
                var pagedClientes = filteredClientes
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                var response = new List<ClienteResponseDto>();
                foreach (var cliente in pagedClientes)
                {
                    var clienteDto = await BuildClienteResponseDto(cliente);
                    response.Add(clienteDto);
                }

                _logger.LogInformation("Búsqueda de clientes: {Total} encontrados, {Returned} devueltos",
                    totalCount, response.Count);

                return ResponseDto<List<ClienteResponseDto>>.CreateSuccess(
                    response, $"Encontrados {totalCount} clientes, mostrando página {request.Page}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en búsqueda de clientes");
                return ResponseDto<List<ClienteResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<List<ClienteResponseDto>>> GetClientesByProvinciaAsync(string provincia)
        {
            try
            {
                var clientesRepo = _unitOfWork.Repository<Cliente>();
                var clientes = await clientesRepo.FindAsync(c =>
                    c.PROVINCIA != null && c.PROVINCIA.Equals(provincia, StringComparison.OrdinalIgnoreCase));

                var response = new List<ClienteResponseDto>();
                foreach (var cliente in clientes)
                {
                    var clienteDto = await BuildClienteResponseDto(cliente);
                    response.Add(clienteDto);
                }

                return ResponseDto<List<ClienteResponseDto>>.CreateSuccess(
                    response, $"Encontrados {response.Count} clientes en {provincia}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener clientes de provincia {Provincia}", provincia);
                return ResponseDto<List<ClienteResponseDto>>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<bool>> ValidarCIFAsync(string cif)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cif))
                {
                    return ResponseDto<bool>.CreateSuccess(false, "CIF vacío o nulo");
                }

                var isValid = ValidarCIFEspanol(cif);
                var message = isValid ? "CIF válido" : "CIF inválido";

                return ResponseDto<bool>.CreateSuccess(isValid, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar CIF {CIF}", cif);
                return ResponseDto<bool>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        public async Task<ResponseDto<object>> GetEstadisticasClientesAsync()
        {
            try
            {
                var clientes = await _unitOfWork.Repository<Cliente>().GetAllAsync();

                var stats = new
                {
                    TotalClientes = clientes.Count(),
                    ClientesConCIF = clientes.Count(c => !string.IsNullOrWhiteSpace(c.CIF)),
                    ClientesConEmail = clientes.Count(c => !string.IsNullOrWhiteSpace(c.E_MAIL)),
                    ClientesConDireccion = clientes.Count(c => !string.IsNullOrWhiteSpace(c.DIRECCION1)),
                    RiesgoTotalConcedido = clientes.Sum(c => c.RIESGOCONCEDIDO),
                    RiesgoPromedio = clientes.Average(c => c.RIESGOCONCEDIDO),
                    ProvinciasMasComunes = clientes
                        .Where(c => !string.IsNullOrWhiteSpace(c.PROVINCIA))
                        .GroupBy(c => c.PROVINCIA)
                        .OrderByDescending(g => g.Count())
                        .Take(5)
                        .Select(g => new { Provincia = g.Key, Count = g.Count() })
                        .ToList(),
                    Timestamp = DateTime.UtcNow
                };

                return ResponseDto<object>.CreateSuccess(stats, "Estadísticas generadas exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar estadísticas de clientes");
                return ResponseDto<object>.CreateError(
                    ApiConstants.ResponseMessages.ERROR_GENERIC, ex.Message);
            }
        }

        #region Métodos Auxiliares

        private async Task<ClienteResponseDto> BuildClienteResponseDto(Cliente cliente)
        {
            var clienteDto = cliente.Adapt<ClienteResponseDto>();

            clienteDto.InfoAdicional = new ClienteInfoAdicional
            {
                TieneCIFValido = !string.IsNullOrWhiteSpace(cliente.CIF) && ValidarCIFEspanol(cliente.CIF),
                TieneEmailValido = !string.IsNullOrWhiteSpace(cliente.E_MAIL) && IsValidEmail(cliente.E_MAIL),
                TieneDireccionCompleta = !string.IsNullOrWhiteSpace(cliente.DIRECCION1) &&
                                       !string.IsNullOrWhiteSpace(cliente.POBLACION),
                EstadoCliente = "Activo", // Por ahora fijo, se puede extender con lógica de negocio
                RiesgoDisponible = cliente.RIESGOCONCEDIDO // Por ahora igual al concedido
            };

            return clienteDto;
        }

        private async Task<(bool IsValid, List<string> Errors)> ValidateClienteBusinessRules(ClienteDto cliente)
        {
            var errors = new List<string>();

            // Validar CIF si está presente
            if (!string.IsNullOrWhiteSpace(cliente.CIF) && !ValidarCIFEspanol(cliente.CIF))
            {
                errors.Add($"CIF {cliente.CIF} no tiene un formato válido");
            }

            // Validar que no exista otro cliente con el mismo CIF
            if (!string.IsNullOrWhiteSpace(cliente.CIF))
            {
                var clientesRepo = _unitOfWork.Repository<Cliente>();
                var clienteConMismoCIF = (await clientesRepo.FindAsync(c =>
                    c.CIF == cliente.CIF && c.CODCLIENTE != cliente.CODCLIENTE)).FirstOrDefault();

                if (clienteConMismoCIF != null)
                {
                    errors.Add($"Ya existe un cliente con CIF {cliente.CIF}");
                }
            }

            // Validar email si está presente
            if (!string.IsNullOrWhiteSpace(cliente.E_MAIL) && !IsValidEmail(cliente.E_MAIL))
            {
                errors.Add($"Email {cliente.E_MAIL} no tiene un formato válido");
            }

            return (!errors.Any(), errors);
        }

        private async Task UpdateExistingCliente(Cliente existente, ClienteDto clienteDto)
        {
            existente.CODCONTABLE = clienteDto.CODCONTABLE;
            existente.NOMBRECLIENTE = clienteDto.NOMBRECLIENTE;
            existente.NOMBRECOMERCIAL = clienteDto.NOMBRECOMERCIAL;
            existente.CIF = clienteDto.CIF;
            existente.ALIAS = clienteDto.ALIAS;
            existente.DIRECCION1 = clienteDto.DIRECCION1;
            existente.POBLACION = clienteDto.POBLACION;
            existente.PROVINCIA = clienteDto.PROVINCIA;
            existente.PAIS = clienteDto.PAIS;
            existente.TELEFONO1 = clienteDto.TELEFONO1;
            existente.TELEFONO2 = clienteDto.TELEFONO2;
            existente.E_MAIL = clienteDto.E_MAIL;
            existente.RIESGOCONCEDIDO = clienteDto.RIESGOCONCEDIDO;
            existente.FACTURARCONIMPUESTO = clienteDto.FACTURARCONIMPUESTO;
            existente.UpdatedAt = DateTime.UtcNow;
        }

        private bool ValidarCIFEspanol(string cif)
        {
            if (string.IsNullOrWhiteSpace(cif)) return false;

            // Patrones básicos para CIF español
            var patterns = new[]
            {
                @"^[A-Z]\d{8}$",      // Letra + 8 dígitos
                @"^\d{8}[A-Z]$",      // 8 dígitos + letra
                @"^[A-Z]\d{7}[A-Z]$"  // Letra + 7 dígitos + letra
            };

            return patterns.Any(pattern => Regex.IsMatch(cif.ToUpper(), pattern));
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
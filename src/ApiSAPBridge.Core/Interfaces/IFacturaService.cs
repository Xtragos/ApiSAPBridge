using ApiSAPBridge.Models.DTOs;

namespace ApiSAPBridge.Core.Interfaces
{
    /// <summary>
    /// Servicio para gestión completa de facturación
    /// </summary>
    public interface IFacturaService
    {
        // Operaciones básicas
        Task<ResponseDto<List<FacturaResponseDto>>> CreateFacturasAsync(List<FacturaCompletaRequest> facturas);
        Task<ResponseDto<List<FacturaResponseDto>>> GetFacturasAsync(bool incluirDetalles = true, bool incluirPagos = true);
        Task<ResponseDto<FacturaResponseDto?>> GetFacturaAsync(string numSerie, int numFactura, int n);

        // Endpoints específicos para SAP
        Task<ResponseDto<FacturacionSAPResponse>> GetFacturasSAPAsync(FacturaSearchRequest request);
        Task<ResponseDto<FacturacionSAPResponse>> GetFacturasByFechaRangoAsync(DateTime fechaInicio, DateTime fechaFin, int pageSize = 50, int page = 1);
        Task<ResponseDto<FacturacionSAPResponse>> GetFacturasByFechaYSerieAsync(DateTime fechaInicio, DateTime fechaFin, string numSerie, int pageSize = 50, int page = 1);
        Task<ResponseDto<FacturacionSAPResponse>> GetFacturasBySerieAsync(string numSerie, int pageSize = 50, int page = 1);

        // Análisis y reportes
        Task<ResponseDto<object>> GetEstadisticasFacturacionAsync(DateTime? fechaInicio = null, DateTime? fechaFin = null);
        Task<ResponseDto<object>> GetResumenVentasPorClienteAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<ResponseDto<object>> GetResumenVentasPorVendedorAsync(DateTime fechaInicio, DateTime fechaFin);

        // Validaciones
        Task<ResponseDto<bool>> ValidarFacturaAsync(FacturaCompletaRequest factura);
    }
}
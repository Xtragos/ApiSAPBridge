using System.ComponentModel.DataAnnotations;

namespace ApiSAPBridge.Models.DTOs
{
    public class FacturaDto
    {
        [Required(ErrorMessage = "El número de serie es requerido")]
        [StringLength(50, ErrorMessage = "El número de serie no puede exceder 50 caracteres")]
        public string NUMSERIE { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de factura es requerido")]
        public int NUMFACTURA { get; set; }

        [Required(ErrorMessage = "El número N es requerido")]
        public int N { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        public DateTime FECHA { get; set; }

        [Required(ErrorMessage = "El código de cliente es requerido")]
        public int CODCLIENTE { get; set; }

        [Required(ErrorMessage = "El código de vendedor es requerido")]
        [StringLength(20, ErrorMessage = "El código de vendedor no puede exceder 20 caracteres")]
        public int CODVENDEDOR { get; set; }

        [Range(0, 999999.99, ErrorMessage = "El total bruto debe estar entre 0 y 999,999.99")]
        public decimal TOTALBRUTO { get; set; }

        [Range(0, 999999.99, ErrorMessage = "El total de impuestos debe estar entre 0 y 999,999.99")]
        public decimal TOTALIMPUESTOS { get; set; }

        [Range(0, 999999.99, ErrorMessage = "El total descuento comercial debe estar entre 0 y 999,999.99")]
        public decimal TOTDTOCOMERCIAL { get; set; }

        [Range(0, 999999.99, ErrorMessage = "El total neto debe estar entre 0 y 999,999.99")]
        public decimal TOTALNETO { get; set; }

        [Required(ErrorMessage = "El tipo de documento es requerido")]
        [StringLength(20, ErrorMessage = "El tipo de documento no puede exceder 20 caracteres")]
        public string TIPODOC { get; set; } = "FACTURA";

        public DateTime FECHACREADO { get; set; } = DateTime.UtcNow;
        public DateTime FECHAMODIFICADO { get; set; } = DateTime.UtcNow;
    }

    public class FacturaResponseDto : FacturaDto
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Información relacionada
        public ClienteResponseDto? Cliente { get; set; }
        public VendedorResponseDto? Vendedor { get; set; }

        // Colecciones relacionadas
        public List<FacturaDetalleResponseDto> Detalles { get; set; } = new List<FacturaDetalleResponseDto>();
        public List<FacturaPagoResponseDto> Pagos { get; set; } = new List<FacturaPagoResponseDto>();

        // Estadísticas calculadas
        public FacturaEstadisticas? Estadisticas { get; set; }
    }

    public class FacturaDetalleDto
    {
        [Required]
        [StringLength(50)]
        public string SERIE { get; set; } = string.Empty;

        [Required]
        public int NUMERO { get; set; }

        [Required]
        public int N { get; set; }

        [Required]
        public int LINEA { get; set; }

        [Required]
        public int CODARTICULO { get; set; }

        [StringLength(100)]
        public string? REFERENCIA { get; set; }

        [Required]
        [StringLength(255)]
        public string DESCRIPCION { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string TALLA { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string COLOR { get; set; } = string.Empty;

        public int TIPOIMPUESTO { get; set; }

        [Range(0, 999999.999)]
        public decimal UNIDADESTOTAL { get; set; }

        [Range(0, 999999.99)]
        public decimal PRECIO { get; set; }

        [Range(0, 100)]
        public decimal DTO { get; set; }

        [Range(0, 999999.99)]
        public decimal TOTAL { get; set; }
    }

    public class FacturaDetalleResponseDto : FacturaDetalleDto
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ArticuloResponseDto? Articulo { get; set; }
        public ImpuestoResponseDto? Impuesto { get; set; }
    }

    public class FacturaPagoDto
    {
        [Required]
        [StringLength(50)]
        public string SERIE { get; set; } = string.Empty;

        [Required]
        public int NUMERO { get; set; }

        [Required]
        public int N { get; set; }

        [Required]
        public int POSICION { get; set; }

        [Required]
        public int CODTIPOPAGO { get; set; }

        [Range(0, 999999.99)]
        public decimal IMPORTE { get; set; }

        [StringLength(100)]
        public string? DESCRIPCION { get; set; }
    }

    public class FacturaPagoResponseDto : FacturaPagoDto
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public FormaPagoResponseDto? FormaPago { get; set; }
    }

    public class FacturaEstadisticas
    {
        public int TotalLineas { get; set; }
        public int TotalFormasPago { get; set; }
        public decimal PromedioLineaImporte { get; set; }
        public decimal PorcentajeDescuento { get; set; }
        public decimal PorcentajeImpuestos { get; set; }
        public bool TieneDescuentos { get; set; }
        public bool EsPagoMultiple { get; set; }
        public string FormaPagoPrincipal { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request completo para crear factura con detalles y pagos
    /// </summary>
    public class FacturaCompletaRequest
    {
        [Required]
        public FacturaDto Factura { get; set; } = null!;

        [Required]
        public List<FacturaDetalleDto> Detalles { get; set; } = new List<FacturaDetalleDto>();

        [Required]
        public List<FacturaPagoDto> Pagos { get; set; } = new List<FacturaPagoDto>();

        public bool ValidarTotales { get; set; } = true;
        public bool ValidarStock { get; set; } = false;
        public bool CalcularTotalesAutomatico { get; set; } = true;
    }

    /// <summary>
    /// Response completo para el formato que consume SAP
    /// </summary>
    public class FacturacionSAPResponse
    {
        public List<FacturaSAPDto> Facturas { get; set; } = new List<FacturaSAPDto>();
        public PaginacionDto Paginacion { get; set; } = new PaginacionDto();
    }

    public class FacturaSAPDto
    {
        public string NUMSERIE { get; set; } = string.Empty;
        public int NUMFACTURA { get; set; }
        public int N { get; set; }
        public string FECHA { get; set; } = string.Empty; // Formato: "yyyy-MM-dd"
        public int CODCLIENTE { get; set; }
        public int CODVENDEDOR { get; set; }
        public decimal TOTALBRUTO { get; set; }
        public decimal TOTALIMPUESTOS { get; set; }
        public decimal TOTDTOCOMERCIAL { get; set; }
        public decimal TOTALNETO { get; set; }
        public string TIPODOC { get; set; } = string.Empty;
        public string FECHACREADO { get; set; } = string.Empty; // Formato ISO
        public string FECHAMODIFICADO { get; set; } = string.Empty; // Formato ISO

        public List<DetalleSAPDto> Detalles { get; set; } = new List<DetalleSAPDto>();
        public List<PagoSAPDto> Pagos { get; set; } = new List<PagoSAPDto>();
    }

    public class DetalleSAPDto
    {
        public string SERIE { get; set; } = string.Empty;
        public int NUMERO { get; set; }
        public int N { get; set; }
        public int LINEA { get; set; }
        public int CODARTICULO { get; set; }
        public string? REFERENCIA { get; set; }
        public string DESCRIPCION { get; set; } = string.Empty;
        public string TALLA { get; set; } = string.Empty;
        public string COLOR { get; set; } = string.Empty;
        public int TIPOIMPUESTO { get; set; }
        public decimal UNIDADESTOTAL { get; set; }
        public decimal PRECIO { get; set; }
        public decimal DTO { get; set; }
        public decimal TOTAL { get; set; }
    }

    public class PagoSAPDto
    {
        public string SERIE { get; set; } = string.Empty;
        public int NUMERO { get; set; }
        public int N { get; set; }
        public int POSICION { get; set; }
        public int CODTIPOPAGO { get; set; }
        public decimal IMPORTE { get; set; }
        public string? DESCRIPCION { get; set; }
    }

    public class PaginacionDto
    {
        public int PaginaActual { get; set; }
        public int PorPagina { get; set; }
        public int TotalFacturas { get; set; }
        public int TotalPaginas { get; set; }
    }

    /// <summary>
    /// Request para búsqueda de facturas
    /// </summary>
    public class FacturaSearchRequest
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? NumSerie { get; set; }
        public int? NumFactura { get; set; }
        public int? CodCliente { get; set; }
        public int? CodVendedor { get; set; }
        public string? TipoDoc { get; set; }

        [Range(1, 1000)]
        public int PageSize { get; set; } = 50;

        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        public bool IncluirDetalles { get; set; } = true;
        public bool IncluirPagos { get; set; } = true;
        public bool IncluirRelaciones { get; set; } = false;
    }
}
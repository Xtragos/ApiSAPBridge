using System.ComponentModel.DataAnnotations;

namespace ApiSAPBridge.Models.DTOs
{
    public class ArticuloDto
    {
        [Required(ErrorMessage = "El código de artículo es requerido")]
        public int CODARTICULO { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(255, ErrorMessage = "La descripción no puede exceder 255 caracteres")]
        [MinLength(3, ErrorMessage = "La descripción debe tener al menos 3 caracteres")]
        public string DESCRIPCION { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción adicional no puede exceder 500 caracteres")]
        public string? DESCRIPADIC { get; set; }

        [Required(ErrorMessage = "El tipo de impuesto es requerido")]
        public int TIPOIMPUESTO { get; set; }

        public int? DPTO { get; set; }
        public int? SECCION { get; set; }
        public int? FAMILIA { get; set; }

        [Range(0, 999999.99, ErrorMessage = "La unidad de compra debe estar entre 0 y 999,999.99")]
        public decimal? UNID1C { get; set; }

        [Range(0, 999999.99, ErrorMessage = "La unidad de venta debe estar entre 0 y 999,999.99")]
        public decimal? UNID1V { get; set; }

        [StringLength(100, ErrorMessage = "La referencia de proveedor no puede exceder 100 caracteres")]
        public string? REFPROVEEDOR { get; set; }

        [StringLength(1)]
        [RegularExpression("^[SN]$", ErrorMessage = "Usa stocks debe ser 'S' o 'N'")]
        public string? USASTOCKS { get; set; }

        public int? IMPUESTOCOMPRA { get; set; }

        [StringLength(1)]
        [RegularExpression("^[SN]$", ErrorMessage = "Descatalogado debe ser 'S' o 'N'")]
        public string? DESCATALOGADO { get; set; }

        [Range(0, 999999.99, ErrorMessage = "Las unidades de traspaso deben estar entre 0 y 999,999.99")]
        public decimal? UDSTRASPASO { get; set; }

        [StringLength(1)]
        [RegularExpression("^[ABCDEFGHIJ]$", ErrorMessage = "Tipo de artículo debe ser una letra de A a J")]
        public string? TIPOARTICULO { get; set; }
    }

    public class ArticuloResponseDto : ArticuloDto
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Información del impuesto asociado
        /// </summary>
        public ImpuestoResponseDto? Impuesto { get; set; }

        /// <summary>
        /// Información del departamento (si aplica)
        /// </summary>
        public DepartamentoResponseDto? Departamento { get; set; }

        /// <summary>
        /// Líneas del artículo (variantes con tallas y colores)
        /// </summary>
        public List<ArticuloLineaResponseDto> Lineas { get; set; } = new List<ArticuloLineaResponseDto>();

        /// <summary>
        /// Precios del artículo en diferentes tarifas
        /// </summary>
        public List<PrecioResponseDto> Precios { get; set; } = new List<PrecioResponseDto>();

        /// <summary>
        /// Estadísticas calculadas del artículo
        /// </summary>
        public ArticuloEstadisticas? Estadisticas { get; set; }
    }

    public class ArticuloEstadisticas
    {
        public int TotalLineas { get; set; }
        public int TotalPrecios { get; set; }
        public int TotalTarifas { get; set; }
        public decimal? PrecioMinimo { get; set; }
        public decimal? PrecioMaximo { get; set; }
        public decimal? PrecioPromedio { get; set; }
        public bool TieneCostesDefinidos { get; set; }
        public bool EstaDescatalogado { get; set; }
        public bool UsaStocks { get; set; }
    }

    /// <summary>
    /// Request para crear artículo completo con líneas y precios
    /// </summary>
    public class ArticuloCompletoRequest
    {
        [Required]
        public ArticuloDto Articulo { get; set; } = null!;

        public List<ArticuloLineaDto> Lineas { get; set; } = new List<ArticuloLineaDto>();
        public List<PrecioDto> Precios { get; set; } = new List<PrecioDto>();

        public bool ValidarIntegridad { get; set; } = true;
        public bool CrearLineasAutomaticas { get; set; } = false;
    }

    /// <summary>
    /// Request de búsqueda avanzada para artículos
    /// </summary>
    public class ArticuloSearchRequest
    {
        [StringLength(100, ErrorMessage = "El término de búsqueda no puede exceder 100 caracteres")]
        public string? SearchTerm { get; set; }

        public int? Departamento { get; set; }
        public int? Seccion { get; set; }
        public int? Familia { get; set; }
        public int? TipoImpuesto { get; set; }

        [StringLength(1)]
        [RegularExpression("^[SN]$", ErrorMessage = "Usa stocks debe ser 'S' o 'N'")]
        public string? UsaStocks { get; set; }

        [StringLength(1)]
        [RegularExpression("^[SN]$", ErrorMessage = "Descatalogado debe ser 'S' o 'N'")]
        public string? Descatalogado { get; set; }

        public decimal? PrecioMinimo { get; set; }
        public decimal? PrecioMaximo { get; set; }

        public bool IncluirLineas { get; set; } = false;
        public bool IncluirPrecios { get; set; } = false;
        public bool IncluirEstadisticas { get; set; } = false;

        [Range(1, 1000, ErrorMessage = "El tamaño de página debe estar entre 1 y 1000")]
        public int PageSize { get; set; } = 50;

        [Range(1, int.MaxValue, ErrorMessage = "La página debe ser mayor a 0")]
        public int Page { get; set; } = 1;
    }
    /// <summary>
    /// Request específico para SAP con líneas integradas
    /// </summary>
    public class ArticuloCompletoSAPRequest : ArticuloDto
    {
        public List<ArticuloLineaDto>? Lineas { get; set; }
    }
}
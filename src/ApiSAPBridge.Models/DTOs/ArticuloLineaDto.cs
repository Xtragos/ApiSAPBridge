using System.ComponentModel.DataAnnotations;

namespace ApiSAPBridge.Models.DTOs
{
    public class ArticuloLineaDto
    {
        [Required(ErrorMessage = "El código de artículo es requerido")]
        public int CODARTICULO { get; set; }

        [Required(ErrorMessage = "La talla es requerida")]
        [StringLength(10, ErrorMessage = "La talla no puede exceder 10 caracteres")]
        public string TALLA { get; set; } = string.Empty;

        [Required(ErrorMessage = "El color es requerido")]
        [StringLength(50, ErrorMessage = "El color no puede exceder 50 caracteres")]
        public string COLOR { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "El código de barras no puede exceder 50 caracteres")]
        public string? CODBARRAS { get; set; }

        [Range(0, 999999.99, ErrorMessage = "El coste medio debe estar entre 0 y 999,999.99")]
        public decimal? COSTEMEDIO { get; set; }

        [Range(0, 999999.99, ErrorMessage = "El coste stock debe estar entre 0 y 999,999.99")]
        public decimal? COSTESTOCK { get; set; }

        [Range(0, 999999.99, ErrorMessage = "El último coste debe estar entre 0 y 999,999.99")]
        public decimal? ULTIMOCOSTE { get; set; }

        [StringLength(50, ErrorMessage = "El código de barras 2 no puede exceder 50 caracteres")]
        public string? CODBARRAS2 { get; set; }

        [StringLength(50, ErrorMessage = "El código de barras 3 no puede exceder 50 caracteres")]
        public string? CODBARRAS3 { get; set; }

        [StringLength(1)]
        [RegularExpression("^[SN]$", ErrorMessage = "Descatalogado debe ser 'S' o 'N'")]
        public string? DESCATALOGADO { get; set; }
    }

    public class ArticuloLineaResponseDto : ArticuloLineaDto
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Información del artículo padre
        /// </summary>
        public ArticuloResponseDto? Articulo { get; set; }

        /// <summary>
        /// Precios específicos de esta línea
        /// </summary>
        public List<PrecioResponseDto> Precios { get; set; } = new List<PrecioResponseDto>();

        /// <summary>
        /// Información adicional calculada
        /// </summary>
        public LineaEstadisticas? Estadisticas { get; set; }
    }

    public class LineaEstadisticas
    {
        public bool TieneCodBarras { get; set; }
        public bool TieneCostesDefinidos { get; set; }
        public bool EstaDescatalogada { get; set; }
        public int TotalPrecios { get; set; }
        public decimal? PrecioMinimo { get; set; }
        public decimal? PrecioMaximo { get; set; }
        public string CodigoBarrasPrincipal { get; set; } = string.Empty;
    }

    public class ArticuloLineaSearchRequest
    {
        public int? CodigoArticulo { get; set; }

        [StringLength(100)]
        public string? SearchTerm { get; set; }

        [StringLength(50)]
        public string? CodigoBarras { get; set; }

        public bool SoloConCostes { get; set; } = false;
        public bool SoloActivas { get; set; } = false;
        public bool IncluirPrecios { get; set; } = false;

        [Range(1, 1000)]
        public int PageSize { get; set; } = 50;

        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;
    }
}
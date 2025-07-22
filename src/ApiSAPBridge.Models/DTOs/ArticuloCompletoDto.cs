using System.ComponentModel.DataAnnotations;

namespace ApiSAPBridge.Models.DTOs
{
    public class ArticuloCompletoDto
    {
        [Required]
        public int CODARTICULO { get; set; }

        [Required]
        [StringLength(255)]
        public string DESCRIPCION { get; set; } = string.Empty;

        [StringLength(500)]
        public string? DESCRIPADIC { get; set; }

        public int? TIPOIMPUESTO { get; set; }

        public int? DPTO { get; set; }

        public int? SECCION { get; set; }

        public int? FAMILIA { get; set; }

        public decimal? UNID1C { get; set; }

        public decimal? UNID1V { get; set; }

        [StringLength(100)]
        public string? REFPROVEEDOR { get; set; }

        [StringLength(1)]
        public string? USASTOCKS { get; set; }

        public int? IMPUESTOCOMPRA { get; set; }

        [StringLength(1)]
        public string? DESCATALOGADO { get; set; }

        public decimal? UDSTRASPASO { get; set; }

        [StringLength(1)]
        public string? TIPOARTICULO { get; set; }

        [Required]
        public List<ArticuloLineaDto> lineas { get; set; } = new List<ArticuloLineaDto>();
    }

    public class ArticuloLineaDto
    {
        [Required]
        public int CODARTICULO { get; set; }

        [Required]
        [StringLength(10)]
        public string TALLA { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string COLOR { get; set; } = string.Empty;

        [StringLength(50)]
        public string? CODBARRAS { get; set; }

        public decimal? COSTEMEDIO { get; set; }

        public decimal? COSTESTOCK { get; set; }

        public decimal? ULTIMOCOSTE { get; set; }

        [StringLength(50)]
        public string? CODBARRAS2 { get; set; }

        [StringLength(50)]
        public string? CODBARRAS3 { get; set; }

        [StringLength(1)]
        public string? DESCATALOGADO { get; set; }
    }
}
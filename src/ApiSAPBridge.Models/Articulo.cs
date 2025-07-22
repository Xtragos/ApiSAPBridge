using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiSAPBridge.Models
{
    public class Articulo
    {
        [Key]
        public int CODARTICULO { get; set; }

        [Required]
        [StringLength(255)]
        public string DESCRIPCION { get; set; } = string.Empty;

        [StringLength(500)]
        public string? DESCRIPADIC { get; set; }

        public int TIPOIMPUESTO { get; set; }

        public int? DPTO { get; set; }

        public int? SECCION { get; set; }

        public int? FAMILIA { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? UNID1C { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? UNID1V { get; set; }

        [StringLength(100)]
        public string? REFPROVEEDOR { get; set; }

        [StringLength(1)]
        public string? USASTOCKS { get; set; }

        public int? IMPUESTOCOMPRA { get; set; }

        [StringLength(1)]
        public string? DESCATALOGADO { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? UDSTRASPASO { get; set; }

        [StringLength(1)]
        public string? TIPOARTICULO { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        [ForeignKey("TIPOIMPUESTO")]
        public virtual Impuesto? Impuesto { get; set; }

        [ForeignKey("DPTO")]
        public virtual Departamento? Departamento { get; set; }

        public virtual ICollection<ArticuloLinea> Lineas { get; set; } = new List<ArticuloLinea>();
        public virtual ICollection<Precio> Precios { get; set; } = new List<Precio>();
    }
}
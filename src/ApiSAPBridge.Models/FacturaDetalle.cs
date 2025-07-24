using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiSAPBridge.Models
{
    [Table("FACTURADETALLES")]
    public class FacturaDetalle
    {
        [Key]
        [Column("SERIE")]
        [StringLength(50)]
        public string SERIE { get; set; } = string.Empty;

        [Key]
        [Column("NUMERO")]
        public int NUMERO { get; set; }

        [Key]
        [Column("N")]
        public int N { get; set; }

        [Key]
        [Column("LINEA")]
        public int LINEA { get; set; }

        [Required]
        [Column("CODARTICULO")]
        public int CODARTICULO { get; set; }

        [Column("REFERENCIA")]
        [StringLength(100)]
        public string? REFERENCIA { get; set; }

        [Required]
        [Column("DESCRIPCION")]
        [StringLength(255)]
        public string DESCRIPCION { get; set; } = string.Empty;

        [Required]
        [Column("TALLA")]
        [StringLength(10)]
        public string TALLA { get; set; } = string.Empty;

        [Required]
        [Column("COLOR")]
        [StringLength(50)]
        public string COLOR { get; set; } = string.Empty;

        [Column("TIPOIMPUESTO")]
        public int TIPOIMPUESTO { get; set; }

        [Column("UNIDADESTOTAL", TypeName = "decimal(18,3)")]
        public decimal UNIDADESTOTAL { get; set; }

        [Column("PRECIO", TypeName = "decimal(18,2)")]
        public decimal PRECIO { get; set; }

        [Column("DTO", TypeName = "decimal(5,2)")]
        public decimal DTO { get; set; }

        [Column("TOTAL", TypeName = "decimal(18,2)")]
        public decimal TOTAL { get; set; }

        // Propiedades de navegación
        [ForeignKey("SERIE,NUMERO,N")]
        public virtual Factura? Factura { get; set; }

        [ForeignKey("CODARTICULO")]
        public virtual Articulo? Articulo { get; set; }

        [ForeignKey("TIPOIMPUESTO")]
        public virtual Impuesto? Impuesto { get; set; }

        // Propiedades de auditoría
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
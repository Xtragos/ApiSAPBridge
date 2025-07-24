using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiSAPBridge.Models
{
    [Table("FACTURAS")]
    public class Factura
    {
        [Key]
        [Column("NUMSERIE")]
        [StringLength(50)]
        public string NUMSERIE { get; set; } = string.Empty;

        [Key]
        [Column("NUMFACTURA")]
        public int NUMFACTURA { get; set; }

        [Key]
        [Column("N")]
        public int N { get; set; }

        [Required]
        [Column("FECHA")]
        public DateTime FECHA { get; set; }

        [Required]
        [Column("CODCLIENTE")]
        public int CODCLIENTE { get; set; }

        [Required]
        [Column("CODVENDEDOR")]
        [StringLength(20)]
        public int CODVENDEDOR { get; set; }

        [Column("TOTALBRUTO", TypeName = "decimal(18,2)")]
        public decimal TOTALBRUTO { get; set; }

        [Column("TOTALIMPUESTOS", TypeName = "decimal(18,2)")]
        public decimal TOTALIMPUESTOS { get; set; }

        [Column("TOTDTOCOMERCIAL", TypeName = "decimal(18,2)")]
        public decimal TOTDTOCOMERCIAL { get; set; }

        [Column("TOTALNETO", TypeName = "decimal(18,2)")]
        public decimal TOTALNETO { get; set; }

        [Required]
        [Column("TIPODOC")]
        [StringLength(20)]
        public string TIPODOC { get; set; } = "FACTURA";

        [Column("FECHACREADO")]
        public DateTime FECHACREADO { get; set; } = DateTime.UtcNow;

        [Column("FECHAMODIFICADO")]
        public DateTime FECHAMODIFICADO { get; set; } = DateTime.UtcNow;

        // Propiedades de navegación
        [ForeignKey("CODCLIENTE")]
        public virtual Cliente? Cliente { get; set; }

        [ForeignKey("CODVENDEDOR")]
        public virtual Vendedor? Vendedor { get; set; }

        public virtual ICollection<FacturaDetalle> Detalles { get; set; } = new List<FacturaDetalle>();
        public virtual ICollection<FacturaPago> Pagos { get; set; } = new List<FacturaPago>();

        // Propiedades de auditoría
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiSAPBridge.Models
{
    [Table("FACTURAPAGOS")]
    public class FacturaPago
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
        [Column("POSICION")]
        public int POSICION { get; set; }

        [Required]
        [Column("CODTIPOPAGO")]
        public int CODTIPOPAGO { get; set; }

        [Column("IMPORTE", TypeName = "decimal(18,2)")]
        public decimal IMPORTE { get; set; }

        [Column("DESCRIPCION")]
        [StringLength(100)]
        public string? DESCRIPCION { get; set; }

        // Propiedades de navegación
        [ForeignKey("SERIE,NUMERO,N")]
        public virtual Factura? Factura { get; set; }

        [ForeignKey("CODTIPOPAGO")]
        public virtual FormaPago? FormaPago { get; set; }

        // Propiedades de auditoría
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
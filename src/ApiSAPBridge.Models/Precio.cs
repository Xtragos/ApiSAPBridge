using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiSAPBridge.Models
{
    public class Precio
    {
        [Key, Column(Order = 0)]
        public int IDTARIFAV { get; set; }

        [Key, Column(Order = 1)]
        public int CODARTICULO { get; set; }

        [Key, Column(Order = 2)]
        [StringLength(10)]
        public string TALLA { get; set; } = string.Empty;

        [Key, Column(Order = 3)]
        [StringLength(50)]
        public string COLOR { get; set; } = string.Empty;

        [StringLength(50)]
        public string? CODBARRAS { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PBRUTO { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal DTO { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PNETO { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        [ForeignKey("IDTARIFAV")]
        public virtual Tarifa Tarifa { get; set; } = null!;

        [ForeignKey("CODARTICULO")]
        public virtual Articulo Articulo { get; set; } = null!;
    }
}
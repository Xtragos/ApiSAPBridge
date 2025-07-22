using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiSAPBridge.Models
{
    public class ArticuloLinea
    {
        [Key, Column(Order = 0)]
        public int CODARTICULO { get; set; }

        [Key, Column(Order = 1)]
        [StringLength(10)]
        public string TALLA { get; set; } = string.Empty;

        [Key, Column(Order = 2)]
        [StringLength(50)]
        public string COLOR { get; set; } = string.Empty;

        [StringLength(50)]
        public string? CODBARRAS { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? COSTEMEDIO { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? COSTESTOCK { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ULTIMOCOSTE { get; set; }

        [StringLength(50)]
        public string? CODBARRAS2 { get; set; }

        [StringLength(50)]
        public string? CODBARRAS3 { get; set; }

        [StringLength(1)]
        public string? DESCATALOGADO { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        [ForeignKey("CODARTICULO")]
        public virtual Articulo Articulo { get; set; } = null!;
    }
}
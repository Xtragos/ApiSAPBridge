using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiSAPBridge.Models
{
    public class Impuesto
    {
        [Key]
        public int TIPOIVA { get; set; }

        [Required]
        [StringLength(255)]
        public string DESCRIPCION { get; set; } = string.Empty;

        [Column(TypeName = "decimal(5,2)")]
        public decimal IVA { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        public virtual ICollection<Articulo> Articulos { get; set; } = new List<Articulo>();
    }
}
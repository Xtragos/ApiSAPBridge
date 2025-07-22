using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Collections.Specialized.BitVector32;

namespace ApiSAPBridge.Models
{
    public class Departamento
    {
        [Key]
        public int NUMDPTO { get; set; }

        [Required]
        [StringLength(255)]
        public string DESCRIPCION { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        public virtual ICollection<Seccion> Secciones { get; set; } = new List<Seccion>();
        public virtual ICollection<Articulo> Articulos { get; set; } = new List<Articulo>();
    }
}
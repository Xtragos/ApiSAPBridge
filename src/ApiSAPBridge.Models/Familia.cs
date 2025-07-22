using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiSAPBridge.Models
{
    public class Familia
    {
        [Key, Column(Order = 0)]
        public int NUMDPTO { get; set; }

        [Key, Column(Order = 1)]
        public int NUMSECCION { get; set; }

        [Key, Column(Order = 2)]
        public int NUMFAMILIA { get; set; }

        [Required]
        [StringLength(255)]
        public string DESCRIPCION { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        [ForeignKey("NUMDPTO,NUMSECCION")]
        public virtual Seccion Seccion { get; set; } = null!;
    }
}

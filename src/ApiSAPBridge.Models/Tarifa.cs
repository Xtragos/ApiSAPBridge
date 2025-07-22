using System.ComponentModel.DataAnnotations;

namespace ApiSAPBridge.Models
{
    public class Tarifa
    {
        [Key]
        public int IDTARIFAV { get; set; }

        [Required]
        [StringLength(255)]
        public string DESCRIPCION { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime FECHAINI { get; set; }

        [DataType(DataType.Date)]
        public DateTime FECHAFIN { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        public virtual ICollection<Precio> Precios { get; set; } = new List<Precio>();
    }
}
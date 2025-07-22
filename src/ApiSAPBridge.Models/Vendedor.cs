using System.ComponentModel.DataAnnotations;

namespace ApiSAPBridge.Models
{
    public class Vendedor
    {
        [Key]
        public int CODVENDEDOR { get; set; }

        [Required]
        [StringLength(255)]
        public string NOMBRE { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
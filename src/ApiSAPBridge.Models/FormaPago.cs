using System.ComponentModel.DataAnnotations;

namespace ApiSAPBridge.Models
{
    public class FormaPago
    {
        [Key]
        [StringLength(10)]
        public string CODFORMAPAGO { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string DESCRIPCION { get; set; } = string.Empty;

        public int NUMVENCIMIENTOS { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
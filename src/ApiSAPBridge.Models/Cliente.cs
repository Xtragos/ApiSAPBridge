using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiSAPBridge.Models
{
    public class Cliente
    {
        [Key]
        public int CODCLIENTE { get; set; }

        [StringLength(50)]
        public string? CODCONTABLE { get; set; }

        [Required]
        [StringLength(255)]
        public string NOMBRECLIENTE { get; set; } = string.Empty;

        [StringLength(255)]
        public string? NOMBRECOMERCIAL { get; set; }

        [StringLength(50)]
        public string? CIF { get; set; }

        [StringLength(255)]
        public string? ALIAS { get; set; }

        [StringLength(500)]
        public string? DIRECCION1 { get; set; }

        [StringLength(255)]
        public string? POBLACION { get; set; }

        [StringLength(255)]
        public string? PROVINCIA { get; set; }

        [StringLength(255)]
        public string? PAIS { get; set; }

        [StringLength(50)]
        public string? TELEFONO1 { get; set; }

        [StringLength(50)]
        public string? TELEFONO2 { get; set; }

        [StringLength(255)]
        public string? E_MAIL { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal RIESGOCONCEDIDO { get; set; } = 0;

        [StringLength(10)]
        public string? FACTURARCONIMPUESTO { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
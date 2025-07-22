using System.ComponentModel.DataAnnotations;

namespace ApiSAPBridge.Models.DTOs
{
    public class PrecioDto
    {
        [Required]
        public int IDTARIFAV { get; set; }

        [Required]
        public int CODARTICULO { get; set; }

        [StringLength(50)]
        public string? CODBARRAS { get; set; }

        [Required]
        [StringLength(10)]
        public string TALLA { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string COLOR { get; set; } = string.Empty;

        [Required]
        public decimal PBRUTO { get; set; }

        public decimal DTO { get; set; }

        [Required]
        public decimal PNETO { get; set; }
    }

    public class PrecioBatchDto
    {
        [Required]
        public int idTarifa { get; set; }

        [Required]
        public List<PrecioBatchItemDto> precios { get; set; } = new List<PrecioBatchItemDto>();
    }

    public class PrecioBatchItemDto
    {
        [Required]
        public int CODARTICULO { get; set; }

        [StringLength(50)]
        public string? CODBARRAS { get; set; }

        [Required]
        [StringLength(10)]
        public string TALLA { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string COLOR { get; set; } = string.Empty;

        [Required]
        public decimal PBRUTO { get; set; }

        public decimal DTO { get; set; }

        [Required]
        public decimal PNETO { get; set; }
    }
}
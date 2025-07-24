using System.ComponentModel.DataAnnotations;

namespace ApiSAPBridge.Models.DTOs
{
    public class DepartamentoDto
    {
        [Required]
        public int NUMDPTO { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "La descripción no puede exceder 255 caracteres")]
        public string DESCRIPCION { get; set; } = string.Empty;
    }

    public class DepartamentoResponseDto : DepartamentoDto
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
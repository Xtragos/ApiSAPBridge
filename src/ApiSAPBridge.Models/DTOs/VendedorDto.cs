using System.ComponentModel.DataAnnotations;

namespace ApiSAPBridge.Models.DTOs
{
    public class VendedorDto
    {
        [Required(ErrorMessage = "El código de vendedor es requerido")]
        public int CODVENDEDOR { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(255, ErrorMessage = "El nombre no puede exceder 255 caracteres")]
        [MinLength(2, ErrorMessage = "El nombre debe tener al menos 2 caracteres")]
        public string NOMBRE { get; set; } = string.Empty;
    }

    public class VendedorResponseDto : VendedorDto
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
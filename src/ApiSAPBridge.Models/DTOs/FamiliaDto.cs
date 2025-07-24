using System.ComponentModel.DataAnnotations;

namespace ApiSAPBridge.Models.DTOs
{
    public class FamiliaDto
    {
        [Required(ErrorMessage = "El número de departamento es requerido")]
        public int NUMDPTO { get; set; }

        [Required(ErrorMessage = "El número de sección es requerido")]
        public int NUMSECCION { get; set; }

        [Required(ErrorMessage = "El número de familia es requerido")]
        public int NUMFAMILIA { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(255, ErrorMessage = "La descripción no puede exceder 255 caracteres")]
        [MinLength(3, ErrorMessage = "La descripción debe tener al menos 3 caracteres")]
        public string DESCRIPCION { get; set; } = string.Empty;
    }

    public class FamiliaResponseDto : FamiliaDto
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Información de la sección a la que pertenece
        /// </summary>
        public SeccionResponseDto? Seccion { get; set; }
    }

    public class FamiliaCreateRequest
    {
        [Required]
        public List<FamiliaDto> Familias { get; set; } = new List<FamiliaDto>();
    }
}
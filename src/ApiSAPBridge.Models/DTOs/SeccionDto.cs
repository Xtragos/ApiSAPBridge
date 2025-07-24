using System.ComponentModel.DataAnnotations;

namespace ApiSAPBridge.Models.DTOs
{
    public class SeccionDto
    {
        [Required(ErrorMessage = "El número de departamento es requerido")]
        public int NUMDPTO { get; set; }

        [Required(ErrorMessage = "El número de sección es requerido")]
        public int NUMSECCION { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(255, ErrorMessage = "La descripción no puede exceder 255 caracteres")]
        [MinLength(3, ErrorMessage = "La descripción debe tener al menos 3 caracteres")]
        public string DESCRIPCION { get; set; } = string.Empty;
    }

    public class SeccionResponseDto : SeccionDto
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Información del departamento al que pertenece
        /// </summary>
        public DepartamentoResponseDto? Departamento { get; set; }
    }

    public class SeccionCreateRequest
    {
        [Required]
        public List<SeccionDto> Secciones { get; set; } = new List<SeccionDto>();
    }
}
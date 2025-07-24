using System.ComponentModel.DataAnnotations;

namespace ApiSAPBridge.Models.DTOs
{
    public class ImpuestoDto
    {
        [Required(ErrorMessage = "El tipo de IVA es requerido")]
        public int TIPOIVA { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(255, ErrorMessage = "La descripción no puede exceder 255 caracteres")]
        [MinLength(3, ErrorMessage = "La descripción debe tener al menos 3 caracteres")]
        public string DESCRIPCION { get; set; } = string.Empty;

        [Required(ErrorMessage = "El porcentaje de IVA es requerido")]
        [Range(0, 100, ErrorMessage = "El IVA debe estar entre 0 y 100")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "El IVA debe tener máximo 2 decimales")]
        public decimal IVA { get; set; }
    }

    public class ImpuestoResponseDto : ImpuestoDto
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Número de artículos que usan este impuesto
        /// </summary>
        public int ArticulosAsociados { get; set; } = 0;
    }

    public class ImpuestoStatsDto
    {
        public int TotalImpuestos { get; set; }
        public decimal IvaMinimo { get; set; }
        public decimal IvaMaximo { get; set; }
        public decimal IvaPromedio { get; set; }
        public int ArticulosTotales { get; set; }
        public DateTime FechaConsulta { get; set; } = DateTime.UtcNow;
    }
}
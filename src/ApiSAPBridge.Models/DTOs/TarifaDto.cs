using System.ComponentModel.DataAnnotations;

namespace ApiSAPBridge.Models.DTOs
{
    public class TarifaDto
    {
        [Required(ErrorMessage = "El ID de tarifa es requerido")]
        public int IDTARIFAV { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(255, ErrorMessage = "La descripción no puede exceder 255 caracteres")]
        [MinLength(3, ErrorMessage = "La descripción debe tener al menos 3 caracteres")]
        public string DESCRIPCION { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de inicio es requerida")]
        [DataType(DataType.Date)]
        public DateTime FECHAINI { get; set; }

        [Required(ErrorMessage = "La fecha de fin es requerida")]
        [DataType(DataType.Date)]
        public DateTime FECHAFIN { get; set; }

        /// <summary>
        /// Validación personalizada: Fecha fin debe ser mayor a fecha inicio
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FECHAFIN <= FECHAINI)
            {
                yield return new ValidationResult(
                    "La fecha de fin debe ser posterior a la fecha de inicio",
                    new[] { nameof(FECHAFIN) });
            }

            if (FECHAINI < DateTime.Today.AddYears(-10))
            {
                yield return new ValidationResult(
                    "La fecha de inicio no puede ser anterior a 10 años",
                    new[] { nameof(FECHAINI) });
            }

            if (FECHAFIN > DateTime.Today.AddYears(10))
            {
                yield return new ValidationResult(
                    "La fecha de fin no puede ser posterior a 10 años",
                    new[] { nameof(FECHAFIN) });
            }
        }
    }

    public class TarifaResponseDto : TarifaDto
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Información del estado de la tarifa
        /// </summary>
        public TarifaEstadoInfo EstadoInfo { get; set; } = new TarifaEstadoInfo();

        /// <summary>
        /// Número de precios asociados (si se solicita)
        /// </summary>
        public int? TotalPrecios { get; set; }
    }

    public class TarifaEstadoInfo
    {
        public bool EstaActiva { get; set; }
        public bool EstaVigente { get; set; }
        public bool EstaVencida { get; set; }
        public bool EsPendiente { get; set; }
        public int DiasParaInicio { get; set; }
        public int DiasParaVencimiento { get; set; }
        public string EstadoDescripcion { get; set; } = string.Empty;
    }

    public class TarifaSearchRequest
    {
        [StringLength(100, ErrorMessage = "El término de búsqueda no puede exceder 100 caracteres")]
        public string? SearchTerm { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FechaDesde { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FechaHasta { get; set; }

        public bool? SoloActivas { get; set; }
        public bool? SoloVigentes { get; set; }
        public bool? IncluirConteoPrecios { get; set; }

        [Range(1, 1000, ErrorMessage = "El tamaño de página debe estar entre 1 y 1000")]
        public int PageSize { get; set; } = 50;

        [Range(1, int.MaxValue, ErrorMessage = "La página debe ser mayor a 0")]
        public int Page { get; set; } = 1;
    }
}
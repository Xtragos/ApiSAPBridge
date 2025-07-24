using System.ComponentModel.DataAnnotations;

namespace ApiSAPBridge.Models.DTOs
{
    public class PrecioDto
    {
        [Required(ErrorMessage = "El ID de tarifa es requerido")]
        public int IDTARIFAV { get; set; }

        [Required(ErrorMessage = "El código de artículo es requerido")]
        public int CODARTICULO { get; set; }

        [Required(ErrorMessage = "La talla es requerida")]
        [StringLength(10, ErrorMessage = "La talla no puede exceder 10 caracteres")]
        public string TALLA { get; set; } = string.Empty;

        [Required(ErrorMessage = "El color es requerido")]
        [StringLength(50, ErrorMessage = "El color no puede exceder 50 caracteres")]
        public string COLOR { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "El código de barras no puede exceder 50 caracteres")]
        public string? CODBARRAS { get; set; }

        [Required(ErrorMessage = "El precio bruto es requerido")]
        [Range(0, 999999.99, ErrorMessage = "El precio bruto debe estar entre 0 y 999,999.99")]
        public decimal PBRUTO { get; set; }

        [Range(0, 100, ErrorMessage = "El descuento debe estar entre 0 y 100")]
        public decimal DTO { get; set; } = 0;

        [Required(ErrorMessage = "El precio neto es requerido")]
        [Range(0, 999999.99, ErrorMessage = "El precio neto debe estar entre 0 y 999,999.99")]
        public decimal PNETO { get; set; }

        /// <summary>
        /// Validación personalizada: Precio neto debe ser coherente con bruto y descuento
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Calcular precio neto esperado
            var precioNetoCalculado = PBRUTO * (1 - DTO / 100);

            // Permitir pequeña diferencia por redondeo (0.01)
            if (Math.Abs(PNETO - precioNetoCalculado) > 0.01m)
            {
                yield return new ValidationResult(
                    $"El precio neto ({PNETO:F2}) no coincide con el calculado ({precioNetoCalculado:F2}) basado en precio bruto y descuento",
                    new[] { nameof(PNETO) });
            }

            if (PBRUTO > 0 && PNETO > PBRUTO)
            {
                yield return new ValidationResult(
                    "El precio neto no puede ser mayor al precio bruto",
                    new[] { nameof(PNETO) });
            }
        }
    }

    public class PrecioResponseDto : PrecioDto
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Información de la tarifa asociada
        /// </summary>
        public TarifaResponseDto? Tarifa { get; set; }

        /// <summary>
        /// Información del artículo asociado
        /// </summary>
        public ArticuloResponseDto? Articulo { get; set; }

        /// <summary>
        /// Información calculada del precio
        /// </summary>
        public PrecioEstadisticas? Estadisticas { get; set; }
    }

    public class PrecioEstadisticas
    {
        public decimal AhorroAbsoluto { get; set; }
        public decimal PorcentajeAhorro { get; set; }
        public bool TieneDescuento { get; set; }
        public bool EsPrecioEspecial { get; set; }
        public string TarifaVigente { get; set; } = string.Empty;
        public bool TarifaActiva { get; set; }
        public decimal? PrecioConImpuesto { get; set; }
    }

    public class PrecioSearchRequest
    {
        public int? IdTarifa { get; set; }
        public int? CodigoArticulo { get; set; }

        [StringLength(100)]
        public string? SearchTerm { get; set; }

        public decimal? PrecioMinimo { get; set; }
        public decimal? PrecioMaximo { get; set; }
        public decimal? DescuentoMinimo { get; set; }
        public decimal? DescuentoMaximo { get; set; }

        public bool SoloTarifasActivas { get; set; } = false;
        public bool SoloConDescuento { get; set; } = false;
        public bool IncluirTarifa { get; set; } = false;
        public bool IncluirArticulo { get; set; } = false;

        [Range(1, 1000)]
        public int PageSize { get; set; } = 50;

        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;
    }

    /// <summary>
    /// Request para operaciones masivas de precios
    /// </summary>
    public class PrecioMasivoRequest
    {
        [Required]
        public List<PrecioDto> Precios { get; set; } = new List<PrecioDto>();

        public bool ValidarTarifasActivas { get; set; } = true;
        public bool ValidarArticulosExistentes { get; set; } = true;
        public bool ValidarLineasExistentes { get; set; } = true;
        public bool SobreescribirExistentes { get; set; } = true;
        public bool CalcularPrecioNetoAutomatico { get; set; } = false;
    }
}
using System.ComponentModel.DataAnnotations;

namespace ApiSAPBridge.Models.DTOs
{
    public class FormaPagoDto
    {
        [Required(ErrorMessage = "El código de forma de pago es requerido")]
        [StringLength(10, ErrorMessage = "El código no puede exceder 10 caracteres")]
        [MinLength(1, ErrorMessage = "El código debe tener al menos 1 caracter")]
        public int CODFORMAPAGO { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(255, ErrorMessage = "La descripción no puede exceder 255 caracteres")]
        [MinLength(3, ErrorMessage = "La descripción debe tener al menos 3 caracteres")]
        public string DESCRIPCION { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de vencimientos es requerido")]
        [Range(1, 99, ErrorMessage = "El número de vencimientos debe estar entre 1 y 99")]
        public int NUMVENCIMIENTOS { get; set; }
    }

    public class FormaPagoResponseDto : FormaPagoDto
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Información adicional sobre el tipo de pago
        /// </summary>
        public string TipoPago => NUMVENCIMIENTOS == 1 ? "Contado" : $"Fraccionado ({NUMVENCIMIENTOS} vencimientos)";

        /// <summary>
        /// Número de clientes que usan esta forma de pago
        /// </summary>
        public int ClientesAsociados { get; set; } = 0;
    }

    public class FormaPagoStatsDto
    {
        public int TotalFormasPago { get; set; }
        public int FormasContado { get; set; }
        public int FormasCredito { get; set; }
        public int VencimientosMaximo { get; set; }
        public double VencimientosPromedio { get; set; }
        public DateTime FechaConsulta { get; set; } = DateTime.UtcNow;
    }
}
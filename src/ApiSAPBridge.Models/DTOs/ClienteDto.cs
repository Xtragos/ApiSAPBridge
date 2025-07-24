using System.ComponentModel.DataAnnotations;

namespace ApiSAPBridge.Models.DTOs
{
    public class ClienteDto
    {
        [Required(ErrorMessage = "El código de cliente es requerido")]
        public int CODCLIENTE { get; set; }

        [StringLength(50, ErrorMessage = "El código contable no puede exceder 50 caracteres")]
        public string? CODCONTABLE { get; set; }

        [Required(ErrorMessage = "El nombre del cliente es requerido")]
        [StringLength(255, ErrorMessage = "El nombre no puede exceder 255 caracteres")]
        [MinLength(2, ErrorMessage = "El nombre debe tener al menos 2 caracteres")]
        public string NOMBRECLIENTE { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "El nombre comercial no puede exceder 255 caracteres")]
        public string? NOMBRECOMERCIAL { get; set; }

        [StringLength(50, ErrorMessage = "El CIF no puede exceder 50 caracteres")]
        [RegularExpression(@"^[A-Z]\d{8}$|^\d{8}[A-Z]$|^[A-Z]\d{7}[A-Z]$",
            ErrorMessage = "El CIF debe tener un formato válido español")]
        public string? CIF { get; set; }

        [StringLength(255, ErrorMessage = "El alias no puede exceder 255 caracteres")]
        public string? ALIAS { get; set; }

        [StringLength(500, ErrorMessage = "La dirección no puede exceder 500 caracteres")]
        public string? DIRECCION1 { get; set; }

        [StringLength(255, ErrorMessage = "La población no puede exceder 255 caracteres")]
        public string? POBLACION { get; set; }

        [StringLength(255, ErrorMessage = "La provincia no puede exceder 255 caracteres")]
        public string? PROVINCIA { get; set; }

        [StringLength(255, ErrorMessage = "El país no puede exceder 255 caracteres")]
        public string? PAIS { get; set; }

        [StringLength(50, ErrorMessage = "El teléfono no puede exceder 50 caracteres")]
        [RegularExpression(@"^(\+34|0034|34)?[6789]\d{8}$|^(\+34|0034|34)?[89]\d{8}$|^(\+34|0034|34)?[9]\d{8}$",
            ErrorMessage = "El teléfono debe tener un formato válido español")]
        public string? TELEFONO1 { get; set; }

        [StringLength(50, ErrorMessage = "El teléfono 2 no puede exceder 50 caracteres")]
        [RegularExpression(@"^(\+34|0034|34)?[6789]\d{8}$|^(\+34|0034|34)?[89]\d{8}$|^(\+34|0034|34)?[9]\d{8}$",
            ErrorMessage = "El teléfono 2 debe tener un formato válido español")]
        public string? TELEFONO2 { get; set; }

        [StringLength(255, ErrorMessage = "El email no puede exceder 255 caracteres")]
        [EmailAddress(ErrorMessage = "El email debe tener un formato válido")]
        public string? E_MAIL { get; set; }

        [Range(0, 999999.99, ErrorMessage = "El riesgo concedido debe estar entre 0 y 999,999.99")]
        public decimal RIESGOCONCEDIDO { get; set; } = 0;

        [StringLength(10, ErrorMessage = "Facturar con impuesto no puede exceder 10 caracteres")]
        public string? FACTURARCONIMPUESTO { get; set; }
    }

    public class ClienteResponseDto : ClienteDto
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Información adicional calculada
        /// </summary>
        public ClienteInfoAdicional? InfoAdicional { get; set; }
    }

    public class ClienteInfoAdicional
    {
        public bool TieneCIFValido { get; set; }
        public bool TieneEmailValido { get; set; }
        public bool TieneDireccionCompleta { get; set; }
        public string EstadoCliente { get; set; } = "Activo"; // Activo, Inactivo, Bloqueado
        public decimal RiesgoDisponible { get; set; }
    }

    public class ClienteSearchRequest
    {
        [StringLength(100, ErrorMessage = "El término de búsqueda no puede exceder 100 caracteres")]
        public string? SearchTerm { get; set; }

        [StringLength(255, ErrorMessage = "La provincia no puede exceder 255 caracteres")]
        public string? Provincia { get; set; }

        [StringLength(255, ErrorMessage = "El país no puede exceder 255 caracteres")]
        public string? Pais { get; set; }

        public decimal? RiesgoMinimo { get; set; }
        public decimal? RiesgoMaximo { get; set; }

        [Range(1, 1000, ErrorMessage = "El tamaño de página debe estar entre 1 y 1000")]
        public int PageSize { get; set; } = 50;

        [Range(1, int.MaxValue, ErrorMessage = "La página debe ser mayor a 0")]
        public int Page { get; set; } = 1;
    }
}
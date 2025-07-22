using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiSAPBridge.Models.Entities
{
    public class ApiLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public string? Level { get; set; }

        public string? Message { get; set; }

        public string? Exception { get; set; }

        public string? Properties { get; set; }

        [StringLength(255)]
        public string? EndpointCalled { get; set; }

        [StringLength(10)]
        public string? HttpMethod { get; set; }

        public string? RequestBody { get; set; }

        public int? ResponseStatus { get; set; }
    }
}

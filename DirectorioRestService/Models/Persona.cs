using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DirectorioRestService.Models
{
    public class Persona
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido paterno es obligatorio.")]
        public string ApellidoPaterno { get; set; } = string.Empty;

        public string? ApellidoMaterno { get; set; }

        [Required(ErrorMessage = "La identificación es obligatoria.")]
        public string Identificacion { get; set; } = string.Empty;

        [JsonIgnore] 
        public List<Factura> Facturas { get; set; } = new List<Factura>();
    }
}

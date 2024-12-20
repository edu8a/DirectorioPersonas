using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DirectorioRestService.Models
{
    public class Factura
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo Fecha es obligatorio.")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El campo Monto es obligatorio.")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "El campo PersonaId es obligatorio.")]
        public int PersonaId { get; set; }

        [JsonIgnore] 
        public Persona? Persona { get; set; }
    }
}

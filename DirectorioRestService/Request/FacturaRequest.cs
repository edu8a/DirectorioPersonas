using System.ComponentModel.DataAnnotations;

namespace DirectorioRestService.Models
{
    public class FacturaRequest
    {
        [Required(ErrorMessage = "El campo Fecha es obligatorio.")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El campo Monto es obligatorio.")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "El campo PersonaId es obligatorio.")]
        public int PersonaId { get; set; }
    }
}

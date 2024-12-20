namespace DirectorioWPFClient.Models
{
    public class Factura
    {
        public int Id { get; set; }
        public int PersonaId { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Monto { get; set; }
    }
}

using DirectorioRestService.Models;
using DirectorioRestService.Services;
using Microsoft.AspNetCore.Mvc;

namespace DirectorioRestService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacturasController : ControllerBase
    {
        private readonly VentasService _ventasService;
        private readonly DirectorioService _directorioService;

        public FacturasController(VentasService ventasService, DirectorioService directorioService)
        {
            _ventasService = ventasService;
            _directorioService = directorioService;

        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var facturas = await _ventasService.GetFacturasAsync();
            return Ok(facturas);
        }


        [HttpGet("{personaId}")]
        public async Task<IActionResult> GetFacturasByPersona(int personaId)
        {
            var facturas = await _ventasService.FindFacturasByPersonaAsync(personaId);
            if (facturas.Count == 0)
                return NotFound($"No se encontraron facturas para la persona con ID: {personaId}");

            return Ok(facturas);
        }
        [HttpPost]
        public async Task<IActionResult> PostFactura([FromBody] FacturaRequest facturaRequest)
        {
          

            try
            {
               
                var persona = await _directorioService.GetPersonaByIdAsync(facturaRequest.PersonaId);
                if (persona == null)
                {
                    return NotFound(new { Message = "La persona especificada no existe." });
                }

              
                var factura = new Factura
                {
                    Fecha = facturaRequest.Fecha,
                    Monto = facturaRequest.Monto,
                    PersonaId = facturaRequest.PersonaId
                };

              
                await _ventasService.AddFacturaAsync(factura);
                return CreatedAtAction(nameof(Get), new { id = factura.Id }, factura);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error al crear la factura.", Error = ex.Message });
            }
        }



    }
}

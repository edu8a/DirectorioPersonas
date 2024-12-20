using DirectorioRestService.Models;
using DirectorioRestService.Repositories;
using Microsoft.Extensions.Logging;

namespace DirectorioRestService.Services
{
    public class VentasService
    {
        private readonly IFacturaRepository _facturaRepository;
        private readonly IPersonaRepository _personaRepository;
        private readonly ILogger<VentasService> _logger;

        public VentasService(IFacturaRepository facturaRepository, IPersonaRepository personaRepository, ILogger<VentasService> logger)
        {
            _facturaRepository = facturaRepository;
            _personaRepository = personaRepository;
            _logger = logger;
        }

        public async Task<List<Factura>> GetFacturasAsync()
        {
            _logger.LogInformation("Obteniendo todas las facturas");
            return await _facturaRepository.GetAllAsync();
        }
        public async Task<List<Factura>> FindFacturasByPersonaAsync(int personaId)
        {
            _logger.LogInformation($"Obteniendo facturas de la persona con ID: {personaId}");
            var facturas = await _facturaRepository.GetAllAsync();
            return facturas.Where(f => f.PersonaId == personaId).ToList();
        }


        public async Task AddFacturaAsync(Factura factura)
        {
           
            var persona = await _personaRepository.GetByIdAsync(factura.PersonaId);
            if (persona == null)
            {
                _logger.LogError($"No se encontró una persona con ID {factura.PersonaId}");
                throw new ArgumentException("La persona asociada no existe.");
            }

            _logger.LogInformation("Agregando una nueva factura con fecha {Fecha} y monto {Monto}", factura.Fecha, factura.Monto);
            await _facturaRepository.AddAsync(factura);
        }
    }
}

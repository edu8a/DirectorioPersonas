using DirectorioRestService.Models;
using DirectorioRestService.Repositories;
using Microsoft.Extensions.Logging;

namespace DirectorioRestService.Services
{
    public class DirectorioService
    {
        private readonly IPersonaRepository _personaRepository;
        private readonly IFacturaRepository _facturaRepository;
        private readonly ILogger<DirectorioService> _logger;

        public DirectorioService(IPersonaRepository personaRepository, IFacturaRepository facturaRepository, ILogger<DirectorioService> logger)
        {
            _personaRepository = personaRepository;
            _facturaRepository = facturaRepository;
            _logger = logger;
        }

        public async Task<List<Persona>> GetPersonasAsync()
        {
            _logger.LogInformation("Obteniendo todas las personas.");
            return await _personaRepository.GetAllAsync();
        }

        public async Task AddPersonaAsync(Persona persona)
        {
            _logger.LogInformation($"Intentando crear nueva persona: {persona.Nombre} {persona.ApellidoPaterno}");

           
            var existente = await _personaRepository.GetByIdentificacionAsync(persona.Identificacion);
            if (existente != null)
            {
                throw new InvalidOperationException($"Ya existe una persona con la identificación {persona.Identificacion}.");
            }

            await _personaRepository.AddAsync(persona);
            _logger.LogInformation($"Persona {persona.Nombre} {persona.ApellidoPaterno} creada exitosamente.");
        }

        public async Task<Persona?> GetPersonaByIdAsync(int id)
        {
            return await _personaRepository.GetByIdAsync(id);
        }
        public async Task<Persona> GetPersonaByIdentificacionAsync(string identificacion)
        {
            _logger.LogInformation($"Buscando persona con identificación: {identificacion}");
            var persona = await _personaRepository.GetByIdentificacionAsync(identificacion);
            if (persona == null)
            {
                _logger.LogWarning($"No se encontró persona con identificación: {identificacion}");
            }
            return persona;
        }
        public async Task DeletePersonaByIdentificacionAsync(string identificacion)
        {
            _logger.LogInformation($"Eliminando persona con identificación {identificacion} y sus facturas asociadas.");

           
            var persona = await _personaRepository.GetByIdentificacionAsync(identificacion);
            if (persona == null)
            {
                _logger.LogWarning($"No se encontró persona con identificación: {identificacion}");
                throw new KeyNotFoundException($"Persona con identificación {identificacion} no encontrada.");
            }

            // eliminar facturas relacionadas
            var facturas = await _facturaRepository.GetAllAsync();
            foreach (var factura in facturas.Where(f => f.PersonaId == persona.Id))
            {
                await _facturaRepository.DeleteAsync(factura.Id);
            }

            // eliminar persona
            await _personaRepository.DeleteAsync(persona.Id);
            _logger.LogInformation($"Persona con identificación {identificacion} eliminada exitosamente.");
        }

    }
}

using System.Linq;
using System.Threading.Tasks;
using DirectorioRestService.Models;
using Microsoft.EntityFrameworkCore;

namespace DirectorioRestService.Repositories
{
    public class PersonaRepository : IPersonaRepository
    {
        private readonly ApplicationDbContext _context;

        public PersonaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Persona>> GetAllAsync()
        {
            return await _context.Personas.ToListAsync();
        }

        public async Task<Persona> GetByIdAsync(int id)
        {
            return await _context.Personas.FindAsync(id);
        }

        public async Task<Persona> GetByIdentificacionAsync(string identificacion)
        {
            return await _context.Personas.FirstOrDefaultAsync(p => p.Identificacion == identificacion);
        }

        public async Task AddAsync(Persona persona)
        {
            await _context.Personas.AddAsync(persona);
            await _context.SaveChangesAsync();
        }

      

        public async Task DeleteAsync(int id)
        {
            var persona = await GetByIdAsync(id);
            if (persona != null)
            {
                _context.Personas.Remove(persona);
                await _context.SaveChangesAsync();
            }
        }

    }
}

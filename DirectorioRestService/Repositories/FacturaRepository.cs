using DirectorioRestService.Models;
using Microsoft.EntityFrameworkCore;

namespace DirectorioRestService.Repositories
{
    public class FacturaRepository : IFacturaRepository
    {
        private readonly ApplicationDbContext _context;

        public FacturaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Factura>> GetAllAsync()
        {
            return await _context.Facturas
                .Include(f => f.Persona) 
                .ToListAsync();
        }


        public async Task<Factura> GetByIdAsync(int id)
        {
            return await _context.Facturas.Include(f => f.Persona).FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task AddAsync(Factura factura)
        {
            await _context.Facturas.AddAsync(factura);
            await _context.SaveChangesAsync();
        }

       

        public async Task DeleteAsync(int id)
        {
            var factura = await GetByIdAsync(id);
            if (factura != null)
            {
                _context.Facturas.Remove(factura);
                await _context.SaveChangesAsync();
            }
        }
    }
}

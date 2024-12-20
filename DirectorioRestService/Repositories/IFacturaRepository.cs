using DirectorioRestService.Models;

namespace DirectorioRestService.Repositories
{
    public interface IFacturaRepository
    {
        Task<List<Factura>> GetAllAsync();
        Task<Factura> GetByIdAsync(int id);
        Task AddAsync(Factura factura);
      
        Task DeleteAsync(int id);
    }
}

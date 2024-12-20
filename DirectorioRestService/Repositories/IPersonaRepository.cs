using DirectorioRestService.Models;

namespace DirectorioRestService.Repositories
{
    public interface IPersonaRepository
    {
        Task<List<Persona>> GetAllAsync();
        Task<Persona> GetByIdAsync(int id);
        Task AddAsync(Persona persona);
      
        Task DeleteAsync(int id);
        Task<Persona> GetByIdentificacionAsync(string identificacion);
    }

}

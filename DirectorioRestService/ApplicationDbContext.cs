using DirectorioRestService.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DirectorioRestService
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Persona> Personas { get; set; }
        public DbSet<Factura> Facturas { get; set; }
    }

}

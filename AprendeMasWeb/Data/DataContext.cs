using AprendeMasWeb.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AprendeMasWeb.Data
{
    public class DataContext : IdentityDbContext
    {
        // Constructor
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        // DbSets para las entidades
        public DbSet<GrupoRegistro> Grupos { get; set; }
        public DbSet<MateriaRegistro> Materias { get; set; }
        public DbSet<GruposMaterias> GruposMaterias { get; set; }
        public DbSet<Actividades> Actividades { get; set; }
    }
}

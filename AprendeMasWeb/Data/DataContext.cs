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
        public DbSet<GrupoRegistro> tbGrupos { get; set; }
        public DbSet<MateriaRegistro> tbMaterias { get; set; }
        public DbSet<GruposMaterias> tbGruposMaterias { get; set; }
        public DbSet<Actividades> tbActividades { get; set; }
    }
}

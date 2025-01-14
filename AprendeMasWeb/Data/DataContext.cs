using AprendeMasWeb.Models;
using AprendeMasWeb.Models.DBModels;
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
        public DbSet<Alumnos> tbAlumnos { get; set; }
        public DbSet<AlumnosTokens> tbAlumnosTokens { get; set; }
        public DbSet<Docentes> tbDocentes { get; set; }
        public DbSet<AlumnosGrupos> tbAlumnosGrupos { get; set; }
        public DbSet<AlumnosMaterias> tbAlumnosMaterias { get; set; }
        public DbSet<AlumnosActividades> tbAlumnosActividades { get; set; }
        public DbSet<EntregablesAlumno> tbEntregablesAlumno {  get; set; }
        public DbSet<Grupos> tbGrupos { get; set; }
        public DbSet<GruposMaterias> tbGruposMaterias { get; set; }
        public DbSet<Materias> tbMaterias { get; set; }
        public  DbSet<MateriasActividades> tbMateriasActividades { get; set;}
        public DbSet<Actividades> tbActividades { get; set; }
        public DbSet<Calificaciones> tbCalificaciones {  get; set; }
        public DbSet<TiposActividades> cTiposActividades { get; set; }
        public DbSet<Avisos> tbAvisos { get; set; }
        public DbSet<EventosAgenda> tbEventosAgenda { get; set; }
        public DbSet<EventosGrupos> tbEventosGrupos { get; set; }
        public DbSet<EventosMaterias> tbEventosMaterias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //AlumnosTokens
            modelBuilder.Entity<AlumnosTokens>()
                .HasOne(a => a.Alumnos)
                .WithMany(a=>a.AlumnosTokens)
                .HasForeignKey(a=>a.AlumnoId)
                .OnDelete(DeleteBehavior.NoAction);

            //AlumnosGrupos
            modelBuilder.Entity<AlumnosGrupos>()
                .HasOne(a => a.Alumnos)
                .WithMany(a => a.AlumnosGrupos)
                .HasForeignKey(a => a.AlumnoId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AlumnosGrupos>()
                .HasOne(a => a.Grupos)
                .WithMany(a => a.AlumnosGrupos)
                .HasForeignKey(a => a.GrupoId)
                .OnDelete(DeleteBehavior.NoAction);

            //Alumnos Materias
            modelBuilder.Entity<AlumnosMaterias>()
                .HasOne(a => a.Alumnos)
                .WithMany(a => a.AlumnosMaterias)
                .HasForeignKey(a => a.AlumnoId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AlumnosMaterias>()
                .HasOne(a => a.Materias)
                .WithMany(a => a.AlumnosMaterias)
                .HasForeignKey(a => a.MateriaId)
                .OnDelete(DeleteBehavior.NoAction);

            //Alumnos Actividades
            modelBuilder.Entity<AlumnosActividades>()
                .HasOne(a => a.Alumnos)
                .WithMany(a => a.AlumnosActividades)
                .HasForeignKey(a => a.ActividadId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<AlumnosActividades>()
                .HasOne(a => a.Actividades)
                .WithMany(a => a.AlumnosActividades)
                .HasForeignKey(a => a.ActividadId)
                .OnDelete(DeleteBehavior.NoAction);


            //Entregables Alumno
            //modelBuilder.Entity<EntregablesAlumno>()
            //    .HasOne(a => a.AlumnosActividades)
            //    .WithOne(a => a.EntregablesAlumno)
            //    .HasForeignKey<AlumnosActividades>(a => a.AlumnoActividadId)
            //    .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AlumnosActividades>()
                .HasOne(a=>a.EntregablesAlumno)
                .WithOne(a=>a.AlumnosActividades)
                .HasForeignKey<EntregablesAlumno>(a=>a.AlumnoActividadId)
                .OnDelete(DeleteBehavior.NoAction);

            //Grupos
            modelBuilder.Entity<Grupos>()
                .HasOne(a => a.Docentes)
                .WithMany(a => a.Grupos)
                .HasForeignKey(a => a.DocenteId)
                .OnDelete(DeleteBehavior.NoAction);

            //Grupos Materias
            modelBuilder.Entity<GruposMaterias>()
                .HasOne(a => a.Grupos)
                .WithMany(a => a.GruposMaterias)
                .HasForeignKey(a => a.GrupoId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<GruposMaterias>()
                .HasOne(a => a.Materias)
                .WithMany(a => a.GruposMaterias)
                .HasForeignKey(a => a.MateriaId)
                .OnDelete(DeleteBehavior.NoAction);



            //Materias
            modelBuilder.Entity<Materias>()
                .HasOne(a => a.Docentes)
                .WithMany(a => a.Materias)
                .HasForeignKey(a => a.DocenteId)
                .OnDelete(DeleteBehavior.NoAction);

            //Materias actividades
            modelBuilder.Entity<MateriasActividades>()
                .HasOne(a => a.Materias)
                .WithMany(a => a.MateriasActividades)
                .HasForeignKey(a => a.MateriaId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<MateriasActividades>()
                .HasOne(a => a.Actividades)
                .WithMany(a => a.MateriasActividades)
                .HasForeignKey(a => a.ActividadId)
                .OnDelete(DeleteBehavior.NoAction);


            //Actividades
            modelBuilder.Entity<Actividades>()
                .HasOne(a => a.TiposActividades)
                .WithMany(a => a.Actividades)
                .HasForeignKey(a=>a.TipoActividadId)
                .OnDelete(DeleteBehavior.NoAction);

            //Calificaciones
            modelBuilder.Entity<Calificaciones>()
                .HasOne(a => a.Actividades)
                .WithMany(a => a.Calificaciones)
                .HasForeignKey(a => a.ActividadId)
                .OnDelete(DeleteBehavior.NoAction);







            //Avisos
            modelBuilder.Entity<Avisos>()
                .HasOne(a => a.Docentes)
                .WithMany(a => a.Avisos)
                .HasForeignKey(a => a.DocenteId)
                .OnDelete(DeleteBehavior.Cascade);

            //Eventos agenda
            modelBuilder.Entity<EventosAgenda>()
                .HasOne(a => a.Docentes)
                .WithMany(a => a.EventosAgendas)
                .HasForeignKey(a => a.DocenteId)
                .OnDelete(DeleteBehavior.Cascade);


            //Eventos grupos
            modelBuilder.Entity<EventosGrupos>()
                .HasOne(a=>a.Grupos)
                .WithMany(a=>a.EventosGrupos)
                .HasForeignKey(a=>a.GrupoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventosGrupos>()
                .HasOne(a=>a.EventosAgenda)
                .WithMany(a=>a.EventosGrupos)
                .HasForeignKey(a=>a.FechaId)
                .OnDelete(DeleteBehavior.Cascade);

            //Eventos materias
            modelBuilder.Entity<EventosMaterias>()
                .HasOne(a=>a.Materias)
                .WithMany(a=>a.EventosMaterias)
                .HasForeignKey(a=>a.MateriaId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<EventosMaterias>()
                .HasOne(a=>a.EventosAgenda)
                .WithMany(a=>a.EventosMaterias)
                .HasForeignKey(a=>a.FechaId)
                .OnDelete(DeleteBehavior.Cascade);



            base.OnModelCreating(modelBuilder);
        }



    }
}

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
		public DbSet<Administrador> tbAdministradores { get; set; } // Nueva tabla Administrador
		public DbSet<tbUsuariosFcmTokens> tbUsuariosFcmTokens { get; set; }
        public DbSet<tbAlumnos> tbAlumnos { get; set; }
        public DbSet<tbDocentes> tbDocentes { get; set; }
        public DbSet<tbAlumnosGrupos> tbAlumnosGrupos { get; set; }
        public DbSet<tbAlumnosMaterias> tbAlumnosMaterias { get; set; }
        public DbSet<tbAlumnosActividades> tbAlumnosActividades { get; set; }
        public DbSet<tbEntregablesAlumno> tbEntregablesAlumno { get; set; }
        public DbSet<tbGrupos> tbGrupos { get; set; }
        public DbSet<tbGruposMaterias> tbGruposMaterias { get; set; }
        public DbSet<tbMaterias> tbMaterias { get; set; }
        public DbSet<tbActividades> tbActividades { get; set; }
        public DbSet<tbCalificaciones> tbCalificaciones { get; set; }
        public DbSet<cTiposActividades> cTiposActividades { get; set; }
        public DbSet<tbAvisos> tbAvisos { get; set; }
        public DbSet<tbEventosAgenda> tbEventosAgenda { get; set; }
		public DbSet<EventosAgendaAlumno> tbEventosAgendaAlumno { get; set; }
		public DbSet<tbEventosGrupos> tbEventosGrupos { get; set; }
        public DbSet<tbEventosMaterias> tbEventosMaterias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            #region UsuarioFcmTokens
            modelBuilder.Entity<tbUsuariosFcmTokens>()
                .HasOne(a=>a.IdentityUser)
                .WithMany()
                .HasForeignKey(a=>a.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            #endregion

            #region AlumnosGrupos
            modelBuilder.Entity<tbAlumnosGrupos>()
                .HasOne(a => a.Alumnos)
                .WithMany(a => a.AlumnosGrupos)
                .HasForeignKey(a => a.AlumnoId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<tbAlumnosGrupos>()
                .HasOne(a => a.Grupos)
                .WithMany(a => a.AlumnosGrupos)
                .HasForeignKey(a => a.GrupoId)
                .OnDelete(DeleteBehavior.NoAction);
            #endregion

            #region  Alumnos Materias
            modelBuilder.Entity<tbAlumnosMaterias>()
                .HasOne(a => a.Alumnos)
                .WithMany(a => a.AlumnosMaterias)
                .HasForeignKey(a => a.AlumnoId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<tbAlumnosMaterias>()
                .HasOne(a => a.Materias)
                .WithMany(a => a.AlumnosMaterias)
                .HasForeignKey(a => a.MateriaId)
                .OnDelete(DeleteBehavior.NoAction);
            #endregion

            #region Alumnos Actividades
            modelBuilder.Entity<tbAlumnosActividades>()
                .HasOne(a => a.Alumnos)
                .WithMany(a => a.AlumnosActividades)
                .HasForeignKey(a => a.AlumnoId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<tbAlumnosActividades>()
                .HasOne(a => a.Actividades)
                .WithMany(a => a.AlumnosActividades)
                .HasForeignKey(a => a.ActividadId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<tbAlumnosActividades>()
                .HasOne(a => a.EntregablesAlumno)
                .WithOne(a => a.AlumnosActividades)
                .HasForeignKey<tbEntregablesAlumno>(a => a.AlumnoActividadId)
                .OnDelete(DeleteBehavior.NoAction);
            #endregion

            #region Grupos
            modelBuilder.Entity<tbGrupos>()
                .HasOne(a => a.Docentes)
                .WithMany(a => a.Grupos)
                .HasForeignKey(a => a.DocenteId)
                .OnDelete(DeleteBehavior.NoAction);
            #endregion

            #region Grupos Materias
            modelBuilder.Entity<tbGruposMaterias>()
                .HasOne(a => a.Grupos)
                .WithMany(a => a.GruposMaterias)
                .HasForeignKey(a => a.GrupoId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<tbGruposMaterias>()
                .HasOne(a => a.Materias)
                .WithMany(a => a.GruposMaterias)
                .HasForeignKey(a => a.MateriaId)
                .OnDelete(DeleteBehavior.NoAction);
            #endregion

            #region Materias
            modelBuilder.Entity<tbMaterias>()
                .HasOne(a => a.Docentes)
                .WithMany(a => a.Materias)
                .HasForeignKey(a => a.DocenteId)
                .OnDelete(DeleteBehavior.NoAction);
            #endregion

            #region Actividades
            modelBuilder.Entity<tbActividades>()
                .HasOne(a => a.TiposActividades)
                .WithMany(a => a.Actividades)
                .HasForeignKey(a => a.TipoActividadId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<tbActividades>()
                .HasOne(a => a.Materias)
                .WithMany(a => a.Actividades)
                .HasForeignKey(a => a.MateriaId)
                .OnDelete(DeleteBehavior.NoAction);
            #endregion

            #region Calificaciones
            modelBuilder.Entity<tbCalificaciones>()
                .HasOne(a => a.EntregablesAlumno)
                .WithMany(a => a.Calificaciones)
                .HasForeignKey(a => a.EntregaId)
                .OnDelete(DeleteBehavior.NoAction);
            #endregion

            #region Avisos
            modelBuilder.Entity<tbAvisos>()
                .HasOne(a => a.Docentes)
                .WithMany(a => a.Avisos)
                .HasForeignKey(a => a.DocenteId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region Eventos agenda
            modelBuilder.Entity<tbEventosAgenda>()
                .HasOne(a => a.Docentes)
                .WithMany(a => a.EventosAgendas)
                .HasForeignKey(a => a.DocenteId)
                .OnDelete(DeleteBehavior.Cascade);
			#endregion

			modelBuilder.Entity<EventosAgendaAlumno>()
			.HasOne(e => e.Alumno)
			.WithMany()
			.HasForeignKey(e => e.AlumnoId)
			.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Administrador>()
				.HasIndex(a => a.Email)
				.IsUnique();

			#region Eventos grupos
			modelBuilder.Entity<tbEventosGrupos>()
                .HasOne(a => a.Grupos)
                .WithMany(a => a.EventosGrupos)
                .HasForeignKey(a => a.GrupoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<tbEventosGrupos>()
                .HasOne(a => a.EventosAgenda)
                .WithMany(a => a.EventosGrupos)
                .HasForeignKey(a => a.FechaId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region Eventos materias
            modelBuilder.Entity<tbEventosMaterias>()
                .HasOne(a => a.Materias)
                .WithMany(a => a.EventosMaterias)
                .HasForeignKey(a => a.MateriaId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<tbEventosMaterias>()
                .HasOne(a => a.EventosAgenda)
                .WithMany(a => a.EventosMaterias)
                .HasForeignKey(a => a.FechaId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion


            base.OnModelCreating(modelBuilder);
        }



    }
}

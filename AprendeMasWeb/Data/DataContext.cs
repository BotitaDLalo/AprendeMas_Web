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

		public DbSet<Alumno> Alumnos { get; set; }
		public DbSet<Docente> Docentes { get; set; }
		public DbSet<TipoActividad> TiposActividades { get; set; }
		public DbSet<RubricaEvaluacion> RubricasEvaluacion { get; set; }
		public DbSet<Notificacion> Notificaciones { get; set; }
		public DbSet<Grupo> Grupos { get; set; }
		public DbSet<Materia> Materias { get; set; }
		public DbSet<GrupoMateria> GruposMaterias { get; set; }
		public DbSet<Actividad> Actividades { get; set; }
		public DbSet<Tarea> Tareas { get; set; }
		public DbSet<Archivo> Archivos { get; set; }
		public DbSet<AlumnoActividad> AlumnosActividades { get; set; }
		public DbSet<Calificacion> Calificaciones { get; set; }
		public DbSet<Aviso> Avisos { get; set; }
		public DbSet<EventoAgenda> EventosAgenda { get; set; }
		public DbSet<Examen> Examenes { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Relación Grupo-Materia
			modelBuilder.Entity<GrupoMateria>()
				.HasOne(gm => gm.Grupo)
				.WithMany(g => g.GrupoMaterias)
				.HasForeignKey(gm => gm.GrupoId)
				.OnDelete(DeleteBehavior.NoAction); // Evita la cascada

			modelBuilder.Entity<GrupoMateria>()
				.HasOne(gm => gm.Materia)
				.WithMany(m => m.GrupoMaterias)
				.HasForeignKey(gm => gm.MateriaId)
				.OnDelete(DeleteBehavior.NoAction); // Evita la cascada


			// Relación Actividad-TipoActividad
			modelBuilder.Entity<Actividad>()
				.HasOne(a => a.TipoActividad)
				.WithMany(ta => ta.Actividades)
				.HasForeignKey(a => a.TipoActividadId);

			// Relación Actividad-Rubrica
			modelBuilder.Entity<Actividad>()
				.HasOne(a => a.Rubrica)
				.WithMany(r => r.Actividades)
				.HasForeignKey(a => a.RubricaId);

			// Relación AlumnoActividad
			modelBuilder.Entity<AlumnoActividad>()
				.HasOne(aa => aa.Alumno)
				.WithMany(a => a.AlumnoActividades)
				.HasForeignKey(aa => aa.AlumnoId);

			modelBuilder.Entity<AlumnoActividad>()
				.HasOne(aa => aa.Actividad)
				.WithMany(a => a.AlumnoActividades)
				.HasForeignKey(aa => aa.ActividadId);

			// Relación Actividad-Calificaciones
			modelBuilder.Entity<Calificacion>()
				.HasOne(c => c.Alumno)
				.WithMany(a => a.Calificaciones)
				.HasForeignKey(c => c.AlumnoId);

			modelBuilder.Entity<Calificacion>()
				.HasOne(c => c.Actividad)
				.WithMany(a => a.Calificaciones)
				.HasForeignKey(c => c.ActividadId);

			// Relación Actividad-Tareas
			modelBuilder.Entity<Tarea>()
				.HasOne(t => t.Actividad)
				.WithMany(a => a.Tareas)
				.HasForeignKey(t => t.ActividadId);

			// Relación Actividad-Archivos
			modelBuilder.Entity<Archivo>()
				.HasOne(ar => ar.Actividad)
				.WithMany(a => a.Archivos)
				.HasForeignKey(ar => ar.ActividadId);

			// Relación Examen-Rubrica
			modelBuilder.Entity<Examen>()
				.HasOne(e => e.Rubrica)
				.WithMany(r => r.Examenes)
				.HasForeignKey(e => e.RubricaId);

			modelBuilder.Entity<EventoAgenda>()
				.HasKey(e => e.FechaId); // Define la clave primaria


			base.OnModelCreating(modelBuilder);
		}
	}
}

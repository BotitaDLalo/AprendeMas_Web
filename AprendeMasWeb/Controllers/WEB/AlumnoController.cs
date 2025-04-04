using AprendeMasWeb.Data;
using AprendeMasWeb.Models.DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AprendeMasWeb.Controllers.WEB
{
    [Authorize(Roles = "Alumno")] // Restringir acceso solo a usuarios con rol Alumno
    public class AlumnoController : Controller
    {
        private readonly DataContext _context;

        public AlumnoController(DataContext context)
        {
            _context = context;
        }

        // Acción para mostrar la vista principal del alumno
        public async Task<IActionResult> Index(int alumnoId)
        {
            var grupos = await _context.tbAlumnosGrupos
                .Where(ag => ag.AlumnoId == alumnoId)
                .Include(ag => ag.Grupos)
                .Select(ag => ag.Grupos)
                .ToListAsync();

            var materias = await _context.tbAlumnosMaterias
                .Where(am => am.AlumnoId == alumnoId)
                .Include(am => am.Materias)
                .Select(am => am.Materias)
                .ToListAsync();

            var clases = new
            {
                Grupos = grupos,
                Materias = materias
            };

            return View(clases);
        }

        // API para obtener las clases del alumno en formato JSON
        [HttpGet("api/Alumno/Clases/{alumnoId}")]
        public async Task<IActionResult> ObtenerClases(int alumnoId)
        {
            var grupos = await _context.tbAlumnosGrupos
                .Where(ag => ag.AlumnoId == alumnoId)
                .Include(ag => ag.Grupos)
                .Select(ag => new
                {
                    Id = ag.Grupos.GrupoId,
                    Nombre = ag.Grupos.NombreGrupo,
                    esGrupo = true,
                    Materias = _context.tbGruposMaterias
                        .Where(gm => gm.GrupoId == ag.Grupos.GrupoId)
                        .Select(gm => new
                        {
                            Id = gm.MateriaId,
                            Nombre = gm.Materias.NombreMateria
                        }).ToList()
                })
                .ToListAsync();

            var materias = await _context.tbAlumnosMaterias
                .Where(am => am.AlumnoId == alumnoId)
                .Include(am => am.Materias)
                .Select(am => new
                {
                    Id = am.Materias.MateriaId,
                    Nombre = am.Materias.NombreMateria,
                    esGrupo = false,
                    Materias = (List<object>)null // Se agrega esta línea para hacer el tipo compatible
                })
                .ToListAsync();

            var clases = grupos.Cast<object>().Concat(materias.Cast<object>()).ToList();
            return Ok(clases);
        }





        public async Task<IActionResult> Clase(string tipo, string nombre)
        {
            if (string.IsNullOrEmpty(tipo) || string.IsNullOrEmpty(nombre))
            {
                return BadRequest("Parámetros inválidos.");
            }

            if (tipo.ToLower() == "grupo")
            {
                var grupo = await _context.tbGrupos.FirstOrDefaultAsync(g => g.NombreGrupo == nombre);
                if (grupo == null) return NotFound("Grupo no encontrado.");
                return View("DetalleGrupo", grupo);
            }
            else if (tipo.ToLower() == "materia")
            {
                var materia = await _context.tbMaterias.FirstOrDefaultAsync(m => m.NombreMateria == nombre);
                if (materia == null) return NotFound("Materia no encontrada.");
                return View("DetalleMateria", materia);
            }

            return BadRequest("Tipo de clase no válido.");
        }

        //public IActionResult Index()
        //      {
        //          return View();
        //      }

        public IActionResult DetalleMateria()
        {
            return View();
        }

        public IActionResult DetalleGrupo()
        {
            return View();
        }



		public async Task<IActionResult> Avisos(int alumnoId, int? materiaId)
		{
			var query = _context.tbAvisos.AsQueryable();

			if (materiaId.HasValue)
			{
				// Filtra solo por la materia seleccionada
				query = query.Where(a => a.MateriaId == materiaId.Value);
			}
			else
			{
				// Filtra por las materias y grupos del alumno
				var grupoIds = await _context.tbAlumnosGrupos
					.Where(ag => ag.AlumnoId == alumnoId)
					.Select(ag => ag.GrupoId)
					.ToListAsync();

				var materiaIds = await _context.tbAlumnosMaterias
					.Where(am => am.AlumnoId == alumnoId)
					.Select(am => am.MateriaId)
					.ToListAsync();

				// Usar la propiedad de tipo nullable correctamente
				query = query.Where(a => grupoIds.Contains(a.GrupoId.GetValueOrDefault()) ||
										 materiaIds.Contains(a.MateriaId.GetValueOrDefault()));
			}

			var avisos = await query.ToListAsync();

			return PartialView("_Avisos", avisos);
		}


		[AllowAnonymous]
		[HttpGet("api/Alumno/Avisos/{alumnoId}/{materiaId?}")]
		public async Task<IActionResult> ObtenerAvisos(int alumnoId, int? materiaId)
		{
			var query = _context.tbAvisos.AsQueryable();

			if (materiaId.HasValue) // Si se selecciona una materia, filtrar solo por ella
			{
				query = query.Where(a => a.MateriaId == materiaId.Value);
			}
			else
			{
				var grupoIds = await _context.tbAlumnosGrupos
					.Where(ag => ag.AlumnoId == alumnoId)
					.Select(ag => ag.GrupoId)
					.ToListAsync();

				var materiaIds = await _context.tbAlumnosMaterias
					.Where(am => am.AlumnoId == alumnoId)
					.Select(am => am.MateriaId)
					.ToListAsync();

				query = query.Where(a => grupoIds.Contains(a.GrupoId.GetValueOrDefault()) ||
										 materiaIds.Contains(a.MateriaId.GetValueOrDefault()));
			}

			var avisos = await query
				.Include(a => a.Docentes) // Asegurar la relación con Docentes
				.Select(a => new
				{
					a.AvisoId,
					a.Titulo,
					a.Descripcion,
					a.FechaCreacion,
					DocenteNombre = a.Docentes != null
						? a.Docentes.Nombre + " " + a.Docentes.ApellidoPaterno + " " + a.Docentes.ApellidoMaterno
						: "Desconocido"
				})
				.ToListAsync();

			if (!avisos.Any())
			{
				return NotFound(new { mensaje = "No hay avisos para esta materia." });
			}

			return Ok(avisos);
		}


        public IActionResult Actividades()
        {
            return PartialView("_Actividades");
        }

        [AllowAnonymous]
        [HttpGet("api/Alumno/Actividades/{materiaId}")]
        public async Task<IActionResult> ObtenerActividades(int materiaId)
        {
            var actividades = await _context.tbActividades
                .Where(a => a.MateriaId == materiaId)
                .OrderByDescending(a => a.FechaCreacion) // Ordenar por fecha de creación, las más recientes primero
                .Select(a => new
                {
                    a.ActividadId,
                    a.NombreActividad,
                    a.Descripcion,
                    a.FechaCreacion,
                    a.FechaLimite,
                    a.Puntaje,
                    TipoActividad = a.TiposActividades != null ? a.TiposActividades.Nombre : "Sin tipo"
                })
                .ToListAsync();

            if (!actividades.Any())
            {
                return NotFound(new { mensaje = "No hay actividades registradas para esta materia." });
            }

            return Ok(actividades);
        }


		[AllowAnonymous]
		[HttpGet("api/Materias/ObtenerIdPorNombre")]
		public async Task<IActionResult> ObtenerIdPorNombre(string nombre)
		{
			var materia = await _context.tbMaterias
				.Where(m => m.NombreMateria == nombre)
				.Select(m => new { materiaId = m.MateriaId })
				.FirstOrDefaultAsync();

			if (materia == null)
				return NotFound(new { mensaje = "Materia no encontrada." });

			return Ok(materia);
		}

		public class EntregaRequest
		{
			public int AlumnoId { get; set; }
			public int ActividadId { get; set; }
			public string Respuesta { get; set; } = string.Empty;
		}

		[HttpPost("api/Alumno/EntregarActividad")]
		public async Task<IActionResult> EntregarActividad([FromBody] EntregaRequest entrega)
		{
			// Verifica si ya se entregó
			var yaExiste = await _context.tbAlumnosActividades
				.AnyAsync(e => e.AlumnoId == entrega.AlumnoId && e.ActividadId == entrega.ActividadId);

			if (yaExiste)
				return BadRequest(new { mensaje = "Ya has entregado esta actividad. " + entrega.AlumnoId + "::" + entrega.ActividadId });

			var alumnoActividad = new tbAlumnosActividades
			{
				AlumnoId = entrega.AlumnoId,
				ActividadId = entrega.ActividadId,
				FechaEntrega = DateTime.Now,
				EstatusEntrega = true
			};

			_context.tbAlumnosActividades.Add(alumnoActividad);
			await _context.SaveChangesAsync();

			var entregable = new tbEntregablesAlumno
			{
				AlumnoActividadId = alumnoActividad.AlumnoActividadId,
				Respuesta = entrega.Respuesta
			};

			_context.tbEntregablesAlumno.Add(entregable);
			await _context.SaveChangesAsync();

			return Ok(new { mensaje = "Actividad entregada correctamente." });
		}





		public IActionResult Calificaciones()
        {
            return PartialView("_Calificaciones");
        }


   

    }



}
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



        public IActionResult Avisos()
        {
            return PartialView("_Avisos");
        }


        [AllowAnonymous]
        [HttpGet("api/Avisos/Materia/{materiaId}")]
        public async Task<IActionResult> ObtenerAvisosPorMateria(int materiaId)
        {
            var avisos = await _context.tbAvisos
                .Where(a => a.MateriaId == materiaId)
                .OrderByDescending(a => a.FechaCreacion)
                .Select(a => new
                {
                    a.AvisoId,
                    a.Titulo,
                    a.Descripcion,
                    a.FechaCreacion,
                    a.DocenteId // si es necesario mostrar quién lo publicó
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
		[HttpGet("api/Alumno/Actividades/{materiaId}/{alumnoId}")]
		public async Task<IActionResult> ObtenerActividades(int materiaId, int alumnoId)
		{
			var actividades = await _context.tbActividades
				.Where(a => a.MateriaId == materiaId)
				.OrderByDescending(a => a.FechaCreacion)
				.Select(a => new
				{
					a.ActividadId,
					a.NombreActividad,
					a.Descripcion,
					a.FechaCreacion,
					a.FechaLimite,
					a.Puntaje,
					TipoActividad = a.TiposActividades != null ? a.TiposActividades.Nombre : "Sin tipo",
					Respuesta = _context.tbEntregablesAlumno
				.Where(e => e.AlumnosActividades!.AlumnoId == alumnoId && e.AlumnosActividades.ActividadId == a.ActividadId)
				.Select(e => e.Respuesta)
				.FirstOrDefault(),

					Calificacion = _context.tbCalificaciones
				.Where(c => c.EntregablesAlumno!.AlumnosActividades!.AlumnoId == alumnoId &&
							c.EntregablesAlumno.AlumnosActividades.ActividadId == a.ActividadId)
				.Select(c => new { c.Calificacion, c.Comentarios })
				.FirstOrDefault()
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

			var actividad = await _context.tbActividades
	   .FirstOrDefaultAsync(a => a.ActividadId == entrega.ActividadId);

			if (actividad == null)
				return NotFound(new { mensaje = "Actividad no encontrada." });

			if (DateTime.Now > actividad.FechaLimite)
				return BadRequest(new { mensaje = "La actividad ya no puede ser entregada. Fecha límite superada." });

			var alumnoActividad = await _context.tbAlumnosActividades
				.FirstOrDefaultAsync(e => e.AlumnoId == entrega.AlumnoId && e.ActividadId == entrega.ActividadId);

			if (alumnoActividad == null)
			{
				// Primera vez que entrega
				alumnoActividad = new tbAlumnosActividades
				{
					AlumnoId = entrega.AlumnoId,
					ActividadId = entrega.ActividadId,
					FechaEntrega = DateTime.Now,
					EstatusEntrega = true
				};

				_context.tbAlumnosActividades.Add(alumnoActividad);
				await _context.SaveChangesAsync();
			}

			// Buscar si ya hay un entregable
			var entregable = await _context.tbEntregablesAlumno
				.FirstOrDefaultAsync(e => e.AlumnoActividadId == alumnoActividad.AlumnoActividadId);

			if (entregable != null)
			{
				entregable.Respuesta = entrega.Respuesta; // 👈 Actualiza la respuesta
				_context.tbEntregablesAlumno.Update(entregable);
			}
			else
			{
				entregable = new tbEntregablesAlumno
				{
					AlumnoActividadId = alumnoActividad.AlumnoActividadId,
					Respuesta = entrega.Respuesta
				};
				_context.tbEntregablesAlumno.Add(entregable);
			}

			await _context.SaveChangesAsync();

			return Ok(new { mensaje = "Actividad entregada correctamente." });
		}




		// Salir de una materia
		[HttpDelete("api/Alumno/SalirDeMateria/{materiaId}/{alumnoId}")]
		public async Task<IActionResult> SalirDeMateria(int materiaId, int alumnoId)
		{
			var registro = await _context.tbAlumnosMaterias
				.FirstOrDefaultAsync(am => am.MateriaId == materiaId && am.AlumnoId == alumnoId);

			if (registro == null)
				return NotFound(new { mensaje = "No estás inscrito en esta materia." });

			_context.tbAlumnosMaterias.Remove(registro);
			await _context.SaveChangesAsync();

			return Ok(new { mensaje = "Saliste de la materia." });
		}

		// Salir de un grupo
		[HttpDelete("api/Alumno/SalirDeGrupo/{grupoId}/{alumnoId}")]
		public async Task<IActionResult> SalirDeGrupo(int grupoId, int alumnoId)
		{
			var registro = await _context.tbAlumnosGrupos
				.FirstOrDefaultAsync(ag => ag.GrupoId == grupoId && ag.AlumnoId == alumnoId);

			if (registro == null)
				return NotFound(new { mensaje = "No estás inscrito en este grupo." });

			_context.tbAlumnosGrupos.Remove(registro);
			await _context.SaveChangesAsync();

			return Ok(new { mensaje = "Saliste del grupo." });
		}






	}



}
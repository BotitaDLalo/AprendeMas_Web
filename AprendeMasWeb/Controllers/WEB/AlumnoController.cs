using AprendeMasWeb.Data;
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
                .Select(ag => new { Id = ag.Grupos.GrupoId, Nombre = ag.Grupos.NombreGrupo, esGrupo = true })
                .ToListAsync();

            var materias = await _context.tbAlumnosMaterias
                .Where(am => am.AlumnoId == alumnoId)
                .Include(am => am.Materias)
                .Select(am => new { Id = am.Materias.MateriaId, Nombre = am.Materias.NombreMateria, esGrupo = false })
                .ToListAsync();

            var clases = grupos.Concat(materias);
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

		public async Task<IActionResult> Avisos(int alumnoId)
		{
			var avisos = await _context.tbAvisos
				.Where(a => _context.tbAlumnosGrupos.Any(ag => ag.AlumnoId == alumnoId && ag.GrupoId == a.GrupoId)
						 || _context.tbAlumnosMaterias.Any(am => am.AlumnoId == alumnoId && am.MateriaId == a.MateriaId))
				.ToListAsync();

			return PartialView("_Avisos", avisos);
		}


        [AllowAnonymous] // Permitir acceso sin autenticación
        [HttpGet("api/Alumno/Avisos/{alumnoId}")]
        public async Task<IActionResult> ObtenerAvisos(int alumnoId)
        {
            var avisos = await _context.tbAvisos
                .Where(a => _context.tbAlumnosGrupos.Any(ag => ag.AlumnoId == alumnoId && ag.GrupoId == a.GrupoId)
                         || _context.tbAlumnosMaterias.Any(am => am.AlumnoId == alumnoId && am.MateriaId == a.MateriaId))
                .Select(a => new
                {
                    a.AvisoId,
                    a.Titulo,
                    a.Descripcion,
                    a.FechaCreacion
                })
                .ToListAsync();

            if (!avisos.Any())
            {
                return NotFound("No hay avisos para este alumno.");
            }

            return Ok(avisos);
        }




        public IActionResult Actividades()
		{
			return PartialView("_Actividades");
		}

		public IActionResult Alumnos()
		{
			return PartialView("_Alumnos");
		}

		public IActionResult Calificaciones()
		{
			return PartialView("_Calificaciones");
		}





		public IActionResult Perfil()
        {
            // Lógica para mostrar el perfil del alumno
            return View();
        }

       

        public IActionResult Materia()
        {
            // Lógica para mostrar las actividades del alumno
            return View();
        }
    }
}
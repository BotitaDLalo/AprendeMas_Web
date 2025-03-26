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
                query = query.Where(a => a.MateriaId == materiaId.Value);
            }
            else
            {
                query = query.Where(a => _context.tbAlumnosGrupos.Any(ag => ag.AlumnoId == alumnoId && ag.GrupoId == a.GrupoId)
                                     || _context.tbAlumnosMaterias.Any(am => am.AlumnoId == alumnoId && am.MateriaId == a.MateriaId));
            }

            var avisos = await query.ToListAsync();

            return PartialView("_Avisos", avisos);
        }


        [AllowAnonymous]
        [HttpGet("api/Alumno/Avisos/{alumnoId}/{materiaId?}")]
        public async Task<IActionResult> ObtenerAvisos(int alumnoId, int? materiaId)
        {
            var grupoIds = await _context.tbAlumnosGrupos
                .Where(ag => ag.AlumnoId == alumnoId)
                .Select(ag => ag.GrupoId)
                .ToListAsync();

            var materiaIds = await _context.tbAlumnosMaterias
                .Where(am => am.AlumnoId == alumnoId)
                .Select(am => am.MateriaId)
                .ToListAsync();

            // Filtrado de avisos
            var avisosQuery = _context.tbAvisos.AsQueryable();

            if (materiaId.HasValue)
            {
                avisosQuery = avisosQuery.Where(a => a.MateriaId == materiaId);
            }
            else
            {
                avisosQuery = avisosQuery.Where(a => grupoIds.Contains(a.GrupoId.GetValueOrDefault()) ||
                                                     materiaIds.Contains(a.MateriaId.GetValueOrDefault()));
            }

            var avisos = await avisosQuery.Select(a => new
            {
                a.AvisoId,
                a.Titulo,
                a.Descripcion,
                a.FechaCreacion
            }).ToListAsync();

            if (!avisos.Any())
            {
                return NotFound(new { mensaje = "No hay avisos para este alumno." });
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



        public async Task<IActionResult> Alumnos(int materiaId)
        {
            var inscritos = await _context.tbAlumnosMaterias
                .Where(am => am.MateriaId == materiaId)
                .ToListAsync();

            // Agregar log para verificar el contenido de inscritos
            Console.WriteLine($"Alumnos inscritos en la materia {materiaId}: {inscritos.Count}");

            if (inscritos == null || !inscritos.Any())
            {
                ViewBag.Mensaje = "No hay alumnos inscritos en esta materia.";
                return PartialView("_Alumnos", new List<dynamic>());
            }

            var alumnos = await _context.tbAlumnosMaterias
                .Where(am => am.MateriaId == materiaId)
                .Include(am => am.Alumnos)
                .Select(am => new
                {
                    am.Alumnos.AlumnoId,
                    NombreCompleto = $"{am.Alumnos.Nombre} {am.Alumnos.ApellidoPaterno} {am.Alumnos.ApellidoMaterno}"
                })
                .ToListAsync();

            Console.WriteLine($"Alumnos encontrados: {alumnos.Count}");

            return PartialView("_Alumnos", alumnos);
        }




        [AllowAnonymous]
        [HttpGet("api/Alumno/Alumnos/{materiaId}")]
        public async Task<IActionResult> ObtenerAlumnos(int materiaId)
        {

            var alumnos = await (from am in _context.tbAlumnosMaterias
                                 join a in _context.tbAlumnos on am.AlumnoId equals a.AlumnoId
                                 where am.MateriaId == materiaId
                                 select new
                                 {
                                     a.AlumnoId,
                                     NombreCompleto = $"{a.Nombre} {a.ApellidoPaterno} {a.ApellidoMaterno}"
                                 }).ToListAsync();


            if (!alumnos.Any())
            {
                return NotFound(new { mensaje = "No hay alumnos inscritos en esta materia." });
            }

            return Ok(alumnos);
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
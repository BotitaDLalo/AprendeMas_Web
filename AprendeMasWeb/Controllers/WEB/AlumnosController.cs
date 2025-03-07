using AprendeMasWeb.Models.DBModels;
using global::AprendeMasWeb.Data;
using global::AprendeMasWeb.Models.DBModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AprendeMasWeb.Controllers.WEB
{
	[Route("api/[controller]")]
	[ApiController]
	public class AlumnosController : ControllerBase
	{
		private readonly DataContext _context;

		public AlumnosController(DataContext context)
		{
			_context = context;
		}

		// Endpoint para unirse a una clase con código de acceso
		[HttpPost("UnirseAClase")]
		public async Task<IActionResult> UnirseAClase([FromBody] UnirseAClaseRequest request)
		{
			if (string.IsNullOrEmpty(request.CodigoAcceso))
			{
				return BadRequest(new { mensaje = "El código de acceso es obligatorio" });
			}

			// Buscar si el código pertenece a un Grupo
			var grupo = await _context.tbGrupos.FirstOrDefaultAsync(g => g.CodigoAcceso == request.CodigoAcceso);

			if (grupo != null)
			{
				// Verificar si el alumno ya está inscrito en el grupo
				var existeRelacion = await _context.tbAlumnosGrupos
					.AnyAsync(ag => ag.AlumnoId == request.AlumnoId && ag.GrupoId == grupo.GrupoId);

				if (!existeRelacion)
				{
					// Agregar el alumno al grupo
					var nuevaRelacion = new AlumnosGrupos
					{
						AlumnoId = request.AlumnoId,
						GrupoId = grupo.GrupoId
					};
					_context.tbAlumnosGrupos.Add(nuevaRelacion);
					await _context.SaveChangesAsync();
				}

				return Ok(new { mensaje = "Te has unido al grupo", nombre = grupo.NombreGrupo, esGrupo = true });
			}

			// Buscar si el código pertenece a una Materia
			var materia = await _context.tbMaterias.FirstOrDefaultAsync(m => m.CodigoAcceso == request.CodigoAcceso);

			if (materia != null)
			{
				// Verificar si el alumno ya está inscrito en la materia
				var existeRelacion = await _context.tbAlumnosMaterias
					.AnyAsync(am => am.AlumnoId == request.AlumnoId && am.MateriaId == materia.MateriaId);

				if (!existeRelacion)
				{
					// Agregar el alumno a la materia
					var nuevaRelacion = new AlumnosMaterias
					{
						AlumnoId = request.AlumnoId,
						MateriaId = materia.MateriaId
					};
					_context.tbAlumnosMaterias.Add(nuevaRelacion);
					await _context.SaveChangesAsync();
				}

				return Ok(new { mensaje = "Te has unido a la materia", nombre = materia.NombreMateria, esGrupo = false });
			}

			return NotFound(new { mensaje = "Código de acceso no válido" });
		}
	}

	public class UnirseAClaseRequest
	{
		public int AlumnoId { get; set; }
		public string CodigoAcceso { get; set; }
	}
}

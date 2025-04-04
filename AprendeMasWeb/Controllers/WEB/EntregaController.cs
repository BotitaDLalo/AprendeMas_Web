using AprendeMasWeb.Data;
using AprendeMasWeb.Models.DBModels;
using Microsoft.AspNetCore.Mvc;

namespace AprendeMasWeb.Controllers.WEB
{
	[ApiController]
	[Route("api/[controller]")]
	public class EntregaController : ControllerBase
	{
		private readonly DataContext _context;

		public EntregaController(DataContext context)
		{
			_context = context;
		}

		public class EntregaDTO
		{
			public int ActividadId { get; set; }
			public int AlumnoId { get; set; }
			public string Respuesta { get; set; }
		}

		[HttpPost("Enviar")]
		public async Task<IActionResult> EnviarEntrega([FromBody] EntregaDTO entrega)
		{
			if (string.IsNullOrEmpty(entrega.Respuesta))
			{
				return BadRequest(new { mensaje = "Debe proporcionar un enlace de entrega." });
			}

			// 1. Crear el registro en tbAlumnosActividades
			var nuevaEntrega = new tbAlumnosActividades
			{
				ActividadId = entrega.ActividadId,
				AlumnoId = entrega.AlumnoId,
				FechaEntrega = DateTime.Now,
				EstatusEntrega = true // Se marca como entregada
			};

			_context.tbAlumnosActividades.Add(nuevaEntrega);
			await _context.SaveChangesAsync();

			// 2. Guardar el enlace en tbEntregablesAlumno
			var entregable = new tbEntregablesAlumno
			{
				AlumnoActividadId = nuevaEntrega.AlumnoActividadId, // Relación con la entrega
				Respuesta = entrega.Respuesta // Enlace enviado por el alumno
			};

			_context.tbEntregablesAlumno.Add(entregable);
			await _context.SaveChangesAsync();

			return Ok(new { mensaje = "Entrega guardada correctamente." });
		}
	}

}

using AprendeMasWeb.Data;
using AprendeMasWeb.Models;
using AprendeMasWeb.Models.DBModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AprendeMasWeb.Controllers.WEB
{
    [Route("WEB/Materias")]
    public class MateriasWebController : Controller
    {
        private readonly DataContext _context;

        public MateriasWebController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var materias = await _context.tbMaterias.ToListAsync();
            return View(materias);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Materias materia)
        {
            if (!ModelState.IsValid)
            {
                return View(materia);
            }

            materia.CodigoAcceso = ObtenerClave();
            _context.tbMaterias.Add(materia);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Editar(int id)
        {
            var materia = await _context.tbMaterias.FindAsync(id);
            if (materia == null) return NotFound();

            return View(materia);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Materias materia)
        {
            if (!ModelState.IsValid)
            {
                return View(materia);
            }

            _context.tbMaterias.Update(materia);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Eliminar(int id)
        {
            var materia = await _context.tbMaterias.FindAsync(id);
            if (materia == null) return NotFound();

            _context.tbMaterias.Remove(materia);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        private string ObtenerClave()
        {
            var random = new Random();
            return new string(Enumerable.Range(0, 8).Select(_ => (char)random.Next('A', 'Z')).ToArray());
        }
    }
}

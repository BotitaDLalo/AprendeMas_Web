using AprendeMasWeb.Data;
using AprendeMasWeb.Models;
using AprendeMasWeb.Models.DBModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AprendeMasWeb.Controllers.WEB
{
    [Route("WEB/Grupos")]
    public class GruposWebController : Controller
    {
        private readonly DataContext _context;

        public GruposWebController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var grupos = await _context.tbGrupos.ToListAsync();
            return View(grupos);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Grupos grupo)
        {
            if (!ModelState.IsValid)
            {
                return View(grupo);
            }

            grupo.CodigoAcceso = ObtenerClave();
            _context.tbGrupos.Add(grupo);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Editar(int id)
        {
            var grupo = await _context.tbGrupos.FindAsync(id);
            if (grupo == null) return NotFound();

            return View(grupo);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Grupos grupo)
        {
            if (!ModelState.IsValid)
            {
                return View(grupo);
            }

            _context.tbGrupos.Update(grupo);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Eliminar(int id)
        {
            var grupo = await _context.tbGrupos.FindAsync(id);
            if (grupo == null) return NotFound();

            _context.tbGrupos.Remove(grupo);
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

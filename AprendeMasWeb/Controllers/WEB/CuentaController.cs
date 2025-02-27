﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using AprendeMasWeb.Data; //Agregar referencia al contexto
using AprendeMasWeb.Models.DBModels; //Se agrega referencia a los modelos
using Microsoft.EntityFrameworkCore; // Agregar para consultas async con _context

namespace AprendeMasWeb.Controllers.WEB
{
    public class CuentaController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly DataContext _context; //Agregar context

        public CuentaController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, DataContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context; //Asignar context
        }

        [HttpGet]
        public IActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IniciarSesion(string email, string password)
        {
            if (!ModelState.IsValid)
                return View();

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
                return View();
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (result.Succeeded)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id) // Guarda el ID del usuario en los claims
        };

                // Verificar si es docente
                var docente = await _context.tbDocentes.FirstOrDefaultAsync(d => d.UserId == user.Id);
                if (docente != null)
                {
                    claims.Add(new Claim("DocenteId", docente.DocenteId.ToString()));
                }

                // Verificar si es alumno
                var alumno = await _context.tbAlumnos.FirstOrDefaultAsync(a => a.UserId == user.Id);
                if (alumno != null)
                {
                    claims.Add(new Claim("AlumnoId", alumno.AlumnoId.ToString()));
                }

                // Crear identidad de claims
                var claimsIdentity = new ClaimsIdentity(claims, "login");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                // Iniciar sesión con los claims
                await _signInManager.SignInWithClaimsAsync(user, isPersistent: false, claims);

                // Redirigir según el rol
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Alumno"))
                {
                    return RedirectToAction("Index", "Alumno");
                }
                else if (roles.Contains("Docente"))
                {
                    return RedirectToAction("Index", "Docente");
                }
            }

            ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
            return View();
        }


        [HttpGet]
        public IActionResult ObtenerDocenteId()
        {
            var docenteId = User.FindFirstValue("DocenteId");
            if(string.IsNullOrEmpty(docenteId))
            {
                return Json(new { error = "No se encontro el DocenteId" });
            }
            return Json(new { docenteId });
        }

        //Verificador de claims guardados desde url o postman>con cookies
        [HttpGet]
        public IActionResult VerificarClaims()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized("El usuario no está autenticado.");
            }

            var claims = HttpContext.User.Claims
                .Select(c => new { c.Type, c.Value })
                .ToList();

            return Ok(claims);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CerrarSesion()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("IniciarSesion", "Cuenta");
        }
    }
}

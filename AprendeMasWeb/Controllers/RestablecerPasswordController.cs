using AprendeMasWeb.Models.RestablecerPassword;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AprendeMasWeb.Controllers
{
    public class RestablecerPasswordController(UserManager<IdentityUser> userManager) : Controller
    {
        private readonly UserManager<IdentityUser> _userManager = userManager;

        [HttpGet]
        public async Task<IActionResult> Index(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("El usuario no existe.");
            }

            var tokenValido = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "ResetPassword", token);

            if (!tokenValido)
            {
                return RedirectToAction("Index", "LinkInvalido");
            }

            var model = new RestablecerPassword { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(RestablecerPassword model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {

                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    return BadRequest();
                }

                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NuevaPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index","Confirmacion");
                }


                return View(model);
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }
    }
}

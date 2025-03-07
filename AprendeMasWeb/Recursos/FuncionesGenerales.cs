using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using AprendeMasWeb.Models;
using AprendeMasWeb.Data;
using AprendeMasWeb.Models.DBModels;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using AprendeMasWeb.Recursos;
using AprendeMasWeb.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Configuration;

namespace AprendeMasWeb.Recursos
{
    public class FuncionesGenerales(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration, RoleManager<IdentityRole> roleManager, DataContext context)
    {
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly SignInManager<IdentityUser> _signInManager = signInManager;
        private readonly IConfiguration _configuration = configuration;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly DataContext _context = context;


        public async Task RegistrarFcmTokenUsuario(string identityUserId, string fcmToken)
        {
            try
            {
                tbUsuariosFcmTokens usuarioToken = new()
                {
                    UserId = identityUserId,
                    Token = fcmToken,
                };

                await _context.tbUsuariosFcmTokens.AddAsync(usuarioToken);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public string GenerarJwt(int idUsuario, IdentityUser emailEncontrado, string rolUsuario)
        {
            var handler = new JwtSecurityTokenHandler();
            var confSecretKey = _configuration["jwt:SecretKey"];
            var jwt = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(confSecretKey ?? throw new ArgumentNullException(confSecretKey, "Token no configurado")));
            var credentials = new SigningCredentials(jwt, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = "Aprende_Mas",
                Audience = "Aprende_Mas",
                SigningCredentials = credentials,
                Expires = DateTime.UtcNow.AddDays(7),
                Subject = GenerarClaims(idUsuario, emailEncontrado, rolUsuario),
            };

            var token = handler.CreateToken(tokenDescriptor);

            var tokenString = handler.WriteToken(token);

            return tokenString;
        }


        private static ClaimsIdentity GenerarClaims(int idUsuario, IdentityUser usuario, string rol)
        {
            var claims = new ClaimsIdentity();

            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, idUsuario.ToString() ?? ""));
            claims.AddClaim(new Claim(ClaimTypes.Name, usuario.UserName ?? ""));
            claims.AddClaim(new Claim(ClaimTypes.Email, usuario.Email ?? ""));
            claims.AddClaim(new Claim(ClaimTypes.Role, rol ?? ""));

            return claims;
        }

    }

}

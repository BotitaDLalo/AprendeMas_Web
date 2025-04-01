using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AprendeMasWeb.Models.DBModels
{
	[Table("tbAdministradores")]
	public class Administrador
	{
		[Key]
		public int AdministradorId { get; set; }

		[Required, StringLength(100)]
		public string Nombre { get; set; }

		[Required, StringLength(150)]
		public string Email { get; set; }

		[Required]
		public string UserId { get; set; } // Relación con IdentityUser

		[ForeignKey("UserId")]
		public IdentityUser Usuario { get; set; }
	}
}

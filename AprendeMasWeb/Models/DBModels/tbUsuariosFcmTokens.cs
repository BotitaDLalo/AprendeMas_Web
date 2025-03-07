using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AprendeMasWeb.Models.DBModels;
using Microsoft.AspNetCore.Identity;

namespace AprendeMasWeb.Models.DBModels
{
    public class tbUsuariosFcmTokens
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TokenId { get; set; }
        public required string UserId { get; set; }
        public required string Token {  get; set; }
        public virtual IdentityUser? IdentityUser { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AprendeMasWeb.Models.DBModels
{
    public class Notificaciones
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required int NotificacionId { get; set; }

        public required int UsuarioId {  get; set; }
        
        public required DateTime FechaNotificacion {  get; set; }


    }
}

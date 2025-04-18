﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AprendeMasWeb.Models.DBModels
{
    public class cTiposActividades
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TipoActividadId { get; set; }
        public required string Nombre { get; set; }
        public virtual ICollection<tbActividades>? Actividades { get; set; }

    }
}

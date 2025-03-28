﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AprendeMasWeb.Models.DBModels
{
    public class tbEntregablesAlumno
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EntregaId {  get; set; }

        public int AlumnoActividadId { get; set; }

        //public string? Enlace {  get; set; }

        //public string? Archivo {  get; set; }

        public string? Respuesta {  get; set; }

        public tbAlumnosActividades? AlumnosActividades { get; set; }
        public virtual ICollection<tbCalificaciones>? Calificaciones { get; set; }

    }
}

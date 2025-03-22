// Obtener el ID del docente almacenado en localStorage
docenteIdGlobal = localStorage.getItem("docenteId");
materiaIdGlobal = localStorage.getItem("materiaIdSeleccionada");
grupoIdGlobal = localStorage.getItem("grupoIdSeleccionado");
actividadIdGlobal = localStorage.getItem("actividadIdSeleccionada");
// Esperar a que el DOM esté completamente cargado antes de ejecutar el código
document.addEventListener("DOMContentLoaded", function () {
    let actividadIdGlobal = localStorage.getItem("actividadIdSeleccionada");

    if (actividadIdGlobal) {
        fetch(`/api/EvaluarActividadesApi/ObtenerActividadPorId/${actividadIdGlobal}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error("Error en la respuesta de la API");
                }
                return response.json();
            })
            .then(data => {
                if (data) {
                    document.getElementById("nombreActividad").innerText = data.nombreActividad || "No disponible";
                    document.getElementById("descripcionActividad").innerText = data.descripcion || "No disponible";

                    document.getElementById("fechaCreacion").innerText =
                        data.fechaCreacion ? new Date(data.fechaCreacion).toLocaleDateString("es-ES") : "No disponible";

                    document.getElementById("fechaLimite").innerText =
                        data.fechaLimite ? new Date(data.fechaLimite).toLocaleDateString("es-ES") : "No disponible";

                    document.getElementById("tipoActividad").innerText = data.tipoActividad || "No disponible";
                    document.getElementById("puntajeMaximo").innerText = data.puntaje || "0";

                    document.getElementById("alumnosEntregados").innerText = data.alumnosEntregados || "0 de 0";
                    document.getElementById("actividadesCalificadas").innerText = data.actividadesCalificadas || "0 de 0";
                } else {
                    console.error("No se encontraron datos válidos para esta actividad.");
                }
            })
            .catch(error => console.error("Error al obtener los datos de la actividad:", error));
    }
});

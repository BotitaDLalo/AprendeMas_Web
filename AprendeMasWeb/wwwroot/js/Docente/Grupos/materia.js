document.addEventListener("DOMContentLoaded", function () {
    const urlParams = new URLSearchParams(window.location.search);
    const materiaId = urlParams.get("materiaId");
    const docenteId = 2; // 🔹 Reemplaza esto con el ID del docente autenticado

    if (materiaId && docenteId) {
        fetch(`/api/DetallesMateriaApi/ObtenerDetallesMateria/${materiaId}/${docenteId}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error("Error en la respuesta de la API");
                }
                return response.json();
            })
            .then(data => {
                console.log("Datos recibidos:", data); // <-- Para depuración

                if (data.nombreMateria && data.codigoAcceso && data.codigoColor) {
                    document.getElementById("materiaNombre").innerText = data.nombreMateria;
                    document.getElementById("codigoAcceso").innerText = data.codigoAcceso;

                    // 🔹 Cambiar color de fondo
                    document.querySelector(".materia-header").style.backgroundColor = data.codigoColor;
                } else {
                    console.error("No se encontraron datos válidos para esta materia.");
                }
            })
            .catch(error => console.error("Error al obtener los datos de la materia:", error));
    }
});


function cambiarSeccion(seccion) {
    const contenido = document.getElementById("contenidoMateria");

    switch (seccion) {
        case "avisos":
            contenido.innerHTML = "<h3>Avisos</h3><p>Aquí se mostrarán los avisos.</p>";
            break;
        case "actividades":
            contenido.innerHTML = "<h3>Actividades</h3><p>Aquí estarán las actividades.</p>";
            break;
        case "alumnos":
            contenido.innerHTML = "<h3>Alumnos</h3><p>Lista de alumnos en la materia.</p>";
            break;
        case "calificaciones":
            contenido.innerHTML = "<h3>Calificaciones</h3><p>Notas de los estudiantes.</p>";
            break;
        default:
            contenido.innerHTML = "<p>Selecciona una opción para ver su contenido.</p>";
    }

    // Actualizar el botón activo
    document.querySelectorAll(".tab-button").forEach(btn => btn.classList.remove("active"));
    document.querySelector(`[onclick="cambiarSeccion('${seccion}')"]`).classList.add("active");
}



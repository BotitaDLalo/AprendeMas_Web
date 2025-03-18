// Obtener el ID del docente almacenado en localStorage
docenteIdGlobal = localStorage.getItem("docenteId");
materiaIdGlobal = localStorage.getItem("materiaIdSeleccionada");
grupoIdGlobal = localStorage.getItem("grupoIdSeleccionado");
// Esperar a que el DOM esté completamente cargado antes de ejecutar el código
document.addEventListener("DOMContentLoaded", function () {
    // Verificar si se tienen ambos IDs antes de hacer la petición
    if (materiaIdGlobal && docenteIdGlobal) {
        fetch(`/api/DetallesMateriaApi/ObtenerDetallesMateria/${materiaIdGlobal}/${docenteIdGlobal}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error("Error en la respuesta de la API");
                }
                return response.json();
            })
            .then(data => {
                if (data.nombreMateria && data.codigoAcceso && data.codigoColor) {
                    document.getElementById("materiaNombre").innerText = data.nombreMateria;
                    document.getElementById("codigoAcceso").innerText = data.codigoAcceso;
                    document.querySelector(".materia-header").style.backgroundColor = data.codigoColor;
                } else {
                    console.error("No se encontraron datos válidos para esta materia.");
                }
            })
            .catch(error => console.error("Error al obtener los datos de la materia:", error));
    }

    //Cambia de seccion forma automatica
    // Leer los parámetros de la URL (materiaId y seccion)
    const urlParams = new URLSearchParams(window.location.search);
    const materiaId = urlParams.get('materiaId'); // Debería estar almacenado también en materiaIdGlobal?? 
    const seccion = urlParams.get('seccion') || 'avisos'; // Si no se pasa, se usa 'avisos' por defecto

    // Cambiar a la sección que se pasa como parámetro
    cambiarSeccion(seccion);  // Llamamos a la función para mostrar la sección correcta

});



function cambiarSeccion(seccion) {
    document.querySelectorAll('.seccion').forEach(div => div.style.display = 'none');
    const seccionMostrar = document.getElementById(`seccion-${seccion}`);
    if (seccionMostrar) {
        seccionMostrar.style.display = 'block';
    }

    document.querySelectorAll('.tab-button').forEach(btn => btn.classList.remove('active'));
    document.querySelector(`button[onclick="cambiarSeccion('${seccion}')"]`).classList.add('active');

    // Cargar datos si se seleccionan secciones dinámicas
    if (seccion === "actividades") {
        cargarActividadesDeMateria(materiaIdGlobal);
    }
    if (seccion === "alumnos") {
        cargarAlumnosAsignados(materiaIdGlobal);
    }
    if (seccion === "avisos") {
        cargarAvisosDeMateria(materiaIdGlobal);
    }
}


//Funciones reutilizables
function convertirUrlsEnEnlaces(texto) {
    const urlRegex = /(https?:\/\/[^\s]+)/g;
    return texto.replace(urlRegex, '<a href="$1" target="_blank">$1</a>');
}


function formatearFecha(fecha) {
    const dateObj = new Date(fecha);
    return dateObj.toLocaleDateString("es-ES", { day: "2-digit", month: "2-digit", year: "numeric" }) +
        " " + dateObj.toLocaleTimeString("es-ES", { hour: "2-digit", minute: "2-digit" });
}
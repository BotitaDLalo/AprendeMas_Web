// Obtener el ID del docente almacenado en localStorage
docenteIdGlobal = localStorage.getItem("docenteId");

// Esperar a que el DOM esté completamente cargado antes de ejecutar el código
document.addEventListener("DOMContentLoaded", function () {
    // Obtener los parámetros de la URL
    const urlParams = new URLSearchParams(window.location.search);
    // Extraer el ID de la materia desde la URL
    const materiaId = urlParams.get("materiaId");
    // Usar el ID del docente almacenado previamente
    const docenteId = docenteIdGlobal;

    // Verificar si se tienen ambos IDs antes de hacer la petición
    if (materiaId && docenteId) {
        // Realizar petición a la API para obtener los detalles de la materia
        fetch(`/api/DetallesMateriaApi/ObtenerDetallesMateria/${materiaId}/${docenteId}`)
            .then(response => {
                // Verificar si la respuesta es correcta
                if (!response.ok) {
                    throw new Error("Error en la respuesta de la API");
                }
                // Convertir la respuesta a JSON
                return response.json();
            })
            .then(data => {
                // Mostrar los datos recibidos en la consola para depuración
                console.log("Datos recibidos:", data);

                // Verificar que los datos contengan la información esperada
                if (data.nombreMateria && data.codigoAcceso && data.codigoColor) {
                    // Asignar el nombre de la materia al elemento correspondiente
                    document.getElementById("materiaNombre").innerText = data.nombreMateria;
                    // Asignar el código de acceso al elemento correspondiente
                    document.getElementById("codigoAcceso").innerText = data.codigoAcceso;

                    // Cambiar el color de fondo del encabezado de la materia
                    document.querySelector(".materia-header").style.backgroundColor = data.codigoColor;
                } else {
                    // Mostrar un error en consola si los datos no son válidos
                    console.error("No se encontraron datos válidos para esta materia.");
                }
            })
            .catch(error =>
                // Capturar y mostrar errores en la consola
                console.error("Error al obtener los datos de la materia:", error)
            );
    }
});

// Función para cambiar la sección mostrada en la interfaz
function cambiarSeccion(seccion) {
    const contenido = document.getElementById("contenidoMateria");

    // Hacer una petición al controlador para cargar la vista parcial
    fetch(`/MateriasSeccion/CargarSeccion?seccion=${seccion}`)
        .then(response => {
            if (!response.ok) {
                throw new Error("Error al cargar la sección.");
            }
            return response.text(); // Convertir la respuesta en HTML
        })
        .then(html => {
            contenido.innerHTML = html; // Insertar el HTML de la vista parcial en el contenedor
        })
        .catch(error => console.error("Error al cargar la sección:", error));

    // Actualizar el botón activo
    document.querySelectorAll(".tab-button").forEach(btn => btn.classList.remove("active"));
    document.querySelector(`[onclick="cambiarSeccion('${seccion}')"]`).classList.add("active");
}


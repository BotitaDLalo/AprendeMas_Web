console.log("Revisando si el archivo JS se carga...");

document.addEventListener("DOMContentLoaded", function () {
    console.log("DOM cargado correctamente");

    const inputBusqueda = document.getElementById("buscarAlumno");
    const listaSugerencias = document.getElementById("sugerenciasAlumnos");

    if (!inputBusqueda || !listaSugerencias) {
        console.error("Error: No se encontraron los elementos en el DOM.");
        return;
    }

    inputBusqueda.addEventListener("input", async function () {
        const query = inputBusqueda.value.trim();
        console.log("Texto ingresado:", query);

        if (query.length < 2) {
            listaSugerencias.innerHTML = "";
            return;
        }

        try {
            console.log("Haciendo petición a la API...");
            const response = await fetch(`/api/DetallesMateriaApi/BuscarAlumnosPorCorreo?correo=${query}`);
            console.log("Respuesta recibida:", response);

            const alumnos = await response.json();
            console.log("Datos recibidos:", alumnos);
        } catch (error) {
            console.error("Error en la búsqueda de alumnos:", error);
        }
    });
});

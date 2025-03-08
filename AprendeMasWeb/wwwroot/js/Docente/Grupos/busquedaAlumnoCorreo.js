inputBusqueda.addEventListener("input", async function () {
    const query = inputBusqueda.value.trim();
    console.log("Texto ingresado:", query); // 👀 Ver qué se está enviando

    if (query.length < 2) {
        listaSugerencias.innerHTML = "";
        return;
    }

    try {
        console.log("Haciendo petición a la API...");
        const response = await fetch(`/api/DetallesMateriaApi/BuscarAlumnosPorCorreo?correo=${query}`);
        console.log("Respuesta recibida:", response); // 👀 Ver la respuesta

        const alumnos = await response.json();
        console.log("Datos recibidos:", alumnos); // 👀 Ver los datos en consola
    } catch (error) {
        console.error("Error en la búsqueda de alumnos:", error);
    }
});

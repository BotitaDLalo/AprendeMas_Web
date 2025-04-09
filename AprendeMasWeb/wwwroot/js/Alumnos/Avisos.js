document.addEventListener("DOMContentLoaded", async function () {
    const urlParams = new URLSearchParams(window.location.search);
    const tipo = urlParams.get("tipo");
    const nombreMateria = urlParams.get("nombre");

    if (tipo === "materia" && nombreMateria) {
        await cargarAvisosMateria(nombreMateria);
    }
});

// Función para cargar avisos de una materia
async function cargarAvisosMateria(nombreMateria) {
    try {
        // Obtener ID de la materia con su nombre (ajústalo si ya tienes la ID directa)
        const responseMateria = await fetch(`/api/Materias/ObtenerIdPorNombre?nombre=${encodeURIComponent(nombreMateria)}`);

        if (!responseMateria.ok) throw new Error("No se encontró la materia.");
        const materia = await responseMateria.json();

        const materiaId = materia.materiaId;

        const response = await fetch(`/api/Avisos/Materia/${materiaId}`);
        if (!response.ok) throw new Error("Error al obtener los avisos");

        const avisos = await response.json();
        mostrarAvisos(avisos);
    } catch (error) {
        console.error("Error al cargar los avisos:", error);
    }
}

// Función para mostrar los avisos en la vista
function mostrarAvisos(avisos) {
    const contenedor = document.getElementById("contenedorAvisos");
    contenedor.innerHTML = "";

    if (avisos.length === 0) {
        contenedor.innerHTML = "<p>No hay avisos disponibles para esta materia.</p>";
        return;
    }

    avisos.forEach(aviso => {
        const avisoElemento = document.createElement("div");
        avisoElemento.classList.add("aviso");

        avisoElemento.innerHTML = `
            <h4>${aviso.titulo}</h4>
            <p>${aviso.descripcion}</p>
            <small>${new Date(aviso.fechaCreacion).toLocaleString()}</small>
            <hr/>
        `;
        contenedor.appendChild(avisoElemento);
    });
}

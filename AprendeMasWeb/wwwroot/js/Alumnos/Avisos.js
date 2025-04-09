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
         <br>
                    <li class="list-group-item1 d-flex align-items-start">
                        <img class="iconos-nav4" src="/Iconos/PERFIL-26.svg" alt="Icono de Grupo" />
                        <div>
           <h5 class="mb-1">${aviso.docenteNombre}</h5>
              <small class="text-muted">Publicado el ${new Date(aviso.fechaCreacion).toLocaleString()}</small>
              <br><br>
               <h4 class="mb-1">${aviso.titulo}</h4>
            <p  class="mb-1"> ${aviso.descripcion}</p>
          
            </div>
                    </li>
            <br>

        `;
        contenedor.appendChild(avisoElemento);
    });
}

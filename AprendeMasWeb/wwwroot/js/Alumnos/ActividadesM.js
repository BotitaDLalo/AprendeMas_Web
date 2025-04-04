document.addEventListener("DOMContentLoaded", async () => {
    const urlParams = new URLSearchParams(window.location.search);
    const nombreMateria = urlParams.get("nombre");

    if (!nombreMateria) {
        document.getElementById("contenedorActividades").innerHTML = "<p>Materia no válida.</p>";
        return;
    }

    try {
        // Obtener el ID de la materia
        const responseMateria = await fetch(`/api/Materias/ObtenerIdPorNombre?nombre=${encodeURIComponent(nombreMateria)}`);
        const materia = await responseMateria.json();

        if (!materia.materiaId) {
            document.getElementById("contenedorActividades").innerHTML = "<p>No se encontró la materia.</p>";
            return;
        }

        const response = await fetch(`/api/Alumno/Actividades/${materia.materiaId}/${alumnoIdGlobal}`);
        if (!response.ok) {
            throw new Error('No se pudo obtener las actividades');
        }
        const actividades = await response.json();

        if (!Array.isArray(actividades)) {
            document.getElementById("contenedorActividades").innerHTML = `<p>${actividades.mensaje}</p>`;
            return;
        }

        const contenedor = document.getElementById("contenedorActividades");
        contenedor.innerHTML = "";

        actividades.forEach(a => {
            const calificacionHTML = a.calificacion
                ? `<p class="card-text"><strong>Calificación:</strong> ${a.calificacion.calificacion} </p>
                   <p class="card-text"><strong>Comentarios:</strong> ${a.calificacion.comentarios || 'Sin comentarios'}</p>`
                : '<p class="card-text"><strong>Calificación:</strong> No evaluada aún</p>';

            const card = `
                <div class="card mb-3 shadow">
                    <div class="card-body">
                        <h5 class="card-title">${a.nombreActividad}</h5>
             <p class="card-text">${a.descripcion}</p>
             <p class="card-text"><strong>Fecha de entrega:</strong> ${new Date(a.fechaLimite).toLocaleDateString()}</p>
             <p class="card-text"><strong>Puntaje:</strong> ${a.puntaje}</p>
             <p class="card-text"><strong>Tipo:</strong> ${a.tipoActividad}</p>
                        ${calificacionHTML}
                        <p id="res-entrega" class="card-text"><strong>Respuesta</strong> </p>
                         <button class="btn btn-primary mt-2" onclick="mostrarFormularioEntrega(${a.actividadId})">Entregar Actividad</button>

                        <div id="formEntrega-${a.actividadId}" class="mt-3" style="display:none;">
                            <textarea class="form-control mb-2" id="respuesta-${a.actividadId}" placeholder="Escribe tu respuesta..."></textarea>
                 <button class="btn btn-success" onclick="enviarEntrega(${a.actividadId})">Enviar</button>
                        </div>
                    </div>
                </div>
            `;

            contenedor.innerHTML += card;
        });

    } catch (error) {
        console.error("Error al cargar actividades:", error);
        document.getElementById("contenedorActividades").innerHTML = "<p>Error al cargar actividades.</p>";
    }
});


function mostrarFormularioEntrega(actividadId) {
    document.getElementById(`formEntrega-${actividadId}`).style.display = "block";
}

async function enviarEntrega(actividadId) {
    const respuesta = document.getElementById(`respuesta-${actividadId}`).value;
    const alumnoId = alumnoIdGlobal; // Asegúrate de tener este claim en el login

    const body = {
        actividadId: actividadId,
        alumnoId: alumnoId,
        respuesta: respuesta
    };

    try {
        const res = await fetch('/api/Alumno/EntregarActividad', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(body)
        });

        const result = await res.json();
        if (res.ok) {
            document.getElementById("res-entrega").innerHTML = "Actividad entregada correctamente.";
            alert("Actividad entregada correctamente.");
        } else {
            document.getElementById("res-entrega").innerHTML = result.mensaje;
            alert("Error: " + result.mensaje);
        }
    } catch (err) {
        console.error("Error al enviar actividad:", err);
        alert("Ocurrió un error al entregar la actividad.");
    }
}
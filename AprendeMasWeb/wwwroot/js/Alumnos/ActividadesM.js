﻿document.addEventListener("DOMContentLoaded", async () => {
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
            const fechaLimite = new Date(a.fechaLimite);
            const hoy = new Date();
            const estaFueraDeTiempo = hoy > fechaLimite;
            const calificada = !!a.calificacion;
            const calificacionValor = a.calificacion?.calificacion ?? 0;
            const aprobado = calificacionValor >= 70; // Cambia el mínimo si usas otra escala


            let etiquetaResultado = "";
            let bloqueoEntrega = "";

            if (calificada) {
                etiquetaResultado = `<span class="badge ${aprobado ? 'bg-success' : 'bg-danger'}">
        ${aprobado ? '✅ Aprobado' : '💡 Por mejorar'}
    </span>
    <span class="badge bg-secondary ms-2">Actividad calificada</span>`;

                // Bloquear edición si ya está calificada
                bloqueoEntrega = 'disabled';
            }

            const calificacionHTML = a.calificacion
                ? `
                <br><p class="card-text"><strong>Calificación:</strong> ${a.calificacion.calificacion}/100</p>
           <p class="card-text"><strong>Comentarios:</strong> ${a.calificacion.comentarios || 'Sin comentarios'}</p>`
                : '<p class="card-text"><strong>Calificación:</strong> No evaluada aún</p>';

            const entregada = !!a.respuesta;

            const estadoEntrega = estaFueraDeTiempo
                ? '<span class="badge bg-danger">⚠︎ Tarea retrasada</span>'
                : '';

            const card = `
    <div class="card mb-3 shadow ${entregada ? 'bg-success bg-opacity-25 border-success' : ''}">
        <div class="card-body">
            <h5 class="card-title">
             <img class="iconos-nav6" src="/Iconos/TABLA-26.svg" alt="Icono de Grupo" />
            ${a.nombreActividad}</h5>
            <br>
            <p class="card-text" style="text-align: justify;">${a.descripcion}</p>
            <p class="card-text"><strong>Fecha de entrega 📆:</strong>
                ${fechaLimite.toLocaleDateString()} ${fechaLimite.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
            </p>
            <p class="card-text"><strong>Puntaje:</strong> ${a.puntaje}%</p>
            <p class="card-text"><strong>Tipo:</strong> ${a.tipoActividad}</p>
            ${estadoEntrega}
            <br>
            <hr>
            <div class="titulo-calificacion">
  <img class="iconos-nav5" src="/Iconos/CALIFICACIONES-26.svg" alt="Icono de Grupo" />
  <h4>Calificación</h4>
  
</div>
  
${calificacionHTML}
            ${etiquetaResultado}
            
            <hr>
            <div class="respuesta-entrega mt-2">
                <p id="res-entrega-${a.actividadId}" class="card-text">
                    <strong>Respuesta:</strong>
                    <a href="${a.respuesta || '#'}" target="_blank">
                        ${a.respuesta || 'No has entregado aún esta actividad.'}
                    </a>
                </p>

                <button class="btn btn-primary btn-editar" onclick="mostrarFormularioEntrega(${a.actividadId})"
                    ${estaFueraDeTiempo || calificada ? 'disabled' : ''}>
                    ${entregada ? 'Editar Entrega' : 'Entregar Actividad'}
                </button>
            </div>

            <div id="formEntrega-${a.actividadId}" class="mt-3" style="display:none;">
                <textarea class="form-control mb-2" id="respuesta-${a.actividadId}" placeholder="Escribe tu respuesta...">${a.respuesta || ''}</textarea>
                <button class="btn btn-success btn-enviar" onclick="enviarEntrega(${a.actividadId})">Enviar</button>
            </div>
        </div>
    </div>`;

            contenedor.innerHTML += card;
        });


    } catch (error) {
        console.error("Error al cargar actividades:", error);
        document.getElementById("contenedorActividades").innerHTML = "<p>No hay actividades aun</p>";
    }
});


function mostrarFormularioEntrega(actividadId) {
    document.getElementById(`formEntrega-${actividadId}`).style.display = "block";
}

async function enviarEntrega(actividadId) {
    const respuesta = document.getElementById(`respuesta-${actividadId}`).value;
    const alumnoId = alumnoIdGlobal;

    const card = document.querySelector(`#formEntrega-${actividadId}`).closest('.card');
    const fechaTexto = card.querySelector("p.card-text strong").nextSibling.textContent.trim();
    const fechaEntrega = new Date(fechaTexto);
    const hoy = new Date();

    if (hoy > fechaEntrega) {
        Swal.fire({
            icon: 'warning',
            title: 'Entrega no permitida',
            html: `La fecha límite fue <strong>${fechaEntrega.toLocaleString()}</strong> y ya ha pasado.`,
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    const body = {
        actividadId,
        alumnoId,
        respuesta
    };

    try {
        const res = await fetch('/api/Alumno/EntregarActividad', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(body)
        });

        const result = await res.json();
        if (res.ok) {
            Swal.fire({
                icon: 'success',
                title: '¡Éxito!',
                text: 'Actividad entregada correctamente.',
                confirmButtonText: 'Aceptar'
            }).then(() => {
                location.reload();
            });
        } else {
            Swal.fire({
                icon: 'error',
                title: '¡Error!',
                text: result.mensaje,
                confirmButtonText: 'Aceptar'
            });
        }

    } catch (err) {
        console.error("Error al enviar actividad:", err);
        alert("Ocurrió un error al entregar la actividad.");
    }
}

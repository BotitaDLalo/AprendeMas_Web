﻿// Esperar a que el DOM esté completamente cargado antes de ejecutar el código
document.addEventListener("DOMContentLoaded", function () {

    cargarActividadesDeMateria();

});

// Función que registra una nueva actividad
async function registrarActividad() {
    let nombre = document.getElementById("nombre").value.trim();
    let descripcion = document.getElementById("descripcion").value.trim();
    let fechaHoraLimite = document.getElementById("fechaHoraLimite").value;
    let puntaje = parseInt(document.getElementById("puntaje").value, 10);

    // Validaciones básicas
    if (!nombre || !descripcion || !fechaHoraLimite || isNaN(puntaje)) {
        Swal.fire({
            icon: "warning",
            title: "Campos incompletos",
            text: "Por favor, completa todos los campos antes de continuar."
        });
        return;
    }

    // Validar que la fecha límite sea mayor a la fecha actual
    let fechaActual = new Date();
    let fechaLimite = new Date(fechaHoraLimite);
    if (fechaLimite <= fechaActual) {
        Swal.fire({
            icon: "warning",
            title: "Fecha inválida",
            text: "La fecha límite debe ser posterior a la fecha actual."
        });
        return;
    }

    // Validar materiaIdGlobal
    if (!materiaIdGlobal) {
        Swal.fire({
            icon: "error",
            title: "Error en materia",
            text: "No se ha identificado la materia seleccionada."
        });
        return;
    }

    let actividad = {
        nombreActividad: nombre,
        descripcion: descripcion,
        fechaLimite: fechaHoraLimite,
        tipoActividadId: 1, // Cambiar si se obtiene dinámicamente
        puntaje: puntaje,
        materiaId: materiaIdGlobal
    };

    try {
        let response = await fetch("/api/DetallesMateriaApi/CrearActividad", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(actividad)
        });

        let data = await response.json();

        if (!response.ok) {
            throw new Error(data.mensaje || `Error HTTP: ${response.status}`);
        }

        Swal.fire({
            position: "top-end",
            title: "Actividad creada",
            text: "La actividad ha sido publicada correctamente.",
            icon: "success",
            timer: 3000,
            showConfirmButton: false
        });

        setTimeout(() => {
            document.getElementById("actividadesForm").reset();
            cargarActividadesDeMateria(); // Recargar las actividades
            cargarActividadesEnSelect(); //Actualiza el select
        }, 2500);

    } catch (error) {
        console.error("Error:", error);
        Swal.fire({
            position: "top-end",
            title: "Error al crear la actividad",
            text: error.message || "Ocurrió un problema al crear la actividad.",
            icon: "error",
            timer: 3000,
            showConfirmButton: false
        });
    }
}



// Función que carga las actividades a la vista.
async function cargarActividadesDeMateria() {
    const listaActividades = document.getElementById("listaActividadesDeMateria");
    listaActividades.innerHTML = ""; // Limpiar contenido anterior

    try {
        mostrarCargando("Cargando actividades...");

        const response = await fetch(`/api/DetallesMateriaApi/ObtenerActividadesPorMateria/${materiaIdGlobal}`);
        if (!response.ok) throw new Error("No se encontraron actividades.");

        const actividades = await response.json();
        renderizarActividades(actividades);
    } catch (error) {
        listaActividades.innerHTML = `<p class="mensaje-error">${error.message}</p>`;
    } finally {
        cerrarCargando();
    }
}


//Renderiza actividades despues de confirmar existencia
function renderizarActividades(actividades) {
    const listaActividades = document.getElementById("listaActividadesDeMateria");
    listaActividades.innerHTML = ""; // Limpiar el contenedor

    if (actividades.length === 0) {
        listaActividades.innerHTML = "<p>No hay actividades registradas para esta materia.</p>";
        return;
    }
    actividades.reverse();

    actividades.forEach(actividad => {
        const actividadItem = document.createElement("div");
        actividadItem.classList.add("actividad-item");
        const descripcionActividadConEnlace = convertirUrlsEnEnlaces(actividad.descripcion);

        actividadItem.innerHTML = `
            <div class="actividad-header">
                <div class="icono">📋</div>
                <div class="info">
                    <strong>${actividad.nombreActividad}</strong>
                    <p class="fecha-publicado">Publicado: ${formatearFecha(actividad.fechaCreacion)}</p>
                    <p class="puntaje" style="font-weight: bold; color: #d35400;">Puntaje: ${actividad.puntaje}</p>
                    <p class="actividad-descripcion oculto">${descripcionActividadConEnlace}</p>
                    <p class="ver-completo">Ver completo</p>
                </div>
                <div class="fecha-entrega">
                    <strong>Fecha de entrega:</strong><br>
                    ${formatearFecha(actividad.fechaLimite)}
                </div>
                <div class="botones-container">
                    <button class="btn-ir-actividades" data-id="${actividad.actividadId}">Ir a actividad</button>
                    <button class="editar-btn" data-id="${actividad.actividadId}">Editar</button>
                    <button class="eliminar-btn" data-id="${actividad.actividadId}">Eliminar</button>
                </div>
            </div>
        `;

        // Mostrar/ocultar descripción al hacer clic en "Ver completo"
        const verCompleto = actividadItem.querySelector(".ver-completo");
        const descripcion = actividadItem.querySelector(".actividad-descripcion");

        verCompleto.addEventListener("click", () => {
            // Alternar entre mostrar y ocultar la descripción
            if (descripcion.classList.contains("oculto")) {
                descripcion.classList.remove("oculto");
                descripcion.classList.add("visible");
            } else {
                descripcion.classList.remove("visible");
                descripcion.classList.add("oculto");
            }
        });

        // Agregar eventos a los botones
        const btnEliminar = actividadItem.querySelector(".eliminar-btn");
        const btnEditar = actividadItem.querySelector(".editar-btn");
        const btnIrActividad = actividadItem.querySelector(".btn-ir-actividades");

        btnEliminar.addEventListener("click", () => eliminarActividad(actividad.actividadId));
        btnEditar.addEventListener("click", () => editarActividad(actividad.actividadId));
        btnIrActividad.addEventListener("click", () => IrAActividad(actividad.actividadId));

        listaActividades.appendChild(actividadItem);
    });
}


// Función que edita actividades
async function editarActividad(actividadId) {
    try {
        // Obtener datos actuales de la actividad
        const response = await fetch(`/api/DetallesMateriaApi/ObtenerActividad/${actividadId}`);
        if (!response.ok) throw new Error("No se pudo obtener la actividad.");

        const actividad = await response.json();

        // Mostrar SweetAlert con los datos actuales
        const { value: formValues } = await Swal.fire({
            title: "Editar Actividad",
            html: `
               <input id="swal-actividad" class="swal2-input" placeholder="Título" value="${actividad.nombreActividad}">
               <textarea id="swal-descripcionActividad" class="swal2-textarea" placeholder="Descripción">${actividad.descripcion}</textarea>
               <input id="swal-fecha" type="datetime-local" class="swal2-input" value="${actividad.fechaLimite}">
               <input id="swal-puntuacion" type="number" class="swal2-input" placeholder="Puntuación (0-100)" min="0" max="100" value="${actividad.puntaje}">
            `,
            focusConfirm: false,
            showCancelButton: true,
            confirmButtonText: "Guardar Cambios",
            cancelButtonText: "Cancelar",
            preConfirm: () => {
                const nombreActividad = document.getElementById("swal-actividad").value.trim();
                const descripcionActividad = document.getElementById("swal-descripcionActividad").value.trim();
                const fechaActividad = document.getElementById("swal-fecha").value;
                const puntajeActividad = parseInt(document.getElementById("swal-puntuacion").value, 10) || 0;

                if (!nombreActividad || !descripcionActividad || !fechaActividad) {
                    Swal.showValidationMessage("Todos los campos son requeridos.");
                    return false;
                }

                return { nombreActividad, descripcionActividad, fechaActividad, puntajeActividad };
            }
        });

        if (!formValues) return; // Si el usuario cancela, no hacer nada

        // Enviar los cambios al backend
        const updateResponse = await fetch(`/api/DetallesMateriaApi/ActualizarActividad`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                actividadId,
                nombreActividad: formValues.nombreActividad,
                descripcion: formValues.descripcionActividad,
                fechaLimite: formValues.fechaActividad,
                puntaje: formValues.puntajeActividad // Aquí corregido
            })
        });

        if (!updateResponse.ok) throw new Error("No se pudo actualizar la actividad.");

        Swal.fire("Actualizado", "La actividad ha sido editada correctamente.", "success");
        cargarActividadesDeMateria(); //Actualiza la vista
        cargarActividadesEnSelect(); //Actualiza en select 
    } catch (error) {
        Swal.fire("Error", error.message, "error");
    }
}



//Funcion para ir a la actividad
async function IrAActividad(actividadIdSeleccionada) {
    //guardar el id de la materia para acceder a la materia en la que se entro y usarla en otro script
    localStorage.setItem("actividadSeleccionada", actividadIdSeleccionada);
    // Redirige a la página de detalles de la materia
    window.open(`/Docente/EvaluarActividades`, '_blank'); //Aqui lleva en la url el id de la actividadSeleccionada
}


//Función para eliminar una actividad
async function eliminarActividad(id) {
    const result = await Swal.fire({
        title: '¿Estás seguro?',
         html: `
                    <p>Esto eliminará <b>la actividad</b> incluyendo:</p>
                    <ul style="text-align: left;">
                        <li>Calificaciones registradas</li>
                        <li>Respuestas del alumno a la actividad</li>
                    </ul>
                    <p>No podrás recuperar esta información después.</p>
                `,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar',
        reverseButtons: true
    });

    if (result.isConfirmed) {
        try {
            // Mostrar cargando mientras se realiza la petición
            mostrarCargando("Eliminando actividad...");

            const response = await fetch(`/api/DetallesMateriaApi/EliminarActividad/${id}`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                }
            });

            cerrarCargando(); // Cerrar el cargando al recibir respuesta

            const data = await response.json();

            if (data.message) {
                Swal.fire(
                    'Eliminado!',
                    `La actividad ha sido eliminada.`,
                    'success'
                );

                cargarActividadesDeMateria(); //Actualiza la vista
                cargarActividadesEnSelect(); //Actualiza el select
            } else {
                Swal.fire(
                    'Error',
                    'No se pudo eliminar la actividad. Intenta nuevamente.',
                    'error'
                );
            }
        } catch (error) {
            cerrarCargando();
            Swal.fire(
                'Error',
                'Hubo un error en la solicitud. Intenta nuevamente.',
                'error'
            );
            console.error('Error al eliminar la actividad:', error);
        }
    } else {
        Swal.fire({
            title: 'Cancelado',
            text: 'La actividad no fue eliminada.',
            icon: 'info',
            timer: 1500,
            showConfirmButton: false
        });
    }
}


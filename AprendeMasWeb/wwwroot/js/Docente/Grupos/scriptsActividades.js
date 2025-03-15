﻿// Obtener el ID del docente almacenado en localStorage
docenteIdGlobal = localStorage.getItem("docenteId");
materiaIdGlobal = localStorage.getItem("materiaIdSeleccionada");
grupoIdGlobal = localStorage.getItem("grupoIdSeleccionado");
// Esperar a que el DOM esté completamente cargado antes de ejecutar el código
document.addEventListener("DOMContentLoaded", function () {

    cargarActividadesDeMateria();

});

//Funcion que registra una nueva actividad
async function registrarActividad() {
    let nombre = document.getElementById("nombre").value;
    let descripcion = document.getElementById("descripcion").value;
    let fechaHoraLimite = document.getElementById("fechaHoraLimite").value;
    let puntaje = document.getElementById("puntaje").value;

    if (!nombre || !descripcion || !fechaHoraLimite || !puntaje) {
        Swal.fire({
            icon: "warning",
            title: "Campos incompletos",
            text: "Por favor, completa todos los campos antes de continuar."
        });
        return;
    }

    let actividad = {
        nombreActividad: nombre,
        descripcion: descripcion,
        fechaLimite: fechaHoraLimite,
        tipoActividadId: 1, // Obtener esto dinámicamente si es necesario
        puntaje: parseInt(puntaje),
        materiaId: materiaIdGlobal //dentro de la materia que se encuentra
    };

    try {
        let response = await fetch("/api/DetallesMateriaApi/CrearActividad", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(actividad)
        });

        if (!response.ok) {
            throw new Error(`Error HTTP: ${response.status}`);
        }

        let data = await response.json();

        Swal.fire({
            position: "top-end",
            title: "Actividad creada",
            text: "La actividad ha sido publicado correctamente.",
            icon: "success",
            timer: 3000,
            showConfirmButton: false
        });

        setTimeout(() => {
            location.reload(); // Recargar la página después de mostrar el mensaje
        }, 2500);
    } catch (error) {
        console.error("Error:", error);
        Swal.fire({
            position: "top-end",
            title: "Error al crear la actividad",
            text: "Ocurrio un problema al crear la actividad..",
            icon: "error",
            timer: 3000,
            showConfirmButton: false
        });
    }
}



// Funcion que carga las actividades a la vista.
async function cargarActividadesDeMateria() {
    const listaActividades = document.getElementById("listaActividadesDeMateria");
    listaActividades.innerHTML = "<p>Cargando actividades...</p>"; // Mostrar mensaje de carga

    try {
        const response = await fetch(`/api/DetallesMateriaApi/ObtenerActividadesPorMateria/${materiaIdGlobal}`);
        if (!response.ok) throw new Error("No se encontraron actividades.");
        const actividades = await response.json();
        renderizarActividades(actividades);
    } catch (error) {
        listaActividades.innerHTML = `<p class="mensaje-error">${error.message}</p>`;
    }
}

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
                    <button class="btn-ir-actividades" data-id="${actividad.materiaActividad}">Ir a actividad</button>
                    <button class="editar-btn" data-id="${actividad.materiaActividad}">Editar</button>
                    <button class="eliminar-btn" data-id="${actividad.materiaActividad}">Eliminar</button>
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

        btnEliminar.addEventListener("click", () => eliminarActividad(actividad.materiaActividad));
        btnEditar.addEventListener("click", () => editarActividad(actividad.materiaActividad));
        btnIrActividad.addEventListener("click", () => IrAActividad(actividad.materiaActividad));

        listaActividades.appendChild(actividadItem);
    });
}


// Funciones para manejar los botones


async function eliminarActividad(id) {
    // Confirmación con SweetAlert
    const result = await Swal.fire({
        title: '¿Estás seguro?',
        text: "¡Esta acción no se puede deshacer!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar',
        reverseButtons: true
    });

    if (result.isConfirmed) {
        // Alerta de confirmación antes de la eliminación
        Swal.fire(
            'Eliminado!',
            `La actividad con ID: ${id} ha sido eliminada.`,
            'success'
        );

        try {
            // Realizar la petición para eliminar la actividad
            const response = await fetch(`/api/DetallesMateriaApi/EliminarActividad/${id}`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                }
            });

            const data = await response.json();

            if (data.message) {
                Swal.fire({
                    title: 'Éxito',
                    text: data.message,
                    icon: 'success',
                    timer: 1500,  // Tiempo en milisegundos (1500ms = 1.5 segundos)
                    showConfirmButton: false  // Esto es opcional, para que no aparezca el botón de "OK"
                });
                location.reload(); // Recargar la página o actualizar la vista pero manda hasta avisos, checar eso
            } else {
                Swal.fire(
                    'Error',
                    'No se pudo eliminar la actividad. Intenta nuevamente.',
                    'error'
                );
            }
        } catch (error) {
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
            timer: 1500,  // El tiempo que se mostrará la alerta (1500ms = 1.5 segundos)
            showConfirmButton: false  // Esto es opcional, para que no aparezca el botón "OK"
        });

    }
}

// Obtener la fecha de ayer
let ayer = new Date();
ayer.setDate(ayer.getDate() - 1);
let fechaAyer = ayer.toISOString().split("T")[0];
document.addEventListener("DOMContentLoaded", async function () {
    await obtenerDocenteId();  //Asegurarse de que el alumnoIdGlobal se obtenga antes de continuar

    const icono = document.getElementById("calendario-icono");
    const panel = document.getElementById("calendario-panel");
    const input = document.getElementById("calendario-input");
    let fechasConEventos = [];
    let calendario; // Variable para almacenar la instancia de Flatpickr

    // Cargar idioma español
    flatpickr.localize(flatpickr.l10ns.es);

    // Función para inicializar Flatpickr
    function inicializarCalendario() {
        fetch(`/api/EventosAgenda/docente/${docenteIdGlobal}`)

            .then(response => response.json())
            .then(data => {
                fechasConEventos = data.map(evento => evento.fechaInicio.split("T")[0]);

                // Si el calendario ya existe, lo destruye antes de crear uno nuevo
                if (calendario) {
                    calendario.destroy();
                }

                // Inicializar Flatpickr
                calendario = flatpickr(input, {
                    inline: true,
                    dateFormat: "Y-m-d",
                    locale: "es",
                    defaultDate: new Date(),
                    minDate: fechaAyer, // Bloquear fechas antes de ayer, permitiendo el día de hoy
                    onChange: function (selectedDates, dateStr) {
                        document.getElementById("modalEvento").style.display = "flex";
                        document.getElementById("fechaSeleccionadaTexto").textContent = dateStr;
                        cargarEventos(dateStr);
                    },
                    onDayCreate: function (dObj, dStr, fp, dayElem) {
                        let fechaDia = dayElem.dateObj.toISOString().split("T")[0];

                        if (fechasConEventos.includes(fechaDia)) {
                            dayElem.style.backgroundColor = "#ffae4c"; // Color anaranjado
                            dayElem.style.color = "000000";
                            dayElem.style.borderRadius = "50%";
                        }

                        // Colorear fechas anteriores al día de hoy de gris
                        if (dayElem.dateObj < ayer) {
                            dayElem.style.backgroundColor = "#888888"; // Gris
                            dayElem.style.color = "#FFFFFF"; // Gris oscuro
                            dayElem.style.pointerEvents = "none"; // Desactivar clics en fechas pasadas
                        }
                    }
                });
            })
            .catch(error => console.error("Error obteniendo eventos:", error));
    }

    // Llamar a la función para inicializar el calendario
    inicializarCalendario();

    // Toggle para mostrar/ocultar el calendario
    icono.addEventListener("click", function (event) {
        event.preventDefault();
        panel.classList.toggle("mostrar");
    });

    // Cierra el panel si se hace clic fuera de él o del icono
    document.addEventListener("click", function (event) {
        if (!icono.contains(event.target) && !panel.contains(event.target)) {
            panel.classList.remove("mostrar");
        }
    });

    // Evento para cerrar el modal
    document.getElementById("cerrar-modal").addEventListener("click", function () {
        document.getElementById("modalEvento").style.display = "none";
    });

    // Evento para agregar un nuevo evento
    document.getElementById("btnAgregarEvento").addEventListener("click", function () {
        document.getElementById("formEventoContainer").style.display = "block";
    });

    // Evento para guardar un nuevo evento
    document.getElementById("guardar-evento").addEventListener("click", function () {
        const fechaInicio = document.getElementById("fecha-inicio").value.trim();
        const fechaFinal = document.getElementById("fecha-final").value.trim();
        const titulo = document.getElementById("tituloEvento").value.trim();
        const descripcion = document.getElementById("descripcionEvento").value.trim();
        const color = document.getElementById("color").value;

        // Validación de campos vacíos
        if (!titulo || !descripcion || !fechaInicio || !fechaFinal || !color) {
            Swal.fire({
                icon: "warning",
                title: "Campos incompletos",
                text: "Por favor, completa todos los campos antes de guardar.",
                confirmButtonColor: "#dc3545"
            });
            return;
        }

        // Validación de fechas
        if (new Date(fechaFinal) <= new Date(fechaInicio)) {
            Swal.fire({
                icon: "error",
                title: "Fechas inválidas",
                text: "La fecha final debe ser posterior a la fecha de inicio.",
                confirmButtonColor: "#dc3545"
            });
            return;
        }

        let evento = {
            DocenteId: docenteIdGlobal,
            FechaInicio: fechaInicio,
            FechaFinal: fechaFinal,
            Titulo: titulo,
            Descripcion: descripcion,
            Color: color
        };

        fetch("/api/EventosAgenda", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(evento)
        })
            .then(response => response.json())
            .then(data => {
                console.log("Evento guardado:", data);

                // Limpiar campos
                document.getElementById("fecha-inicio").value = "";
                document.getElementById("fecha-final").value = "";
                document.getElementById("tituloEvento").value = "";
                document.getElementById("descripcionEvento").value = "";
                document.getElementById("color").value = "#2196F3";

                // Ocultar formulario y modal
                document.getElementById("formEventoContainer").style.display = "none";
                document.getElementById("modalEvento").style.display = "none";

                Swal.fire({
                    icon: "success",
                    title: "Evento guardado",
                    text: "El evento se ha guardado correctamente.",
                    confirmButtonColor: "#007bff"
                });

                inicializarCalendario();
            })
            .catch(error => console.error("Error:", error));
    });



    // Función para cargar eventos en el modal
    function cargarEventos(fecha) {

        fetch(`/api/EventosAgenda/docente/${docenteIdGlobal}`)
            .then(response => response.json())
            .then(data => {
                let eventosDelDia = data.filter(evento => {
                    if (!evento.fechaInicio) return false;
                    let fechaEvento = evento.fechaInicio.split("T")[0];
                    return fechaEvento === fecha;
                });

                let listaEventos = document.getElementById("listaEventos");
                listaEventos.innerHTML = "";

                if (eventosDelDia.length === 0) {
                    listaEventos.innerHTML = "<br><p>No hay eventos para este día.</p>";
                } else {
                    eventosDelDia.forEach(evento => {
                        let item = document.createElement("div");
                        item.classList.add("evento-item");

                        // Crear el título del evento (clic para ver detalles)
                        let titulo = document.createElement("strong");
                        titulo.textContent = evento.titulo;
                        titulo.style.cursor = "pointer";
                        titulo.addEventListener("click", function () {
                            Swal.fire({
                                icon: "info",
                                title: "Detalles del Evento",
                                html: `
                            <b>Título:</b> ${evento.titulo} <br>
                            <b>Descripción:</b> ${evento.descripcion} <br>
                            <b>Inicio:</b> ${evento.fechaInicio} <br>
                            <b>Final:</b> ${evento.fechaFinal}
                        `,
                                position: "center",
                                confirmButtonText: "Aceptar",
                                confirmButtonColor: "#007bff"
                            });
                        });

                        // Crear icono de edición
                        let iconoEditar = document.createElement("span");
                        iconoEditar.innerHTML = "✏️"; // Emoji de lápiz (edición)
                        iconoEditar.style.cursor = "pointer";
                        iconoEditar.style.marginLeft = "10px";
                        iconoEditar.addEventListener("click", function () {
                            editarEvento(evento);
                        });

                        // Crear icono de eliminar
                        let iconoEliminar = document.createElement("span");
                        iconoEliminar.innerHTML = "🗑️"; // Emoji de basura
                        iconoEliminar.style.cursor = "pointer";
                        iconoEliminar.style.marginLeft = "15px";
                        iconoEliminar.addEventListener("click", function () {
                            eliminarEvento(evento.eventoId, fecha);
                        });

                        // Agregar los elementos al contenedor del evento
                        item.appendChild(titulo);
                        item.appendChild(iconoEditar);
                        item.appendChild(iconoEliminar);
                        listaEventos.appendChild(item);
                    });
                }
            })
            .catch(error => console.error("Error cargando eventos:", error));
    }

    function editarEvento(evento) {
        // Llenar el formulario con la información del evento
        document.getElementById("editar-evento-id").value = evento.eventoId;
        document.getElementById("editar-titulo").value = evento.titulo;
        document.getElementById("editar-descripcion").value = evento.descripcion;
        document.getElementById("editar-fecha-inicio").value = evento.fechaInicio.split("T")[0];
        document.getElementById("editar-fecha-final").value = evento.fechaFinal.split("T")[0];
        document.getElementById("editar-color").value = evento.color;

        // Mostrar el modal de edición
        document.getElementById("modal-editar-evento").style.display = "flex";
    }

    // Evento para guardar la edición del evento
    document.getElementById("guardar-edicion-evento").addEventListener("click", function () {
        let eventoId = document.getElementById("editar-evento-id").value;
        let eventoEditado = {
            EventoId: eventoId,
            DocenteId: docenteIdGlobal,
            FechaInicio: document.getElementById("editar-fecha-inicio").value,
            FechaFinal: document.getElementById("editar-fecha-final").value,
            Titulo: document.getElementById("editar-titulo").value,
            Descripcion: document.getElementById("editar-descripcion").value,
            Color: document.getElementById("editar-color").value
        };


        fetch(`/api/EventosAgenda/${eventoId}`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(eventoEditado)
        })
            .then(response => {
                if (response.ok) {
                    Swal.fire({
                        icon: "success",
                        title: "Evento actualizado",
                        text: "Los cambios se han guardado correctamente.",
                        confirmButtonColor: "#007bff"
                    }).then(() => {
                        // Cerrar ambos modales
                        document.getElementById("modal-editar-evento").style.display = "none";
                        document.getElementById("modalEvento").style.display = "none";

                        // Recargar el calendario para actualizar los eventos
                        inicializarCalendario();
                    });
                } else {
                    Swal.fire({
                        icon: "error",
                        title: "Error",
                        text: "No se pudo actualizar el evento.",
                        confirmButtonColor: "#007bff"
                    });
                }
            })
            .catch(error => console.error("Error actualizando evento:", error));
    });



    // Función para eliminar un evento con confirmación de SweetAlert
    function eliminarEvento(eventoId, fecha) {
        Swal.fire({
            title: "¿Estás seguro?",
            text: "Esta acción no se puede deshacer.",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Sí, eliminar",
            cancelButtonText: "Cancelar",
            confirmButtonColor: "#d33",
            cancelButtonColor: "#007bff"
        }).then((result) => {
            if (result.isConfirmed) {
                fetch(`/api/EventosAgenda/${eventoId}`, {
                    method: "DELETE"
                }).then(response => {
                    if (response.ok) {
                        Swal.fire({
                            icon: "success",
                            title: "Evento eliminado",
                            text: "El evento ha sido eliminado correctamente.",
                            confirmButtonColor: "#007bff"
                        });

                        // Volver a cargar los eventos y actualizar el calendario
                        cargarEventos(fecha);
                        inicializarCalendario();
                    } else {
                        Swal.fire({
                            icon: "error",
                            title: "Error",
                            text: "No se pudo eliminar el evento.",
                            confirmButtonColor: "#007bff"
                        });
                    }
                }).catch(error => console.error("Error eliminando evento:", error));
            }
        });
    }
});


// Obtener la fecha y hora actuales
let today = new Date();

// Formatear la fecha al formato requerido por el input datetime-local (YYYY-MM-DDThh:mm)
let formattedDate = today.toISOString().slice(0, 16);

// Establecer el atributo min del input
document.getElementById("fecha-inicio").setAttribute("min", formattedDate);
document.getElementById("fecha-final").setAttribute("min", formattedDate);
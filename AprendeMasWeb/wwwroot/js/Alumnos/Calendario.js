document.addEventListener("DOMContentLoaded", function () {
    const icono = document.getElementById("calendario-icono");
    const panel = document.getElementById("calendario-panel");
    const input = document.getElementById("calendario-input");
    let fechasConEventos = [];
    let calendario; // Variable para almacenar la instancia de Flatpickr

    // Cargar idioma español
    flatpickr.localize(flatpickr.l10ns.es);

    // Función para inicializar Flatpickr
    function inicializarCalendario() {
        fetch("/api/EventosAgendaAlumno/alumno/1")
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
                    onChange: function (selectedDates, dateStr) {
                        document.getElementById("modal-evento").style.display = "flex";
                        document.getElementById("fecha-seleccionada").textContent = dateStr;
                        cargarEventos(dateStr);
                    },
                    onDayCreate: function (dObj, dStr, fp, dayElem) {
                        let fechaDia = dayElem.dateObj.toISOString().split("T")[0];

                        if (fechasConEventos.includes(fechaDia)) {
                            dayElem.style.backgroundColor = "#FFA500"; // Color anaranjado
                            dayElem.style.color = "white";
                            dayElem.style.borderRadius = "50%";
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
        document.getElementById("modal-evento").style.display = "none";
    });

    // Evento para agregar un nuevo evento
    document.getElementById("agregar-evento").addEventListener("click", function () {
        document.getElementById("formulario-evento").style.display = "block";
    });

    // Evento para guardar un nuevo evento
    document.getElementById("guardar-evento").addEventListener("click", function () {
        let evento = {
            AlumnoId: 1, // Reemplazar con el ID real del alumno
            FechaInicio: document.getElementById("fecha-inicio").value,
            FechaFinal: document.getElementById("fecha-final").value,
            Titulo: document.getElementById("titulo").value,
            Descripcion: document.getElementById("descripcion").value,
            Color: document.getElementById("color").value
        };

        fetch("/api/EventosAgendaAlumno", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(evento)
        }).then(response => response.json())
            .then(data => {
                console.log("Evento guardado:", data);

                // Limpiar los campos del formulario
                document.getElementById("fecha-inicio").value = "";
                document.getElementById("fecha-final").value = "";
                document.getElementById("titulo").value = "";
                document.getElementById("descripcion").value = "";
                document.getElementById("color").value = "#000000"; // Color por defecto

                // Ocultar el formulario y el modal
                document.getElementById("formulario-evento").style.display = "none";
                document.getElementById("modal-evento").style.display = "none"; // Cierra el modal de eventos

                // Mostrar alerta de éxito con SweetAlert
                Swal.fire({
                    icon: "success",
                    title: "Evento guardado",
                    text: "El evento se ha guardado correctamente.",
                    confirmButtonColor: "#007bff"
                });

                // Recargar el calendario para que se actualicen las fechas con eventos
                inicializarCalendario();
            })
            .catch(error => console.error("Error:", error));
    });

    // Función para cargar eventos en el modal
    function cargarEventos(fecha) {
        let alumnoId = 1; // Reemplazar con el ID real del alumno

        fetch(`/api/EventosAgendaAlumno/alumno/${alumnoId}`)
            .then(response => response.json())
            .then(data => {
                let eventosDelDia = data.filter(evento => {
                    if (!evento.fechaInicio) return false;
                    let fechaEvento = evento.fechaInicio.split("T")[0];
                    return fechaEvento === fecha;
                });

                let listaEventos = document.getElementById("lista-eventos");
                listaEventos.innerHTML = "";

                if (eventosDelDia.length === 0) {
                    listaEventos.innerHTML = "<br><p>No hay eventos para este día.</p>";
                } else {
                    eventosDelDia.forEach(evento => {
                        let item = document.createElement("div");
                        item.innerHTML = `<strong>${evento.titulo}</strong>`;
                        item.classList.add("evento-item");
                        item.style.cursor = "pointer";
                        item.addEventListener("click", function () {
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
                        listaEventos.appendChild(item);
                    });
                }
            })
            .catch(error => console.error("Error cargando eventos:", error));
    }
});

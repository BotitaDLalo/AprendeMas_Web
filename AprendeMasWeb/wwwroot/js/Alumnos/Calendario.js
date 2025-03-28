document.addEventListener("DOMContentLoaded", function () {
    const icono = document.getElementById("calendario-icono");
    const panel = document.getElementById("calendario-panel");
    const input = document.getElementById("calendario-input");

    // Cargar idioma español
    flatpickr.localize(flatpickr.l10ns.es);

    // Inicializar Flatpickr con formato MX y en español
    flatpickr(input, {
        inline: true,
        dateFormat: "d/m/Y", // Formato MX: día/mes/año
        locale: "es", // Idioma español
        defaultDate: new Date(),
    });

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
});

document.addEventListener("DOMContentLoaded", function () {
    flatpickr("#calendario-input", {
        onChange: function (selectedDates, dateStr) {
            document.getElementById("modal-evento").style.display = "flex";
            document.getElementById("fecha-seleccionada").textContent = dateStr;
            cargarEventos(dateStr);
        }
    });

    document.getElementById("cerrar-modal").addEventListener("click", function () {
        document.getElementById("modal-evento").style.display = "none";
    });

    document.getElementById("agregar-evento").addEventListener("click", function () {
        document.getElementById("formulario-evento").style.display = "block";
    });

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
                document.getElementById("formulario-evento").style.display = "none";
                cargarEventos(evento.FechaInicio);
            })
            .catch(error => console.error("Error:", error));
    });

    function cargarEventos(fecha) {
        let alumnoId = 1; // Reemplazar con el ID real del alumno

        fetch(`/api/EventosAgendaAlumno/alumno/${alumnoId}`)
            .then(response => response.json())
            .then(data => {
                console.log("Eventos recibidos:", data); // Verificar la estructura de la respuesta

                let eventosDelDia = data.filter(evento => {
                    if (!evento.fechaInicio) return false; // Validar que tenga fechaInicio
                    let fechaEvento = evento.fechaInicio.split("T")[0]; // Extraer solo YYYY-MM-DD
                    return fechaEvento === fecha; // Comparar con la fecha seleccionada
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
                                icon: "info", // Icono informativo
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


document.addEventListener("DOMContentLoaded", function () {
    fetch("/api/EventosAgendaAlumno/alumno/1")
        .then(response => response.json())
        .then(data => {
            let fechasConEventos = data.map(evento => evento.FechaInicio.split("T")[0]);

            flatpickr("#calendario-input", {
                enable: fechasConEventos,
                onChange: function (selectedDates, dateStr) {
                    document.getElementById("modal-evento").style.display = "flex";
                    document.getElementById("fecha-seleccionada").textContent = dateStr;
                    cargarEventos(dateStr);
                }
            });
        })
        .catch(error => console.error("Error obteniendo eventos:", error));
});
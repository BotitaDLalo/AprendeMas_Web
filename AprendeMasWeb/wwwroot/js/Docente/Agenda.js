document.addEventListener("DOMContentLoaded", function () {

    flatpickr("#calendario-input", {
        enableTime: false,
        dateFormat: "Y-m-d",  // Formato correcto YYYY-MM-DD
        locale: "es", // Idioma español
        defaultDate: new Date(),
        onChange: function (selectedDates, dateStr) {
            if (selectedDates.length > 0) {
                cargarEventosPorFecha(dateStr);
            }
        }
    });

    function cargarEventosPorFecha(fechaSeleccionada) {
        document.getElementById("fechaSeleccionadaTexto").textContent = fechaSeleccionada;
        document.getElementById("listaEventos").innerHTML = "<p>Cargando...</p>";
        document.getElementById("formEventoContainer").style.display = "none";

        fetch(`/api/EventosAgenda/fecha/${fechaSeleccionada}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error(`Error al obtener eventos: ${response.status}`);
                }
                return response.json();
            })
            .then(data => {
                console.log("Respuesta de la API:", data); // Agrega esta línea
                let listaEventosDiv = document.getElementById("listaEventos");
                listaEventosDiv.innerHTML = "";

                if (data.mensaje) {
                    listaEventosDiv.innerHTML = `<p>${data.mensaje}</p>`;
                } else {
                    data.forEach(evento => {
                        console.log("Evento recibido:", evento); // También imprime cada evento
                        let eventoDiv = document.createElement("div");
                        eventoDiv.classList.add("evento-item");
                        eventoDiv.innerHTML = `
                <h3 class="evento-titulo" data-id="${evento.eventoId}">${evento.titulo}</h3>
                <div class="evento-detalle" style="display: none;">
                    <p><strong>Descripción:</strong> ${evento.descripcion}</p>
                    <p><strong>Inicio:</strong> ${evento.fechaInicio}</p>
                    <p><strong>Fin:</strong> ${evento.fechaFinal}</p>
                    <p><strong>Color:</strong> ${evento.color}</p>
                </div>
            `;
                        listaEventosDiv.appendChild(eventoDiv);
                    });

                    document.querySelectorAll(".evento-titulo").forEach(titulo => {
                        titulo.addEventListener("click", function () {
                            let detalle = this.nextElementSibling;
                            detalle.style.display = detalle.style.display === "none" ? "block" : "none";
                        });
                    });
                }
            })


            .catch(error => {
                console.error("Error al cargar eventos:", error);
                document.getElementById("listaEventos").innerHTML = `<p>Error al cargar eventos.</p>`;
            });

        document.getElementById("modalEvento").style.display = "flex";
    }


    // Cerrar modal al hacer clic fuera del contenido
    document.getElementById("modalEvento").addEventListener("click", function (event) {
        if (event.target === this) { // Verifica si el clic fue en el fondo y no en el contenido del modal
            this.style.display = "none";
        }
    });
});
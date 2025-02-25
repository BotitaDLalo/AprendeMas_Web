document.addEventListener("DOMContentLoaded", function () {
    // Cargar idioma español
    flatpickr.localize(flatpickr.l10ns.es);
    flatpickr("#calendario-input", {
        inline: true,
        dateFormat: "d/m/Y", // Formato MX: día/mes/año
        locale: "es", // Idioma español
        defaultDate: new Date(),
        onChange: function (selectedDates, dateStr) {
            if (selectedDates.length > 0) {
                abrirModal(dateStr);
            }
        }
    });

    function abrirModal(fechaSeleccionada) {
        document.getElementById("fechaInicio").value = fechaSeleccionada;
        document.getElementById("modalEvento").style.display = "flex";
    }

    document.querySelector(".close-modal12").addEventListener("click", function () {
        document.getElementById("modalEvento").style.display = "none";
    });

    // Cerrar modal al hacer clic fuera del contenido
    document.getElementById("modalEvento").addEventListener("click", function (event) {
        if (event.target === this) { // Verifica si el clic fue en el fondo y no en el contenido del modal
            this.style.display = "none";
        }
    });

    document.getElementById("formEvento").addEventListener("submit", function (e) {
        e.preventDefault();

        const evento = {
            DocenteId: 1, // Cambia esto dinámicamente según el usuario logueado
            Titulo: document.getElementById("titulo").value,
            Descripcion: document.getElementById("descripcion").value,
            FechaInicio: document.getElementById("fechaInicio").value,
            FechaFinal: document.getElementById("fechaFinal").value,
            Color: document.getElementById("color").value
        };

        fetch("/api/EventosAgenda", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(evento)
        })
            .then(response => response.json())
            .then(data => {
                mostrarModalConfirmacion(data.mensaje);
                document.getElementById("modalEvento").style.display = "none";
            })
            .catch(error => console.error("Error:", error));
    });
});
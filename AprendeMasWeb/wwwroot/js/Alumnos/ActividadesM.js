function cargarActividades(materiaId) {
    if (!materiaId) {
        console.error("MateriaId no está definido.");
        $("#actividades-container").html("<p>Error al obtener actividades.</p>");
        return;
    }

    $.get("/api/Alumno/Actividades/" + materiaId, function (data) {
        var actividadesHtml = "";
        if (data.length > 0) {
            data.forEach(function (actividad) {
                actividadesHtml += `
                <br>
                    <li class="list-group-item">
                        <h5>${actividad.nombreActividad}</h5>
                        <p>${actividad.descripcion}</p>
                        <small class="text-muted">Fecha límite: ${new Date(actividad.fechaLimite).toLocaleDateString()}</small>
                        <br>
                        <small class="text-muted">Puntaje: ${actividad.puntaje}</small>
                        <br>
                        <button class="btn btn-primary btn-sm mt-2" onclick="abrirModalEntrega(${actividad.actividadId}, '${actividad.nombreActividad}', '${actividad.descripcion}', '${actividad.fechaLimite}', ${actividad.puntaje})">
                            Entregar Actividad
                        </button>
                    </li>`;
            });
        } else {
            actividadesHtml = "<p>No hay actividades registradas para esta materia.</p>";
        }
        $("#actividades-container").html(actividadesHtml);
    }).fail(function () {
        $("#actividades-container").html("<p>Error al cargar las actividades.</p>");
    });
}

function abrirModalEntrega(id, titulo, descripcion, fechaLimite, puntaje) {
    $("#actividadTitulo").text(titulo);
    $("#actividadDescripcion").text(descripcion);
    $("#actividadFechaLimite").text(new Date(fechaLimite).toLocaleDateString());
    $("#actividadPuntaje").text(puntaje);
    $("#actividadId").val(id);

    $("#modalEntregaActividad").modal("show");
}

function enviarEntrega() {
    var actividadId = $("#actividadId").val();
    var respuesta = $("#respuestaActividad").val();

    if (!respuesta) {
        alert("Por favor, ingresa un enlace de entrega.");
        return;
    }

    $.post("/api/Alumno/EntregarActividad", { actividadId: actividadId, respuesta: respuesta }, function (response) {
        alert("Entrega realizada con éxito.");
        $("#modalEntregaActividad").modal("hide");
        $("#respuestaActividad").val(""); // Limpiar el campo después de enviar
    }).fail(function () {
        alert("Error al enviar la entrega. Inténtalo de nuevo.");
    });
}

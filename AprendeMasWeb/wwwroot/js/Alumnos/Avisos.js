$(document).ready(function () {
    if (!alumnoIdGlobal) {
        console.error("alumnoIdGlobal no está definido.");
        $("#avisos-container").html("<p>Error al obtener los avisos.</p>");
        return;
    }

    $.get("/api/Alumno/Avisos/" + alumnoIdGlobal, function (data) {
        var avisosHtml = "";
        if (data.length > 0) {
            data.forEach(function (aviso) {
                avisosHtml += `
                        <li class="list-group-item">
                            <h5>${aviso.titulo}</h5>
                            <p>${aviso.descripcion}</p>
                            <small class="text-muted">Publicado el ${new Date(aviso.fechaCreacion).toLocaleString()}</small>
                        </li>`;
            });
        } else {
            avisosHtml = "<p>No hay avisos disponibles.</p>";
        }
        $("#avisos-container").html(avisosHtml);
    }).fail(function () {
        $("#avisos-container").html("<p>Error al cargar los avisos.</p>");
    });
});

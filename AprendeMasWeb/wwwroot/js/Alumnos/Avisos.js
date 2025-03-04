$(document).ready(function () {
    var alumnoId = 2; // Debes cambiar esto dinámicamente según el usuario

    $.get("/api/Alumno/Avisos/" + alumnoId, function (data) {
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
    });
});
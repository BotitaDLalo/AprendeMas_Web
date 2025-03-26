$(document).ready(function () {
    if (!alumnoIdGlobal || alumnoIdGlobal === "null") {
        console.error("alumnoIdGlobal no está definido.");
        $("#avisos-container").html('<li class="list-group-item text-danger">Error al obtener los avisos.</li>');
        return;
    }

    $.get(`/api/Alumno/Avisos/${alumnoIdGlobal}`)
        .done(function (data) {
            var avisosHtml = "";
            if (data.length > 0) {
                data.forEach(function (aviso) {
                    avisosHtml += `
                        <li class="list-group-item">
                            <h5 class="mb-1">${aviso.titulo}</h5>
                            <p class="mb-1">${aviso.descripcion}</p>
                            <small class="text-muted">Publicado el ${new Date(aviso.fechaCreacion).toLocaleString()}</small>
                        </li>`;
                });
            } else {
                avisosHtml = '<li class="list-group-item text-warning">No hay avisos disponibles.</li>';
            }
            $("#avisos-container").html(avisosHtml);
        })
        .fail(function (xhr) {
            console.error("Error al cargar avisos:", xhr.status, xhr.statusText);
            let errorMessage = xhr.status === 404
                ? '<li class="list-group-item text-warning">No hay avisos disponibles.</li>'
                : '<li class="list-group-item text-danger">Error al cargar los avisos.</li>';
            $("#avisos-container").html(errorMessage);
        });
});

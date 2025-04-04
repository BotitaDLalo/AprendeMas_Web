$(document).ready(function () {
    if (!alumnoIdGlobal || alumnoIdGlobal === "null") {
        console.error("alumnoIdGlobal no está definido.");
        $("#avisos-container").html('<li class="list-group-item text-danger">Error al obtener los avisos.</li>');
        return;
    }

    function cargarAvisos(materiaId = null) {
        let url = `/api/Alumno/Avisos/${alumnoIdGlobal}`;
        if (materiaId) {
            url += `/${materiaId}`; // Agregar el ID de la materia si se selecciona
        }

        $.get(url)
            .done(function (data) {
                let avisosHtml = "";
                if (data.length > 0) {
                    data.forEach(function (aviso) {
                        avisosHtml += `
                        <br>
                        
                        <li class="list-group-item1 d-flex align-items-start">
                            <img class="iconos-nav4" src="/Iconos/PERFIL-26.svg" alt="Icono de Grupo" />
                            <div>
                                <h5 class="mb-1">${aviso.docenteNombre}</h5>
                                <small class="text-muted">Publicado el ${new Date(aviso.fechaCreacion).toLocaleString()}</small>
                                <h5 class="mb-1">${aviso.titulo}</h5>
                                <p class="mb-1">${aviso.descripcion}</p>
                            </div>
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
    }

    // Llamar a la función al cargar la página para mostrar avisos generales
    $(document).ready(function () {
        cargarAvisos();
    });

    // Llamar a la función cuando se seleccione una materia
    $(".materia-item").on("click", function () {
        let materiaId = $(this).data("materia-id"); // Suponiendo que cada materia tiene un `data-materia-id`
        cargarAvisos(materiaId);
    });
});

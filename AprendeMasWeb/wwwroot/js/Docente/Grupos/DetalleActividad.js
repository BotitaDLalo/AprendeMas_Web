let actividadesData = {};
// Obtener el ID del docente almacenado en localStorage
let docenteIdGlobal = localStorage.getItem("docenteId");
let materiaIdGlobal = localStorage.getItem("materiaIdSeleccionada");
let grupoIdGlobal = localStorage.getItem("grupoIdSeleccionado");
let actividadIdGlobal = localStorage.getItem("actividadSeleccionada");
let puntajeMaximo = null;
// Esperar a que el DOM esté completamente cargado antes de ejecutar el código
document.addEventListener("DOMContentLoaded", function () {
    if (actividadIdGlobal != null && materiaIdGlobal != null) {

        // Mostramos el loader
        mostrarCargando("Cargando detalles de la actividad...");

        fetch(`/api/EvaluarActividadesApi/ObtenerActividadPorId/${actividadIdGlobal}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error("Error en la respuesta de la API");
                }
                return response.json();
            })
            .then(data => {
                cerrarCargando(); // Cerramos el loader

                if (data) {
                    document.getElementById("nombreActividad").innerText = data.nombreActividad || "No disponible";
                    document.getElementById("descripcionActividad").innerHTML = convertirUrlsEnEnlaces(data.descripcion) || "No disponible";

                    document.getElementById("fechaCreacion").innerText =
                        data.fechaCreacion ? new Date(data.fechaCreacion).toLocaleDateString("es-ES") : "No disponible";

                    document.getElementById("fechaLimite").innerText =
                        data.fechaLimite ? new Date(data.fechaLimite).toLocaleDateString("es-ES") : "No disponible";

                    document.getElementById("tipoActividad").innerText = data.tipoActividad || "Actividad";
                    document.getElementById("puntajeMaximo").innerText = data.puntaje || "0";
                    puntajeMaximo = data.puntaje;
                } else {
                    console.error("No se encontraron datos válidos para esta actividad.");
                }
            })
            .catch(error => {
                cerrarCargando(); // Cierra aunque haya error
                console.error("Error al obtener los datos de la actividad:", error);
                Swal.fire("Error", "No se pudo cargar la información de la actividad.", "error");
            });
    }

    prepararAlumnosYActividades();
});


async function prepararAlumnosYActividades() {
    await AlumnosDeMateriaParaActividades();
    await obtenerActividadesParaEvaluar();
}


//Obtener los alumnos que estan en la materia seleciconada para guardarlos en un array
async function AlumnosDeMateriaParaActividades() {
    try {
        const response = await fetch(`/api/EvaluarActividadesApi/AlumnosParaCalificarActividades/${materiaIdGlobal}`);

        if (!response.ok) {
            throw new Error("No se pudieron cargar los alumnos.");
        }

        const alumnos = await response.json();

        // Guardar en localStorage
        localStorage.setItem(`alumnos_materia_${materiaIdGlobal}`, JSON.stringify(alumnos));

        console.log("Alumnos guardados en localStorage:", alumnos);

    } catch (error) {
        console.error("Error al cargar alumnos:", error);
    }
}




async function obtenerActividadesParaEvaluar() {
    try {
        // Recuperar los alumnos desde localStorage
        let alumnos = JSON.parse(localStorage.getItem(`alumnos_materia_${materiaIdGlobal}`)) || [];

        // Recuperar el ID de la actividad desde localStorage
        let actividadId = localStorage.getItem("actividadSeleccionada");

        // Verificar si los datos existen antes de enviarlos
        if (!actividadId || alumnos.length === 0) {
            console.error("No hay datos suficientes para enviar la solicitud.");
            return;
        }

        // Construir el objeto de la solicitud
        let requestData = {
            Alumnos: alumnos, // El array de alumnos
            ActividadId: parseInt(actividadId) // Convertir a número si es necesario
        };

        // Hacer la solicitud al backend
        let response = await fetch("/api/EvaluarActividadesApi/ObtenerActividadesParaEvaluar", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(requestData)
        });

        // Convertir la respuesta a JSON
        let data = await response.json();
        // Guardar los datos en la variable global
        actividadesData = data;


        // Mostrar los resultados en la consola
        console.log("Actividades No Entregadas:", data.noEntregados);
        console.log("Actividades Entregadas:", data.entregados);
        // Llamar a la función para renderizar la lista de alumnos en la vista
        renderizarAlumnos(data);
    } catch (error) {
        console.error("Error al obtener las actividades:", error);
    }
}


// Función para renderizar los alumnos en la vista
function renderizarAlumnos(data) {
    const listaEntregados = document.getElementById("listaAlumnosEntregados");
    const listaNoEntregados = document.getElementById("listaAlumnosSinEntregar");

    // Limpiar las listas antes de agregar nuevos elementos
    listaEntregados.innerHTML = "";
    listaNoEntregados.innerHTML = "";

    // Renderizar alumnos que entregaron actividad
    data.entregados.forEach(alumno => {
        const fechaEntrega = new Date(alumno.fechaEntrega).toLocaleDateString("es-ES");
        const fechaCalificacion = alumno.entrega?.fechaCalificacionAsignada
            ? new Date(alumno.entrega.fechaCalificacion).toLocaleDateString("es-ES")
            : "Sin calificar";

        const alumnoHTML = `
            <div class="list-group-item d-flex justify-content-between align-items-center">
                <div>
                    <h5 class="mb-1" style="font-weight: bold; color: #333;">${alumno.nombre} ${alumno.apellidoPaterno} ${alumno.apellidoMaterno}</h5>
                    <p class="mb-1" style="color: #777;">Entregó: ${fechaEntrega}</p>
                </div>
                <span class="badge bg-success">Entregado</span>
                <button class="btn btn-primary btn-sm" onclick="verRespuesta(${alumno.alumnoActividadId})">Ver Respuesta</button>
                <button class="btn btn-warning btn-sm" onclick="abrirModalCalificar(${alumno.entrega.entregaId}, ${puntajeMaximo})">Calificar</button>
                <p class="mb-1" style="color: #777;">Calificado el: ${fechaCalificacion}</p>
            </div>
        `;

        listaEntregados.innerHTML += alumnoHTML;
    });

    // Renderizar alumnos que NO entregaron actividad
    data.noEntregados.forEach(alumno => {
        const alumnoHTML = `
            <div class="list-group-item d-flex justify-content-between align-items-center">
                <div>
                    <h5 class="mb-1" style="font-weight: bold; color: #333;">${alumno.nombre} ${alumno.apellidoPaterno} ${alumno.apellidoMaterno}</h5>
                    <p class="mb-1" style="color: #777;">Entregó: Sin entregar</p>
                </div>
                <span class="badge bg-danger">No entregado</span>
            </div>
        `;

        listaNoEntregados.innerHTML += alumnoHTML;
    });
}

function convertirUrlsEnEnlaces(texto) {
    const urlRegex = /(https?:\/\/[^\s]+)/g;
    return texto.replace(urlRegex, '<a href="$1" target="_blank">$1</a>');
}
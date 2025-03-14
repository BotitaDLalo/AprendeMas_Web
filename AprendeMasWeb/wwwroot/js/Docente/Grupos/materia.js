// Obtener el ID del docente almacenado en localStorage
docenteIdGlobal = localStorage.getItem("docenteId");
materiaIdGlobal = localStorage.getItem("materiaIdSeleccionada");
grupoIdGlobal = localStorage.getItem("grupoIdSeleccionado");
// Esperar a que el DOM esté completamente cargado antes de ejecutar el código
document.addEventListener("DOMContentLoaded", function () {
    // Verificar si se tienen ambos IDs antes de hacer la petición
    if (materiaIdGlobal && docenteIdGlobal) {
        fetch(`/api/DetallesMateriaApi/ObtenerDetallesMateria/${materiaIdGlobal}/${docenteIdGlobal}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error("Error en la respuesta de la API");
                }
                return response.json();
            })
            .then(data => {
                console.log("Datos recibidos:", data);

                if (data.nombreMateria && data.codigoAcceso && data.codigoColor) {
                    document.getElementById("materiaNombre").innerText = data.nombreMateria;
                    document.getElementById("codigoAcceso").innerText = data.codigoAcceso;
                    document.querySelector(".materia-header").style.backgroundColor = data.codigoColor;
                } else {
                    console.error("No se encontraron datos válidos para esta materia.");
                }
            })
            .catch(error => console.error("Error al obtener los datos de la materia:", error));
    }
    //Cargar los avisos asinados a la materia
    cargarAvisosDeMateria(materiaIdGlobal);
    // Cargar alumnos asignados a la materia
    cargarAlumnosAsignados(materiaIdGlobal);
    //Cargar actividades a la materia
    cargarActividadesDeMateria(materiaIdGlobal);

    //delegacion de evento 
    document.addEventListener("click", async function (event) {
        if (event.target.id === "btnAsignarAlumno") {
            const correo = document.getElementById("buscarAlumno").value.trim();
            if (!correo) {
                Swal.fire({
                    position: "top-end",
                    icon: "question",
                    title: "Ingrese un correo valido.",
                    showConfirmButton: false,
                    timer: 2500
                });
                return;
            }

            try {
                const response = await fetch(`/api/DetallesMateriaApi/AsignarAlumnoMateria?correo=${correo}&materiaId=${materiaIdGlobal}`, {
                    method: "POST"
                });

                const data = await response.json();

                if (!response.ok) {
                    Swal.fire({
                        title: "Error",
                        text: data.mensaje || "Error al asignar alumno.",
                        icon: "error",
                        confirmButtonColor: "#d33",
                    });
                    return;
                }
                document.getElementById("buscarAlumno").value = "";
                Swal.fire({
                    position: "top-end",
                    title: "Asignado",
                    text: "Alumno asignado correctamente.",
                    icon: "success",
                    timer: 2500
                });
                cargarAlumnosAsignados(materiaIdGlobal);
            } catch (error) {
                Swal.fire({
                    title: "Error",
                    text: "Hubo un problema al asignar al alumno. Inténtalo de nuevo.",
                    icon: "error",
                    confirmButtonColor: "#d33",
                });
            }
        }
    });


   
    // Funcionalidad de búsqueda de alumnos en tiempo real (sugerencias de correo)
    const inputBuscar = document.getElementById("buscarAlumno");
    const listaSugerencias = document.getElementById("sugerenciasAlumnos");
    let indexSugerenciaSeleccionada = -1;

    if (inputBuscar) {
        inputBuscar.addEventListener("input", async function () {
            const query = inputBuscar.value.trim();
            if (query.length < 3) {
                listaSugerencias.innerHTML = "";
                return;
            }

            try {
                const response = await fetch(`/api/DetallesMateriaApi/BuscarAlumnosPorCorreo?query=${query}`);
                if (!response.ok) throw new Error("Error al buscar alumnos");

                const alumnos = await response.json();
                listaSugerencias.innerHTML = "";

                if (alumnos.length === 0) {
                    listaSugerencias.innerHTML = `<li class="list-group-item text-muted">No se encontraron resultados</li>`;
                    return;
                }

                alumnos.forEach((alumno, index) => {
                    const li = document.createElement("li");
                    li.classList.add("list-group-item", "list-group-item-action");
                    li.textContent = `${alumno.nombre} ${alumno.apellidoPaterno} ${alumno.apellidoMaterno} - ${alumno.email}`;

                    li.addEventListener("click", function () {
                        inputBuscar.value = alumno.email;
                        listaSugerencias.innerHTML = "";
                    });

                    if (index === indexSugerenciaSeleccionada) {
                        li.classList.add("active");
                    }

                    listaSugerencias.appendChild(li);
                });

            } catch (error) {
                console.error("Error al buscar alumnos:", error);
            }
        });
    
        // Navegación con teclas en las sugerencias
        inputBuscar.addEventListener("keydown", function (e) {
            const sugerencias = listaSugerencias.getElementsByTagName("li");

            if (e.key === "ArrowDown") {
                if (indexSugerenciaSeleccionada < sugerencias.length - 1) {
                    indexSugerenciaSeleccionada++;
                    actualizarSugerencias();
                }
            } else if (e.key === "ArrowUp") {
                if (indexSugerenciaSeleccionada > 0) {
                    indexSugerenciaSeleccionada--;
                    actualizarSugerencias();
                }
            } else if (e.key === "Enter" && indexSugerenciaSeleccionada >= 0) {
                const selectedSugerencia = sugerencias[indexSugerenciaSeleccionada];
                if (selectedSugerencia) {
                    const correo = selectedSugerencia.textContent.split(" - ")[1];
                    if (correo) {
                        inputBuscar.value = correo;
                        listaSugerencias.innerHTML = "";
                    }
                }
            }
        });
        function actualizarSugerencias() {
            const sugerencias = listaSugerencias.getElementsByTagName("li");
            for (let i = 0; i < sugerencias.length; i++) {
                sugerencias[i].classList.remove("active");
            }
            if (indexSugerenciaSeleccionada >= 0 && indexSugerenciaSeleccionada < sugerencias.length) {
                sugerencias[indexSugerenciaSeleccionada].classList.add("active");
            }
        }
        document.addEventListener("click", function (event) {
            if (!inputBuscar.contains(event.target) && !listaSugerencias.contains(event.target)) {
                listaSugerencias.innerHTML = "";
            }
        });
    }
});

//Funcion que carga los avisos a la vista.
async function cargarAvisosDeMateria(materiaIdGlobal) {
    const listaAvisos = document.getElementById("listaDeAvisos");
    listaAvisos.innerHTML = "<p class='aviso-cargando'>Cargando avisos...</p>";

    try {
        const response = await fetch(`/api/DetallesMateriaApi/ObtenerAvisos?IdMateria=${materiaIdGlobal}`);
        if (!response.ok) throw new Error("No se encontraron avisos.");
        const avisos = await response.json();
        renderizarAvisos(avisos);
    } catch (error) {
        listaAvisos.innerHTML = `<p class="aviso-error">${error.message}</p>`;
    }
}

function renderizarAvisos(avisos) {
    const listaAvisos = document.getElementById("listaDeAvisos");
    listaAvisos.innerHTML = "";

    if (avisos.length === 0) {
        listaAvisos.innerHTML = "<p class='aviso-no-hay'>No hay avisos registrados para esta materia.</p>";
        return;
    }

    avisos.forEach(aviso => {
        const avisoItem = document.createElement("div");
        avisoItem.classList.add("aviso-item");

        const descripcionConEnlaces = convertirUrlsEnEnlaces(aviso.descripcion);

        avisoItem.innerHTML = `
            <div class="aviso-header">
                <div class="aviso-icono">📢</div>
                <div class="aviso-info">
                    <strong class="aviso-titulo">${aviso.titulo}</strong>
                    <p class="aviso-fecha">Publicado: ${new Date(aviso.fechaCreacion).toLocaleString()}</p>
                    <p class="aviso-descripcion oculto">${descripcionConEnlaces}</p>
                    <p class="aviso-ver-mas">Ver más</p>
                </div>
                <div class="aviso-botones">
                    <button class="aviso-eliminar-btn" data-id="${aviso.avisoId}">Eliminar</button>
                </div>
            </div>
        `;

        const verMas = avisoItem.querySelector(".aviso-ver-mas");
        const descripcion = avisoItem.querySelector(".aviso-descripcion");
        verMas.addEventListener("click", () => {
            descripcion.classList.toggle("oculto");
        });

        const btnEliminar = avisoItem.querySelector(".aviso-eliminar-btn");
        btnEliminar.addEventListener("click", () => eliminarAviso(aviso.avisoId));

        listaAvisos.appendChild(avisoItem);
    });
}

function convertirUrlsEnEnlaces(texto) {
    const urlRegex = /(https?:\/\/[^\s]+)/g;
    return texto.replace(urlRegex, '<a href="$1" target="_blank">$1</a>');
}

//Funcion que carga las actividades a la vista.

async function cargarActividadesDeMateria(materiaIdGlobal) {
    const listaActividades = document.getElementById("listaActividadesDeMateria");
    listaActividades.innerHTML = "<p>Cargando actividades...</p>"; // Mostrar mensaje de carga

    try {
        const response = await fetch(`/api/DetallesMateriaApi/ObtenerActividadesPorMateria/${materiaIdGlobal}`);
        if (!response.ok) throw new Error("No se encontraron actividades.");
        const actividades = await response.json();
        renderizarActividades(actividades);
    } catch (error) {
        listaActividades.innerHTML = `<p class="mensaje-error">${error.message}</p>`;
    }
}

function renderizarActividades(actividades) {
    const listaActividades = document.getElementById("listaActividadesDeMateria");
    listaActividades.innerHTML = ""; // Limpiar el contenedor

    if (actividades.length === 0) {
        listaActividades.innerHTML = "<p>No hay actividades registradas para esta materia.</p>";
        return;
    }

    actividades.forEach(actividad => {
        const actividadItem = document.createElement("div");
        actividadItem.classList.add("actividad-item");

        actividadItem.innerHTML = `
            <div class="actividad-header">
                <div class="icono">📋</div>
                <div class="info">
                    <strong>${actividad.nombreActividad}</strong>
                    <p class="fecha-publicado">Publicado: ${formatearFecha(actividad.fechaCreacion)}</p>
                    <p class="puntaje" style="font-weight: bold; color: #d35400;">Puntaje: ${actividad.puntaje}</p>
                    <p class="actividad-descripcion oculto">${actividad.descripcion}</p>
                    <p class="ver-completo">Ver completo</p>
                </div>
                <div class="fecha-entrega">
                    <strong>Fecha de entrega:</strong><br>
                    ${formatearFecha(actividad.fechaLimite)}
                </div>
                <div class="botones-container">
                    <button class="eliminar-btn" data-id="${actividad.materiaActividad}">Eliminar</button>
                </div>
            </div>
        `;

        // Mostrar/ocultar descripción al hacer clic en "Ver completo"
        const verCompleto = actividadItem.querySelector(".ver-completo");
        const descripcion = actividadItem.querySelector(".actividad-descripcion");
        verCompleto.addEventListener("click", () => {
            descripcion.classList.toggle("oculto");
        });

        // Agregar eventos a los botones
        const btnEliminar = actividadItem.querySelector(".eliminar-btn");

        btnEliminar.addEventListener("click", () => eliminarActividad(actividad.materiaActividad));

        listaActividades.appendChild(actividadItem);
    });
}

function formatearFecha(fecha) {
    const dateObj = new Date(fecha);
    return dateObj.toLocaleDateString("es-ES", { day: "2-digit", month: "2-digit", year: "numeric" }) +
        " " + dateObj.toLocaleTimeString("es-ES", { hour: "2-digit", minute: "2-digit" });
}



// Funciones para manejar los botones


async function eliminarActividad(id) {
    // Confirmación con SweetAlert
    const result = await Swal.fire({
        title: '¿Estás seguro?',
        text: "¡Esta acción no se puede deshacer!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar',
        reverseButtons: true
    });

    if (result.isConfirmed) {
        // Alerta de confirmación antes de la eliminación
        Swal.fire(
            'Eliminado!',
            `La actividad con ID: ${id} ha sido eliminada.`,
            'success'
        );

        try {
            // Realizar la petición para eliminar la actividad
            const response = await fetch(`/api/DetallesMateriaApi/EliminarActividad/${id}`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                }
            });

            const data = await response.json();

            if (data.message) {
                Swal.fire({
                    title: 'Éxito',
                    text: data.message,
                    icon: 'success',
                    timer: 1500,  // Tiempo en milisegundos (1500ms = 1.5 segundos)
                    showConfirmButton: false  // Esto es opcional, para que no aparezca el botón de "OK"
                });
                location.reload(); // Recargar la página o actualizar la vista pero manda hasta avisos, checar eso
            } else {
                Swal.fire(
                    'Error',
                    'No se pudo eliminar la actividad. Intenta nuevamente.',
                    'error'
                );
            }
        } catch (error) {
            Swal.fire(
                'Error',
                'Hubo un error en la solicitud. Intenta nuevamente.',
                'error'
            );
            console.error('Error al eliminar la actividad:', error);
        }
    } else {
        Swal.fire({
            title: 'Cancelado',
            text: 'La actividad no fue eliminada.',
            icon: 'info',
            timer: 1500,  // El tiempo que se mostrará la alerta (1500ms = 1.5 segundos)
            showConfirmButton: false  // Esto es opcional, para que no aparezca el botón "OK"
        });

    }
}

//Carga los alumnos a la materia y los muestra en el div
async function cargarAlumnosAsignados(materiaIdGlobal) {
     try {
            // Hacer la petición al servidor
         const response = await fetch(`/api/DetallesMateriaApi/ObtenerAlumnosPorMateria/${materiaIdGlobal}`);

         if (!response.ok) {
             throw new Error("No se pudieron cargar los alumnos.");
         }

         // Convertir la respuesta a JSON
         const alumnos = await response.json();

         // Seleccionar el contenedor donde se mostrará la lista
         const contenedor = document.getElementById("listaAlumnosAsignados");
         contenedor.innerHTML = ""; // Limpiar contenido anterior
          // Verificar si hay alumnos
         if (alumnos.length === 0) {
             contenedor.innerHTML = `<p class="text-muted">No hay alumnos asignados a esta materia.</p>`;
             return;
         }

         // Crear la lista de alumnos
         alumnos.forEach(alumno => {
             //  Crear el div del alumno
             const divAlumno = document.createElement("div");
             divAlumno.classList.add("d-flex", "justify-content-between", "align-items-center", "p-2", "mb-2");
             divAlumno.style.background = "#f8f9fa"; // Color de fondo
             divAlumno.style.borderRadius = "8px"; // Bordes redondeados

             //  Agregar el nombre del alumno
             const spanNombre = document.createElement("span");
             spanNombre.textContent = `${alumno.nombre} ${alumno.apellidoPaterno} ${alumno.apellidoMaterno}`;
             divAlumno.appendChild(spanNombre);

             //  Contenedor de botón
             const divBotones = document.createElement("div");

             //  Botón "Eliminar del grupo" dentro de un menú desplegable
             const dropdown = document.createElement("div");
             dropdown.classList.add("dropdown");

             const btnDropdown = document.createElement("button");
             btnDropdown.classList.add("btn", "btn-danger", "btn-sm", "dropdown-toggle");
             btnDropdown.textContent = "Opciones";
             btnDropdown.setAttribute("data-bs-toggle", "dropdown");

             const dropdownMenu = document.createElement("ul");
             dropdownMenu.classList.add("dropdown-menu");

             const eliminarItem = document.createElement("li");
             const eliminarLink = document.createElement("a");
             eliminarLink.classList.add("dropdown-item");
             eliminarLink.href = "#";
             eliminarLink.textContent = "Eliminar del grupo";
             eliminarLink.onclick = function () {
                 eliminardelgrupo(alumno.alumnoMateriaId);
             };

             eliminarItem.appendChild(eliminarLink);
             dropdownMenu.appendChild(eliminarItem);
             dropdown.appendChild(btnDropdown);
             dropdown.appendChild(dropdownMenu);

             divBotones.appendChild(dropdown);
             divAlumno.appendChild(divBotones);

             // Agregar alumno a la lista
             contenedor.appendChild(divAlumno);
         });

     } catch (error) {
         console.error("Error al cargar alumnos:", error);
     }
}

function cambiarSeccion(seccion) {
    document.querySelectorAll('.seccion').forEach(div => div.style.display = 'none');
    const seccionMostrar = document.getElementById(`seccion-${seccion}`);
    if (seccionMostrar) {
        seccionMostrar.style.display = 'block';
    }

    document.querySelectorAll('.tab-button').forEach(btn => btn.classList.remove('active'));
    document.querySelector(`button[onclick="cambiarSeccion('${seccion}')"]`).classList.add('active');

    // Cargar datos si se seleccionan secciones dinámicas
    if (seccion === "actividades") {
        cargarActividadesDeMateria(materiaIdGlobal);
    }
    if (seccion === "alumnos") {
        cargarAlumnosAsignados(materiaIdGlobal);
    }
    if (seccion === "avisos") {
        cargarAvisosDeMateria(materiaIdGlobal);
    }
}



async function eliminardelgrupo(alumnoMateriaId) {
    try {
        const confirmacion = await Swal.fire({
            title: "¿Estás seguro?",
            text: "Esta acción eliminará al alumno del grupo.",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#d33",
            cancelButtonColor: "#3085d6",
            confirmButtonText: "Sí, eliminar",
            cancelButtonText: "Cancelar"
        });

        if (!confirmacion.isConfirmed) return;

        const response = await fetch(`/api/DetallesMateriaApi/EliminarAlumnoDeMateria/${alumnoMateriaId}`, {
            method: "DELETE",
        });

        if (!response.ok) {
            throw new Error("No se pudo eliminar al alumno del grupo.");
        }

        Swal.fire({
            position: "top-end",
            title: "Eliminado",
            text: "El alumno ha sido eliminado del grupo correctamente.",
            icon: "success",
            timer: 2500
        });

        cargarAlumnosAsignados(materiaIdGlobal); // Recargar la lista

    } catch (error) {
        Swal.fire({
            position: "top-end",
            title: "Error",
            text: "Hubo un problema al eliminar al alumno.",
            icon: "error",
            timer: 2500
        });

        console.error("Error al eliminar alumno:", error);
    }
}

async function registrarActividad() {
    let nombre = document.getElementById("nombre").value;
    let descripcion = document.getElementById("descripcion").value;
    let fechaHoraLimite = document.getElementById("fechaHoraLimite").value;
    let puntaje = document.getElementById("puntaje").value;

    if (!nombre || !descripcion || !fechaHoraLimite || !puntaje) {
        Swal.fire({
            icon: "warning",
            title: "Campos incompletos",
            text: "Por favor, completa todos los campos antes de continuar."
        });
        return;
    }

    let actividad = {
        nombreActividad: nombre,
        descripcion: descripcion,
        fechaLimite: fechaHoraLimite,
        tipoActividadId: 1, // Obtener esto dinámicamente si es necesario
        puntaje: parseInt(puntaje),
        materiaId: materiaIdGlobal //dentro de la materia que se encuentra
    };

    try {
        let response = await fetch("/api/DetallesMateriaApi/CrearActividad", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(actividad)
        });

        if (!response.ok) {
            throw new Error(`Error HTTP: ${response.status}`);
        }

        let data = await response.json();

        Swal.fire({
            position: "top-end",
            title: "Actividad creada",
            text: "La actividad ha sido publicado correctamente.",
            icon: "success",
            timer: 3000,
            showConfirmButton: false
        });

        setTimeout(() => {
            location.reload(); // Recargar la página después de mostrar el mensaje
        }, 2500);
    } catch (error) {
        console.error("Error:", error);
        Swal.fire({
            position: "top-end",
            title: "Error al crear la actividad",
            text: "Ocurrio un problema al crear la actividad..",
            icon: "error",
            timer: 3000,
            showConfirmButton: false
        });
    }
}


async function publicarAviso() {
    // Obtener valores de los inputs
    let titulo = document.getElementById("titulo").value.trim();
    let descripcion = document.getElementById("descripcionAviso").value.trim();

    // Validar que los campos no estén vacíos
    if (!titulo || !descripcion) {
        Swal.fire({
            position: "top-end",
            title: "Campos vacíos",
            text: "Por favor, completa todos los campos.",
            icon: "warning",
            timer: 2500,
            showConfirmButton: false
        });
        return;
    }

    // Variables globales que ya tienes en tu archivo .js
    let docenteId = docenteIdGlobal;
    let grupoId = grupoIdGlobal;
    let materiaId = materiaIdGlobal;

    // Crear objeto con los datos a enviar
    let avisoData = {
        DocenteId: docenteId,
        Titulo: titulo,
        Descripcion: descripcion,
        GrupoId: grupoId,
        MateriaId: materiaId
    };

    try {
        // Enviar datos al controlador
        let response = await fetch("/api/DetallesMateriaApi/CrearAviso", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(avisoData)
        });

        let result = await response.json();

        if (response.ok) {
            Swal.fire({
                position: "top-end",
                title: "Aviso creado",
                text: "El aviso ha sido publicado correctamente.",
                icon: "success",
                timer: 3000,
                showConfirmButton: false
            });

            setTimeout(() => {
                location.reload(); // Recargar la página después de mostrar el mensaje
            }, 3000);

        } else {
            Swal.fire({
                position: "top-end",
                title: "Error",
                text: result.mensaje || "Error al crear el aviso.",
                icon: "error",
                timer: 3000,
                showConfirmButton: false
            });
        }
    } catch (error) {
        console.error("Error:", error);
        Swal.fire({
            position: "top-end",
            title: "Error",
            text: "Hubo un problema al enviar el aviso.",
            icon: "error",
            timer: 3000,
            showConfirmButton: false
        });
    }
}



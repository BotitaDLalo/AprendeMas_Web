//Crea un nuevo grupo, con la posibilidad de agregar una materia sin grupo, y crear directamente varias materia para ese grupo
async function guardarGrupo() {
    const nombre = document.getElementById("nombreGrupo").value;
    const descripcion = document.getElementById("descripcionGrupo").value;
    const color = "#2196F3";
    const checkboxes = document.querySelectorAll(".materia-checkbox:checked");

    if (nombre.trim() === '') {
        Swal.fire({
            position: "top-end",
            icon: "question",
            title: "Ingrese nombre del grupo.",
            showConfirmButton: false,
            timer: 2500
        });
        return;
    }

    // Obtener IDs de materias seleccionadas en los checkboxes
    const materiasSeleccionadas = Array.from(checkboxes).map(cb => cb.value);

    // Obtener materias creadas en los inputs
    const materiasNuevas = [];
    document.querySelectorAll(".materia-item").forEach(materiaDiv => {
        const nombreMateria = materiaDiv.querySelector(".nombreMateria").value.trim();
        const descripcionMateria = materiaDiv.querySelector(".descripcionMateria").value.trim();
        if (nombreMateria) {
            materiasNuevas.push({ NombreMateria: nombreMateria, Descripcion: descripcionMateria });
        }
    });

    // Crear el grupo en la base de datos
    const response = await fetch('/api/GruposApi/CrearGrupo', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            NombreGrupo: nombre,
            Descripcion: descripcion,
            CodigoColor: color,
            DocenteId: docenteIdGlobal
        })
    });

    if (response.ok) {
        const grupoCreado = await response.json();
        const grupoId = grupoCreado.grupoId;

        // Guardar materias nuevas directamente asociadas al grupo
        for (const materia of materiasNuevas) {
            const responseMateria = await fetch('/api/MateriasApi/CrearMateria', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    NombreMateria: materia.NombreMateria,
                    Descripcion: materia.Descripcion,
                    CodigoColor: color, // Enviamos el color de la materia
                    DocenteId: docenteIdGlobal
                })
            });

            if (responseMateria.ok) {
                const materiaCreada = await responseMateria.json();
                materiasSeleccionadas.push(materiaCreada.materiaId);
            }
        }

        // Asociar materias seleccionadas al grupo
        if (materiasSeleccionadas.length > 0) {
            await asociarMateriasAGrupo(grupoId, materiasSeleccionadas);
        }

        Swal.fire({
            position: "top-end",
            icon: "success",
            title: "Grupo registrado correctamente.",
            showConfirmButton: false,
            timer: 2000
        });
        document.getElementById("gruposForm").reset();
        cargarGrupos();
        cargarMateriasSinGrupo();
        cargarMaterias();
    } else {
        Swal.fire({
            position: "top-end",
            icon: "error",
            title: "Error al registrar grupo.",
            showConfirmButton: false,
            timer: 2000
        });
    }
}

//funcion que ayuda a agregar materias nuevas para el grupo
function agregarMateria() {
    const materiasContainer = document.getElementById("listaMaterias");

    const materiaDiv = document.createElement("div");
    materiaDiv.classList.add("materia-item");

    materiaDiv.innerHTML = `
        <input type="text" placeholder="Nombre de la Materia" class="nombreMateria">
        <input type="text" placeholder="Descripción" class="descripcionMateria">
        <button type="button" onclick="removerDeLista(this)">❌</button>
    `;

    materiasContainer.appendChild(materiaDiv);
}

// Remover materia del formulario antes de enviarla
function removerDeLista(button) {
    button.parentElement.remove();
}



//Funcion para obtener los grupos de la base de datos y mostrarlos

async function cargarGrupos() { // Lógica para actualizar la lista de grupos en vista
    const response = await fetch(`/api/GruposApi/ObtenerGrupos/${docenteIdGlobal}`); // Solicita los grupos del docente

    if (response.ok) { // Si la respuesta es exitosa
        const grupos = await response.json(); // Convierte la respuesta a formato JSON
        const listaGrupos = document.getElementById("listaGrupos"); // Obtiene el elemento donde se mostrarán los grupos

        if (grupos.length === 0) { // Si no hay grupos registrados
            listaGrupos.innerHTML = "<p> No hay grupos registrados.</p>"; // Muestra un mensaje indicando que no hay grupos
            return; // Sale de la función
        }

        listaGrupos.innerHTML = grupos.map(grupo => `
        <div class="grupo-card mb-3" style="background-color: ${grupo.codigoColor || '#FFA500'};  
            border-radius: 12px; width: 400px; padding: 15px; margin-bottom: 15px;
            cursor: pointer; transition: all 0.3s ease-in-out;" 
             onmouseover="this.style.transform='translateY(-5px)'; this.style.boxShadow='0px 4px 12px rgba(0, 0, 0, 0.3)';" 
             onmouseout="this.style.transform='none'; this.style.boxShadow='none';"
             onclick="handleCardClick(${grupo.grupoId})">
    
         <div class="d-flex justify-content-between align-items-center">
             <h5 class="text-white mb-0">
                <strong class="font-weight-bold">${grupo.nombreGrupo}</strong> - ${grupo.descripcion || "Sin descripción"}
             </h5>
             
         <div class="dropdown">
             <button class="btn btn-link text-white p-0" type="button" data-bs-toggle="dropdown" aria-expanded="false" onclick="event.stopPropagation();">
                 <i class="fas fa-cog"></i> <!-- Icono de engranaje -->
             </button>
                     <ul class="dropdown-menu dropdown-menu-end">
                     <li><a class="dropdown-item" href="#" onclick="editarGrupo(${grupo.grupoId})">Editar</a></li> <!-- Opción para editar grupo -->
                     <li><a class="dropdown-item" href="#" onclick="eliminarGrupo(${grupo.grupoId})">Eliminar</a></li> <!-- Opción para eliminar grupo -->
                     <li><a class="dropdown-item" href="#" onclick="agregarMateriaAlGrupo(${grupo.grupoId})">Crear Materia</a></li> <!-- Opción para agregar materia al grupo -->
                     <li><a class="dropdown-item" href="#" onclick="crearAvisoGrupal(${grupo.grupoId})">Aviso Grupal</a></li> <!-- Opción para aviso grupal grupo -->
                     </ul>
                 </div>
            </div>
        </div>
        <!-- Contenedor donde se mostrarán las materias -->
         <div id="materiasContainer-${grupo.grupoId}" class="materias-container" style="display: none; padding-left: 20px;"></div>
        `).join(''); // Muestra los grupos como tarjetas dinámicas
    } else {
        let timerInterval; // Variable para almacenar el intervalo del temporizador

        Swal.fire({
            title: "Error al cargar los grupos.", // Mensaje de error en la alerta
            html: "Se reintentará automáticamente en: <b></b>.", // Mensaje con temporizador dinámico
            timer: 4000, // Tiempo en milisegundos antes de que la alerta se cierre automáticamente
            timerProgressBar: true, // Muestra una barra de progreso indicando el tiempo restante
            allowOutsideClick: false, // Evita que el usuario cierre la alerta haciendo clic fuera de ella
            showCancelButton: true, // Muestra un botón de cancelar dentro de la alerta
            cancelButtonText: "Cerrar sesión", // Texto que aparecerá en el botón de cancelar

            didOpen: () => {
                // Se ejecuta cuando la alerta se abre
                Swal.showLoading(); // Muestra un indicador de carga
                const timer = Swal.getPopup().querySelector("b"); // Obtiene el elemento <b> para mostrar el tiempo restante
                timerInterval = setInterval(() => {
                    timer.textContent = `${Math.floor(Swal.getTimerLeft() / 1000)} segundos`; // Actualiza el temporizador en segundos
                }, 100); // Se actualiza cada 100ms
            },

            willClose: () => {
                // Se ejecuta cuando la alerta está a punto de cerrarse
                clearInterval(timerInterval); // Detiene la actualización del temporizador
            }

        }).then((result) => {
            // Se ejecuta cuando la alerta se cierra manualmente o por el temporizador
            if (result.dismiss === Swal.DismissReason.timer) {
                // Si la alerta se cerró automáticamente por el temporizador
                console.log("Reintentando cargar los grupos.");
                if (intentosAcceder < 6) {
                    inicializar(); // Llama a la función inicializar para reintentar la carga
                    intentosAcceder++;
                } else {
                    AlertaCierreSesion();
                }
            } else if (result.dismiss === Swal.DismissReason.cancel) {
                // Si el usuario hizo clic en "Cerrar sesión"
                console.log("El usuario decidió cerrar sesión.");
                cerrarSesion(); // Llama a la función para cerrar sesión
            }
        });
    }
    document.getElementById('gruposModal').addEventListener('hidden.bs.modal', function () {
        cargarGrupos(); // Vuelve a cargar los grupos cuando se cierra el modal
    });
}




// Función para cargar materias de un grupo cuando se hace clic en la card del grupo
async function handleCardClick(grupoId) {
    localStorage.setItem("grupoIdSeleccionado", grupoId); //Se guardar el localstorage el id del grupo seleccionado

    // Ocultar todas las materias de otros grupos
    document.querySelectorAll("[id^='materiasContainer-']").forEach(container => {
        if (container.id !== `materiasContainer-${grupoId}`) {
            container.style.display = "none";
            container.innerHTML = "";
        }
    });

    const materiasContainer = document.getElementById(`materiasContainer-${grupoId}`);

    if (materiasContainer.style.display === "block") {
        // Si las materias están visibles, ocultarlas
        materiasContainer.style.display = "none";
        materiasContainer.innerHTML = "";
    } else {
        // Si están ocultas, obtener las materias y mostrarlas
        const response = await fetch(`/api/GruposApi/ObtenerMateriasPorGrupo/${grupoId}`);
        if (response.ok) {
            const materias = await response.json();
            if (materias.length === 0) {
                materiasContainer.innerHTML = "<p>Aún no hay materias registradas para este grupo.</p>";
            } else {
                materiasContainer.innerHTML = `
                    <div class="container-cards">
                        ${materias.map(materia => `
                            <div class="card card-custom" style="border-radius: 10px;">
                                <div class="card-header-custom" style="background-color: ${materia.codigoColor || '#000'};">
                                    <div class="d-flex justify-content-between align-items-center">
                                        <span class="text-dark">${materia.nombreMateria}</span>
                                        <div class="dropdown">
                                            <button class="btn btn-link p-0 text-dark" data-bs-toggle="dropdown" aria-expanded="false">
                                                <i class="fas fa-ellipsis-v"></i>
                                            </button>
                                            <ul class="dropdown-menu dropdown-menu-end">
                                                <li><a class="dropdown-item" href="#" onclick="editarMateria(${materia.materiaId},'${materia.nombreMateria}','${materia.descripcion}')">Editar</a></li>
                                                <li><a class="dropdown-item" href="#" onclick="eliminarMateria(${materia.materiaId})">Eliminar</a></li>
                                            </ul>
                                        </div>
                                    </div>
                                </div>


                                <div class="card-body card-body-custom">
    <p class="card-text">
        ${materia.actividadesRecientes.length > 0
                            ? materia.actividadesRecientes.map((actividad, index) => {
                                    const fechaFormateada = new Date(actividad.fechaCreacion).toLocaleDateString('es-ES', {
                                        day: '2-digit',
                                        month: '2-digit',
                                        year: 'numeric'
                                    });
                                    return `
                    <div class="actividad-item">
                        <a href="#" class="actividad-link" data-id="${actividad.actividadId}">
                            ${actividad.nombreActividad}
                        </a>
                        <p class="actividad-fecha">Asignada: ${fechaFormateada}</p>
                    </div>
                `;
                                }).join('')
                                : "<p class='sin-actividades'>Sin actividades recientes</p>"
        }
    </p>
</div>



                                <div class="card-footer card-footer-custom">
                                    <button class="btn btn-sm btn-primary" onclick="irAMateria(${materia.materiaId})">Ver Materia</button>
                                    <div class="icon-container">
                                        <img class="icon-action" src="https://cdn-icons-png.flaticon.com/512/1828/1828817.png" alt="Ver Actividades" title="Ver Actividades" onclick="irAMateria(${materia.materiaId}, 'actividades')">
                                        <img class="icon-action" src="https://cdn-icons-png.flaticon.com/512/847/847969.png" alt="Ver Integrantes" title="Ver Integrantes" onclick="irAMateria(${materia.materiaId}, 'alumnos')">
                                    </div>
                                </div>
                            </div>
                        `).join('')}
                    </div>
                `;
            }
            materiasContainer.style.display = "block";
        } else {
            Swal.fire({
                position: "top-end",
                icon: "error",
                title: "Error al obtener las materias del grupo.",
                showConfirmButton: false,
                timer: 2000
            });
        }
    }
}



//Funciones de contenedor de grupo
function editarGrupo(id) {
    alert("Editar grupo " + id); // Muestra una alerta indicando que el grupo será editado
}

async function eliminarGrupo(grupoId) {
    Swal.fire({
        title: "¿Qué deseas eliminar?",
        text: "Elige si deseas eliminar solo el grupo o también las materias que contiene.",
        icon: "warning",
        showCancelButton: true,
        showDenyButton: true,
        confirmButtonText: "Eliminar solo grupo",
        denyButtonText: "Eliminar grupo y materias",
        cancelButtonText: "Cancelar"
    }).then(async (result) => {
        if (result.isConfirmed) {
            // Llamar al controlador que elimina solo el grupo
            const response = await fetch(`/api/GruposApi/EliminarGrupo/${grupoId}`, { method: "DELETE" });
            if (response.ok) {
                await Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "El grupo ha sido eliminado.",
                    showConfirmButton: false,
                    timer: 2000
                });
                inicializar();
            } else {
                await Swal.fire({
                    position: "top-end",
                    icon: "error",
                    title: "No se pudo eliminar el grupo.",
                    showConfirmButton: false,
                    timer: 2000
                });
            }
        } else if (result.isDenied) {
            // Llamar al nuevo controlador que elimina grupo y materias
            const response = await fetch(`/api/GruposApi/EliminarGrupoConMaterias/${grupoId}`, { method: "DELETE" });
            if (response.ok) {
                await Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "El grupo y sus materias han sido eliminados.",
                    showConfirmButton: false,
                    timer: 2000
                });
                inicializar();
            } else {
                await Swal.fire({
                    position: "top-end",
                    icon: "error",
                    title: "No se pudo eliminar el grupo y sus materias.",
                    showConfirmButton: false,
                    timer: 2000
                });
            }
        }
    });
}


function agregarMateriaAlGrupo(id) {
    alert("Agregar Materia Al Grupo " + id); // Muestra una alerta indicando que el grupo será desactivado
}

function crearAvisoGrupal(id) {
    alert("Crear aviso al grupo " + id);
}

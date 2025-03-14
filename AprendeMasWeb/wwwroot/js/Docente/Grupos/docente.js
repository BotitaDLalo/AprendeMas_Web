let docenteIdGlobal = null; //Variable global para almacenar el docenteId
let materiasPorCrear = []; // Lista de materias a crear
let intentosAcceder = 0;
// Agregar una nueva materia al formulario
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



async function obtenerDocenteId() {
    try {
        // Hacemos una solicitud para obtener el docenteId desde el servidor
        const response = await fetch('/Cuenta/ObtenerDocenteId'); // Llamar al controlador
        const data = await response.json(); // Convertimos la respuesta en formato JSON
        if (data.docenteId) {
            docenteIdGlobal = data.docenteId; // Guardamos el docenteId en la variable global
            localStorage.setItem("docenteId", docenteIdGlobal); // Guardamos el docenteId en el almacenamiento local

            // Alerta con diseño de Toast
            const Toast = Swal.mixin({
                toast: true,
                position: "top-end",
                showConfirmButton: false,
                timer: 1500,
                timerProgressBar: true,
                didOpen: (toast) => {
                    toast.onmouseleave = Swal.resumeTimer;
                }
            });

            Toast.fire({
                icon: "success",
                title: "Todo correcto."
            });

        }
    } catch (error) {
        AlertaCierreSesion(); // Si existe un error, cierra la sesión
    }
}


function alertaDeErroresGenerales(error) {
    // Mensaje de error por defecto
    let mensajeError = "Ocurrió un error inesperado.";

    // Si el error tiene un mensaje, lo usamos
    if (error && error.message) {
        mensajeError = error.message;
    }

    // Enlace para enviar un correo con el error incluido en el cuerpo
    const enlaceCorreo = `mailto:soporte@tuempresa.com?subject=Error%20en%20la%20aplicación
        &body=Hola,%20tengo%20un%20problema%20en%20la%20aplicación.%0A%0ADetalles%20del%20error:%0A${encodeURIComponent(mensajeError)}
        %0A%0APor%20favor,%20ayuda.`.replace(/\s+/g, ''); // Limpia espacios innecesarios

    // Mostrar alerta
    Swal.fire({
        icon: "error",
        title: "Oops...",
        text: mensajeError,
        position: "center",
        allowOutsideClick: false//, // Evita que se cierre con un clic afuera
        //footer: `<a href="${enlaceCorreo}" target="_blank">Si el problema persiste, contáctanos.</a>`
    });
}


function AlertaCierreSesion() { //funcion que activa la alerta y posteriormente cierra sesion
    let timerInterval;
    Swal.fire({
        title: "Parece que se perdió la conexión con tu sesión.",
        html: "La cerraremos por seguridad y podrás volver a iniciar sesión en <b></b>.",
        timer: 5000,
        timerProgressBar: true,
        position: "center",
        allowOutsideClick: false, // Evita que se cierre al hacer clic fuera
        didOpen: () => {
            Swal.showLoading();
            const timer = Swal.getPopup().querySelector("b");
            timerInterval = setInterval(() => {
                timer.textContent = `${Math.floor(Swal.getTimerLeft() / 1000)} segundos`;
            }, 100);
        },
        willClose: () => {
            clearInterval(timerInterval);
            cerrarSesion();
        }
    }).then((result) => {
        if (result.dismiss === Swal.DismissReason.timer) {
            console.log("Cerrando sesión automáticamente.");
        }
    }); //Se cierra la sesion al no obtener el id del docente, ya que es necesario para todo. raramente se activara esto, pero es mejor tenerlo.
}


// 🔹 Función para cerrar sesión
async function cerrarSesion() {
    try {
        // Realiza una solicitud POST al endpoint de cierre de sesión
        const response = await fetch('/Cuenta/CerrarSesion', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded', // Especifica el tipo de contenido
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value // Obtiene el token de verificación CSRF
            }
        });

        if (response.ok) {
            console.log("Sesión cerrada correctamente."); // Mensaje en consola indicando que la sesión se cerró con éxito
            window.location.href = "/Cuenta/IniciarSesion"; // Redirige al usuario a la página de inicio de sesión
        } else {
            // En caso de error en la respuesta del servidor, muestra un mensaje de alerta con SweetAlert2
            Swal.fire({
                icon: "error",
                title: "Oops...",
                text: "No se pudo cerrar sesión.",
                position: "center",
                allowOutsideClick: false//,// Evita que la alerta se cierre al hacer clic fuera de ella
               // footer: '<a href="mailto:soporte@tuempresa.com?subject=Problema%20con%20cierre%20de%20sesión&body=Hola,%20tengo%20un%20problema%20al%20cerrar%20sesión.%20Por%20favor,%20ayuda." target="_blank">Si el problema persiste, contáctanos.</a>'
            });
        }
    } catch (error) {
        // Captura cualquier error inesperado (por ejemplo, problemas de conexión) y muestra una alerta
        alertaDeErroresGenerales(error);
    }
}

//Guarda las materias en la tabla tbMaterias -------------------
async function guardarMateriaSinGrupo() {
    const nombre = document.getElementById("nombreMateria").value; // Obtenemos el nombre de la materia desde el input
    const descripcion = document.getElementById("descripcionMateria").value; // Obtenemos la descripción de la materia
    const color = "#2196F3"; // Asignamos un color predeterminado para la materia

    if (nombre.trim() === '') { // Verificamos que el nombre de la materia no esté vacío
        Swal.fire({
            position: "top-end",
            icon: "question",
            title: "Ingrese nombre de la materia.",
            showConfirmButton: false,
            timer: 2500
        });// Mostramos una alerta si el nombre está vacío
        return;
    }

    // Enviamos una solicitud POST al servidor para guardar la materia
    const response = await fetch('/api/MateriasApi/CrearMateria', {
        method: 'POST', // Indicamos que la solicitud será de tipo POST
        headers: { 'Content-Type': 'application/json' }, // Especificamos que el cuerpo de la solicitud será JSON
        body: JSON.stringify({
            NombreMateria: nombre, // Enviamos el nombre de la materia
            Descripcion: descripcion, // Enviamos la descripción de la materia
            CodigoColor: color, // Enviamos el color de la materia
            DocenteId: docenteIdGlobal // Enviamos el docenteId obtenido previamente
        })
    });

    if (response.ok) { // Verificamos si la respuesta es exitosa
        Swal.fire({
            position: "top-end",
            icon: "success",
            title: "Materia registrada correctamente.",
            showConfirmButton: false,
            timer: 2000
        });
        ;// Mostramos una alerta de éxito
        document.getElementById("materiasForm").reset(); // Limpiamos el formulario
        cargarMateriasSinGrupo(); // Recargamos la lista de materias sin grupo
    } else {
        Swal.fire({
            position: "top-end",
            icon: "error",
            title: "Error al registrar materia.",
            showConfirmButton: false,
            timer: 2000
        }); // Mostramos una alerta si hubo un error al guardar
    }
}
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


// Función para asociar materias al grupo
async function asociarMateriasAGrupo(grupoId, materias) {
    const response = await fetch('/api/GruposApi/AsociarMaterias', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ GrupoId: grupoId, MateriaIds: materias })
    });

    if (!response.ok) {
        Swal.fire({
            position: "top-end",
            icon: "error",
            title: "Error al asociar materias con grupo.",
            showConfirmButton: false,
            timer: 2000
        });
    }
}
// Función para cargar las materias disponibles en el modal 
async function cargarMaterias() {
    try {
        // Hacemos una solicitud GET al servidor para obtener las materias sin grupo
        const response = await fetch(`/api/MateriasApi/ObtenerMateriasSinGrupo/${docenteIdGlobal}`);
        if (response.ok) { // Verificamos si la respuesta es exitosa
            const materias = await response.json(); // Convertimos la respuesta en formato JSON
            const contenedorMaterias = document.getElementById("materiasLista"); // Obtenemos el contenedor donde se mostrarán las materias
            contenedorMaterias.innerHTML = ""; // Limpiamos cualquier contenido previo en el contenedor

            if (materias.length === 0) { // Si no hay materias disponibles, mostramos un mensaje
                contenedorMaterias.innerHTML = "<p>No hay materias disponibles.</p>";
                return;
            }

            // Recorremos todas las materias obtenidas y las mostramos en el contenedor
            materias.forEach(materia => {
                const checkbox = document.createElement("input"); // Creamos un checkbox para cada materia
                checkbox.type = "checkbox"; // Definimos que sea un checkbox
                checkbox.className = "materia-checkbox"; // Asignamos una clase para identificarlos
                checkbox.value = materia.materiaId; // Asignamos el ID de la materia como valor del checkbox

                const label = document.createElement("label"); // Creamos una etiqueta para el checkbox
                label.appendChild(checkbox); // Añadimos el checkbox a la etiqueta
                label.appendChild(document.createTextNode(" " + materia.nombreMateria)); // Añadimos el nombre de la materia a la etiqueta

                const div = document.createElement("div"); // Creamos un contenedor div para cada materia
                div.className = "form-check"; // Asignamos una clase para estilo
                div.appendChild(label); // Añadimos el label al div

                contenedorMaterias.appendChild(div); // Añadimos el div al contenedor
            });
        }
    } catch (error) {
        console.error("Error al cargar materias:", error); // Mostramos un mensaje de error si hay un problema al cargar las materias
    }
}


// Cargar materias sin grupo -------------------------------------------------------
async function cargarMateriasSinGrupo() {
    const response = await fetch(`/api/MateriasApi/ObtenerMateriasSinGrupo/${docenteIdGlobal}`) // Hace una solicitud GET para obtener las materias sin grupo usando el DocenteId global
    if (response.ok) { // Si la respuesta es exitosa
        const materiasSinGrupo = await response.json(); // Convierte la respuesta a formato JSON
        const listaMateriasSinGrupo = document.getElementById("listaMateriasSinGrupo"); // Obtiene el elemento donde se mostrarán las materias sin grupo

        if (materiasSinGrupo.length === 0) { // Si no hay materias sin grupo
            listaMateriasSinGrupo.innerHTML = "<p>No hay materias registradas.</p>"; // Muestra un mensaje indicando que no hay materias
            return; // Sale de la función
        }

        // Asegurar que todas las cards estén dentro de un solo contenedor
        listaMateriasSinGrupo.innerHTML = `
            <div class="container-cards">
                ${materiasSinGrupo.map(materiaSinGrupo => `
                    <div class="card card-custom" style=" border-radius: 10px; /* Define el radio de las esquinas */">
                        <div class="card-header-custom" style="background-color: ${materiaSinGrupo.codigoColor || '#000'};  ">
                            <div class="d-flex justify-content-between align-items-center">
                                <span class="text-dark">${materiaSinGrupo.nombreMateria}</span> <!-- Muestra el nombre de la materia -->
                                <div class="dropdown">
                                    <button class="btn btn-link p-0 text-dark" data-bs-toggle="dropdown" aria-expanded="false">
                                        <i class="fas fa-ellipsis-v"></i>
                                    </button>
                                    <ul class="dropdown-menu dropdown-menu-end">
                                        <li><a class="dropdown-item" href="#" onclick="editarMateria(${materiaSinGrupo.materiaId},'${materiaSinGrupo.nombreMateria}','${materiaSinGrupo.descripcion}')">Editar</a></li> <!-- Opción para editar materia -->
                                        <li><a class="dropdown-item" href="#" onclick="eliminarMateria(${materiaSinGrupo.materiaId})">Eliminar</a></li> <!-- Opción para eliminar materia -->
                                        <li><a class="dropdown-item" href="#" onclick="desabilitarMateria(${materiaSinGrupo.materiaId})">Desactivar</a></li> <!-- Opción para desactivar materia -->
                                    </ul>
                                </div>
                            </div>
                        </div>

                        <div class="card-body card-body-custom" style="background-color: #e0e0e0">
                            <p class="card-text">${materiaSinGrupo.descripcion || "Sin descripción"}</p> <!-- Muestra la descripción de la materia o un mensaje por defecto -->
                        </div>

                        <div class="card-footer card-footer-custom">
                            <button class="btn btn-sm btn-primary" onclick="irAMateria(${materiaSinGrupo.materiaId})">Ver Materia</button> <!-- Botón para ver los detalles de la materia -->
                            <div class="icon-container">
                                <img class="icon-action" src="https://cdn-icons-png.flaticon.com/512/1828/1828817.png" alt="Ver Actividades" title="Ver Actividades" onclick="verActividades(${materiaSinGrupo.materiaId})"> <!-- Icono para ver actividades -->
                                <img class="icon-action" src="https://cdn-icons-png.flaticon.com/512/847/847969.png" alt="Ver Integrantes" title="Ver Integrantes" onclick="verIntegrantes(${materiaSinGrupo.materiaId})"> <!-- Icono para ver los integrantes -->
                                <img class="icon-action" src="https://cdn-icons-png.flaticon.com/512/535/535285.png" alt="Destacar" title="Destacar Materia" onclick="destacarMateria(${materiaSinGrupo.materiaId})"> <!-- Icono para destacar la materia -->
                            </div>
                        </div>
                    </div>
                `).join('')} <!-- Muestra todas las materias sin grupo como tarjetas dinámicas -->
            </div>
        `;
    } else {
        let timerInterval; // Variable para almacenar el intervalo del temporizador

        Swal.fire({
            title: "Error al cargar materias sin grupo asignado.", // Título de la alerta
            html: "Se reintentará automáticamente en: <b></b>.", // Mensaje con temporizador dinámico
            timer: 4000, // Tiempo en milisegundos antes de que la alerta se cierre automáticamente
            timerProgressBar: true, // Muestra una barra de progreso indicando el tiempo restante
            allowOutsideClick: false, // Evita que el usuario cierre la alerta haciendo clic fuera de ella
            showCancelButton: true, // Muestra un botón de cancelar
            cancelButtonText: "Cerrar sesión", // Texto del botón de cancelar

            didOpen: () => {
                // Se ejecuta cuando la alerta se abre
                Swal.showLoading(); // Muestra un indicador de carga
                const timer = Swal.getPopup().querySelector("b"); // Obtiene el elemento <b> para mostrar el tiempo restante
                timerInterval = setInterval(() => {
                    timer.textContent = `${Math.floor(Swal.getTimerLeft() / 1000)} segundos`; // Actualiza el temporizador en segundos
                }, 100); // Actualiza el temporizador cada 100ms
            },

            willClose: () => {
                // Se ejecuta cuando la alerta está a punto de cerrarse
                clearInterval(timerInterval); // Detiene la actualización del temporizador
            }

        }).then((result) => {
            // Se ejecuta cuando la alerta se cierra manualmente o por el temporizador
            if (result.dismiss === Swal.DismissReason.timer) {
                // Si la alerta se cierra automáticamente por el temporizador
                console.log("Reintentando cargar las materias sin grupo.");
                if (intentosAcceder < 6) {
                    inicializar(); // Llama a la función inicializar para reintentar la carga
                    intentosAcceder++;
                } else {
                    AlertaCierreSesion();
                }
            } else if (result.dismiss === Swal.DismissReason.cancel) {
                // Si el usuario hace clic en "Cerrar sesión"
                console.log("El usuario decidió cerrar sesión.");
                cerrarSesion(); // Llama a la función para cerrar sesión
            }
        });

    }
    document.getElementById('materiasModal').addEventListener('hidden.bs.modal', function () {
        cargarMateriasSinGrupo(); // Vuelve a cargar las materias sin grupo cuando se cierra el modal
    });
}





// Función para redirigir a la vista Materias dentro del controlador Docente
function irAMateria(materiaIdSeleccionada) {
    //guardar el id de la materia para acceder a la materia en la que se entro y usarla en otro script
    localStorage.setItem("materiaIdSeleccionada", materiaIdSeleccionada);
    window.location.href = `/Docente/MateriasDetalles?materiaId=${materiaIdSeleccionada}`; // Redirige a la página de detalles de la materia
}


//Funcion para obtener los grupos de la base de datos. -----------------------------------------------------
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
                     <li><a class="dropdown-item" href="#" onclick="desactivarGrupo(${grupo.grupoId})">Desactivar</a></li> <!-- Opción para desactivar grupo -->
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




//Funcionalidades de los iconos de las Cards de materias.----------------------------------------------------------------
function verActividades(MateriaId) {
    alert(`Ver actividades del grupo ID: ${MateriaId}`); // Muestra una alerta con el ID de la materia
    // Aquí puedes redirigir o cargar las actividades relacionadas con el grupo
}

function verIntegrantes(MateriaId) {
    alert(`Ver integrantes del grupo ID: ${MateriaId}`); // Muestra una alerta con el ID de la materia
    // Aquí puedes abrir un modal o redirigir para mostrar los integrantes
}

function destacarMateria(MateriaId) {
    alert(`Grupo ID: ${MateriaId} marcado como destacado`); // Muestra una alerta con el ID de la materia destacada
    // Aquí puedes implementar la lógica para destacar la materia
}

async function editarMateria(materiaId, nombreActual, descripcionActual) {
    const { value: formValues } = await Swal.fire({
        title: "Editar Materia",
        html: `
            <div style="display: flex; flex-direction: column; gap: 10px; text-align: left;">
                <div style="display: flex; align-items: center; gap: 10px;">
                    <label for="swal-nombre" style="width: 100px;">Materia</label>
                    <input id="swal-nombre" class="swal2-input"  placeholder="Nombre" value="${nombreActual}">
                </div>
                <div style="display: flex; align-items: center; gap: 5px;">
                    <label for="swal-descripcion" style="width: 100px;">Descripción</label>
                    <input id="swal-descripcion" class="swal2-input" placeholder="Descripción" value="${descripcionActual}">
                </div>
            </div>
        `,
        focusConfirm: false,
        showCancelButton: true,
        confirmButtonText: "Guardar",
        cancelButtonText: "Cancelar",
        preConfirm: () => {
            return {
                nombre: document.getElementById("swal-nombre").value,
                descripcion: document.getElementById("swal-descripcion").value
            };
        }
    });

    if (formValues) {
        // Enviar los datos al servidor para actualizar la materia
        const response = await fetch(`/api/MateriasApi/ActualizarMateria/${materiaId}`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(formValues)
        });

        if (response.ok) {
            Swal.fire({
                position: "top-end",
                icon: "success",
                title: "Materia actualizada correctamente",
                showConfirmButton: false,
                timer: 2000
            });
            cargarMaterias(); // Recargar la lista de materias
        } else {
            Swal.fire({
                position: "top-end",
                icon: "error",
                title: "Error al actualizar la materia",
                showConfirmButton: false,
                timer: 2000
            });
        }
    }
}

function desactivarMateria(MateriaId) {

}

async function eliminarMateria(MateriaId) {
    const confirmacion = await Swal.fire({
        title: "¿Estás seguro?",
        text: "No podrás recuperar esta materia después de eliminarla.",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Sí, eliminar",
        cancelButtonText: "Cancelar"
    });

    if (confirmacion.isConfirmed) {
        try {
            const response = await fetch(`/api/MateriasApi/EliminarMateria/${MateriaId}`, {
                method: "DELETE"
            });

            const resultado = await response.json();

            if (response.ok) {
                await Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: resultado.mensaje || "Eliminado.",
                    showConfirmButton: false,
                    timer: 2000
                });
                // Se ejecuta funcion inicializar para actualizar vista completa
                inicializar();
            } else {
                await Swal.fire({
                    position: "top-end",
                    icon: "error",
                    title: resultado.mensaje || "No se pudo eliminar el grupo y sus materias.",
                    showConfirmButton: false,
                    timer: 2000
                });
            }
        } catch (error) {
            await Swal.fire({
                position: "top-end",
                icon: "error",
                title: "Ocurrio un problema al eliminar la materia.",
                showConfirmButton: false,
                timer: 2000
            });
        }
    }
}
//Funcionalidades de los iconos de las cards de grupos

//esta funcion oculta la materias de los demas

// Función para cargar materias de un grupo cuando se hace clic en la card del grupo
async function handleCardClick(grupoId) {
    localStorage.setItem("grupoIdSeleccionado", grupoId);

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
                                                <li><a class="dropdown-item" href="#" onclick="desactivarMateria(${materia.materiaId})">Desactivar</a></li>
                                            </ul>
                                        </div>
                                    </div>
                                </div>
                                <div class="card-body card-body-custom" style="background-color: #e0e0e0">
                                    <p class="card-text">${materia.descripcion || "Sin descripción"}</p>
                                </div>
                                <div class="card-footer card-footer-custom">
                                    <button class="btn btn-sm btn-primary" onclick="irAMateria(${materia.materiaId})">Ver Materia</button>
                                    <div class="icon-container">
                                        <img class="icon-action" src="https://cdn-icons-png.flaticon.com/512/1828/1828817.png" alt="Ver Actividades" title="Ver Actividades" onclick="verActividades(${materia.materiaId})">
                                        <img class="icon-action" src="https://cdn-icons-png.flaticon.com/512/847/847969.png" alt="Ver Integrantes" title="Ver Integrantes" onclick="verIntegrantes(${materia.materiaId})">
                                        <img class="icon-action" src="https://cdn-icons-png.flaticon.com/512/535/535285.png" alt="Destacar" title="Destacar Materia" onclick="destacarMateria(${materia.materiaId})">
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


function desactivarGrupo(id) {
    alert("Desactivar grupo " + id); // Muestra una alerta indicando que el grupo será desactivado
}

// Ejecutar primero la obtención del DocenteId y luego cargar los datos
async function inicializar() {
    await obtenerDocenteId(); // Espera a que el ID se obtenga antes de continuar
    if (docenteIdGlobal) { // Si el DocenteId es válido
        cargarMateriasSinGrupo(docenteIdGlobal); // Carga las materias sin grupo
        cargarGrupos(docenteIdGlobal); // Carga los grupos
    } else {        
        // Si no se obtiene el DocenteId, muestra un error
        AlertaCierreSesion();
    }
}

//Prioriza la ejecucion al cargar index
// Llamar a la función inicializadora cuando se cargue la página
document.addEventListener("DOMContentLoaded", () => {
    inicializar(); // Carga inicial de datos
    // ✅ Se ejecuta SOLO cuando se abre el modal
    document.getElementById("gruposModal").addEventListener("shown.bs.modal", cargarMaterias);
});



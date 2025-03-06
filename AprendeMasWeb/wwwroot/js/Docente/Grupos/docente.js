//usar el claim como variable global dentro del inicio de sesion.
let docenteIdGlobal = null; //Variable global para almacenar el docenteId

//Funcion que busca el claim del docenteId y usarlo en este archivo
async function obtenerDocenteId() {
    try {
        // Hacemos una solicitud para obtener el docenteId desde el servidor
        const response = await fetch('/Cuenta/ObtenerDocenteId'); // Llamar al controlador
        const data = await response.json(); // Convertimos la respuesta en formato JSON
        if (data.docenteId) {
            docenteIdGlobal = data.docenteId; // Guardamos el docenteId en la variable global
            localStorage.setItem("docenteId", docenteIdGlobal); // Guardamos el docenteId en el almacenamiento local
            Swal.fire({
                position: "top-end",
                icon: "success",
                title: "Inicio sesión correctamente.",
                position: "center",
                showConfirmButton: false,
                timer: 2000
            });// Mostramos aviso que se inicio sesion correctamente
        } 
    } catch (error) {
        AlertaCierreSesion(); //si existe un error fuera de la aplicacion cierra la sesión
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
        allowOutsideClick: false, // Evita que se cierre con un clic afuera
        footer: `<a href="${enlaceCorreo}" target="_blank">Si el problema persiste, contáctanos.</a>`
    });
}


function AlertaCierreSesion() { //funcion que activa la alerta y posteriormente cierra sesion
    let timerInterval;
    Swal.fire({
        title: "Parece que se perdió la conexión con tu sesión.",
        html: "La cerraremos por seguridad y podrás volver a iniciar sesión en: <br> <b></b>.",
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
                allowOutsideClick: false, // Evita que la alerta se cierre al hacer clic fuera de ella
                footer: '<a href="mailto:soporte@tuempresa.com?subject=Problema%20con%20cierre%20de%20sesión&body=Hola,%20tengo%20un%20problema%20al%20cerrar%20sesión.%20Por%20favor,%20ayuda." target="_blank">Si el problema persiste, contáctanos.</a>'
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
            position: "center",
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
            position: "center",
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
            position: "center",
            timer: 2000
        }); // Mostramos una alerta si hubo un error al guardar
    }
}

//Guarda el grupo con o sin materias enlazadas --------------
async function guardarGrupo() { // Lógica para guardar el grupo
    const nombre = document.getElementById("nombreGrupo").value; // Obtenemos el nombre del grupo
    const descripcion = document.getElementById("descripcionGrupo").value; // Obtenemos la descripción del grupo
    const color = "#2196F3"; // Asignamos un color predeterminado para el grupo
    const checkboxes = document.querySelectorAll(".materia-checkbox:checked"); // Obtenemos todos los checkboxes seleccionados

    if (nombre.trim() === '') { // Verificamos que el nombre del grupo no esté vacío
        Swal.fire({
            position: "top-end",
            icon: "question",
            title: "Ingrese nombre del grupo.",
            position: "center",
            showConfirmButton: false,
            timer: 2500
        });// Mostramos una alerta si el nombre está vacío
        return;
    }

    // Obtenemos los IDs de las materias seleccionadas para asociarlas al grupo
    const materiasSeleccionadas = Array.from(checkboxes).map(cb => cb.value); // Creamos un array con los valores (IDs) de las materias seleccionadas

    // Enviamos una solicitud POST al servidor para guardar el grupo
    const response = await fetch('/api/GruposApi/CrearGrupo', {
        method: 'POST', // Indicamos que la solicitud será de tipo POST
        headers: { 'Content-Type': 'application/json' }, // Especificamos que el cuerpo de la solicitud será JSON
        body: JSON.stringify({
            NombreGrupo: nombre, // Enviamos el nombre del grupo
            Descripcion: descripcion, // Enviamos la descripción del grupo
            CodigoColor: color, // Enviamos el color del grupo
            DocenteId: docenteIdGlobal // Enviamos el docenteId
        })
    });

    if (response.ok) { // Verificamos si la respuesta es exitosa
        const grupoCreado = await response.json(); // Obtenemos el grupo creado junto con su ID

        // Si se han seleccionado materias, las asociamos al grupo recién creado
        if (materiasSeleccionadas.length > 0) {
            await asociarMateriasAGrupo(grupoCreado.grupoId, materiasSeleccionadas); // Llamamos a la función para asociar las materias al grupo
        }

        Swal.fire({
            position: "top-end",
            icon: "success",
            title: "Grupo registrado correctamente.",
            position: "center",
            showConfirmButton: false,
            timer: 2000
        });
        ;// Mostramos una alerta de éxito
        document.getElementById("gruposForm").reset(); // Limpiamos el formulario
        cargarGrupos(); // Recargamos la lista de grupos
        cargarMaterias(); //Recarga las materias disponibles para enlazar
    } else {
        Swal.fire({
            position: "top-end",
            icon: "error",
            title: "Error al registrar grupo.",
            position: "center",
            showConfirmButton: false,
            timer: 2000
        }); // Mostramos una alerta si hubo un error al guardar
    }
}

// Función para asociar materias al grupo
async function asociarMateriasAGrupo(grupoId, materias) {
    // Enviamos una solicitud POST para asociar las materias al grupo
    const response = await fetch('/api/GruposApi/AsociarMaterias', {
        method: 'POST', // Indicamos que la solicitud será de tipo POST
        headers: { 'Content-Type': 'application/json' }, // Especificamos que el cuerpo de la solicitud será JSON
        body: JSON.stringify({
            GrupoId: grupoId, // Enviamos el ID del grupo
            MateriaIds: materias // Enviamos los IDs de las materias seleccionadas
        })
    });

    if (!response.ok) { // Si la respuesta no es exitosa, mostramos una alerta de error
        Swal.fire({
            position: "top-end",
            icon: "error",
            title: "Error al asociar materias con grupo.",
            showConfirmButton: false,
            position: "center",
            timer: 2000
        }); // Mostramos una alerta si hubo un error al guardar
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
                                        <li><a class="dropdown-item" href="#" onclick="openEditModal(${materiaSinGrupo.materiaId})">Editar</a></li> <!-- Opción para editar materia -->
                                        <li><a class="dropdown-item" href="#" onclick="openDeleteModal(${materiaSinGrupo.materiaId})">Eliminar</a></li> <!-- Opción para eliminar materia -->
                                        <li><a class="dropdown-item" href="#" onclick="openDisableModal(${materiaSinGrupo.materiaId})">Desactivar</a></li> <!-- Opción para desactivar materia -->
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
            position: "center",
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
                inicializar(); // Llama a la función inicializar para reintentar la carga
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
function irAMateria(materiaId) {
    window.location.href = `/Docente/MateriasDetalles?materiaId=${materiaId}`; // Redirige a la página de detalles de la materia
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
            position: "center",
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
                inicializar(); // Reintenta la carga de grupos automáticamente
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
//Funcionalidades de los iconos de las cards de grupos
// Función para cargar materias de un grupo cuando se hace clic en la card del grupo
async function handleCardClick(grupoId) {
    console.log(grupoId);
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
                materiasContainer.innerHTML = "<p>Aun no hay materias registradas para este grupo.</p>";
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
                                                <li><a class="dropdown-item" href="#" onclick="openEditModal(${materia.materiaId})">Editar</a></li>
                                                <li><a class="dropdown-item" href="#" onclick="openDeleteModal(${materia.materiaId})">Eliminar</a></li>
                                                <li><a class="dropdown-item" href="#" onclick="openDisableModal(${materia.materiaId})">Desactivar</a></li>
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
                position: "center",
                timer: 2000
            }); // Mostramos una alerta si hubo un error al mostrar las materias del grupo
        }
    }
}

function editarGrupo(id) {
    alert("Editar grupo " + id); // Muestra una alerta indicando que el grupo será editado
}

function eliminarGrupo(id) {
    alert("Eliminar grupo " + id); // Muestra una alerta indicando que el grupo será eliminado
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
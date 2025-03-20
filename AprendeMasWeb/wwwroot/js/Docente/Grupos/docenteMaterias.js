
//Funcion para guarda las materias registradas sin un grupo
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


//Funcion para cargar materias sin grupo existentes al modal de crear grupo
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


// Cargar materias que fueron creadas sin un grupo a la vista principal.
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
                                    </ul>
                                </div>
                            </div>
                        </div>

                        <div class="card-body card-body-custom" style="background-color: #ffffff">
    <p class="card-text">
    ${materiaSinGrupo.actividadesRecientes.length > 0
        ? materiaSinGrupo.actividadesRecientes.map(actividad => {
            const fechaFormateada = new Date(actividad.fechaCreacion).toLocaleDateString('es-ES', {
                day: '2-digit',
                month: '2-digit',
                year: 'numeric'
            });
            return `
                <a href="#" onclick="verActividad(${actividad.actividadId})">
                    ${actividad.nombreActividad} (${fechaFormateada})
                </a>
            `;
        }).join('<br>')
        : "Sin actividades recientes"}
</p>

</div>


                        <div class="card-footer card-footer-custom">
                            <button class="btn btn-sm btn-primary" onclick="irAMateria(${materiaSinGrupo.materiaId})">Ver Materia</button> <!-- Botón para ver los detalles de la materia -->
                            <div class="icon-container">
                                <img class="icon-action" src="https://cdn-icons-png.flaticon.com/512/1828/1828817.png" alt="Ver Actividades" title="Ver Actividades" onclick="irAMateria(${materiaSinGrupo.materiaId}, 'actividades')"> <!-- Icono para ver actividades -->
                                <img class="icon-action" src="https://cdn-icons-png.flaticon.com/512/847/847969.png" alt="Ver Integrantes" title="Ver Integrantes" onclick="irAMateria(${materiaSinGrupo.materiaId}, 'alumnos')"> <!-- Icono para ver los integrantes -->
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




//Funcionalidades de los iconos de las Card de la materia
function verActividades(MateriaId) {
    alert(`Ver actividades del grupo ID: ${MateriaId}`); // Muestra una alerta con el ID de la materia
    // Aquí puedes redirigir o cargar las actividades relacionadas con el grupo
}

function verIntegrantes(MateriaId) {
    alert(`Ver integrantes del grupo ID: ${MateriaId}`); // Muestra una alerta con el ID de la materia
    // Aquí puedes abrir un modal o redirigir para mostrar los integrantes
}


//Funcion para editar nombre y descripcion de una materia. Sin funcionar aun.
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
                NombreMateria: document.getElementById("swal-nombre").value, // Nombre correcto
                Descripcion: document.getElementById("swal-descripcion").value // Nombre correcto
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


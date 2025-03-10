// Obtener el ID del docente almacenado en localStorage
docenteIdGlobal = localStorage.getItem("docenteId");
materiaIdGlobal = localStorage.getItem("materiaId");

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

    // Cargar alumnos asignados a la materia
    cargarAlumnosAsignados(materiaIdGlobal);
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

   
    // 🔹 Funcionalidad de búsqueda de alumnos en tiempo real (sugerencias de correo)
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


// Escuchar el evento de clic en #contenedor-dinamico
document.getElementById("contenedor-dinamico").addEventListener("click", async function (event) {
    // Verifica si el clic fue en #seccion-alumnos
    if (event.target.id === "seccion-alumnos") {
        // Obtener el materiaId, por ejemplo, desde un atributo data-* en #seccion-alumnos
        cargarAlumnosAsignados(materiaIdGlobal);
    }
});

// ✅ Función para cambiar la sección mostrada en la interfaz
function cambiarSeccion(seccion) {
    document.querySelectorAll('.seccion').forEach(div => div.style.display = 'none');
    const seccionMostrar = document.getElementById(`seccion-${seccion}`);
    if (seccionMostrar) {
        seccionMostrar.style.display = 'block';
    }
    document.querySelectorAll('.tab-button').forEach(btn => btn.classList.remove('active'));
    document.querySelector(`button[onclick="cambiarSeccion('${seccion}')"]`).classList.add('active');
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

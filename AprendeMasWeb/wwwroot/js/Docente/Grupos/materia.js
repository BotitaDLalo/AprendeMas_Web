/*
// Obtener el ID del docente almacenado en localStorage
docenteIdGlobal = localStorage.getItem("docenteId");
materiaIdGlobal = localStorage.getItem("materiaId");
// Esperar a que el DOM esté completamente cargado antes de ejecutar el código
document.addEventListener("DOMContentLoaded", function () {
    // Obtener los parámetros de la URL
    const urlParams = new URLSearchParams(window.location.search);
    // Extraer el ID de la materia desde la URL
    const materiaId = urlParams.get("materiaId");
    // Usar el ID del docente almacenado previamente
    const docenteId = docenteIdGlobal;

    // Verificar si se tienen ambos IDs antes de hacer la petición
    if (materiaId && docenteId) {
        // Realizar petición a la API para obtener los detalles de la materia
        fetch(`/api/DetallesMateriaApi/ObtenerDetallesMateria/${materiaId}/${docenteId}`)
            .then(response => {
                // Verificar si la respuesta es correcta
                if (!response.ok) {
                    throw new Error("Error en la respuesta de la API");
                }
                // Convertir la respuesta a JSON
                return response.json();
            })
            .then(data => {
                // Mostrar los datos recibidos en la consola para depuración
                console.log("Datos recibidos:", data);

                // Verificar que los datos contengan la información esperada
                if (data.nombreMateria && data.codigoAcceso && data.codigoColor) {
                    // Asignar el nombre de la materia al elemento correspondiente
                    document.getElementById("materiaNombre").innerText = data.nombreMateria;
                    // Asignar el código de acceso al elemento correspondiente
                    document.getElementById("codigoAcceso").innerText = data.codigoAcceso;

                    // Cambiar el color de fondo del encabezado de la materia
                    document.querySelector(".materia-header").style.backgroundColor = data.codigoColor;
                } else {
                    // Mostrar un error en consola si los datos no son válidos
                    console.error("No se encontraron datos válidos para esta materia.");
                }
            })
            .catch(error =>
                // Capturar y mostrar errores en la consola
                console.error("Error al obtener los datos de la materia:", error)
            );
    }
    
    // 🔹 Funcionalidad de búsqueda de alumnos en tiempo real
    const inputBuscar = document.getElementById("buscarAlumno");
    const listaSugerencias = document.getElementById("sugerenciasAlumnos");
    let indexSugerenciaSeleccionada = -1; // Para llevar el índice de la sugerencia seleccionada

    if (inputBuscar) { // Verificar si el input existe en la página antes de ejecutar el código
        inputBuscar.addEventListener("input", async function () {
            const query = inputBuscar.value.trim();
            if (query.length < 3) {
                listaSugerencias.innerHTML = ""; // Limpiar la lista si el texto es muy corto
                return;
            }

            try {
                const response = await fetch(`/api/DetallesMateriaApi/BuscarAlumnosPorCorreo?query=${query}`);
                if (!response.ok) throw new Error("Error al buscar alumnos");

                const alumnos = await response.json();
                listaSugerencias.innerHTML = ""; // Limpiar sugerencias anteriores

                if (alumnos.length === 0) {
                    listaSugerencias.innerHTML = `<li class="list-group-item text-muted">No se encontraron resultados</li>`;
                    return;
                }

                // Agregar las sugerencias a la lista
                alumnos.forEach((alumno, index) => {
                    const li = document.createElement("li");
                    li.classList.add("list-group-item", "list-group-item-action");

                    // Mostrar el nombre completo y el correo
                    li.textContent = `${alumno.nombre} ${alumno.apellidoPaterno} ${alumno.apellidoMaterno} - ${alumno.email}`;

                    // Añadir evento para seleccionar la sugerencia
                    li.addEventListener("click", function () {
                        inputBuscar.value = alumno.email; // Rellenar input con el correo seleccionado
                        listaSugerencias.innerHTML = ""; // Limpiar sugerencias
                    });

                    // Añadir clase activa si es la sugerencia seleccionada
                    if (index === indexSugerenciaSeleccionada) {
                        li.classList.add("active");
                    }

                    listaSugerencias.appendChild(li);
                });

            } catch (error) {
                console.error("Error al buscar alumnos:", error);
            }
        });

        // Navegación con las teclas de flecha
        inputBuscar.addEventListener("keydown", function (e) {
            const sugerencias = listaSugerencias.getElementsByTagName("li");

            // Si se presiona la flecha hacia abajo
            if (e.key === "ArrowDown") {
                if (indexSugerenciaSeleccionada < sugerencias.length - 1) {
                    indexSugerenciaSeleccionada++;
                    actualizarSugerencias();
                }
            }
            // Si se presiona la flecha hacia arriba
            else if (e.key === "ArrowUp") {
                if (indexSugerenciaSeleccionada > 0) {
                    indexSugerenciaSeleccionada--;
                    actualizarSugerencias();
                }
            }
            // Si se presiona "Enter"
            else if (e.key === "Enter" && indexSugerenciaSeleccionada >= 0) {
                const selectedSugerencia = sugerencias[indexSugerenciaSeleccionada];
                if (selectedSugerencia) {
                    const correo = selectedSugerencia.textContent.split(" - ")[1]; // Poner el correo seleccionado
                    if (correo) {
                        inputBuscar.value = correo; // Rellenar el input con el correo seleccionado
                        listaSugerencias.innerHTML = ""; // Limpiar sugerencias
                    }
                }
            }
        });

        // Función para actualizar las clases de las sugerencias
        function actualizarSugerencias() {
            const sugerencias = listaSugerencias.getElementsByTagName("li");
            // Eliminar la clase "active" de todas las sugerencias
            for (let i = 0; i < sugerencias.length; i++) {
                sugerencias[i].classList.remove("active");
            }

            // Añadir la clase "active" a la sugerencia seleccionada
            if (indexSugerenciaSeleccionada >= 0 && indexSugerenciaSeleccionada < sugerencias.length) {
                sugerencias[indexSugerenciaSeleccionada].classList.add("active");
            }
        }

        // Ocultar la lista de sugerencias si el usuario hace clic fuera
        document.addEventListener("click", function (event) {
            if (!inputBuscar.contains(event.target) && !listaSugerencias.contains(event.target)) {
                listaSugerencias.innerHTML = "";
            }
        });
    }
});

// Función para cambiar la sección mostrada en la interfaz
function cambiarSeccion(seccion) {
    // Oculta todas las secciones
    document.querySelectorAll('.seccion').forEach(div => div.style.display = 'none');

    // Muestra solo la sección seleccionada
    const seccionMostrar = document.getElementById(`seccion-${seccion}`);
    if (seccionMostrar) {
        seccionMostrar.style.display = 'block';
    }

    // Actualizar los botones activos
    document.querySelectorAll('.tab-button').forEach(btn => btn.classList.remove('active'));
    document.querySelector(`button[onclick="cambiarSeccion('${seccion}')"]`).classList.add('active');
}
*/






/*
// Obtener el ID del docente almacenado en localStorage
docenteIdGlobal = localStorage.getItem("docenteId");
materiaIdGlobal = localStorage.getItem("materiaId");

// Esperar a que el DOM esté completamente cargado antes de ejecutar el código
document.addEventListener("DOMContentLoaded", function () {
    // Obtener los parámetros de la URL
    const urlParams = new URLSearchParams(window.location.search);
    const materiaId = urlParams.get("materiaId");
    const docenteId = docenteIdGlobal;

    // Verificar si se tienen ambos IDs antes de hacer la petición
    if (materiaId && docenteId) {
        fetch(`/api/DetallesMateriaApi/ObtenerDetallesMateria/${materiaId}/${docenteId}`)
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

    cargarAlumnosAsignados(materiaIdGlobal);

    // Evento para asignar alumnos
    document.getElementById("btnAsignarAlumno").addEventListener("click", async function () {
        const correo = document.getElementById("buscarAlumno").value.trim();
        if (!correo) {
            alert("Por favor, ingresa un correo válido.");
            return;
        }

        try {
            const response = await fetch(`/api/DetallesMateriaApi/AsignarAlumnoMateria?correo=${correo}&materiaId=${materiaIdGlobal}`, {
                method: "POST"
            });

            const data = await response.json();

            if (!response.ok) {
                alert(data.mensaje || "Error al asignar alumno.");
                return;
            }

            alert("Alumno asignado correctamente.");
            cargarAlumnosAsignados(materiaIdGlobal);
        } catch (error) {
            console.error("Error en la asignación:", error);
            alert("Hubo un error, inténtalo de nuevo.");
        }
    });
});

// ✅ Función para cargar alumnos asignados a la materia
async function cargarAlumnosAsignados(materiaId) {
    try {
        const response = await fetch(`/api/DetallesMateriaApi/ObtenerAlumnosPorMateria/${materiaId}`);
        if (!response.ok) {
            throw new Error("No se pudieron cargar los alumnos.");
        }
        const alumnos = await response.json();
        const contenedor = document.getElementById("listaAlumnosAsignados");
        contenedor.innerHTML = "";

        if (alumnos.length === 0) {
            contenedor.innerHTML = `<p class="text-muted">No hay alumnos asignados a esta materia.</p>`;
            return;
        }

        const ul = document.createElement("ul");
        ul.classList.add("list-group");

        alumnos.forEach(alumno => {
            const li = document.createElement("li");
            li.classList.add("list-group-item", "d-flex", "justify-content-between", "align-items-center");
            li.textContent = `${alumno.nombre} ${alumno.apellidoPaterno} ${alumno.apellidoMaterno} `;

            const btnEliminar = document.createElement("button");
            btnEliminar.classList.add("btn", "btn-danger", "btn-sm");
            btnEliminar.textContent = "Eliminar";
            btnEliminar.addEventListener("click", async function () {
                if (confirm(`¿Seguro que deseas eliminar a ${alumno.nombre} de esta materia?`)) {
                    await eliminarAlumnoDeMateria(alumno.email, materiaId);
                    cargarAlumnosAsignados(materiaId);
                }
            });

            li.appendChild(btnEliminar);
            ul.appendChild(li);
        });

        contenedor.appendChild(ul);
    } catch (error) {
        console.error("Error al cargar alumnos:", error);
    }
}

// ✅ Función para eliminar alumno de la materia
async function eliminarAlumnoDeMateria(correo, materiaId) {
    try {
        const response = await fetch(`/api/AsignacionAlumnoMateria/EliminarAlumnoMateria?correo=${correo}&materiaId=${materiaId}`, {
            method: "DELETE"
        });
        const data = await response.json();
        if (!response.ok) {
            alert(data.mensaje || "Error al eliminar alumno.");
            return;
        }
        alert("Alumno eliminado correctamente.");
    } catch (error) {
        console.error("Error al eliminar alumno:", error);
        alert("Hubo un error, inténtalo de nuevo.");
    }
}

// Función para cambiar la sección mostrada en la interfaz
function cambiarSeccion(seccion) {
    document.querySelectorAll('.seccion').forEach(div => div.style.display = 'none');
    const seccionMostrar = document.getElementById(`seccion-${seccion}`);
    if (seccionMostrar) {
        seccionMostrar.style.display = 'block';
    }
    document.querySelectorAll('.tab-button').forEach(btn => btn.classList.remove('active'));
    document.querySelector(`button[onclick="cambiarSeccion('${seccion}')"]`).classList.add('active');
}
*/

// Obtener el ID del docente almacenado en localStorage
docenteIdGlobal = localStorage.getItem("docenteId");
materiaIdGlobal = localStorage.getItem("materiaId");

// Esperar a que el DOM esté completamente cargado antes de ejecutar el código
document.addEventListener("DOMContentLoaded", function () {
    const urlParams = new URLSearchParams(window.location.search);
    const materiaId = urlParams.get("materiaId");
    const docenteId = docenteIdGlobal;

    // Verificar si se tienen ambos IDs antes de hacer la petición
    if (materiaId && docenteId) {
        fetch(`/api/DetallesMateriaApi/ObtenerDetallesMateria/${materiaId}/${docenteId}`)
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

    // Evento para asignar alumnos
    document.getElementById("btnAsignarAlumno").addEventListener("click", async function () {
        const correo = document.getElementById("buscarAlumno").value.trim();
        if (!correo) {
            alert("Por favor, ingresa un correo válido.");
            return;
        }

        try {
            const response = await fetch(`/api/DetallesMateriaApi/AsignarAlumnoMateria?correo=${correo}&materiaId=${materiaIdGlobal}`, {
                method: "POST"
            });

            const data = await response.json();

            if (!response.ok) {
                alert(data.mensaje || "Error al asignar alumno.");
                return;
            }

            alert("Alumno asignado correctamente.");
            cargarAlumnosAsignados(materiaIdGlobal);
        } catch (error) {
            console.error("Error en la asignación:", error);
            alert("Hubo un error, inténtalo de nuevo.");
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

// ✅ Función para cargar alumnos asignados a la materia
async function cargarAlumnosAsignados(materiaId) {
    try {
        const response = await fetch(`/api/DetallesMateriaApi/ObtenerAlumnosPorMateria/${materiaId}`);
        if (!response.ok) {
            throw new Error("No se pudieron cargar los alumnos.");
        }
        const alumnos = await response.json();
        const contenedor = document.getElementById("listaAlumnosAsignados");
        contenedor.innerHTML = "";

        if (alumnos.length === 0) {
            contenedor.innerHTML = `<p class="text-muted">No hay alumnos asignados a esta materia.</p>`;
            return;
        }

        const ul = document.createElement("ul");
        ul.classList.add("list-group");

        alumnos.forEach(alumno => {
            const li = document.createElement("li");
            li.classList.add("list-group-item", "d-flex", "justify-content-between", "align-items-center");
            li.textContent = `${alumno.nombre} ${alumno.apellidoPaterno} ${alumno.apellidoMaterno} `;

            const btnEliminar = document.createElement("button");
            btnEliminar.classList.add("btn", "btn-danger", "btn-sm");
            btnEliminar.textContent = "Eliminar";
            btnEliminar.addEventListener("click", async function () {
                if (confirm(`¿Seguro que deseas eliminar a ${alumno.nombre} de esta materia?`)) {
                    await eliminarAlumnoDeMateria(alumno.email, materiaId);
                    cargarAlumnosAsignados(materiaId);
                }
            });

            li.appendChild(btnEliminar);
            ul.appendChild(li);
        });

        contenedor.appendChild(ul);
    } catch (error) {
        console.error("Error al cargar alumnos:", error);
    }
}

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

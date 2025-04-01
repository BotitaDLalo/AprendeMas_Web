document.addEventListener("DOMContentLoaded", async function () {
    console.log("Cargando clases...");
    await cargarClases();
});

// Obtener el ID del alumno
async function obtenerAlumnoId() {
    console.log("Obteniendo alumno ID...");
    return alumnoIdGlobal; // Asegúrate de que este valor está definido correctamente
}

// Función para unirse a una clase con código
async function unirseAClase() {
    const codigoAcceso = document.getElementById("codigoAccesoInput").value.trim();
    const alumnoId = alumnoIdGlobal; // Asegúrate de que este valor esté definido correctamente

    if (!codigoAcceso) {
        Swal.fire({
            icon: "warning",
            title: "Código requerido",
            text: "Por favor, ingresa un código de acceso.",
            position: "center"
        });
        return;
    }

    try {
        const response = await fetch('/api/Alumnos/UnirseAClase', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                AlumnoId: alumnoId,
                CodigoAcceso: codigoAcceso
            })
        });

        const data = await response.json();

        if (response.ok) {
            Swal.fire({
                icon: "success",
                title: "Unido con éxito",
                text: data.mensaje,
                position: "center"
            }).then(() => {
                document.getElementById("codigoAccesoInput").value = ""; // ✅ Limpia el campo de entrada
                closeModal(); // Cierra el modal (si aplica)
                cargarClases(); // Recargar las clases después de unirse
            });

        } else {
            Swal.fire({
                icon: "error",
                title: "Error",
                text: data.mensaje,
                position: "center"
            }).then(() => {
                document.getElementById("codigoAccesoInput").value = ""; // ✅ Limpia el campo si el código es incorrecto
            });
        }

    } catch (error) {
        console.error("Error al intentar unirse a la clase:", error);
        Swal.fire({
            icon: "error",
            title: "Error",
            text: "Ocurrió un error. Intenta nuevamente.",
            position: "center"
        });
    }
}


document.addEventListener("DOMContentLoaded", async function () {
    console.log("Cargando clases...");
    await cargarClases();
});

// Variables globales para el orden actual
let ordenActual = {
    grupos: "nombre", // Puede ser "nombre" o "fecha"
    materias: "nombre"
};

// Función para ordenar grupos o materias
function ordenarClases(lista, tipo) {
    if (ordenActual[tipo] === "nombre") {
        return lista.sort((a, b) => a.nombre.localeCompare(b.nombre));
    } else {
        /*return lista.sort((a, b) => new Date(a.fechaCreacion) - new Date(b.fechaCreacion));*/
        // Ordenar por fecha de la más reciente a la más antigua
        return lista.sort((a, b) => new Date(b.fechaCreacion) - new Date(a.fechaCreacion));
    }
}

// Función para cambiar el orden y recargar la vista
function cambiarOrden(tipo) {
    ordenActual[tipo] = ordenActual[tipo] === "nombre" ? "fecha" : "nombre";
    cargarClases(); // Recargar con el nuevo orden
}

// Cargar clases del alumno
async function cargarClases() {
    const alumnoId = await obtenerAlumnoId();
    console.log("Cargando clases del alumno con ID:", alumnoId);

    try {
        const response = await fetch(`/api/Alumno/Clases/${alumnoId}`);
        let clases = await response.json();
        console.log("Clases recibidas:", clases);

        const contenedor = document.getElementById("contenedorClases");
        if (!contenedor) {
            console.error("No se encontró el elemento contenedorClases en el HTML.");
            return;
        }

        contenedor.innerHTML = ""; // Limpiar contenido previo

        if (!clases.length) {
            contenedor.innerHTML = "<p>No tienes clases registradas.</p>";
            return;
        }

        // Separar en grupos y materias
        let grupos = clases.filter(clase => clase.esGrupo);
        let materias = clases.filter(clase => !clase.esGrupo);

        // Ordenar antes de mostrarlas
        grupos = ordenarClases(grupos, "grupos");
        materias = ordenarClases(materias, "materias");

        // Agregar encabezado de Grupos con icono de filtro
        if (grupos.length > 0) {
            let etiquetaGrupo = document.createElement("h3");
            etiquetaGrupo.innerHTML = `
            <img src="/Iconos/GRUPO-26.svg" class="icono-materia" alt="Icono de Materias">
            Grupos
                <img src="/Iconos/FILTRO-26.svg" class="icono-filtro" onclick="cambiarOrden('grupos')" title="Ordenar por ${ordenActual.grupos === 'nombre' ? 'fecha' : 'nombre'}">`;
            etiquetaGrupo.classList.add("separador");
            contenedor.appendChild(etiquetaGrupo);

            grupos.forEach(clase => agregarCardClase(clase, contenedor));
        }

        // Agregar encabezado de Materias con icono de filtro
        if (materias.length > 0) {
            let etiquetaMateria = document.createElement("h3");
            etiquetaMateria.innerHTML = `
            <img src="/Iconos/GRUPO-26.svg" class="icono-materia" alt="Icono de Materias">
            Materias
                <img src="/Iconos/FILTRO-26.svg" class="icono-filtro" onclick="cambiarOrden('materias')" title="Ordenar por ${ordenActual.materias === 'nombre' ? 'fecha' : 'nombre'}">`;
            etiquetaMateria.classList.add("separador");
            contenedor.appendChild(etiquetaMateria);

            materias.forEach(clase => agregarCardClase(clase, contenedor));
        }

    } catch (error) {
        console.error("Error al cargar las clases:", error);
        alert("Ocurrió un error al cargar las clases.");
    }
}


// Agregar clase a la vista
function agregarCardClase(clase) {
    if (!clase.nombre) {
        console.warn("Intento de agregar clase sin nombre.");
        return;
    }

    console.log(`Agregando clase: ${clase.nombre} (Grupo: ${clase.esGrupo})`);

    const contenedor = document.getElementById("contenedorClases");
    if (!contenedor) {
        console.error("No se encontró el elemento contenedorClases en el HTML.");
        return;
    }

    const card = document.createElement("div");
    card.classList.add("class-card");

    const etiqueta = clase.esGrupo ? "Grupo" : "Materia";

   



    card.innerHTML = `
    
        <p class="card-etiqueta">
            <img class="iconos-nav2" src="/Iconos/TABLAB.svg" alt="Icono de Grupo" />
            ${etiqueta} - ${clase.nombre}
        </p>
        <div class="menu-container">
            <button class="menu-button">⋮</button>
            <div class="menu-options2">
                <button onclick="eliminarClase('${clase.id}', ${clase.esGrupo})">🗑 Eliminar</button>
            </div>
        </div>
    
`;

    // Agregar evento para mostrar/ocultar el menú
    const menuButton = card.querySelector(".menu-button");
    const menuOptions = card.querySelector(".menu-options2");

    menuButton.addEventListener("click", (event) => {
        event.stopPropagation();
        menuOptions.classList.toggle("show-menu");
    });

    // Ocultar el menú si se hace clic fuera
    document.addEventListener("click", (event) => {
        if (!menuButton.contains(event.target)) {
            menuOptions.classList.remove("show-menu");
        }
    });

    // Si la clase es una materia, hacer clic para ir a su página
    if (!clase.esGrupo) {
        card.addEventListener("click", function () {
            window.location.href = `/Alumno/Clase?tipo=materia&nombre=${encodeURIComponent(clase.nombre)}`;
        });
    }

    // Si es un grupo con materias, agregar la vista de materias en formato de cards cuadradas
    if (clase.esGrupo && clase.materias && clase.materias.length > 0) {
        let contenedorMaterias = document.createElement("div");
        contenedorMaterias.classList.add("materias-grid"); // Nueva clase CSS para el grid
        contenedorMaterias.style.display = "none"; // Inicialmente oculto

        clase.materias.forEach(materia => {
            let materiaCard = document.createElement("div");
            materiaCard.classList.add("materia-card");


            materiaCard.innerHTML = `
                <div class="card-container1">
                    <p class="card-etiqueta">
                    <img class="iconos-nav2" src="/Iconos/TABLAB.svg" alt="Icono de Grupo" />
                    ${materia.nombre}</p>
                     <hr class="card-separator">
            <img class="iconos-nav2" src="/Iconos/TABLA-26.svg" alt="Icono de Horario" />
            <img class="iconos-nav2" src="/Iconos/PAR-26.svg" alt="Icono de Participación" />
            <img class="iconos-nav2" src="/Iconos/ESTRELLA-26.svg" alt="Icono de Favorito" />

                </div>
            `;

            // ✅ Agregar evento para ir a la página de la materia
            materiaCard.addEventListener("click", function (event) {
                event.stopPropagation(); // Evita que el grupo también se active
                window.location.href = `/Alumno/Clase?tipo=materia&nombre=${encodeURIComponent(materia.nombre)}`;
            });

            contenedorMaterias.appendChild(materiaCard);
        });

        card.appendChild(contenedorMaterias);

        // Hacer que al hacer clic sobre el grupo se desplieguen/oculten las materias
        card.addEventListener("click", function () {
            contenedorMaterias.style.display = contenedorMaterias.style.display === "none" ? "grid" : "none";
        });
    }

    contenedor.appendChild(card);
}

// Ver clase al hacer clic
function verClase(nombre, esGrupo) {
    if (!nombre || nombre === "undefined") {
        alert("Error: La clase no tiene un nombre válido.");
        return;
    }

    console.log(`Redirigiendo a la clase: ${nombre} (Grupo: ${esGrupo})`);
    const tipo = esGrupo ? 'grupo' : 'materia';
    window.location.href = `/Alumno/Clase?tipo=${tipo}&nombre=${encodeURIComponent(nombre)}`;
}

async function eliminarClase(id, esGrupo) {
    const tipo = esGrupo ? "grupo" : "materia";

    const confirmacion = await Swal.fire({
        title: `¿Eliminar ${tipo}?`,
        text: `¿Estás seguro de que quieres eliminar este ${tipo}? Esta acción no se puede deshacer.`,
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Sí, eliminar",
        cancelButtonText: "Cancelar"
    });
 
    if (confirmacion.isConfirmed) {
        try {
            const response = await fetch(`/api/Alumno/EliminarClase/${id}`, {
                method: "DELETE",
                headers: { "Content-Type": "application/json" }
            });

            const data = await response.json();

            if (response.ok) {
                Swal.fire("Eliminado", `${tipo} eliminado con éxito.`, "success");
                cargarClases(); // Recargar la lista de clases
            } else {
                Swal.fire("Error", data.mensaje || "No se pudo eliminar.", "error");
            }
        } catch (error) {
            console.error("Error al eliminar:", error);
            Swal.fire("Error", "Ocurrió un problema al eliminar.", "error");
        }
    }
}


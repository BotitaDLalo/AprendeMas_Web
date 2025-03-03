document.addEventListener("DOMContentLoaded", async function () {
    console.log("Cargando clases...");
    await cargarClases();
});

async function obtenerAlumnoId() {
    console.log("Obteniendo alumno ID...");
    return 1; // Debes cambiar esto para obtener el ID real
}

async function unirseAClase() {
    const codigoAcceso = document.getElementById("codigoAccesoInput").value;
    const alumnoId = 1; // Aquí debes obtener el ID del alumno autenticado

    if (!codigoAcceso) {
        Swal.fire({
            icon: "warning",
            title: "Código requerido",
            text: "Por favor, ingresa un código de acceso."
        });
        return;
    }

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
        agregarCardClase(data.nombre, data.esGrupo);

        Swal.fire({
            icon: "success",
            title: "Unido con éxito",
            text: data.mensaje
        }).then(() => {
            closeModal(); // Cierra el modal (si aplica)
            cargarClases(); // Recarga las clases en la vista
        });

    } else {
        Swal.fire({
            icon: "error",
            title: "Error",
            text: data.mensaje
        });
    }
}

async function cargarClases() {
    const alumnoId = await obtenerAlumnoId();
    console.log("Cargando clases del alumno con ID:", alumnoId);

    try {
        const response = await fetch(`/api/Alumno/Clases/${alumnoId}`);
        const clases = await response.json();
        console.log("Clases recibidas:", clases);

        const contenedor = document.getElementById("contenedorClases");
        if (!contenedor) {
            console.error("No se encontró el elemento contenedorClases en el HTML.");
            return;
        }

        contenedor.innerHTML = ""; // Limpiar contenido previo

        if (!clases.length) {
            console.warn("No hay clases para mostrar.");
            contenedor.innerHTML = "<p>No tienes clases registradas.</p>";
            return;
        }

        clases.forEach(clase => {
            // Usamos 'clase.nombre' en vez de 'clase.Nombre'
            agregarCardClase(clase.nombre, clase.esGrupo);
        });

    } catch (error) {
        console.error("Error al cargar las clases:", error);
        alert("Ocurrió un error al cargar las clases.");
    }
}

function agregarCardClase(nombre, esGrupo) {
    if (!nombre) {
        console.warn("Intento de agregar clase sin nombre.");
        return;
    }

    console.log(`Agregando clase: ${nombre} (Grupo: ${esGrupo})`);

    const contenedor = document.getElementById("contenedorClases");
    if (!contenedor) {
        console.error("No se encontró el elemento contenedorClases en el HTML.");
        return;
    }

    const card = document.createElement("div");
    card.classList.add("class-card");

    const nombreEscapado = encodeURIComponent(nombre);

    // Agregar etiquetas para Materia o Grupo
    const etiqueta = esGrupo ? "Grupo" : "Materia";
    card.innerHTML = `
  <br><br><br><br>
  <div class="card-container1">
  
    <p class="card-etiqueta">
    <img class="iconos-nav2" src="/Iconos/TABLAB.svg" alt="Icono de Calendario" />
    ${etiqueta} - ${nombre}
</p> <!-- Etiqueta que indica si es Grupo o Materia -->
    <hr class="card-separator">
    <img class="iconos-nav" src="/Iconos/TABLA-26.svg" alt="Icono de Calendario" /> 
    <img class="iconos-nav" src="/Iconos/PAR-26.svg" alt="Icono de Calendario" /> 
    <img class="iconos-nav" src="/Iconos/ESTRELLA-26.svg" alt="Icono de Calendario" /> 

    <button class="card-button" onclick="verClase('${nombreEscapado}', ${esGrupo})">Entrar a la clase</button>
  </div>
`;



    contenedor.appendChild(card);
}

function verClase(nombre, esGrupo) {
    if (!nombre || nombre === "undefined") {
        alert("Error: La clase no tiene un nombre válido.");
        return;
    }

    console.log(`Redirigiendo a la clase: ${nombre} (Grupo: ${esGrupo})`);
    const tipo = esGrupo ? 'grupo' : 'materia';
    window.location.href = `/Alumno/Clase?tipo=${tipo}&nombre=${encodeURIComponent(nombre)}`;
}

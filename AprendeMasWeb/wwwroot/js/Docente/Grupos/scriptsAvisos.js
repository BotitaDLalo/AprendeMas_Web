// Obtener el ID del docente almacenado en localStorage
docenteIdGlobal = localStorage.getItem("docenteId");
materiaIdGlobal = localStorage.getItem("materiaIdSeleccionada");
grupoIdGlobal = localStorage.getItem("grupoIdSeleccionado");

document.addEventListener("DOMContentLoaded", function () {
    //Cargar los avisos asinados a la materia
    cargarAvisosDeMateria();
});

//Funcion para publicar un aviso
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




//Funcion que carga los avisos a la vista.
async function cargarAvisosDeMateria() {
    const listaAvisos = document.getElementById("listaDeAvisosDeMateria");
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
    const listaAvisos = document.getElementById("listaDeAvisosDeMateria");
    listaAvisos.innerHTML = "";

    if (avisos.length === 0) {
        listaAvisos.innerHTML = "<p>No hay avisos registrados para esta materia.</p>";
        return;
    }
    avisos.forEach(aviso => {
        const avisoItem = document.createElement("div");
        avisoItem.classList.add("aviso-item");

        const descripcionAvisoConEnlace = convertirUrlsEnEnlaces(aviso.descripcion);

        avisoItem.innerHTML = `
            <div class="aviso-header">
                <div class="aviso-icono">📢</div>
                <div class="aviso-info">
                    <strong>${aviso.titulo}</strong>
                    <p class="aviso-fecha">Publicado: ${new Date(aviso.fechaCreacion).toLocaleString()}</p>
                    <p class="aviso-descripcion oculto">${descripcionAvisoConEnlace}</p>
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

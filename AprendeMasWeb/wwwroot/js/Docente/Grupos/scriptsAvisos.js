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
                document.getElementById("avisosForm").reset();//Resetear  formulario
                cargarAvisosDeMateria();
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



// Funcion que carga los avisos a la vista.
async function cargarAvisosDeMateria() {
    const listaAvisos = document.getElementById("listaDeAvisosDeMateria");
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
    listaAvisos.innerHTML = ""; // Limpiar el contenedor

    if (avisos.length === 0) {
        listaAvisos.innerHTML = "<p>No hay actividades registradas para esta materia.</p>";
        return;
    }
    avisos.reverse();

    avisos.forEach(aviso => {
        const avisoItem = document.createElement("div");
        avisoItem.classList.add("aviso-item");
        const descripcionAvisoConEnlace = convertirUrlsEnEnlaces(aviso.descripcion);

        avisoItem.innerHTML = `
            <div class="aviso-header">
                <div class="aviso-icono">📢</div>
                <div class="aviso-info">
                    <strong>${aviso.titulo}</strong>
                    <p class="aviso-fecha-publicado">Publicado: ${formatearFecha(aviso.fechaCreacion)}</p>
                    <p class="actividad-descripcion oculto">${descripcionAvisoConEnlace}</p>
                    <p class="ver-completo">Ver completo</p>
                </div>
                <div class="aviso-botones-container">
                    <button class="aviso-editar-btn" data-id="${aviso.avisoId}">Editar</button>
                    <button class="aviso-eliminar-btn" data-id="${aviso.avisoId}">Eliminar</button>
                </div>
            </div>
        `;

        // Mostrar/ocultar descripción al hacer clic en "Ver completo"
        const verCompleto = avisoItem.querySelector(".ver-completo");
        const descripcion = avisoItem.querySelector(".actividad-descripcion");

        verCompleto.addEventListener("click", () => {
            // Alternar entre mostrar y ocultar la descripción
            if (descripcion.classList.contains("oculto")) {
                descripcion.classList.remove("oculto");
                descripcion.classList.add("visible");
            } else {
                descripcion.classList.remove("visible");
                descripcion.classList.add("oculto");
            }
        });

        // Agregar eventos a los botones
        const btnEliminar = avisoItem.querySelector(".aviso-eliminar-btn");
        const btnEditar = avisoItem.querySelector(".aviso-editar-btn");

        btnEliminar.addEventListener("click", () => eliminarAviso(aviso.avisoId));
        btnEditar.addEventListener("click", () => editarAviso(aviso.avisoId));

        listaAvisos.appendChild(avisoItem);
    });
}


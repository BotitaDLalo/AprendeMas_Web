
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
        await Swal.fire({
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
        await Swal.fire({
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
    mostrarCargando("Cargando materias..."); // Mostrar indicador de carga

    try {
        const response = await fetch(`/api/MateriasApi/ObtenerMateriasSinGrupo/${docenteIdGlobal}`);
        
        if (response.ok) {
            const materiasSinGrupo = await response.json();
            const listaMateriasSinGrupo = document.getElementById("listaMateriasSinGrupo");

            // Limpiar contenido anterior y crear el contenedor con Bootstrap Grid
            listaMateriasSinGrupo.innerHTML = "";
            const rowContainer = document.createElement("div");
            rowContainer.classList.add("row", "g-3");

            if (materiasSinGrupo.length === 0) {
                const mensaje = document.createElement("p");
                mensaje.classList.add("text-center", "text-muted");
                mensaje.textContent = "No hay materias registradas.";
                listaMateriasSinGrupo.appendChild(mensaje);
                cerrarCargando(); // Cerrar indicador de carga
                return;
            }

            materiasSinGrupo.forEach(materia => {
                const col = document.createElement("div");
                col.classList.add("col-md-3");

                const card = document.createElement("div");
                card.classList.add("materia-card", "bg-light", "mb-3", "shadow-sm");

            card.style.maxWidth = "100%";

            // Header
            const header = document.createElement("div");
            header.classList.add("card-header", "bg-primary", "text-white", "fs-4");
            header.style.display = "flex";
            header.style.justifyContent = "space-between";
            header.textContent = materia.nombreMateria;

            // Crear el dropdown
            const dropdown = document.createElement("div");
            dropdown.classList.add("dropdown");

            const button = document.createElement("button");
            button.classList.add("btn", "btn-link", "p-0", "text-white");
            button.setAttribute("data-bs-toggle", "dropdown");
            button.setAttribute("aria-expanded", "false");

            const icon = document.createElement("i");
            icon.classList.add("fas", "fa-ellipsis-v");
            button.appendChild(icon);

            const ul = document.createElement("ul");
            ul.classList.add("dropdown-menu", "dropdown-menu-end");

            const editLi = document.createElement("li");
            const editLink = document.createElement("a");
            editLink.classList.add("dropdown-item");
            editLink.href = "#";
            editLink.onclick = () => editarMateria(materia.materiaId);
            editLink.textContent = "Editar";
            editLi.appendChild(editLink);

            const deleteLi = document.createElement("li");
            const deleteLink = document.createElement("a");
            deleteLink.classList.add("dropdown-item");
            deleteLink.href = "#";
            deleteLink.onclick = () => eliminarMateria(materia.materiaId);
            deleteLink.textContent = "Eliminar";
            deleteLi.appendChild(deleteLink);

            // Añadir los elementos al menú desplegable
            ul.appendChild(editLi);
            ul.appendChild(deleteLi);

            // Añadir el botón y el menú al dropdown
            dropdown.appendChild(button);
            dropdown.appendChild(ul);

            // Añadir el dropdown al header
            header.appendChild(dropdown);

            // Body
            const body = document.createElement("div");
            body.classList.add("materia-card-body");

            const title = document.createElement("h5");
            title.classList.add("materia-card-title");

            const description = document.createElement("p");
            description.classList.add("materia-card-text");
            description.textContent = materia.descripcion || "Sin descripción";

            body.appendChild(title);
            body.appendChild(description);

            // Actividades Recientes - Crear una sección para las actividades
            if (materia.actividadesRecientes && materia.actividadesRecientes.length > 0) {
                const actividadesContainer = document.createElement("div");
                actividadesContainer.classList.add("mt-3"); // Margen superior para separar las actividades

                materia.actividadesRecientes.forEach(actividad => {
                    const actividadItem = document.createElement("div");
                    actividadItem.classList.add("actividad-item");

                    const fechaFormateada = new Date(actividad.fechaCreacion).toLocaleDateString('es-ES', {
                        day: '2-digit',
                        month: '2-digit',
                        year: 'numeric'
                    });

                    const actividadLink = document.createElement("a");
                    actividadLink.href = "#";
                    actividadLink.classList.add("actividad-link");
                    actividadLink.textContent = actividad.nombreActividad;
                    //actividadLink.setAttribute("data-id", actividad.actividadId, materia.materiaId);
                    actividadLink.setAttribute("data-actividad-id", actividad.actividadId);
                    actividadLink.setAttribute("data-materia-id", materia.materiaId);

                    const actividadFecha = document.createElement("p");
                    actividadFecha.classList.add("actividad-fecha");
                    actividadFecha.textContent = `Asignada: ${fechaFormateada}`;

                    actividadItem.appendChild(actividadLink);
                    actividadItem.appendChild(actividadFecha);

                    actividadesContainer.appendChild(actividadItem);
                });

                body.appendChild(actividadesContainer); // Agregar actividades al cuerpo de la tarjeta
            }

            // Footer
            const footer = document.createElement("div");
            footer.classList.add("card-footer", "d-flex", "justify-content-between", "align-items-center");

            const btnVerMateria = document.createElement("button");
            btnVerMateria.classList.add("btn", "btn-sm", "btn-primary");
            btnVerMateria.textContent = "Ver Materia";
            btnVerMateria.onclick = () => irAMateria(materia.materiaId);

            // Contenedor de iconos
            const iconContainer = document.createElement("div");
            iconContainer.classList.add("d-flex", "gap-2");

            const icons = [
                { src: "https://cdn-icons-png.flaticon.com/512/1828/1828817.png", title: "Ver Actividades", onclick: () => irAMateria(materia.materiaId, 'actividades') },
                { src: "https://cdn-icons-png.flaticon.com/512/847/847969.png", title: "Ver Integrantes", onclick: () => irAMateria(materia.materiaId, 'alumnos') },
            ];

            icons.forEach(({ src, title, onclick }) => {
                const img = document.createElement("img");
                img.classList.add("icon-action");
                img.src = src;
                img.alt = title;
                img.title = title;
                img.onclick = onclick;
                iconContainer.appendChild(img);
            });

            footer.appendChild(btnVerMateria);
            footer.appendChild(iconContainer);
            // Construcción de la card
            card.appendChild(header);
            card.appendChild(body);
            card.appendChild(footer);
            col.appendChild(card);

            // Agregar la columna al contenedor de la fila
                rowContainer.appendChild(col);
            });

            listaMateriasSinGrupo.appendChild(rowContainer);
            cerrarCargando(); // Cerrar indicador de carga

        } else {
            throw new Error("Error al cargar las materias.");
        }
    } catch (error) {
        // Si hay un error (por ejemplo, desconexión de red), el evento 'offline' ya lo maneja globalmente
        Swal.fire({
            title: "Error al cargar materias",
            text: "Por favor verifica tu conexión a internet.",
            icon: "error",
            confirmButtonText: "Reintentar",
        }).then(() => {
            cargarMateriasSinGrupo(); // Reintentar la carga
        });
    }
}



// Función para editar nombre y descripción de una materia. Sin funcionar aún.
async function editarMateria(materiaId) {
    // Obtener datos actuales del aviso
    try {
        const response = await fetch(`/api/MateriasApi/ObtenerMateria/${materiaId}`);
        if (!response.ok) throw new Error("No se pudo obtener la materia.");

        const materia = await response.json();

        // Mostrar SweetAlert con los datos actuales
        const { value: formValues } = await Swal.fire({
            title: "Editar Materia",
            html: `
                <input id="swal-materia" class="swal2-input" placeholder="Título" value="${materia.nombreMateria}">
                <textarea id="swal-descripcionMateria" class="swal2-textarea" placeholder="Descripción">${materia.descripcion}</textarea>
            `,
            focusConfirm: false,
            showCancelButton: true,
            confirmButtonText: "Guardar Cambios",
            cancelButtonText: "Cancelar",
            preConfirm: () => {
                const nombreMateria = document.getElementById("swal-materia").value.trim();
                const descripcionMateria = document.getElementById("swal-descripcionMateria").value.trim();

                if (!nombreMateria || !descripcionMateria) {
                    // Validación: Si alguno de los campos está vacío, mostrar mensaje de error
                    Swal.showValidationMessage("Todos los campos son requeridos.");
                    return false; // No continuar con la confirmación
                }

                return { materia: nombreMateria, descripcionMateria };
            }
        });

        if (!formValues) return; // Si el usuario cancela, no hacer nada

        // Enviar los cambios al backend
        const updateResponse = await fetch(`/api/MateriasApi/ActualizarMateria`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                materiaId,
                nombreMateria: formValues.materia,
                descripcion: formValues.descripcionMateria,
                docenteId: docenteIdGlobal
            })
        });

        if (!updateResponse.ok) throw new Error("No se pudo actualizar la materia.");
        await Swal.fire({
            position: "top-end",
            icon: "success",
            title: "Actualizado",
            text: "La materia ha sido editada correctamente.",
            showConfirmButton: false,
            timer: 2500
        });

        // Recargar para reflejar los cambios
        cargarGrupos();
        cargarMaterias();
        cargarMateriasSinGrupo();
    }
    catch (error) {
        await Swal.fire("Error", error.message, "error");
    }
}




async function eliminarMateria(MateriaId) {
    const confirmacion = await Swal.fire({
        title: "¿Estás seguro?",
        html: `
                    <p>Esto eliminará <b>todo lo que contenga la materia</b> incluyendo:</p>
                    <ul style="text-align: left;">
                        <li>Avisos</li>
                        <li>Actividades</li>
                        <li>Alumnos asignados</li>
                        <li>Calificaciones</li>
                    </ul>
                    <p>No podrás recuperar esta información después.</p>
                `,
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
                    title: resultado.mensaje || "No se pudo eliminar la materia.",
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


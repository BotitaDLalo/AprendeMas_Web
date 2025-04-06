//Crea un nuevo grupo, con la posibilidad de agregar una materia sin grupo, y crear directamente varias materia para ese grupo
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

//funcion que ayuda a agregar materias nuevas para el grupo
function agregarMateria() {
    const materiasContainer = document.getElementById("listaMaterias");

    const materiaDiv = document.createElement("div");
    materiaDiv.classList.add("materia-item");

    materiaDiv.innerHTML = `
        <div class="materia-fields">
            <input type="text" placeholder="Nombre de la Materia" class="nombreMateria" />
            <input type="text" placeholder="Descripción" class="descripcionMateria" />
        </div>
        <button type="button" class="btn-remover" onclick="removerDeLista(this)">❌</button>
    `;

    materiasContainer.appendChild(materiaDiv);
}


// Remover materia del formulario antes de enviarla
function removerDeLista(button) {
    button.parentElement.remove();
}



// Función para obtener los grupos de la base de datos y mostrarlos
async function cargarGrupos() {
    mostrarCargando("Cargando grupos..."); // Mostrar indicador de carga

    try {
        const response = await fetch(`/api/GruposApi/ObtenerGrupos/${docenteIdGlobal}`);

        if (response.ok) {
            const grupos = await response.json();
            const listaGrupos = document.getElementById("listaGrupos");
            listaGrupos.innerHTML = "";

            if (grupos.length === 0) {
                const mensaje = document.createElement("p");
                mensaje.classList.add("text-center", "text-muted");
                mensaje.textContent = "No hay grupos registrados.";
                listaGrupos.appendChild(mensaje);
                cerrarCargando(); // Cerrar indicador de carga
                return;
            }

            grupos.forEach(grupo => {
                // 📌 Tarjeta principal
                const card = document.createElement("div");
                card.id = `grupo-${grupo.grupoId}`;
                card.classList.add("grupo-card", "bg-primary", "text-white", "mb-3");
                card.style.cursor = "pointer";
                card.style.maxWidth = "20em";
                card.style.height = "5em";
                card.style.display = "block";
                card.style.alignItems = "center";
                card.style.transition = "transform 0.3s ease, box-shadow 0.3s ease";
                card.onmouseover = () => {
                    card.style.transform = "translateY(-10px)";
                    card.style.boxShadow = "0 10px 20px rgba(0, 0, 0, 0.2)";
                };
                card.onmouseout = () => {
                    card.style.transform = "";
                    card.style.boxShadow = "";
                };

                // 📌 Imagen del grupo
                const img = document.createElement("img");
                img.classList.add("card-img-top");
                img.src = "/Iconos/1-26.svg";
                img.alt = "Grupo";
                img.style.maxWidth = "25%";
                img.style.margin = "auto";
                img.style.padding = "0.5em";
                img.style.borderRadius = "0.7em";

                // 📌 Contenedor del contenido
                const cardBody = document.createElement("div");
                cardBody.classList.add("grupo-card-body");
                cardBody.style.display = "flex";
                cardBody.style.justifyContent = "space-between";
                cardBody.style.alignItems = "center";
                cardBody.style.flex = "1";
                cardBody.style.overflow = "hidden";

                // 📌 Sección de texto
                const textSection = document.createElement("div");
                textSection.classList.add("text-section");
                textSection.style.maxWidth = "100%";
                textSection.style.overflow = "hidden";
                textSection.style.display = "flex";
                textSection.style.flexDirection = "column";
                textSection.style.justifyContent = "center";

                const title = document.createElement("h5");
                title.classList.add("card-title");
                title.textContent = grupo.nombreGrupo;
                title.style.whiteSpace = "nowrap";
                title.style.overflow = "hidden";
                title.style.textOverflow = "ellipsis";
                title.style.margin = "0";
                title.style.fontWeight = "bold";

                const description = document.createElement("p");
                description.classList.add("card-text");
                description.textContent = grupo.descripcion || "Sin descripción";
                description.style.whiteSpace = "nowrap";
                description.style.overflow = "hidden";
                description.style.textOverflow = "ellipsis";
                description.style.margin = "0";

                textSection.appendChild(title);
                textSection.appendChild(description);

                // 📌 Sección del botón (Icono de engranaje)
                const ctaSection = document.createElement("div");
                ctaSection.classList.add("cta-section");
                ctaSection.style.maxWidth = "40%";
                ctaSection.style.display = "flex";
                ctaSection.style.flexDirection = "column";
                ctaSection.style.justifyContent = "center";

                const settingsButton = document.createElement("button");
                settingsButton.classList.add("btn", "btn-link", "text-white", "p-0");
                settingsButton.type = "button";
                settingsButton.setAttribute("data-bs-toggle", "dropdown");
                settingsButton.setAttribute("aria-expanded", "false");
                settingsButton.onclick = (event) => event.stopPropagation();
                settingsButton.style.width = "3em";
                settingsButton.style.height = "3em";
                settingsButton.style.display = "flex";
                settingsButton.style.alignItems = "center";
                settingsButton.style.justifyContent = "center";
                settingsButton.style.border = "none";
                settingsButton.style.outline = "none";
                settingsButton.style.textDecoration = "none";

                const settingsIcon = document.createElement("i");
                settingsIcon.classList.add("fas", "fa-cog");
                settingsIcon.style.fontSize = "1.5em";


                // Crear el menú dropdown con clases de Bootstrap
                const dropdownMenu = document.createElement("div");
                dropdownMenu.classList.add("dropdown-menu");

                // Añadir items al dropdown
                const dropdownItems = [
                    { text: "Compartir Acceso", action: () => CompartirAcceso(grupo.codigoAcceso, grupo.nombreGrupo) },
                    { text: "Eliminar", action: () => eliminarGrupo(grupo.grupoId) },
                    { text: "Aviso Grupal", action: () => crearAvisoGrupal(grupo.grupoId) },
                    { text: "Crear Materia", action: () => agregarMateriaAlGrupo(grupo.grupoId) },
                    { text: "Editar Grupo", action: () => editarGrupo(grupo.grupoId) }
                ];

                dropdownItems.forEach(item => {
                    const dropdownItem = document.createElement("a");
                    dropdownItem.classList.add("dropdown-item");
                    dropdownItem.href = "#";
                    dropdownItem.textContent = item.text;
                    dropdownItem.addEventListener("click", (e) => {
                        e.preventDefault();
                        item.action();
                    });
                    dropdownMenu.appendChild(dropdownItem);
                });

                // Ensamblar todos los elementos
                settingsButton.appendChild(settingsIcon);
                ctaSection.appendChild(settingsButton);
                ctaSection.appendChild(dropdownMenu);

                // 📌 Contenedor de materias (inicialmente oculto)
                const materiasContainer = document.createElement("div");
                materiasContainer.id = `materiasContainer-${grupo.grupoId}`;
                materiasContainer.classList.add("materias-container");
                materiasContainer.style.display = "none";
                materiasContainer.style.paddingLeft = "20px";
                materiasContainer.style.marginBottom = "20px";

                // 📌 Evento al hacer clic en la tarjeta
                card.onclick = () => {
                    handleCardClick(grupo.grupoId);
                };

                // 📌 Estructura final
                cardBody.appendChild(textSection);
                cardBody.appendChild(ctaSection);

                const contentWrapper = document.createElement("div");
                contentWrapper.style.display = "flex";
                contentWrapper.style.width = "100%";
                contentWrapper.appendChild(img);
                contentWrapper.appendChild(cardBody);

                card.appendChild(contentWrapper);

                listaGrupos.appendChild(card);
                listaGrupos.appendChild(materiasContainer);
            });

            cerrarCargando(); // Cerrar indicador de carga

        } else {
            throw new Error("Error al cargar los grupos.");
        }
    } catch (error) {
        // El evento 'offline' ya maneja la reconexión globalmente
        Swal.fire({
            title: "Error al cargar los grupos.",
            html: "Reintentando en <b></b> segundos...",
            timer: 4000,
            timerProgressBar: true,
            allowOutsideClick: false,
            showCancelButton: true,
            cancelButtonText: "Cerrar sesión",
            didOpen: () => {
                Swal.showLoading();
                const timer = Swal.getPopup().querySelector("b");
                let interval = setInterval(() => {
                    timer.textContent = `${Math.floor(Swal.getTimerLeft() / 1000)}`;
                }, 100);
            },
            willClose: () => clearInterval(timerInterval)
        }).then((result) => {
            if (result.dismiss === Swal.DismissReason.timer) {
                cargarGrupos(); // Reintentar la carga
            } else if (result.dismiss === Swal.DismissReason.cancel) {
                cerrarSesion();
            }
        });
    }
    document.getElementById('gruposModal').addEventListener('hidden.bs.modal', cargarGrupos);
}


// Función para cargar materias de un grupo cuando se hace clic en la card del grupo
async function handleCardClick(grupoId) {
    const grupoCard = document.getElementById(`grupo-${grupoId}`);

    localStorage.setItem("grupoIdSeleccionado", grupoId); //Se guardar el localstorage el id del grupo seleccionado

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
                const rowContainer = document.createElement("div");
                rowContainer.classList.add("row", "g-3");

                materias.forEach(materia => {
                    const col = document.createElement("div");
                    col.classList.add("col-md-3"); // Ajusta el tamaño de la tarjeta en la fila

                    const card = document.createElement("div");
                    card.classList.add("card", "bg-light", "mb-3", "shadow-sm");
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
                    body.classList.add("card-body");

                    const title = document.createElement("h5");
                    title.classList.add("card-title");

                    const description = document.createElement("p");
                    description.classList.add("card-text");
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
                           // actividadLink.setAttribute("data-id", actividad.actividadId, materia.materiaId);
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
                grupoCard.insertAdjacentElement("afterend", rowContainer);
                materiasContainer.appendChild(rowContainer);
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

async function CompartirAcceso(codigoAcceso, nombreGrupo) {
    try {
        // Crear el mensaje completo
        const mensaje = `Únete al grupo "${nombreGrupo}" con el siguiente código de acceso: ${codigoAcceso}`;

        // Copiar al portapapeles
        await navigator.clipboard.writeText(mensaje);

        // Notificar al usuario con SweetAlert
        Swal.fire({
            icon: 'success',
            title: 'Código copiado',
            text: 'El código de acceso al grupo ha sido copiado al portapapeles.',
            showConfirmButton: false,
            timer: 1500
        });
    } catch (error) {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'No se pudo copiar al portapapeles.',
            showConfirmButton: true
        });
        console.error('Error al copiar al portapapeles:', error);
    }
}



//Funciones de contenedor de grupo
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
            // Eliminar solo el grupo
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
            // Segunda confirmación antes de eliminar grupo y materias
            const confirmacion = await Swal.fire({
                title: "¿Estás completamente seguro?",
                html: `
                    <p>Esto eliminará <b>todas las materias del grupo</b> y además:</p>
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
                confirmButtonText: "Sí, eliminar todo",
                cancelButtonText: "Cancelar"
            });

            if (confirmacion.isConfirmed) {
                // Eliminar grupo y materias
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
        }
    });
}



//Funcion para agregar al grupo una nueva materia
async function agregarMateriaAlGrupo(id) {
    const { value: formValues } = await Swal.fire({
        title: "Añadir materia al grupo",
        html:
            '<input id="swal-nombre" class="swal2-input" placeholder="Nombre de la materia">' +
            '<input id="swal-descripcion" class="swal2-input" placeholder="Descripción">',
        focusConfirm: false,
        showCancelButton: true,
        confirmButtonText: "Agregar",
        preConfirm: () => {
            return {
                nombre: document.getElementById("swal-nombre").value.trim(),
                descripcion: document.getElementById("swal-descripcion").value.trim()
            };
        }
    });

    if (!formValues || !formValues.nombre || !formValues.descripcion) {
        Swal.fire("Atención", "Debes completar todos los campos.", "warning");
        return;
    }

    if (typeof docenteIdGlobal === "undefined") {
        Swal.fire("Error", "No se ha encontrado el ID del docente.", "error");
        return;
    }

    const nuevaMateria = {
        NombreMateria: formValues.nombre,
        Descripcion: formValues.descripcion,
        CodigoColor: "#2196F3",
        DocenteId: docenteIdGlobal
    };

    try {
        const response = await fetch(`/api/MateriasApi/AgregarMateriaAlGrupo/${id}`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(nuevaMateria)
        });

        let data;
        try {
            data = await response.json();
        } catch {
            data = { mensaje: "Error desconocido en el servidor." };
        }

        if (response.ok) {
            Swal.fire("Éxito", "Materia creada y asociada al grupo correctamente", "success");
            cargarGrupos();
        } else {
            Swal.fire("Error", data.mensaje || "Ocurrió un error", "error");
        }
    } catch (error) {
        console.error("Error al agregar la materia:", error);
        Swal.fire("Error", "Ocurrió un error al intentar agregar la materia.", "error");
    }
}






// Función para editar nombre y descripción de una materia. Sin funcionar aún.
async function editarGrupo(grupoId) {
    // Obtener datos actuales del aviso
    try {
        const response = await fetch(`/api/GruposApi/ObtenerGrupo/${grupoId}`);
        if (!response.ok) throw new Error("No se pudo obtener el grupo.");

        const grupo = await response.json();

        // Mostrar SweetAlert con los datos actuales
        const { value: formValues } = await Swal.fire({
            title: "Editar Grupo",
            html: `
                <input id="swal-grupo" class="swal2-input" placeholder="Título" value="${grupo.nombreGrupo}">
                <textarea id="swal-descripcionGrupo" class="swal2-textarea" placeholder="Descripción">${grupo.descripcion}</textarea>
            `,
            focusConfirm: false,
            showCancelButton: true,
            confirmButtonText: "Guardar Cambios",
            cancelButtonText: "Cancelar",
            preConfirm: () => {
                const nombreGrupo = document.getElementById("swal-grupo").value.trim();
                const descripcionGrupo = document.getElementById("swal-descripcionGrupo").value.trim();

                if (!nombreGrupo || !descripcionGrupo) {
                    // Validación: Si alguno de los campos está vacío, mostrar mensaje de error
                    Swal.showValidationMessage("Todos los campos son requeridos.");
                    return false; // No continuar con la confirmación
                }

                return { grupo: nombreGrupo, descripcionGrupo };
            }
        });

        if (!formValues) return; // Si el usuario cancela, no hacer nada

        // Enviar los cambios al backend
        const updateResponse = await fetch(`/api/GruposApi/ActualizarGrupo`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                grupoId,
                nombreGrupo: formValues.grupo,
                descripcion: formValues.descripcionGrupo,
                docenteId: docenteIdGlobal
            })
        });

        if (!updateResponse.ok) throw new Error("No se pudo actualizar el grupo.");

        Swal.fire("Actualizado", "El grupo ha sido editado correctamente.", "success");

        // Recargar para reflejar los cambios
        cargarGrupos();
    }
    catch (error) {
        Swal.fire("Error", error.message, "error");
    }
}



async function crearAvisoGrupal(id) {
    const result = await Swal.fire({
        title: "Crear Aviso",
        html:
            '<input id="tituloAviso" class="swal2-input" placeholder="Título del aviso">' +
            '<textarea id="descripcionAviso" class="swal2-textarea" placeholder="Descripción del aviso"></textarea>',
        showCancelButton: true,
        confirmButtonText: "Crear",
        cancelButtonText: "Cancelar",
        preConfirm: () => {
            const titulo = document.getElementById("tituloAviso").value.trim();
            const descripcion = document.getElementById("descripcionAviso").value.trim();

            if (!titulo || !descripcion) {
                Swal.showValidationMessage("Debes completar todos los campos");
                return false;
            }
            return { titulo, descripcion };
        }
    });

    if (result.isConfirmed) {
        const datos = {
            GrupoId: id,
            Titulo: result.value.titulo,
            Descripcion: result.value.descripcion,
            DocenteId: docenteIdGlobal
        };

        try {
            const response = await fetch("/api/DetallesMateriaApi/CrearAvisoPorGrupo", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(datos)
            });

            const data = await response.json();
            if (data.mensaje) {
                Swal.fire("Éxito", data.mensaje, "success");
            } else {
                Swal.fire("Error", "No se pudo crear el aviso", "error");
            }
        } catch (error) {
            console.error("Error al enviar el aviso:", error);
            Swal.fire("Error", "Ocurrió un error al crear el aviso", "error");
        }
    }
}



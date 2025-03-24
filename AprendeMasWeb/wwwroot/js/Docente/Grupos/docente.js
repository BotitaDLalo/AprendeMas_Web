let docenteIdGlobal = null; //Variable para almacenar el docenteId
let materiasPorCrear = []; // Lista de materias a crear
let intentosAcceder = 0;


//Funcion que obtiene informacion del docente.
async function obtenerDocenteId() {
    try {
        // Hacemos una solicitud para obtener el docenteId desde el servidor
        const response = await fetch('/Cuenta/ObtenerDocenteId'); // Llamar al controlador
        const data = await response.json(); // Convertimos la respuesta en formato JSON
        if (data.docenteId) {
            docenteIdGlobal = data.docenteId; // Guardamos el docenteId en la variable 
            localStorage.setItem("docenteId", docenteIdGlobal); // Guardamos el docenteIdGlobal en el almacenamiento local

            // Alerta con diseño de Toast
            const Toast = Swal.mixin({
                toast: true,
                position: "top-end",
                showConfirmButton: false,
                timer: 1500,
                timerProgressBar: true,
                didOpen: (toast) => {
                    toast.onmouseleave = Swal.resumeTimer;
                }
            });

            Toast.fire({
                icon: "success",
                title: "Todo correcto."
            });

        }
    } catch (error) {
        AlertaCierreSesion(); // Si existe un error, cierra la sesión
    }
}





// Función para asociar materias al grupo
async function asociarMateriasAGrupo(grupoId, materias) {
    const response = await fetch('/api/GruposApi/AsociarMaterias', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ GrupoId: grupoId, MateriaIds: materias })
    });

    if (!response.ok) {
        Swal.fire({
            position: "top-end",
            icon: "error",
            title: "Error al asociar materias con grupo.",
            showConfirmButton: false,
            timer: 2000
        });
    }
}



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



async function cargarMateriasSinGrupo() {
    const response = await fetch(`/api/MateriasApi/ObtenerMateriasSinGrupo/${docenteIdGlobal}`);
    if (response.ok) {
        const materiasSinGrupo = await response.json();
        const listaMateriasSinGrupo = document.getElementById("listaMateriasSinGrupo");

        // Limpiar contenido anterior y crear el contenedor con Bootstrap Grid
        listaMateriasSinGrupo.innerHTML = "";
        const rowContainer = document.createElement("div");
        rowContainer.classList.add("row", "g-3"); // "g-3" agrega un pequeño espacio entre las filas

        if (materiasSinGrupo.length === 0) {
            const mensaje = document.createElement("p");
            mensaje.classList.add("text-center", "text-muted");
            mensaje.textContent = "No hay materias registradas.";
            listaMateriasSinGrupo.appendChild(mensaje);
            return;
        }

        materiasSinGrupo.forEach(materia => {
            const col = document.createElement("div");
            col.classList.add("col-md-3"); // Ajusta el tamaño de la tarjeta en la fila

            const card = document.createElement("div");
            card.classList.add("card", "bg-light", "mb-3", "shadow-sm");
            card.style.maxWidth = "100%";

            // Header
            // Crear el header
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

            const deactivateLi = document.createElement("li");
            const deactivateLink = document.createElement("a");
            deactivateLink.classList.add("dropdown-item");
            deactivateLink.href = "#";
            deactivateLink.onclick = () => desactivarMateria(materia.materiaId);
            deactivateLink.textContent = "Desactivar";
            deactivateLi.appendChild(deactivateLink);

            // Añadir los elementos al menú desplegable
            ul.appendChild(editLi);
            ul.appendChild(deleteLi);
            ul.appendChild(deactivateLi);

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
                { src: "https://cdn-icons-png.flaticon.com/512/1828/1828817.png", title: "Ver Actividades", onclick: () => verActividades(materia.materiaId) },
                { src: "https://cdn-icons-png.flaticon.com/512/847/847969.png", title: "Ver Integrantes", onclick: () => verIntegrantes(materia.materiaId) },
                { src: "https://cdn-icons-png.flaticon.com/512/535/535285.png", title: "Destacar Materia", onclick: () => destacarMateria(materia.materiaId) }
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

        // Agregar todas las tarjetas dentro del contenedor de filas
        listaMateriasSinGrupo.appendChild(rowContainer);

    } else {
        Swal.fire({
            title: "Error al cargar materias",
            html: "Reintentando en <b></b> segundos...",
            timer: 4000,
            timerProgressBar: true,
            allowOutsideClick: false,
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
                cargarMateriasSinGrupo();
            }
        });
    }
}


// Función para redirigir a la vista Materias dentro del controlador Docente
function irAMateria(materiaIdSeleccionada,seccion = 'avisos') {
    //guardar el id de la materia en localstorage para obtenerla en otros scripts
    localStorage.setItem("materiaIdSeleccionada", materiaIdSeleccionada);
    // Redirige a la página de detalles de la materia
    window.location.href = `/Docente/MateriasDetalles?materiaId=${materiaIdSeleccionada}&seccion=${seccion}`;
}


//Funcion para obtener los grupos de la base de datos. -----------------------------------------------------
//async function cargarGrupos() { // Lógica para actualizar la lista de grupos en vista
//    const response = await fetch(`/api/GruposApi/ObtenerGrupos/${docenteIdGlobal}`); // Solicita los grupos del docente

//    if (response.ok) { // Si la respuesta es exitosa
//        const grupos = await response.json(); // Convierte la respuesta a formato JSON
//        const listaGrupos = document.getElementById("listaGrupos"); // Obtiene el elemento donde se mostrarán los grupos

//        if (grupos.length === 0) { // Si no hay grupos registrados
//            listaGrupos.innerHTML = "<p> No hay grupos registrados.</p>"; // Muestra un mensaje indicando que no hay grupos
//            return; // Sale de la función
//        }

//        listaGrupos.innerHTML = grupos.map(grupo => `
//        <div class="grupo-card mb-3" style="background-color: ${grupo.codigoColor || '#FFA500'};
//            border-radius: 12px; width: 400px; padding: 15px; margin-bottom: 15px;
//            cursor: pointer; transition: all 0.3s ease-in-out;"
//             onmouseover="this.style.transform='translateY(-5px)'; this.style.boxShadow='0px 4px 12px rgba(0, 0, 0, 0.3)';"
//             onmouseout="this.style.transform='none'; this.style.boxShadow='none';"
//             onclick="handleCardClick(${grupo.grupoId})">

//         <div class="d-flex justify-content-between align-items-center">
//             <h5 class="text-white mb-0">
//                <strong class="font-weight-bold">${grupo.nombreGrupo}</strong> - ${grupo.descripcion || "Sin descripción"}
//             </h5>

        // <div class="dropdown">
        //     <button class="btn btn-link text-white p-0" type="button" data-bs-toggle="dropdown" aria-expanded="false" onclick="event.stopPropagation();">
        //         <i class="fas fa-cog"></i> <!-- Icono de engranaje -->
        //     </button>
        //             <ul class="dropdown-menu dropdown-menu-end">
        //             <li><a class="dropdown-item" href="#" onclick="editarGrupo(${grupo.grupoId})">Editar</a></li> <!-- Opción para editar grupo -->
        //             <li><a class="dropdown-item" href="#" onclick="eliminarGrupo(${grupo.grupoId})">Eliminar</a></li> <!-- Opción para eliminar grupo -->
        //             <li><a class="dropdown-item" href="#" onclick="desactivarGrupo(${grupo.grupoId})">Desactivar</a></li> <!-- Opción para desactivar grupo -->
        //             </ul>
        //         </div>
        //    </div>
        //</div>
//        <!-- Contenedor donde se mostrarán las materias -->
//         <div id="materiasContainer-${grupo.grupoId}" class="materias-container" style="display: none; padding-left: 20px;"></div>
//        `).join(''); // Muestra los grupos como tarjetas dinámicas
//    } else {
//        let timerInterval; // Variable para almacenar el intervalo del temporizador

//        Swal.fire({
//            title: "Error al cargar los grupos.", // Mensaje de error en la alerta
//            html: "Se reintentará automáticamente en: <b></b>.", // Mensaje con temporizador dinámico
//            timer: 4000, // Tiempo en milisegundos antes de que la alerta se cierre automáticamente
//            timerProgressBar: true, // Muestra una barra de progreso indicando el tiempo restante
//            allowOutsideClick: false, // Evita que el usuario cierre la alerta haciendo clic fuera de ella
//            showCancelButton: true, // Muestra un botón de cancelar dentro de la alerta
//            cancelButtonText: "Cerrar sesión", // Texto que aparecerá en el botón de cancelar

//            didOpen: () => {
//                // Se ejecuta cuando la alerta se abre
//                Swal.showLoading(); // Muestra un indicador de carga
//                const timer = Swal.getPopup().querySelector("b"); // Obtiene el elemento <b> para mostrar el tiempo restante
//                timerInterval = setInterval(() => {
//                    timer.textContent = `${Math.floor(Swal.getTimerLeft() / 1000)} segundos`; // Actualiza el temporizador en segundos
//                }, 100); // Se actualiza cada 100ms
//            },

//            willClose: () => {
//                // Se ejecuta cuando la alerta está a punto de cerrarse
//                clearInterval(timerInterval); // Detiene la actualización del temporizador
//            }

//        }).then((result) => {
//            // Se ejecuta cuando la alerta se cierra manualmente o por el temporizador
//            if (result.dismiss === Swal.DismissReason.timer) {
//                // Si la alerta se cerró automáticamente por el temporizador
//                console.log("Reintentando cargar los grupos.");
//                inicializar(); // Reintenta la carga de grupos automáticamente
//            } else if (result.dismiss === Swal.DismissReason.cancel) {
//                // Si el usuario hizo clic en "Cerrar sesión"
//                console.log("El usuario decidió cerrar sesión.");
//                cerrarSesion(); // Llama a la función para cerrar sesión
//            }
//        });
//    }
//    document.getElementById('gruposModal').addEventListener('hidden.bs.modal', function () {
//        cargarGrupos(); // Vuelve a cargar los grupos cuando se cierra el modal
//    });
//}




//Funcionalidades de los iconos de las Cards de materias.----------------------------------------------------------------

async function cargarGrupos() {
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
            return;
        }

        grupos.forEach(grupo => {
            // 📌 Tarjeta principal
            const card = document.createElement("div");
            card.classList.add("card", "bg-primary", "text-white", "mb-3");
            card.style.cursor = "pointer";
            card.style.maxWidth = "30em";
            card.style.height = "6em";
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
            cardBody.classList.add("card-body");
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


            // ... (el código anterior hasta crear el settingsButton)

            // Crear el menú dropdown con clases de Bootstrap
            const dropdownMenu = document.createElement("div");
            dropdownMenu.classList.add("dropdown-menu");

            // Añadir items al dropdown
            const dropdownItems = [
                //{ text: "Editar", action: () => console.log("Opción 1 seleccionada") },
                { text: "Eliminar", action: () => eliminarGrupo(grupo.grupoId) },
                //{ text: "Desactivar", action: () => console.log("Opción 3 seleccionada") }
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
    } else {
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
                cargarGrupos();
            } else if (result.dismiss === Swal.DismissReason.cancel) {
                cerrarSesion();
            }
        });
    }

    document.getElementById('gruposModal').addEventListener('hidden.bs.modal', cargarGrupos);
}

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

function editarMateria(MateriaId) {

}

function desactivarMateria(MateriaId) {

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
//Funcionalidades de los iconos de las cards de grupos

//esta funcion oculta la materias de los demas

// Función para cargar materias de un grupo cuando se hace clic en la card del grupo
async function handleCardClick(grupoId) {
    localStorage.setItem("grupoIdSeleccionado", grupoId);

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
                //materiasContainer.innerHTML = `
                //    <div class="container-cards"">
                //        ${materias.map(materia => `
                //            <div class="card card-custom" style="border-radius: 10px;">
                //                <div class="card-header-custom" style="background-color: ${materia.codigoColor || '#000'};">
                //                    <div class="d-flex justify-content-between align-items-center">
                //                        <span class="text-dark">${materia.nombreMateria}</span>
                //                        <div class="dropdown">
                //                            <button class="btn btn-link p-0 text-dark" data-bs-toggle="dropdown" aria-expanded="false">
                //                                <i class="fas fa-ellipsis-v"></i>
                //                            </button>
                //                            <ul class="dropdown-menu dropdown-menu-end">
                //                                <li><a class="dropdown-item" href="#" onclick="editarMateria(${materia.materiaId})">Editar</a></li>
                //                                <li><a class="dropdown-item" href="#" onclick="eliminarMateria(${materia.materiaId})">Eliminar</a></li>
                //                                <li><a class="dropdown-item" href="#" onclick="desactivarMateria(${materia.materiaId})">Desactivar</a></li>
                //                            </ul>
                //                        </div>
                //                    </div>
                //                </div>
                //                <div class="card-body card-body-custom" style="background-color: #e0e0e0">
                //                    <p class="card-text">${materia.descripcion || "Sin descripción"}</p>
                //                </div>
                //                <div class="card-footer card-footer-custom">
                //                    <button class="btn btn-sm btn-primary" onclick="irAMateria(${materia.materiaId})">Ver Materia</button>
                //                    <div class="icon-container">
                //                        <img class="icon-action" src="https://cdn-icons-png.flaticon.com/512/1828/1828817.png" alt="Ver Actividades" title="Ver Actividades" onclick="verActividades(${materia.materiaId})">
                //                        <img class="icon-action" src="https://cdn-icons-png.flaticon.com/512/847/847969.png" alt="Ver Integrantes" title="Ver Integrantes" onclick="verIntegrantes(${materia.materiaId})">
                //                        <img class="icon-action" src="https://cdn-icons-png.flaticon.com/512/535/535285.png" alt="Destacar" title="Destacar Materia" onclick="destacarMateria(${materia.materiaId})">
                //                    </div>
                //                </div>
                //            </div>
                //        `).join('')}
                //    </div>
                //`;

                const rowContainer = document.createElement("div");
                rowContainer.classList.add("row", "g-3");

                materias.forEach(materia => {
                    const col = document.createElement("div");
                    col.classList.add("col-md-3"); // Ajusta el tamaño de la tarjeta en la fila

                    const card = document.createElement("div");
                    card.classList.add("card", "bg-light", "mb-3", "shadow-sm");
                    card.style.maxWidth = "100%";

                    // Header
                    // Crear el header
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

                    const deactivateLi = document.createElement("li");
                    const deactivateLink = document.createElement("a");
                    deactivateLink.classList.add("dropdown-item");
                    deactivateLink.href = "#";
                    deactivateLink.onclick = () => desactivarMateria(materia.materiaId);
                    deactivateLink.textContent = "Desactivar";
                    deactivateLi.appendChild(deactivateLink);

                    // Añadir los elementos al menú desplegable
                    ul.appendChild(editLi);
                    ul.appendChild(deleteLi);
                    ul.appendChild(deactivateLi);

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
                        { src: "https://cdn-icons-png.flaticon.com/512/1828/1828817.png", title: "Ver Actividades", onclick: () => verActividades(materia.materiaId) },
                        { src: "https://cdn-icons-png.flaticon.com/512/847/847969.png", title: "Ver Integrantes", onclick: () => verIntegrantes(materia.materiaId) },
                        { src: "https://cdn-icons-png.flaticon.com/512/535/535285.png", title: "Destacar Materia", onclick: () => destacarMateria(materia.materiaId) }
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

function editarGrupo(id) {
    alert("Editar grupo " + id); // Muestra una alerta indicando que el grupo será editado
}

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
            // Llamar al controlador que elimina solo el grupo
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
            // Llamar al nuevo controlador que elimina grupo y materias
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
    });
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
    // Se ejecuta solo cuando se abre el modal
    document.getElementById("gruposModal").addEventListener("shown.bs.modal", cargarMaterias);


    // Delegación de eventos: escucha los clics en el contenedor padre
    document.body.addEventListener("click", function (event) {
        let link = event.target.closest(".actividad-link"); // Detecta si el clic fue en un enlace de actividad
        if (link) {
            event.preventDefault(); // Evita la recarga de la página si es un <a>
            let actividadId = link.getAttribute("data-id"); // Obtener el ID correcto
            verActividad(actividadId);
        }
    });
});


function verActividad(id) {
    alert("Actividad seleccionada con ID:" + id);
    // Aquí se redirigira a la actividad seleccionada pasando como parametros a la actividad.
}


//Generar algun claim para reemplazar en la consulta deseada. 

//Me guarda las materias las asigna sin grupo, no hay enlace, son las que se guarda rellenar el modal de MateriasModal ------------------
async function guardarMateriaSinGrupo() { //Logica para guardar materia
    const nombre = document.getElementById("nombreMateria").value;
    const descripcion = document.getElementById("descripcionMateria").value;
    const color = document.getElementById("codigoColorMateria").value;

    if (nombre.trim() === '') {
        alert('Ingrese Nombre De La Materia.');
        return;
    }
    const response = await fetch('/api/MateriasApi/CrearMateria', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            NombreMateria: nombre,
            Descripcion: descripcion,
            CodigoColor: color,
            DocenteId: 2 //Remplazar por el claim del docente.
        })
    });
    if (response.ok) {
        alert('Materia guardada con éxito.');
        document.getElementById("materiasForm").reset();
        cargarMateriasSinGrupo();
    } else {
        alert('Error al guardar la materia.');
    }
}
//Guarda los grupos del modal GruposModal -------------------------------------------------------------

async function guardarGrupo() { // Lógica para guardar el grupo
    const nombre = document.getElementById("nombreGrupo").value;
    const descripcion = document.getElementById("descripcionGrupo").value;
    const color = document.getElementById("codigoColorGrupo").value;
    const checkboxes = document.querySelectorAll(".materia-checkbox:checked");

    if (nombre.trim() === '') {
        alert('Ingrese Nombre Del Grupo.');
        return;
    }

    // Obtener IDs de las materias seleccionadas
    const materiasSeleccionadas = Array.from(checkboxes).map(cb => cb.value);

    const response = await fetch('/api/GruposApi/CrearGrupo', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            NombreGrupo: nombre,
            Descripcion: descripcion,
            CodigoColor: color,
            DocenteId: 2 // Remplazar por el claim que usa el docente.
        })
    });

    if (response.ok) {
        const grupoCreado = await response.json(); // Obtener el ID del grupo creado

        if (materiasSeleccionadas.length > 0) {
            await asociarMateriasAGrupo(grupoCreado.GrupoId, materiasSeleccionadas);
        }

        alert('Grupo guardado con éxito.');
        document.getElementById("gruposForm").reset();
        cargarGrupos(); // Función para recargar la lista de grupos
    } else {
        alert('Error al guardar el grupo.');
    }
}

// Función para asociar materias al grupo
async function asociarMateriasAGrupo(grupoId, materias) {
    const response = await fetch('/api/GruposApi/AsociarMaterias', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            GrupoId: grupoId,
            MateriaIds: materias
        })
    });

    if (!response.ok) {
        alert('Error al asociar materias al grupo.');
    }
}


// Función para cargar las materias disponibles en el modal 
async function cargarMaterias() {
    try {
        const response = await fetch('/api/MateriasApi/ObtenerMateriasSinGrupo/2'); // solo  para checar funcionamiento se agrega la funcion de cargar materias sin grupo
        if (response.ok) {
            const materias = await response.json();
            const contenedorMaterias = document.getElementById("materiasLista");
           // contenedorMaterias.innerHTML = ""; // Limpiar antes de agregar

            if (materias.length === 0) {
                contenedorMaterias.innerHTML = "<p>No hay materias disponibles.</p>";
                return;
            }

            materias.forEach(materia => {
                const checkbox = document.createElement("input");
                checkbox.type = "checkbox";
                checkbox.className = "materia-checkbox";
                checkbox.value = materia.materiaId;

                const label = document.createElement("label");
                label.appendChild(checkbox);
                label.appendChild(document.createTextNode(" " + materia.nombreMateria));

                const div = document.createElement("div");
                div.className = "form-check";
                div.appendChild(label);

                contenedorMaterias.appendChild(div);
            });
        }
    } catch (error) {
        console.error("Error al cargar materias:", error);
    }
}

// Llamar a cargarMaterias cuando se abre el modal de grupos
document.getElementById("gruposModal").addEventListener("shown.bs.modal", cargarMaterias);

/*
async function guardarGrupo() { //Logica para guadar el grupo
    const nombre = document.getElementById("nombreGrupo").value;
    const descripcion = document.getElementById("descripcionGrupo").value;
    const color = document.getElementById("codigoColorGrupo").value;


    if (nombre.trim() === '') {
        alert('Ingrese Nombre Del Grupo.');
        return;
    }

    const response = await fetch('/api/GruposApi/CrearGrupo', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            NombreGrupo: nombre,
            Descripcion: descripcion,
            CodigoColor: color,
            DocenteId: 2 // Remplazar por el claim que usa el docente.
        })
    });

    if (response.ok) {
        alert('Grupo guardado con éxito.');
        document.getElementById("gruposForm").reset();
        cargarGrupos(); // Función para recargar la lista de grupos
    } else {
        alert('Error al guardar el grupo.');
    }
}

*/

// Cargar materias sin grupo -------------------------------------------------------
async function cargarMateriasSinGrupo(docenteId) {
    docenteId = 2;
    console.log("DocenteId: ", docenteId);

    const response = await fetch(`/api/MateriasApi/ObtenerMateriasSinGrupo/${docenteId}`)
    if (response.ok) {
        const materiasSinGrupo = await response.json();
        const listaMateriasSinGrupo = document.getElementById("listaMateriasSinGrupo");

        if (materiasSinGrupo.length === 0) {
            listaMateriasSinGrupo.innerHTML = "<p>No hay materias registradas.</p>";
            return;
        }

        // Asegurar que todas las cards estén dentro de un solo contenedor

        listaMateriasSinGrupo.innerHTML = `
    

            <div class="container-cards">
                ${materiasSinGrupo.map(materiaSinGrupo => `
                    <div class="card card-custom" style=" border-radius: 10px; /* Define el radio de las esquinas */">
                        <div class="card-header-custom" style="background-color: ${materiaSinGrupo.codigoColor || '#000'};  ">
                            <div class="d-flex justify-content-between align-items-center">
                                <span class="text-dark">${materiaSinGrupo.nombreMateria}</span> <!--Cambiar color del texto-->
                                <div class="dropdown">
                                    <button class="btn btn-link p-0 text-dark" data-bs-toggle="dropdown" aria-expanded="false">
                                        <i class="fas fa-ellipsis-v"></i>
                                    </button>
                                    <ul class="dropdown-menu dropdown-menu-end">
                                        <li><a class="dropdown-item" href="#" onclick="openEditModal(${materiaSinGrupo.materiaId})">Editar</a></li>
                                        <li><a class="dropdown-item" href="#" onclick="openDeleteModal(${materiaSinGrupo.materiaId})">Eliminar</a></li>
                                        <li><a class="dropdown-item" href="#" onclick="openDisableModal(${materiaSinGrupo.materiaId})">Desactivar</a></li>
                                    </ul>
                                </div>
                            </div>
                        </div>

                        <div class="card-body card-body-custom" style="background-color: #e0e0e0">
                            <p class="card-text">${materiaSinGrupo.descripcion || "Sin descripción"}</p>
                        </div>

                        <div class="card-footer card-footer-custom">
                            <button class="btn btn-sm btn-primary" onclick="window.location.href='/Docente/MateriasDetalles?materiaId=${materiaSinGrupo.materiaId}'">Ver Materia</button>
                            <div class="icon-container">
                                <img class="icon-action" src="https://cdn-icons-png.flaticon.com/512/1828/1828817.png" alt="Ver Actividades" title="Ver Actividades" onclick="verActividades(${materiaSinGrupo.materiaId})">
                                <img class="icon-action" src="https://cdn-icons-png.flaticon.com/512/847/847969.png" alt="Ver Integrantes" title="Ver Integrantes" onclick="verIntegrantes(${materiaSinGrupo.materiaId})">
                                <img class="icon-action" src="https://cdn-icons-png.flaticon.com/512/535/535285.png" alt="Destacar" title="Destacar Materia" onclick="destacarGrupo(${materiaSinGrupo.materiaId})">
                            </div>
                        </div>
                    </div>
                `).join('')}
            </div>
        `;
    } else {
        console.error('Error al cargar los grupos.');
    }

    document.getElementById('materiasModal').addEventListener('hidden.bs.modal', function () {
        cargarMateriasSinGrupo(docenteId);
    });
}


// Función para redirigir a la vista Materias dentro del controlador Docente
function irAMateria(materiaId) {

    window.location.href = `/Docente/MateriasDetalles?materiaId=${materiaId}`;
}


//---------------------------------------------------------------------------------------------------------------------------------------------------------------

async function cargarGrupos(docenteId) { // Lógica para actualizar la lista de grupos en vista
    docenteId = 2;
    console.log("DocenteId: ", docenteId); // Muestra el docenteId solo para depuración

    const response = await fetch(`/api/GruposApi/ObtenerGrupos/${docenteId}`);

    if (response.ok) {
        const grupos = await response.json();
        const listaGrupos = document.getElementById("listaGrupos");

        if (grupos.length === 0) {
            listaGrupos.innerHTML = "<p> No hay grupos registrados.</p>";
            return;
        }

        listaGrupos.innerHTML = grupos.map(grupo => `
        <div class="grupo-card " style="background-color: ${grupo.codigoColor || '#FFA500'};  border-radius: 10px;width: 220px; " 
        onmouseover="this.style.transform='translateY(-5px)'; this.style.boxShadow='0px 4px 10px rgba(0, 0, 0, 0.2)';" 
        onmouseout="this.style.transform='none'; this.style.boxShadow='none';">
            <div class="grupo-header">
                <i class="fas fa-clipboard-list"></i>
                <span>${grupo.nombreGrupo}</span>
            </div>
            <p class="grupo-descripcion">${grupo.descripcion || "Sin descripción"}</p>
        </div>
        `).join('');
    } else {
        console.error('Error al cargar los grupos.');
    }

    document.getElementById('gruposModal').addEventListener('hidden.bs.modal', function () {
        cargarGrupos(docenteId);
    });
}





//--------------------------------------------------------------------------------------------------------------------------------------
// NUEVA FUNCIÓN PARA CARGAR EL MODAL DINÁMICAMENTE

document.getElementById("gestionarGruposBtn").addEventListener("click", async function (event) {
    const modalContainer = document.getElementById("modalContainer");

    // Si el modal ya está cargado, no volver a cargarlo
    if (!modalContainer.innerHTML.trim()) {
        event.preventDefault(); // Evitar que el modal intente abrirse antes de que se cargue

        try {
            const response = await fetch('/Docente/GruposModal');

            if (response.ok) {
                const modalHtml = await response.text();
                modalContainer.innerHTML = modalHtml;

                // Ahora que el modal está cargado, forzamos a que se abra
                const modalElement = new bootstrap.Modal(document.getElementById('gruposModal'));
                modalElement.show();
            } else {
                console.error('Error al cargar el modal.');
            }
        } catch (error) {
            console.error('Error al cargar el modal:', error);
        }
    }
    // Si ya está cargado, no evitamos el comportamiento por defecto
});



function verActividades(MateriaId) {
    alert(`Ver actividades del grupo ID: ${MateriaId}`);
    // Aquí puedes redirigir o cargar las actividades relacionadas con el grupo
}

function verIntegrantes(grupoId) {
    alert(`Ver integrantes del grupo ID: ${grupoId}`);
    // Aquí puedes abrir un modal o redirigir para mostrar los integrantes
}

function destacarGrupo(grupoId) {
    alert(`Grupo ID: ${grupoId} marcado como destacado`);
    // Aquí puedes implementar la lógica para destacar el grupo
}



document.addEventListener('DOMContentLoaded', function () {
    cargarGrupos(); // Esta función cargará los grupos automáticamente al abrir el Index
    cargarMateriasSinGrupo(); //Cargara las materias sin grupo asignado al abrir index
}); /*Nos aseguramos de que el contenedor ya está disponible en el DOM 
antes de que el JavaScript intente agregarle contenido. */

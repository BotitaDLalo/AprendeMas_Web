
//usar el claim como variable global dentro del inicio de sesion.
let docenteIdGlobal = null; //Variable global para almacenar el docenteId

//Funcion que busca el claim del docenteId y usarlo en este archivo
async function obtenerDocenteId() {
    try {
        const response = await fetch('/Cuenta/ObtenerDocenteId'); // Llamar al controlador
        const data = await response.json();
        if (data.docenteId) {
            docenteIdGlobal = data.docenteId;
            console.log("DocenteId obtenido:", docenteIdGlobal);
        } else {
            console.error("Error: No se encontró el DocenteId.");
        }
    } catch (error) {
        console.error("Error al obtener el DocenteId:", error);
    }
}

//Guarda las materias en la tabla tbMaterias -------------------
async function guardarMateriaSinGrupo() { 
    const nombre = document.getElementById("nombreMateria").value;
    const descripcion = document.getElementById("descripcionMateria").value;
    const color = "#2196F3";

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
            DocenteId: docenteIdGlobal 
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
//Guarda el grupo con o sin materias enlazadas --------------

async function guardarGrupo() { // Lógica para guardar el grupo
    const nombre = document.getElementById("nombreGrupo").value;
    const descripcion = document.getElementById("descripcionGrupo").value;
    const color = "#2196F3";
    const checkboxes = document.querySelectorAll(".materia-checkbox:checked");

    if (nombre.trim() === '') {
        alert('Ingrese Nombre Del Grupo.');
        return;
    }

    // Obtener IDs de las materias seleccionadas que seran agregadas a el grupo.
    const materiasSeleccionadas = Array.from(checkboxes).map(cb => cb.value);

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
        const grupoCreado = await response.json(); // Obtener el ID del grupo que se acaba de crear para 
                                                    //utilizarlo al crear enlace con materias.

        if (materiasSeleccionadas.length > 0) {
            await asociarMateriasAGrupo(grupoCreado.grupoId, materiasSeleccionadas);
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
        const response = await fetch(`/api/MateriasApi/ObtenerMateriasSinGrupo/${docenteIdGlobal}`); // solo  para checar funcionamiento se agrega la funcion de cargar materias sin grupo
        if (response.ok) {
            const materias = await response.json();
            const contenedorMaterias = document.getElementById("materiasLista");
            contenedorMaterias.innerHTML = ""; // Limpiar antes de agregar

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


// Cargar materias sin grupo -------------------------------------------------------
async function cargarMateriasSinGrupo() {
    console.log("DocenteId: ", docenteIdGlobal);

    const response = await fetch(`/api/MateriasApi/ObtenerMateriasSinGrupo/${docenteIdGlobal}`)
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
                                <img class="icon-action" src="https://cdn-icons-png.flaticon.com/512/535/535285.png" alt="Destacar" title="Destacar Materia" onclick="destacarMateria(${materiaSinGrupo.materiaId})">
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
        cargarMateriasSinGrupo();
    });
}


// Función para redirigir a la vista Materias dentro del controlador Docente
function irAMateria(materiaId) {

    window.location.href = `/Docente/MateriasDetalles?materiaId=${materiaId}`;
}


//Funcion para obtener los grupos de la base de datos. -----------------------------------------------------

async function cargarGrupos() { // Lógica para actualizar la lista de grupos en vista
    const response = await fetch(`/api/GruposApi/ObtenerGrupos/${docenteIdGlobal}`);

    if (response.ok) {
        const grupos = await response.json();
        const listaGrupos = document.getElementById("listaGrupos");

        if (grupos.length === 0) {
            listaGrupos.innerHTML = "<p> No hay grupos registrados.</p>";
            return;
        }

        listaGrupos.innerHTML = grupos.map(grupo => `
        <div class="grupo-card mb-3" style="background-color: ${grupo.codigoColor || '#FFA500'};  
            border-radius: 12px; width: 400px; padding: 15px; margin-bottom: 15px;
            cursor: pointer; transition: all 0.3s ease-in-out;" 
             onmouseover="this.style.transform='translateY(-5px)'; this.style.boxShadow='0px 4px 12px rgba(0, 0, 0, 0.3)';" 
             onmouseout="this.style.transform='none'; this.style.boxShadow='none';"
             onclick="handleCardClick(${grupo.grupoId })">
    
         <div class="d-flex justify-content-between align-items-center">
             <h5 class="text-white mb-0">
                <strong class="font-weight-bold">${grupo.nombreGrupo}</strong> - ${grupo.descripcion || "Sin descripción"}
             </h5>
             
         <div class="dropdown">
             <button class="btn btn-link text-white p-0" type="button" data-bs-toggle="dropdown" aria-expanded="false" onclick="event.stopPropagation();">
                 <i class="fas fa-cog"></i> <!-- Icono de engranaje -->
             </button>
                     <ul class="dropdown-menu dropdown-menu-end">
                     <li><a class="dropdown-item" href="#" onclick="editarGrupo(${grupo.grupoId})">Editar</a></li>
                     <li><a class="dropdown-item" href="#" onclick="eliminarGrupo(${grupo.grupoId})">Eliminar</a></li>
                     <li><a class="dropdown-item" href="#" onclick="desactivarGrupo(${grupo.grupoId})">Desactivar</a></li>
                     </ul>
                 </div>
            </div>
        </div>
        `).join('');
    } else {
        console.error('Error al cargar los grupos.');
    }

    document.getElementById('gruposModal').addEventListener('hidden.bs.modal', function () {
        cargarGrupos();
    });
}



// FUNCIÓN PARA CARGAR EL MODAL DINÁMICAMENTE ------------------------------------------

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


//Funcionalidades de los iconos de las  Cards de materias.----------------------------------------------------------------
function verActividades(MateriaId) {
    alert(`Ver actividades del grupo ID: ${MateriaId}`);
    // Aquí puedes redirigir o cargar las actividades relacionadas con el grupo
}

function verIntegrantes(MateriaId) {
    alert(`Ver integrantes del grupo ID: ${MateriaId}`);
    // Aquí puedes abrir un modal o redirigir para mostrar los integrantes
}

function destacarMateria(MateriaId) {
    alert(`Grupo ID: ${MateriaId} marcado como destacado`);
    // Aquí puedes implementar la lógica para destacar la materia
}
//Funcionalidades de los iconos de las cards de grupos
function handleCardClick(id) {
    console.log("Card clickeada, puedes agregar funcionalidad aquí. ID:", id);
    // Aquí puedes agregar la funcionalidad al dar clic en la card
}

function editarGrupo(id) {
    alert("Editar grupo " + id);
}

function eliminarGrupo(id) {
    alert("Eliminar grupo " + id);
}

function desactivarGrupo(id) {
    alert("Desactivar grupo " + id);
}

// Ejecutar primero la obtención del DocenteId y luego cargar los datos
async function inicializar() {
    await obtenerDocenteId(); // Espera a que el ID se obtenga antes de continuar
    if (docenteIdGlobal) {
        cargarMateriasSinGrupo(docenteIdGlobal);
        cargarGrupos(docenteIdGlobal);
    } else {
        console.error("No se pudo obtener el DocenteId.");
    }
}

//Prioriza la ejecucion al cargar index
// Llamar a la función inicializadora cuando se cargue la página
document.addEventListener("DOMContentLoaded", inicializar);

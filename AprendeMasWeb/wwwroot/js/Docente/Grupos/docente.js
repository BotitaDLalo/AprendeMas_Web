let docenteIdGlobal = null; //Variable global para almacenar el docenteId
let materiasPorCrear = []; // Lista de materias a crear
let intentosAcceder = 0;


//Funcion que obtiene informacion del docente.
async function obtenerDocenteId() {
    try {
        // Hacemos una solicitud para obtener el docenteId desde el servidor
        const response = await fetch('/Cuenta/ObtenerDocenteId'); // Llamar al controlador
        const data = await response.json(); // Convertimos la respuesta en formato JSON
        if (data.docenteId) {
            docenteIdGlobal = data.docenteId; // Guardamos el docenteId en la variable global
            localStorage.setItem("docenteId", docenteIdGlobal); // Guardamos el docenteId en el almacenamiento local

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



// Función para redirigir a la vista Materias dentro del controlador Docente
function irAMateria(materiaIdSeleccionada,seccion = 'avisos') {
    //guardar el id de la materia para acceder a la materia en la que se entro y usarla en otro script
    localStorage.setItem("materiaIdSeleccionada", materiaIdSeleccionada);
    // Redirige a la página de detalles de la materia
    window.location.href = `/Docente/MateriasDetalles?materiaId=${materiaIdSeleccionada}&seccion=${seccion}`;
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
});


// Esperar a que el DOM esté completamente cargado antes de ejecutar el código
document.addEventListener("DOMContentLoaded", function () {

    cargarActividadesEnSelect();
});

//Script para consultar las calificaciones de actividades
async function consultarCalificaciones() {
    const select = document.getElementById("select-actividad");
    const actividadSeleccionada = select.value;
    const textoSeleccionado = select.options[select.selectedIndex].text;

    if (!actividadSeleccionada || select.selectedIndex === 0) {
        await Swal.fire("Seleccione actividad", "Para consultar calificaciones.", "question");
        return;
    }

    Swal.fire({
        title: `Cargando calificaciones de: ${textoSeleccionado}`,
        timerProgressBar: true,
        didOpen: () => {
            Swal.showLoading();
        },
        allowOutsideClick: false,
        allowEscapeKey: false,
        allowEnterKey: false
    });

    try {
        const response = await fetch(`/api/DetallesMateriaApi/ObtenerCalificacionesPorActividad/${actividadSeleccionada}`);
        const datos = await response.json();

        if (!response.ok) {
            throw new Error("Error al obtener las calificaciones");
        }

        const tbody = document.getElementById("tbody-calificaciones");
        tbody.innerHTML = ""; // Limpiar tabla

        datos.forEach(item => {
            const fila = document.createElement("tr");

            // Formato de calificación
            let calificacionFormateada = item.calificacion;
            if (!isNaN(item.calificacion)) {
                calificacionFormateada = `${item.calificacion} / ${item.puntajeMaximo}`;
            }

            fila.innerHTML = `
                <td>${item.nombreCompleto}</td>
                <td>${item.comentarios}</td>
                <td>${calificacionFormateada}</td>
            `;

            // Si NO entregó, aplicar clase especial
            if (item.estatusEntrega === "No entregado") {
                fila.classList.add("bg-danger", "text-white"); // Bootstrap
            }

            tbody.appendChild(fila);
        });

        Swal.close();
        Swal.fire({
            icon: "success",
            title: `Renderizado completado`,
            showConfirmButton: false,
            timer: 1800,
            position: "top-end"
        });

    } catch (error) {
        Swal.fire("Error", error.message, "error");
        console.error("Error:", error);
    }
}




async function cargarActividadesEnSelect() {
    const selectActividad = document.getElementById("select-actividad");
    selectActividad.innerHTML = "<option>Cargando actividades...</option>"; // Mensaje de carga

    try {
        const response = await fetch(`/api/DetallesMateriaApi/ObtenerActividadParaSelect/${materiaIdGlobal}`);
        if (!response.ok) throw new Error("No se encontraron actividades.");

        const actividades = await response.json();
        actividades.reverse(); //cambia orde de muestra
        // Limpiar el select y agregar la opción por defecto
        selectActividad.innerHTML = '<option selected disabled>Selecciona una actividad</option>';

        // Agregar actividades al select
        actividades.forEach(actividad => {
            const option = document.createElement("option");
            option.value = actividad.actividadId;
            option.textContent = actividad.nombreActividad;
            selectActividad.appendChild(option);
        });

    } catch (error) {
        selectActividad.innerHTML = '<option disabled>Error al cargar</option>';
        console.error(error);
    }
}

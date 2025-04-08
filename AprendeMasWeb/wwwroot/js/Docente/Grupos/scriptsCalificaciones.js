// Esperar a que el DOM esté completamente cargado antes de ejecutar el código
document.addEventListener("DOMContentLoaded", function () {

    cargarActividadesEnSelect();
});
//Funcion para consultar actviidades por actividad
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

        // Ordenar por Apellido (Nombre completo alfabéticamente)
        datos.sort((a, b) => a.nombreCompleto.localeCompare(b.nombreCompleto));

        const tbody = document.getElementById("tbody-calificaciones");
        tbody.innerHTML = ""; // Limpiar tabla

        datos.forEach(item => {
            const fila = document.createElement("tr");

            let calificacionFormateada = item.calificacion;
            if (!isNaN(item.calificacion)) {
                calificacionFormateada = `${item.calificacion} / ${item.puntajeMaximo}`;
            }

            let porcentaje = (item.calificacion / item.puntajeMaximo) * 100;
            let bgColor = "";
            let textColor = "#333";

            if (item.estatusEntrega === "No entregado") {
                bgColor = "rgba(231, 76, 60, 0.15)";
                textColor = "#a94442";
            } else if (porcentaje < 50) {
                bgColor = "rgba(241, 196, 15, 0.15)";
            } else if (porcentaje >= 50 && porcentaje <= 75) {
                bgColor = "rgba(241, 196, 15, 0.2)";
            } else {
                bgColor = "rgba(46, 204, 113, 0.15)";
            }

            fila.innerHTML = `
                <td style="font-size: 1.05rem; font-weight: 500; color: ${textColor};">${item.nombreCompleto}</td>
                <td style="font-size: 1.05rem; color: ${textColor};">${item.comentarios}</td>
                <td style="font-size: 1.05rem; font-weight: 600; color: ${textColor};">${calificacionFormateada}</td>
            `;

            fila.style.backgroundColor = bgColor;
            fila.style.transition = "background-color 0.3s ease";

            tbody.appendChild(fila);
        });

        Swal.close();
        Swal.fire({
            icon: "success",
            title: `Renderizado completado`,
            showConfirmButton: false,
            timer: 1500,
            position: "top-end"
        });

    } catch (error) {
        Swal.fire("Error", error.message, "error");
        console.error("Error:", error);
    }
}

//Exportat a excel
function exportarExcel() {
    const select = document.getElementById("select-actividad");
    const actividadSeleccionada = select.value;
    const textoSeleccionado = select.options[select.selectedIndex].text.trim();

    if (!actividadSeleccionada || select.selectedIndex === 0) {
        Swal.fire("Seleccione actividad", "Para exportar calificaciones.", "question");
        return;
    }

    const tabla = document.getElementById("tabla-calificaciones");

    // Verificar si la tabla tiene filas
    const filas = tabla.querySelectorAll("tbody tr");
    if (filas.length === 0) {
        Swal.fire("No hay datos", "La tabla no tiene calificaciones para exportar.", "info");
        return;
    }

    const wb = XLSX.utils.table_to_book(tabla, { sheet: "Calificaciones" });

    // Limpiar el nombre de la actividad por si tiene caracteres inválidos
    const nombreLimpio = textoSeleccionado.replace(/[\\/:*?"<>|]/g, "");

    // Obtener fecha actual en formato YYYY-MM-DD
    const fecha = new Date();
    const fechaFormateada = fecha.toISOString().split('T')[0]; // YYYY-MM-DD

    // Construir el nombre del archivo
    const nombreArchivo = `Calificaciones de ${nombreLimpio} - ${fechaFormateada}.xlsx`;

    XLSX.writeFile(wb, nombreArchivo);
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

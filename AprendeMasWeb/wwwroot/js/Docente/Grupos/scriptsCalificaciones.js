// Esperar a que el DOM esté completamente cargado antes de ejecutar el código
document.addEventListener("DOMContentLoaded", function () {

    cargarActividadesEnSelect();
});

async function consultarCalificaciones() {
    const select = document.getElementById("select-actividad");
    const actividadSeleccionada = select.value;
    const textoSeleccionado = select.options[select.selectedIndex].text;

    if (!actividadSeleccionada || select.selectedIndex === 0) {
        await Swal.fire("Seleccione actividad", "Para consultar calificaciones.", "question");
        return;
    }

    await Swal.fire({
        position: "top-end",
        icon: "success",
        title: `Renderizando: ${textoSeleccionado}`,
        showConfirmButton: false,
        timer: 1900
    });
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

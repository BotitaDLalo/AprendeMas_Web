document.addEventListener("DOMContentLoaded", function () {
    console.log("Detalle de clase cargado...");

    // Verifica que los elementos existen antes de asignar eventos
    const avisosTab = document.getElementById("avisos-tab");
    const actividadesTab = document.getElementById("actividades-tab");
  

    if (avisosTab) {
        avisosTab.addEventListener("click", () => cargarContenido("avisos", "Avisos"));
    }

    if (actividadesTab) {
        actividadesTab.addEventListener("click", () => cargarContenido("actividades", "Actividades"));
    }



    // Cargar Avisos por defecto
    if (avisosTab) {
        cargarContenido("actividades", "Actividades");
        cargarContenido("avisos", "Avisos");
    }
});


function cargarContenido(seccion, nombreVista) {
    console.log(`Cargando ${nombreVista}...`);

    const contenidoDiv = document.getElementById(`contenido${nombreVista}`);

    if (!contenidoDiv) {
        console.error(`El contenedor contenido${nombreVista} no existe.`);
        return;
    }

    // Verifica si ya hay contenido antes de hacer la petición
    if (contenidoDiv.innerHTML.trim() !== "") {
        console.log(`${nombreVista} ya está cargado, no se hace nueva petición.`);
        return;
    }

    fetch(`/Alumno/${nombreVista}`)
        .then(response => response.text())
        .then(html => {
            contenidoDiv.innerHTML = html;

            // Llamar función si se cargaron avisos
            if (nombreVista === "Avisos" && typeof cargarAvisos === "function") {
                cargarAvisos();
            }
        })
        .catch(error => console.error(`Error al cargar ${nombreVista}:`, error));



}



document.querySelectorAll(".nav-link").forEach(tab => {
    tab.addEventListener("click", function () {
        const targetId = this.getAttribute("href").replace("#", "");

        document.querySelectorAll(".tab-pane").forEach(pane => {
            pane.classList.remove("show", "active");
        });

        document.getElementById(targetId).classList.add("show", "active");
        cargarContenido(targetId, targetId.charAt(0).toUpperCase() + targetId.slice(1));
    });
});

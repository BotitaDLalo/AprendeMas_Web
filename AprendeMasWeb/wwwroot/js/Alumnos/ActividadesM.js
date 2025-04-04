document.addEventListener("DOMContentLoaded", async () => {
        const urlParams = new URLSearchParams(window.location.search);
        const nombreMateria = urlParams.get("nombre");

        if (!nombreMateria) {
            document.getElementById("contenedorActividades").innerHTML = "<p>Materia no válida.</p>";
            return;
        }

        try {
            // Obtener el ID de la materia por nombre
            const responseMateria = await fetch(`/api/Materias/ObtenerIdPorNombre?nombre=${encodeURIComponent(nombreMateria)}`);
            const materia = await responseMateria.json();

            if (!materia.materiaId) {
                document.getElementById("contenedorActividades").innerHTML = "<p>No se encontró la materia.</p>";
                return;
            }

            const response = await fetch(`/api/Alumno/Actividades/${materia.materiaId}`);
            const actividades = await response.json();

            if (!Array.isArray(actividades)) {
                document.getElementById("contenedorActividades").innerHTML = `<p>${actividades.mensaje}</p>`;
                return;
            }

            const contenedor = document.getElementById("contenedorActividades");
            contenedor.innerHTML = "";

            actividades.forEach(a => {
                const card = `
                    <div class="card mb-3 shadow">
                        <div class="card-body">
                            <h5 class="card-title">${a.nombreActividad}</h5>
                            <p class="card-text">${a.descripcion}</p>
                            <p class="card-text"><strong>Fecha de entrega:</strong> ${new Date(a.fechaLimite).toLocaleDateString()}</p>
                            <p class="card-text"><strong>Puntaje:</strong> ${a.puntaje}</p>
                            <p class="card-text"><strong>Tipo:</strong> ${a.tipoActividad}</p>
                        </div>
                    </div>
                `;
                contenedor.innerHTML += card;
            });

        } catch (error) {
            console.error("Error al cargar actividades:", error);
            document.getElementById("contenedorActividades").innerHTML = "<p>Error al cargar actividades.</p>";
        }
    });
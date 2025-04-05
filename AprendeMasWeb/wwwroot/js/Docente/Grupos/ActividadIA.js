const apiKey = "AIzaSyAkcRmqwYgV1M4cr5bOHh0symB38KP8yMY";
const apiUrl = `https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=${apiKey}`;

function mostrarOpcionesSugerencias(texto) {
    const opciones = texto.split(/\d\./).filter(op => op.trim());
    let html = '';

    opciones.forEach((op, index) => {
        html += `
        <div class="list-group-item d-flex align-items-start">
            <div class="me-3 mt-1"> <!-- Contenedor del radio -->
                <input class="form-check-input" type="radio" 
                       name="opcionDescripcion" id="opcion${index}" 
                       value="${op.trim().replace(/"/g, '&quot;')}">
            </div>
            <div class="w-100"> <!-- Contenedor del texto -->
                <label class="form-check-label fw-normal" for="opcion${index}">
                    ${op.trim()}
                </label>
            </div>
        </div>`;
    });

    document.getElementById('sugerenciasLista').innerHTML = html ||
        '<p class="text-muted">No se recibieron sugerencias válidas</p>';
}

document.getElementById('btnSugerencias').addEventListener('click', async () => {
    const nombre = document.getElementById('nombre').value;
    const descripcion = document.getElementById('descripcion').value;

    // Mostrar spinner de Bootstrap
    document.getElementById('sugerenciasLista').innerHTML = `
        <div class="text-center py-4">
            <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Cargando...</span>
            </div>
            <p class="mt-2">Generando sugerencias...</p>
        </div>`;

    try {
        const sugerencias = await obtenerRecomendaciones(nombre, descripcion);
        mostrarOpcionesSugerencias(sugerencias);
    } catch (error) {
        document.getElementById('sugerenciasLista').innerHTML = `
            <div class="alert alert-danger">
                Error al generar sugerencias: ${error.message}
            </div>`;
    }
});


async function obtenerRecomendaciones(nombre, descripcion) {
    const prompt = `A partir de la siguiente actividad: Título: "${nombre}", Descripción: "${descripcion}", 
                    genera solo y unicamente tres versiones mejoradas de la descripción, mas claras, completas, bien estructuraras
                    y principalmente en forma de instrucciones dirigidas al alumno. 
                    Cada versión debe ser clara, completa y bien estructurada, con un lenguaje directo y comprensible. 
                    Evita explicaciones en tercera persona. En su lugar, utiliza frases como "Resuelve", "Realiza", "Contesta", etc., 
                    para que el alumno entienda exactamente qué debe hacer. 
                    Devuelve SOLO y unicamente  las tres versiones numeradas (1, 2 y 3), sin texto adicional.
                    Y si desde el parecer del titulo y descripcion suena como si se esperara una respuesta rapido decir al final de cada descripcion:
                    Contesta directamente en la plataforma
                    y si suenan como una "informe", "ensayo", "investigación", "análisis", "resumen", "presentación", "trabajo", "reporte", "cuento", "historia" similar
                    incluir en la descripcion :
                    agregar link del archivo 
                    Si no hay título y descripción, muestra este aviso: Se necesita un título y descripción para mostrar sugerencias.`;


    const requestBody = {
        contents: [{
            parts: [{
                text: prompt
            }]
        }]
    };

    try {
        const response = await fetch(apiUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(requestBody)
        });

        if (!response.ok) {
            throw new Error(`Error HTTP! estado: ${response.status}`);
        }

        const data = await response.json();
        console.log("Respuesta completa:", data);

        // Extrae el texto de la respuesta
        const text = data.candidates[0].content.parts[0].text;
        return limpiarTexto(text);

    } catch (error) {
        console.error("Error:", error);
        return "⚠️ Error al obtener sugerencias. Inténtalo de nuevo.";
    }
}

// FUNCIÓN PARA LIMPIAR TEXTO
function limpiarTexto(texto) {
    return texto
        .replace(/\*\*/g, '')
        .replace(/^"|"$/g, '')
        .replace(/<br>/g, '\n')
        .replace(/\n+/g, '\n')
        .trim();
}

document.getElementById('btnAplicarSugerencia').addEventListener('click', function () {
    const seleccionado = document.querySelector('input[name="opcionDescripcion"]:checked');
    const descripcionTextarea = document.getElementById('descripcion');

    if (seleccionado && descripcionTextarea) {
        descripcionTextarea.value = seleccionado.value;
        bootstrap.Modal.getInstance('#sugerenciasModal').hide();
    } else {
        alert('¡Por favor selecciona una opción antes de continuar!');
    }
});

async function guardarMateria() {
    const nombre = document.getElementById("nombreMateria").value;
    const descripcion = document.getElementById("descripcionMateria").value;
    const color = document.getElementById("codigoColorMateria").value;

    const response = await fetch('/api/Materias/CrearMateriaSinGrupo', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ NombreMateria: nombre, Descripcion: descripcion, CodigoColor: color })
    });

    if (response.ok) {
        alert('Materia guardada con éxito.');
        document.getElementById("materiasForm").reset();
        // Recargar la lista de materias...
    } else {
        alert('Error al guardar la materia.');
    }
}

async function guardarGrupo() {
    const nombre = document.getElementById("nombreGrupo").value;
    const descripcion = document.getElementById("descripcionGrupo").value;
    const color = document.getElementById("codigoColorGrupo").value;

    const response = await fetch('/api/Grupos/CrearGrupo', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ NombreGrupo: nombre, Descripcion: descripcion, CodigoColor: color })
    });

    if (response.ok) {
        alert('Grupo guardado con éxito.');
        document.getElementById("gruposForm").reset();
        // Recargar la lista de grupos...
    } else {
        alert('Error al guardar el grupo.');
    }
}

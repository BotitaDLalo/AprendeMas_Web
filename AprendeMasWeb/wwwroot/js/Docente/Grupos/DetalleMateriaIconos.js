function copiarCodigoAcceso() {
    const codigoElemento = document.getElementById("codigoAcceso");
    const materiaElemento = document.getElementById("materiaNombre");

    const codigo = codigoElemento.innerText;
    const nombreMateria = materiaElemento.innerText;

    const textoCopiar = `Únete a mi materia "${nombreMateria}" con el siguiente código de acceso: ${codigo}`;

    // Crear un input temporal para copiar el texto completo
    const inputTemp = document.createElement("input");
    document.body.appendChild(inputTemp);
    inputTemp.value = textoCopiar;
    inputTemp.select();
    document.execCommand("copy");
    document.body.removeChild(inputTemp);

    // Cambiar temporalmente el ícono para indicar que se copió
    const icono = document.querySelector(".copiar-icono");
    icono.classList.remove("fa-copy");
    icono.classList.add("fa-check");

    setTimeout(() => {
        icono.classList.remove("fa-check");
        icono.classList.add("fa-copy");
    }, 2000); // Volver al ícono de copiar después de 2 segundos
}

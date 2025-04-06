let intentoReconectar = false;

window.addEventListener("offline", () => {
    // Al detectar desconexión, mostramos una alerta pero no cerramos la sesión
    Swal.fire({
        title: "Parece que se ha perdido la conexión.",
        text: "Estamos esperando a que te reconectes para continuar.",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Intentar reconectar",
        cancelButtonText: "Cerrar sesión",
    }).then((result) => {
        if (result.isConfirmed) {
            intentoReconectar = true;
            // Aquí podrías intentar reconectar y ejecutar una función de reconexión si es necesario
        } else {
            // Si el usuario cancela la reconexión, entonces cerramos sesión
            AlertaCierreSesion();
        }
    });
});

window.addEventListener("online", async () => {
    // Si el usuario se reconecta, intentamos obtener el docenteId
    if (intentoReconectar) {
        await obtenerDocenteId();  // Reintentar obtener el docenteId
        intentoReconectar = false;  // Restablecer la variable
        Swal.fire({
            title: "Conexión restaurada",
            text: "La conexión a Internet se ha restablecido.",
            icon: "success",
            timer: 1500,
        });
    }
});

// Función para obtener docenteId y manejar la reconexión
async function obtenerDocenteId() {
    try {
        const response = await fetch('/Cuenta/ObtenerDocenteId');
        const data = await response.json();
        if (data.docenteId) {
            docenteIdGlobal = data.docenteId;
            localStorage.setItem("docenteId", docenteIdGlobal);
        } else {
            throw new Error("No se pudo obtener el docenteId.");
        }
    } catch (error) {
        // Si no se puede obtener el docenteId (por ejemplo, por problemas de conexión)
        AlertaCierreSesion();
    }
}

// Función de cierre de sesión
function AlertaCierreSesion() {
    Swal.fire({
        title: "Ocurrio un error al iniciar sesión.",
        text: "La cerraremos por seguridad y podrás volver a iniciar sesión.",
        icon: "error",
        timer: 5000,
        timerProgressBar: true,
        didOpen: () => {
            Swal.showLoading();
        },
        willClose: () => {
            cerrarSesion();
        }
    });
}

// Función para cerrar sesión
async function cerrarSesion() {
    try {
        const response = await fetch('/Cuenta/CerrarSesion', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            }
        });
        if (response.ok) {
            window.location.href = "/Cuenta/IniciarSesion";
        } else {
            Swal.fire("Error", "No se pudo cerrar sesión.", "error");
        }
    } catch (error) {
        Swal.fire("Error", "No se pudo cerrar sesión debido a un error.", "error");
    }
}


//Funcion que detecta errores generales
function alertaDeErroresGenerales(error) {
    // Mensaje de error por defecto
    let mensajeError = "Ocurrió un error inesperado.";

    // Si el error tiene un mensaje, lo usamos
    if (error && error.message) {
        mensajeError = error.message;
    }

    // Enlace para enviar un correo con el error incluido en el cuerpo
    const enlaceCorreo = `mailto:soporte@siexasistemas.com?subject=Error%20en%20la%20aplicación
        &body=Hola,%20tengo%20un%20problema%20en%20la%20aplicación.%0A%0ADetalles%20del%20error:%0A${encodeURIComponent(mensajeError)}
        %0A%0APor%20favor,%20ayuda.`.replace(/\s+/g, ''); // Limpia espacios innecesarios

    // Mostrar alerta
    Swal.fire({
        icon: "error",
        title: "Oops...",
        text: mensajeError,
        position: "center",
        allowOutsideClick: false//, // Evita que se cierre con un clic afuera
        //footer: `<a href="${enlaceCorreo}" target="_blank">Si el problema persiste, contáctanos.</a>`
    });
}



function mostrarCargando(titulo = "Cargando...") {
    Swal.fire({
        title: titulo,
        timerProgressBar: true,
        didOpen: () => {
            Swal.showLoading();
        },
        allowOutsideClick: false,
        allowEscapeKey: false,
        allowEnterKey: false
    });
}

function cerrarCargando() {
    Swal.close();
}
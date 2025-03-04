// Variable global para almacenar el alumnoId
let alumnoIdGlobal = null;

// Función para obtener el alumnoId desde el servidor
async function obtenerAlumnoId() {
    try {
        console.log("Obteniendo alumno ID...");

        // Hacer la petición al servidor para obtener el ID del alumno
        const response = await fetch('/Cuenta/ObtenerAlumnoId');
        const data = await response.json();

        if (data.alumnoId) {
            alumnoIdGlobal = data.alumnoId; // Guardamos el ID en la variable global
            localStorage.setItem("alumnoId", alumnoIdGlobal); // También lo guardamos en localStorage
            console.log("Alumno ID obtenido:", alumnoIdGlobal);

            Swal.fire({
                position: "top-end",
                icon: "success",
                title: "Inicio de sesión correcto.",
                showConfirmButton: false,
                position: "center",
                timer: 2500
            });

            return alumnoIdGlobal;
        } else {
            // Si no obtenemos el ID, forzamos cierre de sesión
            manejarErrorSesion();
            return null;
        }
    } catch (error) {
        console.error("Error al obtener el ID del alumno:", error);
        manejarErrorSesion();
        return null;
    }
}

// Función para manejar error en la sesión
function manejarErrorSesion() {
    let timerInterval;
    Swal.fire({
        title: "Parece que se perdió la conexión con tu sesión.",
        html: "La cerraremos por seguridad y podrás volver a iniciar sesión en: <b></b>.",
        timer: 5000,
        timerProgressBar: true,
        allowOutsideClick: false,
        position: "center",
        didOpen: () => {
            Swal.showLoading();
            const timer = Swal.getPopup().querySelector("b");
            timerInterval = setInterval(() => {
                timer.textContent = `${Math.floor(Swal.getTimerLeft() / 1000)} segundos`;
            }, 100);
        },
        willClose: () => {
            clearInterval(timerInterval);
            cerrarSesion();
        }
    }).then((result) => {
        if (result.dismiss === Swal.DismissReason.timer) {
            console.log("Cerrando sesión automáticamente.");
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
            console.log("Sesión cerrada correctamente.");
            window.location.href = "/Cuenta/IniciarSesion";
        } else {
            Swal.fire({
                icon: "error",
                title: "Oops...",
                text: "No se pudo cerrar sesión.",
                allowOutsideClick: false,
                position: "center"  
            });
        }
    } catch (error) {
        Swal.fire({
            icon: "error",
            title: "Oops...",
            text: "No se pudo cerrar sesión.",
            allowOutsideClick: false,
            position: "center"
        });
    }
}

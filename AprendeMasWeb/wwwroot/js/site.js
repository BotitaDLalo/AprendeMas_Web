// Espera a que el documento esté completamente cargado antes de ejecutar el código
document.addEventListener("DOMContentLoaded", function () {
    const logoutForm = document.getElementById("logoutForm"); // Obtiene el formulario de cierre de sesión por su ID

    logoutForm.addEventListener("submit", function (event) {
        event.preventDefault();

        Swal.fire({
            title: "¿Seguro que quieres cerrar sesión?",
            text: "Tendrás que volver a iniciar sesión para continuar.",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#d33",
            cancelButtonColor: "#3085d6",
            confirmButtonText: "Sí, cerrar sesión",
            cancelButtonText: "Cancelar"
        }).then((result) => {
            if (result.isConfirmed) {
                cerrarSesionFormal(); // nueva función real con loader y limpieza de localstorage
            }
        });
    });

});

// Función para cerrar sesión
async function cerrarSesionFormal() {
    try {
        Swal.fire({
            title: 'Cerrando sesión...',
            text: 'Por favor espera.',
            allowOutsideClick: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });

        const response = await fetch('/Cuenta/CerrarSesion', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            }
        });

        if (response.ok) {
            localStorage.clear(); // Limpia el almacenamiento
            window.location.href = "/Cuenta/IniciarSesion";
        } else {
            Swal.fire({
                icon: "error",
                title: "Oops...",
                text: "No se pudo cerrar sesión.",
                position: "center",
                allowOutsideClick: false
            });
        }
    } catch (error) {
        Swal.close();
        alertaDeErroresGenerales(error);
    }
}


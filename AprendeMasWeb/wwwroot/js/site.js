// Espera a que el documento esté completamente cargado antes de ejecutar el código
document.addEventListener("DOMContentLoaded", function () {
    const logoutForm = document.getElementById("logoutForm"); // Obtiene el formulario de cierre de sesión por su ID

    logoutForm.addEventListener("submit", function (event) {
        event.preventDefault(); // Evita que el formulario se envíe inmediatamente

        // Muestra una alerta de confirmación antes de cerrar sesión
        Swal.fire({
            title: "¿Seguro que quieres cerrar sesión?", // Título de la alerta
            text: "Tendrás que volver a iniciar sesión para continuar.", // Mensaje informativo
            icon: "warning", // Tipo de alerta (advertencia)
            showCancelButton: true, // Muestra botón de cancelar
            confirmButtonColor: "#d33", // Color del botón de confirmación
            cancelButtonColor: "#3085d6", // Color del botón de cancelar
            confirmButtonText: "Sí, cerrar sesión", // Texto del botón de confirmación
            cancelButtonText: "Cancelar" // Texto del botón de cancelar
        }).then((result) => {
            if (result.isConfirmed) { // Si el usuario confirma cerrar sesión
                let timerInterval;

                // Muestra una segunda alerta con cuenta regresiva antes de cerrar sesión
                Swal.fire({
                    title: "Se está cerrando sesión.", // Mensaje de cierre de sesión
                    html: "Por seguridad, serás enviado al inicio de sesión", // Mensaje sin contador de tiempo
                    timer: 5000, // Tiempo en milisegundos antes de cerrar sesión (5s)
                    timerProgressBar: true, // Muestra una barra de progreso en la alerta
                    allowOutsideClick: false, // Evita que se cierre al hacer clic fuera de la alerta
                    didOpen: () => {
                        Swal.showLoading(); // Muestra un indicador de carga
                    },
                    willClose: () => {
                        cerrarSesion(); // Llama a la función para cerrar sesión
                    }
                }).then((result) => {
                    if (result.dismiss === Swal.DismissReason.timer) {
                        console.log("Cerrando sesión."); // Mensaje en la consola cuando se cierra la sesión automáticamente
                    }
                });

            }
        });
    });
});

// 🔹 Función para cerrar sesión
async function cerrarSesion() {
    try {
        // Realiza una solicitud POST al endpoint de cierre de sesión
        const response = await fetch('/Cuenta/CerrarSesion', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded', // Especifica el tipo de contenido
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value // Obtiene el token de verificación CSRF
            }
        });

        if (response.ok) {
            console.log("Sesión cerrada correctamente."); // Mensaje en consola indicando que la sesión se cerró con éxito
            window.location.href = "/Cuenta/IniciarSesion"; // Redirige al usuario a la página de inicio de sesión
        } else {
            // En caso de error en la respuesta del servidor, muestra un mensaje de alerta con SweetAlert2
            Swal.fire({
                icon: "error",
                title: "Oops...",
                text: "No se pudo cerrar sesión.",
                position: "center",
                allowOutsideClick: false//,// Evita que la alerta se cierre al hacer clic fuera de ella
                // footer: '<a href="mailto:soporte@tuempresa.com?subject=Problema%20con%20cierre%20de%20sesión&body=Hola,%20tengo%20un%20problema%20al%20cerrar%20sesión.%20Por%20favor,%20ayuda." target="_blank">Si el problema persiste, contáctanos.</a>'
            });
        }
    } catch (error) {
        // Captura cualquier error inesperado (por ejemplo, problemas de conexión) y muestra una alerta
        alertaDeErroresGenerales(error);
    }
}

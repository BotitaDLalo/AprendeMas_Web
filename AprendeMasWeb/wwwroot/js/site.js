//// Espera a que el documento esté completamente cargado antes de ejecutar el código
//document.addEventListener("DOMContentLoaded", function () {
//    const logoutForm = document.getElementById("logoutForm"); // Obtiene el formulario de cierre de sesión por su ID

//    logoutForm.addEventListener("submit", function (event) {
//        event.preventDefault(); // Evita que el formulario se envíe inmediatamente

//        // Muestra una alerta de confirmación antes de cerrar sesión
//        Swal.fire({
//            title: "¿Seguro que quieres cerrar sesión?", // Título de la alerta
//            text: "Tendrás que volver a iniciar sesión para continuar.", // Mensaje informativo
//            icon: "warning", // Tipo de alerta (advertencia)
//            showCancelButton: true, // Muestra botón de cancelar
//            confirmButtonColor: "#d33", // Color del botón de confirmación
//            cancelButtonColor: "#3085d6", // Color del botón de cancelar
//            confirmButtonText: "Sí, cerrar sesión", // Texto del botón de confirmación
//            cancelButtonText: "Cancelar" // Texto del botón de cancelar
//        }).then((result) => {
//            if (result.isConfirmed) { // Si el usuario confirma cerrar sesión
//                let timerInterval;

//                // Muestra una segunda alerta con cuenta regresiva antes de cerrar sesión
//                Swal.fire({
//                    title: "Se está cerrando sesión.", // Mensaje de cierre de sesión
//                    html: "Por seguridad, serás enviado al inicio de sesión en: <b></b>.", // Mensaje con contador de tiempo
//                    timer: 5000, // Tiempo en milisegundos antes de cerrar sesión (5s)
//                    timerProgressBar: true, // Muestra una barra de progreso en la alerta
//                    allowOutsideClick: false, // Evita que se cierre al hacer clic fuera de la alerta
//                    didOpen: () => {
//                        Swal.showLoading(); // Muestra un indicador de carga
//                        const timer = Swal.getPopup().querySelector("b"); // Encuentra el elemento donde se mostrará el tiempo restante
//                        timerInterval = setInterval(() => {
//                            timer.textContent = `${Math.floor(Swal.getTimerLeft() / 1000)} segundos`; // Actualiza el contador en segundos
//                        }, 100);
//                    },
//                    willClose: () => {
//                        clearInterval(timerInterval); // Detiene el contador cuando la alerta se cierra
//                        cerrarSesion(); // Llama a la función para cerrar sesión
//                    }
//                }).then((result) => {
//                    if (result.dismiss === Swal.DismissReason.timer) {
//                        console.log("Cerrando sesión."); // Mensaje en la consola cuando se cierra la sesión automáticamente
//                    }
//                });
//            }
//        });
//    });
//});

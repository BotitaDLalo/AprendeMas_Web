// Función para alternar la visibilidad de la contraseña
function togglePassword() {
    var passwordInput = document.getElementById("Password"); // Obtiene el campo de contraseña
    if (passwordInput.type === "password") {
        passwordInput.type = "text"; // Si está en modo "password", lo cambia a "text" para mostrar la contraseña
    } else {
        passwordInput.type = "password"; // Si está en modo "text", lo cambia a "password" para ocultarla
    }
}

// Agrega un event listener al ícono de ojo para alternar la visibilidad de la contraseña
document.getElementById("togglePassword").addEventListener("click", function () {
    const passwordInput = document.getElementById("Password"); // Obtiene el campo de contraseña
    const icon = this.querySelector("i"); // Obtiene el ícono dentro del botón de mostrar/ocultar

    if (passwordInput.type === "password") {
        passwordInput.type = "text"; // Muestra la contraseña
        icon.classList.remove("fa-eye"); // Quita el ícono de ojo abierto
        icon.classList.add("fa-eye-slash"); // Agrega el ícono de ojo tachado
    } else {
        passwordInput.type = "password"; // Oculta la contraseña
        icon.classList.remove("fa-eye-slash"); // Quita el ícono de ojo tachado
        icon.classList.add("fa-eye"); // Agrega el ícono de ojo abierto
    }
});

// Función para validar la contraseña mientras el usuario escribe
function validatePassword() {
    const password = document.getElementById("Password").value; // Obtiene el valor del campo de contraseña

    // Reglas de validación de la contraseña
    const uppercaseRule = /[A-Z]/.test(password); // Verifica si contiene al menos una letra mayúscula
    const specialRule = /[!@#$%^&*(),.?":{}|<>]/.test(password); // Verifica si contiene un carácter especial
    const numberRule = /[0-9]/.test(password); // Verifica si contiene al menos un número
    const lengthRule = password.length >= 8; // Verifica si tiene al menos 8 caracteres

    // Actualiza visualmente las reglas según se cumplan o no
    updateRule("rule-uppercase", uppercaseRule); // Regla de mayúscula
    updateRule("rule-special", specialRule); // Regla de carácter especial
    updateRule("rule-number", numberRule); // Regla de número
    updateRule("rule-length", lengthRule); // Regla de longitud

    // Cambia el estilo del input dependiendo de si la contraseña es válida o no
    const input = document.getElementById("Password");
    if (uppercaseRule && specialRule && numberRule && lengthRule) {
        input.classList.add("valid"); // Agrega la clase de válido si todas las reglas se cumplen
        input.classList.remove("invalid"); // Quita la clase de inválido
    } else {
        input.classList.add("invalid"); // Agrega la clase de inválido si falta alguna regla
        input.classList.remove("valid"); // Quita la clase de válido
    }
}

// Función para actualizar visualmente cada regla de validación
function updateRule(ruleId, isValid) {
    const ruleElement = document.getElementById(ruleId); // Obtiene el elemento que muestra la regla
    if (isValid) {
        ruleElement.classList.add("valid"); // Si la regla se cumple, agrega la clase "valid"
    } else {
        ruleElement.classList.remove("valid"); // Si no se cumple, quita la clase "valid"
    }
}

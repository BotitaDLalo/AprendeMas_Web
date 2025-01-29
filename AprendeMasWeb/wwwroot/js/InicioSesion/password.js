function togglePassword() {
    var passwordInput = document.getElementById("Password");
    if (passwordInput.type === "password") {
        passwordInput.type = "text";
    } else {
        passwordInput.type = "password";
    }
}

document.getElementById("togglePassword").addEventListener("click", function () {
    const passwordInput = document.getElementById("Password");
    const icon = this.querySelector("i");
    if (passwordInput.type === "password") {
        passwordInput.type = "text";
        icon.classList.remove("fa-eye");
        icon.classList.add("fa-eye-slash");
    } else {
        passwordInput.type = "password";
        icon.classList.remove("fa-eye-slash");
        icon.classList.add("fa-eye");
    }
});


function validatePassword() {
    const password = document.getElementById("Password").value;

    // Reglas de validación
    const uppercaseRule = /[A-Z]/.test(password); // Al menos una mayúscula
    const specialRule = /[!@#$%^&*(),.?":{}|<>]/.test(password); // Carácter especial
    const numberRule = /[0-9]/.test(password); // Al menos un número
    const lengthRule = password.length >= 8; // Longitud mínima

    // Actualizar reglas dinámicamente
    updateRule("rule-uppercase", uppercaseRule);
    updateRule("rule-special", specialRule);
    updateRule("rule-number", numberRule);
    updateRule("rule-length", lengthRule);

    // Cambiar el borde del input según validez general
    const input = document.getElementById("Password");
    if (uppercaseRule && specialRule && numberRule && lengthRule) {
        input.classList.add("valid");
        input.classList.remove("invalid");
    } else {
        input.classList.add("invalid");
        input.classList.remove("valid");
    }
}

function updateRule(ruleId, isValid) {
    const ruleElement = document.getElementById(ruleId);
    if (isValid) {
        ruleElement.classList.add("valid"); // Oculta la regla cumplida
    } else {
        ruleElement.classList.remove("valid"); // Muestra la regla si no está cumplida
    }
}

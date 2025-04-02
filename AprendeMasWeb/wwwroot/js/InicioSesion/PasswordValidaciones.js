// Expresión regular para validar la contraseña
const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{8,}$/;

document.getElementById('password').addEventListener('input', function () {
    const password = this.value;

    // Validar cada criterio de la contraseña y ocultarlo si se cumple
    document.getElementById('length').classList.toggle('completed', password.length >= 8);
    document.getElementById('uppercase').classList.toggle('completed', /[A-Z]/.test(password));
    document.getElementById('lowercase').classList.toggle('completed', /[a-z]/.test(password));
    document.getElementById('number').classList.toggle('completed', /\d/.test(password));
    document.getElementById('special').classList.toggle('completed', /[@$!%*?&#]/.test(password));

    // Si la contraseña cumple con los requisitos, se marca como válida
    if (passwordRegex.test(password)) {
        this.classList.add('is-valid');
        this.classList.remove('is-invalid');
    } else {
        this.classList.add('is-invalid');
        this.classList.remove('is-valid');
    }
});

// Validar antes de enviar el formulario
document.getElementById('registerButton').addEventListener('click', function (event) {
    const passwordField = document.getElementById('password');

    if (!passwordRegex.test(passwordField.value)) {
        passwordField.classList.add('is-invalid');
        passwordField.classList.remove('is-valid');
        event.preventDefault(); // Evita que se envíe el formulario si la contraseña no es válida
    }
});

// Función para mostrar/ocultar la contraseña
function togglePassword() {
    const passwordField = document.getElementById('password');
    const eyeIcon = document.getElementById('eyeIcon');
    if (passwordField.type === 'password') {
        passwordField.type = 'text';
        eyeIcon.classList.remove('fa-eye');
        eyeIcon.classList.add('fa-eye-slash');
    } else {
        passwordField.type = 'password';
        eyeIcon.classList.remove('fa-eye-slash');
        eyeIcon.classList.add('fa-eye');
    }
}
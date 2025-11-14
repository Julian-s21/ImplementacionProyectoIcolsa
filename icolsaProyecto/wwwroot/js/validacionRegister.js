// ==============================
// VALIDACIÓN DE REGISTRO DE USUARIO
// ==============================
document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("registerForm");

    const nombre = document.getElementById("Nombre_Usuario");
    const correo = document.getElementById("Correo_Usuario");
    const pass = document.getElementById("Contrasena_Usuario");
    const confirmPass = document.getElementById("Confirmar_Contrasena");

    const confirmPassError = document.getElementById("ConfirmPassError");

    const contrasenasInseguras = [
        "123456", "password", "qwerty", "abc123", "contraseña", "admin", "usuario",
        "12345678", "111111", "123123", "000000", "iloveyou", "1234", "welcome", "letmein"
    ];

    const dominiosProhibidos = [
        "tempmail.com", "mailinator.com", "guerrillamail.com", "10minutemail.com",
        "yopmail.com", "trashmail.com", "fakeinbox.com"
    ];

    // ==============================
    // Evaluar fuerza de la contraseña
    // ==============================
    const evaluarFuerza = (val) => {
        let score = 0;
        if (val.length >= 6) score += 1;
        if (val.length >= 10) score += 1;
        if (/[A-Z]/.test(val)) score += 1;
        if (/[a-z]/.test(val)) score += 1;
        if (/[0-9]/.test(val)) score += 1;
        if (/[!@#$%^&*(),.?":{}|<>_\-]/.test(val)) score += 1;
        return score;
    };

    const actualizarBarraFuerza = (val) => {
        const barra = document.getElementById("passStrength");
        const texto = document.getElementById("strengthText");
        const score = evaluarFuerza(val);

        barra.style.width = `${(score / 6) * 100}%`;

        if (score <= 2) {
            barra.className = "progress-bar bg-danger";
            texto.textContent = "Muy débil";
        } else if (score <= 4) {
            barra.className = "progress-bar bg-warning";
            texto.textContent = "Media";
        } else {
            barra.className = "progress-bar bg-success";
            texto.textContent = "Fuerte";
        }
    };

    // ==============================
    // VALIDACIONES GENERALES
    // ==============================
    form.addEventListener("submit", function (e) {
        e.preventDefault();
        let errores = [];

        // Limpiar estados previos
        [nombre, correo, pass, confirmPass].forEach(i => i.classList.remove("is-invalid"));
        confirmPassError.textContent = "";

        // -----------------------
        // VALIDACIÓN NOMBRE
        // -----------------------
        const nombreVal = nombre.value.trim();
        if (!nombreVal) {
            errores.push("El nombre es obligatorio.");
            nombre.classList.add("is-invalid");
        } else if (nombreVal.length < 2) {
            errores.push("El nombre debe tener al menos 2 caracteres.");
            nombre.classList.add("is-invalid");
        } else if (/[^a-zA-ZáéíóúÁÉÍÓÚñÑ\s]/.test(nombreVal)) {
            errores.push("El nombre solo puede contener letras y espacios.");
            nombre.classList.add("is-invalid");
        }

        // -----------------------
        // VALIDACIÓN CORREO
        // -----------------------
        const correoVal = correo.value.trim();
        if (!correoVal) {
            errores.push("El correo electrónico es obligatorio.");
            correo.classList.add("is-invalid");
        } else {
            const [parteUsuario, parteDominio] = correoVal.split("@");
            if (!parteDominio || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(correoVal)) {
                errores.push("Formato de correo inválido.");
                correo.classList.add("is-invalid");
            } else if (dominiosProhibidos.some(d => correoVal.toLowerCase().endsWith(d))) {
                errores.push("No se permiten correos temporales.");
                correo.classList.add("is-invalid");
            }
        }

        // -----------------------
        // VALIDACIÓN CONTRASEÑA
        // -----------------------
        const passVal = pass.value;
        if (!passVal.trim()) {
            errores.push("La contraseña es obligatoria.");
            pass.classList.add("is-invalid");
        } else {
            if (passVal.length < 6) {
                errores.push("La contraseña debe tener al menos 6 caracteres.");
                pass.classList.add("is-invalid");
            } else {
                if (!/[A-Za-z]/.test(passVal)) errores.push("Debe contener al menos una letra.");
                if (!/[0-9]/.test(passVal)) errores.push("Debe incluir al menos un número.");
                if (!/[!@#$%^&*(),.?\":{}|<>_\-]/.test(passVal)) errores.push("Debe incluir un carácter especial.");
                if (contrasenasInseguras.includes(passVal.toLowerCase())) errores.push("Contraseña demasiado común.");
            }
        }

        // -----------------------
        // VALIDAR CONFIRMAR CONTRASEÑA
        // -----------------------
        const confirmVal = confirmPass.value;
        if (!confirmVal.trim()) {
            errores.push("Debes confirmar tu contraseña.");
            confirmPass.classList.add("is-invalid");
            confirmPassError.textContent = "Campo obligatorio.";
        } else if (passVal !== confirmVal) {
            errores.push("Las contraseñas no coinciden.");
            confirmPass.classList.add("is-invalid");
            confirmPassError.textContent = "No coinciden.";
        }

        actualizarBarraFuerza(passVal);

        // -----------------------
        // MOSTRAR ERRORES
        // -----------------------
        if (errores.length > 0) {
            Swal.fire({
                icon: 'error',
                title: 'Errores de validación',
                html: errores.map(e => `<p class='mb-1'>❌ ${e}</p>`).join(''),
                confirmButtonColor: '#6f42c1',
                width: 500,
                scrollbarPadding: false
            });
            return;
        }

        // Si todo es correcto
        Swal.fire({
            title: 'Registrando usuario...',
            text: 'Por favor espera mientras validamos tus datos.',
            icon: 'info',
            showConfirmButton: false,
            timer: 1500,
            timerProgressBar: true
        }).then(() => {
            form.submit();
        });
    });

    // ==============================
    // VALIDACIÓN EN TIEMPO REAL
    // ==============================
    [nombre, correo, pass, confirmPass].forEach(input => {
        input.addEventListener("input", () => {
            input.classList.remove("is-invalid");
            if (input.id === "Confirmar_Contrasena") confirmPassError.textContent = "";
        });
    });

    pass.addEventListener("input", () => actualizarBarraFuerza(pass.value));
});

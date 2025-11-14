document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("loginForm");
    if (!form) return;

    const correo = document.getElementById("Correo_Usuario");
    const pass = document.getElementById("Contrasena_Usuario");
    const strengthBar = document.getElementById("passStrength");
    const strengthText = document.getElementById("strengthText");
    const togglePass = document.getElementById("togglePass");

    const dominiosProhibidos = [
        "tempmail.com", "mailinator.com", "guerrillamail.com",
        "10minutemail.com", "yopmail.com", "trashmail.com", "fakeinbox.com"
    ];

    // ========================
    // Mostrar / ocultar pass
    // ========================
    togglePass.addEventListener("click", () => {
        const icon = togglePass.querySelector("i");
        const isPassword = pass.type === "password";
        pass.type = isPassword ? "text" : "password";
        icon.classList.toggle("bi-eye");
        icon.classList.toggle("bi-eye-slash");
    });

    // ========================
    // Fuerza de contraseña
    // ========================
    const evaluarFuerza = (val) => {
        let score = 0;
        if (val.length >= 6) score++;
        if (/[A-Z]/.test(val)) score++;
        if (/[a-z]/.test(val)) score++;
        if (/[0-9]/.test(val)) score++;
        if (/[!@#$%^&*(),.?":{}|<>_\-]/.test(val)) score++;
        return score;
    };

    const actualizarBarra = (val) => {
        const score = evaluarFuerza(val);
        strengthBar.style.width = `${(score / 5) * 100}%`;

        if (score <= 2) {
            strengthBar.className = "progress-bar bg-danger";
            strengthText.textContent = "Débil";
        } else if (score === 3) {
            strengthBar.className = "progress-bar bg-warning";
            strengthText.textContent = "Media";
        } else {
            strengthBar.className = "progress-bar bg-success";
            strengthText.textContent = "Fuerte";
        }
    };

    pass.addEventListener("input", () => actualizarBarra(pass.value));

    // ========================
    // Limpieza en tiempo real
    // ========================
    [correo, pass].forEach(i => {
        i.addEventListener("input", () => i.classList.remove("is-invalid", "is-valid"));
    });

    // ========================
    // Validar formulario
    // ========================
    form.addEventListener("submit", (e) => {
        e.preventDefault();
        let errores = [];

        correo.classList.remove("is-invalid");
        pass.classList.remove("is-invalid");

        const correoVal = correo.value.trim();
        const passVal = pass.value.trim();

        // --- correo ---
        if (!correoVal) {
            errores.push("El correo electrónico es obligatorio.");
            correo.classList.add("is-invalid");
        } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(correoVal)) {
            errores.push("Formato de correo inválido.");
            correo.classList.add("is-invalid");
        } else if (dominiosProhibidos.some(d => correoVal.toLowerCase().endsWith(d))) {
            errores.push("No se permiten correos temporales.");
            correo.classList.add("is-invalid");
        } else {
            correo.classList.add("is-valid");
        }

        // --- contraseña ---
        if (!passVal) {
            errores.push("La contraseña es obligatoria.");
            pass.classList.add("is-invalid");
        } else if (passVal.length < 6) {
            errores.push("La contraseña debe tener al menos 6 caracteres.");
            pass.classList.add("is-invalid");
        } else {
            pass.classList.add("is-valid");
        }

        // --- mostrar errores ---
        if (errores.length > 0) {
            Swal.fire({
                icon: 'error',
                title: 'Errores de validación',
                html: errores.map(e => `<p class='mb-1'>❌ ${e}</p>`).join(''),
                confirmButtonColor: '#6f42c1',
                width: 450,
            });
            return;
        }

        // --- enviar si todo OK ---
        Swal.fire({
            title: 'Verificando credenciales...',
            text: 'Por favor espera un momento.',
            icon: 'info',
            showConfirmButton: false,
            timer: 1500,
            timerProgressBar: true
        }).then(() => {
            form.submit();
        });
    });
});

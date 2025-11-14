// ==============================
// VALIDACIÓN DE RECUPERAR CONTRASEÑA
// ==============================
document.addEventListener("DOMContentLoaded", () => {
    const form = document.querySelector("form[asp-action='ForgotPassword']");
    const correo = document.getElementById("correo");
    const nueva = document.getElementById("nueva");
    const confirmar = document.getElementById("confirmar");
    const barra = document.getElementById("passStrength");
    const texto = document.getElementById("strengthText");

    const contrasenasInseguras = [
        "123456", "password", "qwerty", "abc123", "contraseña", "admin", "usuario",
        "12345678", "111111", "123123", "000000", "iloveyou", "welcome", "letmein"
    ];

    const dominiosProhibidos = [
        "tempmail.com", "mailinator.com", "guerrillamail.com", "10minutemail.com",
        "yopmail.com", "trashmail.com", "fakeinbox.com"
    ];

    // ==============================
    // EVALUAR FUERZA DE CONTRASEÑA
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
        const score = evaluarFuerza(val);
        barra.style.width = `${(score / 6) * 100}%`;

        if (!val.trim()) {
            barra.className = "progress-bar bg-secondary";
            texto.textContent = "Escribe una contraseña segura";
            return;
        }

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
    // VALIDACIÓN PRINCIPAL
    // ==============================
    form.addEventListener("submit", (e) => {
        e.preventDefault();
        let errores = [];

        // Limpiar clases previas
        [correo, nueva, confirmar].forEach(i => i.classList.remove("is-invalid"));

        // -----------------------
        // VALIDAR CORREO
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
        // VALIDAR NUEVA CONTRASEÑA
        // -----------------------
        const nuevaVal = nueva.value.trim();
        const correoUsuario = correo.value.split("@")[0]?.toLowerCase() || "";

        if (!nuevaVal) {
            errores.push("La nueva contraseña es obligatoria.");
            nueva.classList.add("is-invalid");
        } else {
            if (nuevaVal.length < 6) errores.push("Debe tener al menos 6 caracteres.");
            if (!/[A-Za-z]/.test(nuevaVal)) errores.push("Debe contener al menos una letra.");
            if (!/[A-Z]/.test(nuevaVal)) errores.push("Debe incluir una letra mayúscula.");
            if (!/[a-z]/.test(nuevaVal)) errores.push("Debe incluir una letra minúscula.");
            if (!/[0-9]/.test(nuevaVal)) errores.push("Debe incluir al menos un número.");
            if (!/[!@#$%^&*(),.?\":{}|<>_\-]/.test(nuevaVal)) errores.push("Debe incluir un carácter especial.");
            if (/\s/.test(nuevaVal)) errores.push("No debe contener espacios en blanco.");
            if (/(\w)\1{2,}/.test(nuevaVal)) errores.push("Evita repetir el mismo carácter varias veces seguidas.");
            if (/1234|abcd|qwerty/i.test(nuevaVal)) errores.push("Evita secuencias comunes como '1234' o 'abcd'.");
            if (nuevaVal.toLowerCase().includes(correoUsuario)) errores.push("No debe incluir tu correo o parte de él.");
            if (contrasenasInseguras.includes(nuevaVal.toLowerCase())) errores.push("Contraseña demasiado común.");
        }

        // -----------------------
        // VALIDAR CONFIRMAR CONTRASEÑA
        // -----------------------
        const confirmarVal = confirmar.value.trim();
        if (!confirmarVal) {
            errores.push("Debes confirmar tu contraseña.");
            confirmar.classList.add("is-invalid");
        } else if (nuevaVal !== confirmarVal) {
            errores.push("Las contraseñas no coinciden.");
            confirmar.classList.add("is-invalid");
        }

        actualizarBarraFuerza(nuevaVal);

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

        // -----------------------
        // ÉXITO
        // -----------------------
        Swal.fire({
            title: 'Procesando...',
            text: 'Estamos verificando tu información.',
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
    [correo, nueva, confirmar].forEach(input => {
        input.addEventListener("input", () => {
            input.classList.remove("is-invalid");
        });
    });

    nueva.addEventListener("input", () => actualizarBarraFuerza(nueva.value));
});

// Auth Modal JavaScript
document.addEventListener("DOMContentLoaded", function () {
    const showLoginBtn = document.getElementById("showLogin");
    const showRegisterBtn = document.getElementById("showRegister");
    const loginContainer = document.getElementById("loginFormContainer");
    const registerContainer = document.getElementById("registerFormContainer");

    if (showLoginBtn && loginContainer) {
        showLoginBtn.addEventListener("click", function () {
            loginContainer.classList.remove("d-none");
            if (registerContainer) registerContainer.classList.add("d-none");
        });
    }

    if (showRegisterBtn && registerContainer) {
        showRegisterBtn.addEventListener("click", function () {
            if (loginContainer) loginContainer.classList.add("d-none");
            registerContainer.classList.remove("d-none");
        });
    }
});
window.showToast = function (message, type = "success") {
    const toastEl = document.getElementById("toastMessage");
    const toastBody = document.getElementById("toastBody");
    const toastIcon = document.getElementById("toastIcon");

    if (!toastEl || !toastBody || !toastIcon) {
        console.warn("Toast elements not found in DOM.");
        return;
    }

    toastBody.innerText = message;

    let bgClass = "bg-success";
    let iconClass = "bi-check-circle";

    switch (type) {
        case "error":
            bgClass = "bg-danger";
            iconClass = "bi-exclamation-circle";
            break;
        case "info":
            bgClass = "bg-primary";
            iconClass = "bi-info-circle";
            break;
        case "warning":
            bgClass = "bg-warning text-dark";
            iconClass = "bi-exclamation-triangle";
            break;
    }

    toastEl.className = `toast align-items-center border-0 text-white ${bgClass}`;
    toastIcon.className = `bi fs-5 ${iconClass}`;

    const toast = new bootstrap.Toast(toastEl);
    toast.show();
};

$(document).ready(function () {
    $('#contactForm').submit(function (e) {
        e.preventDefault();

        const formData = {
            name: $('input[name="Name"]').val(),
            email: $('input[name="Email"]').val(),
            message: $('textarea[name="Message"]').val()
        };

        $.ajax({
            type: "POST",
            url: "/Contact/Send",
            contentType: "application/json",
            data: JSON.stringify(formData),
            success: function (response) {
                showToast("✅ " + response.message, true);
                $('#contactForm')[0].reset();
            },
            error: function (xhr) {
                const err = xhr.responseJSON?.message || " Failed to send. Please try again.";
                showToast(err, false);
            }
        });
    });

    function showToast(message, isSuccess) {
        const toastEl = $('#toastMessage');
        const toastBody = $('#toastBody');
        const toastIcon = $('#toastIcon');

        toastBody.text(message);
        toastEl.removeClass('bg-success bg-danger');
        toastEl.addClass(isSuccess ? 'bg-success' : 'bg-danger');
        toastIcon.removeClass('bi-check-circle-fill bi-exclamation-triangle-fill');
        toastIcon.addClass(isSuccess ? 'bi-check-circle-fill' : 'bi-exclamation-triangle-fill');

        const toast = new bootstrap.Toast(toastEl[0]);
        toast.show();
    }
});

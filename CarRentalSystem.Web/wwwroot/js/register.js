$('#ajaxRegisterForm').submit(function (e) {
    e.preventDefault();
    var formData = new FormData(this);

    // Get CSRF token
    var token = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: '/Account/RegisterCustomerAjax',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        headers: {
            'RequestVerificationToken': token 
        },
        success: function (response) {
            $('#registerError').html('');
            $('#registerModal').modal('hide');
            showToast("Registration successful!", "success");
            $('#ajaxRegisterForm')[0].reset();
            location.reload();
        },
        error: function (xhr) {
            let errMsg = "";
            if (xhr.responseJSON && xhr.responseJSON.errors) {
                errMsg = xhr.responseJSON.errors.join("<br/>");
            } else if (xhr.responseJSON && xhr.responseJSON.message) {
                errMsg = xhr.responseJSON.message;
            } else {
                errMsg = "Something went wrong!";
            }
            $('#registerError').html(errMsg);
        }
    });
});

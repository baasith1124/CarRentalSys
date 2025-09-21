$('#ajaxRegisterForm').submit(function (e) {
    e.preventDefault();
    
    // Get form data
    var formData = new FormData(this);
    var registrationData = {
        FullName: formData.get('FullName'),
        Email: formData.get('Email'),
        Password: formData.get('Password'),
        ConfirmPassword: formData.get('ConfirmPassword')
    };

    // Validate form data
    if (!registrationData.FullName || !registrationData.Email || !registrationData.Password || !registrationData.ConfirmPassword) {
        $('#registerError').html('All fields are required.');
        return;
    }

    if (registrationData.Password !== registrationData.ConfirmPassword) {
        $('#registerError').html('Passwords do not match.');
        return;
    }

    // Send OTP for registration
    sendRegistrationOTP(registrationData);
});

function sendRegistrationOTP(registrationData) {
    var token = $('input[name="__RequestVerificationToken"]').val();

    // Debug logging
    console.log('Registration data:', registrationData);
    console.log('CSRF Token:', token);

    var requestData = {
        email: registrationData.Email,
        purpose: 'Registration'
        // __RequestVerificationToken: token // Temporarily disabled for testing
    };

    console.log('Request data being sent:', requestData);

    $.ajax({
        url: '/Account/SendRegistrationOTP',
        type: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        data: JSON.stringify(requestData),
        success: function (response) {
            console.log('OTP Response:', response);
            if (response.success) {
                $('#registerError').html('');
                $('#registerModal').modal('hide');

                // Show OTP verification modal
                if (typeof showOTPVerificationModal === 'function') {
                    showOTPVerificationModal(registrationData.Email, registrationData);
                } else {
                    showToast("OTP sent to your email. Please check and enter the code.", "info");
                }
            } else {
                $('#registerError').html(response.message || 'Failed to send OTP. Please try again.');
            }
        },
        error: function (xhr) {
            console.log('AJAX Error:', xhr);
            console.log('Status:', xhr.status);
            console.log('Response:', xhr.responseText);
            
            let errMsg = "Failed to send OTP. Please try again.";
            if (xhr.responseJSON && xhr.responseJSON.message) {
                errMsg = xhr.responseJSON.message;
            } else if (xhr.responseJSON && xhr.responseJSON.errors) {
                errMsg = Array.isArray(xhr.responseJSON.errors) 
                    ? xhr.responseJSON.errors.join(', ') 
                    : xhr.responseJSON.errors;
            }
            
            console.log('Error message to display:', errMsg);
            $('#registerError').html(errMsg);
        }
    });
}

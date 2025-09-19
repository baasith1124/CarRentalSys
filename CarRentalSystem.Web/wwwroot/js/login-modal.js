// Login Modal JavaScript
document.addEventListener("DOMContentLoaded", function () {
    // Handle login form submission
    const loginForm = document.getElementById('loginForm');
    if (loginForm) {
        loginForm.addEventListener('submit', function(e) {
            e.preventDefault();
            
            const formData = new FormData(this);
            
            fetch('/Account/Login', {
                method: 'POST',
                body: formData
            })
            .then(response => {
                if (response.ok) {
                    // Close modal and reload page to show authenticated state
                    const modal = bootstrap.Modal.getInstance(document.getElementById('authModal'));
                    if (modal) modal.hide();
                    window.location.reload();
                } else {
                    // Handle login error
                    return response.text().then(text => {
                        throw new Error('Login failed');
                    });
                }
            })
            .catch(error => {
                alert('Login failed. Please check your credentials.');
            });
        });
    }

    // Handle switch to register
    const switchToRegister = document.getElementById('switchToRegister');
    if (switchToRegister) {
        switchToRegister.addEventListener('click', function(e) {
            e.preventDefault();
            const showRegisterBtn = document.getElementById('showRegister');
            if (showRegisterBtn) showRegisterBtn.click();
        });
    }
});
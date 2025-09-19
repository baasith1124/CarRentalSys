// Login Modal JavaScript Functionality

function toggleLoginPassword() {
    const field = document.querySelector('#loginModal input[name="Password"]');
    const toggle = field.parentElement.querySelector('.password-toggle-3d i');
    
    if (field.type === 'password') {
        field.type = 'text';
        toggle.className = 'bi bi-eye-slash';
    } else {
        field.type = 'password';
        toggle.className = 'bi bi-eye';
    }
}

// Enhanced form validation with 3D effects
document.addEventListener('DOMContentLoaded', function() {
    const form = document.getElementById('ajaxLoginForm');
    if (!form) return;
    
    const inputs = form.querySelectorAll('.form-control-3d');
    
    inputs.forEach(input => {
        input.addEventListener('blur', function() {
            validateLoginField(this);
        });
        
        input.addEventListener('input', function() {
            clearLoginFieldError(this);
        });
    });
    
    // Form submission handling
    form.addEventListener('submit', function(e) {
        e.preventDefault();
        
        let isValid = true;
        inputs.forEach(input => {
            if (!validateLoginField(input)) {
                isValid = false;
            }
        });
        
        if (isValid) {
            submitLoginForm();
        }
    });
    
    function validateLoginField(field) {
        const value = field.value.trim();
        const fieldName = field.name;
        let isValid = true;
        let errorMessage = '';
        
        // Remove existing error styling
        field.classList.remove('is-invalid');
        const errorElement = field.parentElement.querySelector('.validation-error-3d');
        if (errorElement) {
            errorElement.textContent = '';
        }
        
        // Validation rules
        if (!value) {
            isValid = false;
            errorMessage = `${getLoginFieldLabel(fieldName)} is required.`;
        } else if (fieldName === 'Email' && !isValidEmail(value)) {
            isValid = false;
            errorMessage = 'Please enter a valid email address.';
        } else if (fieldName === 'Password' && value.length < 6) {
            isValid = false;
            errorMessage = 'Password must be at least 6 characters long.';
        }
        
        if (!isValid) {
            field.classList.add('is-invalid');
            if (errorElement) {
                errorElement.textContent = errorMessage;
            }
        }
        
        return isValid;
    }
    
    function clearLoginFieldError(field) {
        field.classList.remove('is-invalid');
        const errorElement = field.parentElement.querySelector('.validation-error-3d');
        if (errorElement) {
            errorElement.textContent = '';
        }
    }
    
    function getLoginFieldLabel(fieldName) {
        const labels = {
            'Email': 'Email',
            'Password': 'Password'
        };
        return labels[fieldName] || fieldName;
    }
    
    function isValidEmail(email) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    }
    
    function submitLoginForm() {
        const formData = new FormData(form);
        const submitButton = form.querySelector('.btn-login-3d');
        const errorDisplay = document.getElementById('loginError');
        
        // Show loading state
        const originalText = submitButton.querySelector('.btn-text').textContent;
        submitButton.querySelector('.btn-text').textContent = 'Signing In...';
        submitButton.disabled = true;
        
        // Clear previous errors
        if (errorDisplay) {
            errorDisplay.style.display = 'none';
            errorDisplay.textContent = '';
        }
        
        fetch('/Account/Login', {
            method: 'POST',
            body: formData,
            headers: {
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            }
        })
        .then(response => {
            if (response.ok) {
                // Success - redirect to dashboard or home
                window.location.href = '/Dashboard';
            } else {
                return response.text().then(text => {
                    // Try to parse as JSON for error messages
                    try {
                        const data = JSON.parse(text);
                        throw new Error(data.message || 'Login failed');
                    } catch (e) {
                        throw new Error('Invalid email or password');
                    }
                });
            }
        })
        .catch(error => {
            console.error('Login error:', error);
            showLoginErrors([error.message || 'Login failed. Please try again.']);
        })
        .finally(() => {
            // Reset button state
            submitButton.querySelector('.btn-text').textContent = originalText;
            submitButton.disabled = false;
        });
    }
    
    function showLoginErrors(errors) {
        const errorDisplay = document.getElementById('loginError');
        if (errorDisplay) {
            errorDisplay.innerHTML = errors.map(error => `<div>${error}</div>`).join('');
            errorDisplay.style.display = 'block';
        }
    }
});

// Modal event handlers
document.addEventListener('DOMContentLoaded', function() {
    const loginModal = document.getElementById('loginModal');
    if (loginModal) {
        // Reset form when modal is hidden
        loginModal.addEventListener('hidden.bs.modal', function() {
            const form = document.getElementById('ajaxLoginForm');
            if (form) {
                form.reset();
                // Clear all validation errors
                const errorElements = form.querySelectorAll('.validation-error-3d');
                errorElements.forEach(element => {
                    element.textContent = '';
                });
                const invalidFields = form.querySelectorAll('.is-invalid');
                invalidFields.forEach(field => {
                    field.classList.remove('is-invalid');
                });
                // Hide error display
                const errorDisplay = document.getElementById('loginError');
                if (errorDisplay) {
                    errorDisplay.style.display = 'none';
                }
            }
        });
        
        // Focus first input when modal is shown
        loginModal.addEventListener('shown.bs.modal', function() {
            const firstInput = loginModal.querySelector('.form-control-3d');
            if (firstInput) {
                firstInput.focus();
            }
        });
    }
});

// Global variable to store car ID for booking context
window.selectedCarId = null;

// Global function to show registration modal (changed from login modal for better UX)
function showLoginModal(carId = null) {
    // Store car ID for booking context
    window.selectedCarId = carId;
    
    // Show registration modal directly for better conversion
    const modal = new bootstrap.Modal(document.getElementById('registerModal'));
    modal.show();
}

// Global function to show register modal (for switching between modals)
function showRegisterModal() {
    const loginModal = bootstrap.Modal.getInstance(document.getElementById('loginModal'));
    if (loginModal) {
        loginModal.hide();
    }
    
    // Show register modal if it exists
    const registerModal = document.getElementById('registerModal');
    if (registerModal) {
        const modal = new bootstrap.Modal(registerModal);
        modal.show();
    }
}

// Handle forgot password link
document.addEventListener('DOMContentLoaded', function() {
    const forgotLink = document.querySelector('.forgot-link-3d');
    if (forgotLink) {
        forgotLink.addEventListener('click', function(e) {
            e.preventDefault();
            // You can implement forgot password functionality here
            alert('Forgot password functionality will be implemented soon!');
        });
    }
});

// Handle Google login
function handleGoogleLogin(event) {
    // Close the modal before redirecting to Google
    const loginModal = bootstrap.Modal.getInstance(document.getElementById('loginModal'));
    if (loginModal) {
        loginModal.hide();
    }
    
    // Show loading message
    showGoogleLoginLoading();
    
    // Allow the default navigation to proceed
    // The user will be redirected to Google OAuth and then back to ExternalLoginCallback
}

function showGoogleLoginLoading() {
    // Create a loading overlay
    const loadingOverlay = document.createElement('div');
    loadingOverlay.id = 'google-login-loading';
    loadingOverlay.style.cssText = `
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background: rgba(0, 0, 0, 0.8);
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        z-index: 9999;
        backdrop-filter: blur(10px);
    `;
    
    loadingOverlay.innerHTML = `
        <div style="
            background: var(--glass-bg-dark);
            border: 1px solid var(--glass-border-light);
            border-radius: var(--radius-2xl);
            padding: 3rem;
            text-align: center;
            box-shadow: var(--shadow-3d-strong);
            backdrop-filter: var(--glass-backdrop-strong);
        ">
            <div style="
                width: 80px;
                height: 80px;
                background: linear-gradient(135deg, #4285F4, #34A853, #FBBC05, #EA4335);
                border-radius: 50%;
                display: flex;
                align-items: center;
                justify-content: center;
                margin: 0 auto 1.5rem;
                font-size: 2rem;
                color: white;
                animation: iconFloat 2s ease-in-out infinite;
            ">
                <i class="bi bi-google"></i>
            </div>
            <h3 style="color: white; margin-bottom: 1rem; font-family: var(--font-heading);">
                Redirecting to Google...
            </h3>
            <p style="color: rgba(255, 255, 255, 0.8); margin-bottom: 0;">
                Please complete authentication with Google
            </p>
            <div style="
                width: 40px;
                height: 40px;
                border: 3px solid rgba(255, 255, 255, 0.3);
                border-top: 3px solid #4285F4;
                border-radius: 50%;
                animation: spin 1s linear infinite;
                margin: 1.5rem auto 0;
            "></div>
        </div>
        <style>
            @keyframes spin {
                0% { transform: rotate(0deg); }
                100% { transform: rotate(360deg); }
            }
        </style>
    `;
    
    document.body.appendChild(loadingOverlay);
    
    // Remove loading overlay after 5 seconds (in case something goes wrong)
    setTimeout(() => {
        const overlay = document.getElementById('google-login-loading');
        if (overlay) {
            overlay.remove();
        }
    }, 5000);
}
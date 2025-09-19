// Registration Modal JavaScript Functionality

function togglePassword(fieldId) {
    const field = document.querySelector(`input[name="${fieldId}"]`);
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
    const form = document.getElementById('ajaxRegisterForm');
    if (!form) return;
    
    const inputs = form.querySelectorAll('.form-control-3d');
    
    inputs.forEach(input => {
        input.addEventListener('blur', function() {
            validateField(this);
        });
        
        input.addEventListener('input', function() {
            clearFieldError(this);
        });
    });
    
    // Form submission handling
    form.addEventListener('submit', function(e) {
        e.preventDefault();
        
        let isValid = true;
        inputs.forEach(input => {
            if (!validateField(input)) {
                isValid = false;
            }
        });
        
        if (isValid) {
            submitRegistrationForm();
        }
    });
    
    function validateField(field) {
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
            errorMessage = `${getFieldLabel(fieldName)} is required.`;
        } else if (fieldName === 'Email' && !isValidEmail(value)) {
            isValid = false;
            errorMessage = 'Please enter a valid email address.';
        } else if (fieldName === 'Password' && value.length < 6) {
            isValid = false;
            errorMessage = 'Password must be at least 6 characters long.';
        } else if (fieldName === 'ConfirmPassword') {
            const password = document.querySelector('input[name="Password"]').value;
            if (value !== password) {
                isValid = false;
                errorMessage = 'Passwords do not match.';
            }
        }
        
        if (!isValid) {
            field.classList.add('is-invalid');
            if (errorElement) {
                errorElement.textContent = errorMessage;
            }
        }
        
        return isValid;
    }
    
    function clearFieldError(field) {
        field.classList.remove('is-invalid');
        const errorElement = field.parentElement.querySelector('.validation-error-3d');
        if (errorElement) {
            errorElement.textContent = '';
        }
    }
    
    function getFieldLabel(fieldName) {
        const labels = {
            'FullName': 'Full Name',
            'Email': 'Email',
            'Password': 'Password',
            'ConfirmPassword': 'Confirm Password'
        };
        return labels[fieldName] || fieldName;
    }
    
    function isValidEmail(email) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    }
    
    function submitRegistrationForm() {
        const formData = new FormData(form);
        const submitButton = form.querySelector('.btn-register-3d');
        const errorDisplay = document.getElementById('registerError');
        
        // Show loading state
        const originalText = submitButton.querySelector('.btn-text').textContent;
        submitButton.querySelector('.btn-text').textContent = 'Creating Account...';
        submitButton.disabled = true;
        
        // Clear previous errors
        if (errorDisplay) {
            errorDisplay.style.display = 'none';
            errorDisplay.textContent = '';
        }
        
        fetch('/Account/RegisterCustomerAjax', {
            method: 'POST',
            body: formData,
            headers: {
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            }
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // Success - show success message and close modal
                showSuccessMessage('Account created successfully! You can now book cars.');
                setTimeout(() => {
                    const modal = bootstrap.Modal.getInstance(document.getElementById('registerModal'));
                    if (modal) {
                        modal.hide();
                    }
                    // Stay on current page and refresh to show logged-in state
                    window.location.reload();
                }, 2000);
            } else {
                // Show errors
                showErrors(data.errors || ['Registration failed. Please try again.']);
            }
        })
        .catch(error => {
            console.error('Registration error:', error);
            showErrors(['An error occurred. Please try again.']);
        })
        .finally(() => {
            // Reset button state
            submitButton.querySelector('.btn-text').textContent = originalText;
            submitButton.disabled = false;
        });
    }
    
    function showErrors(errors) {
        const errorDisplay = document.getElementById('registerError');
        if (errorDisplay) {
            errorDisplay.innerHTML = errors.map(error => `<div>${error}</div>`).join('');
            errorDisplay.style.display = 'block';
        }
    }
    
    function showSuccessMessage(message = 'Account created successfully! Redirecting...') {
        const errorDisplay = document.getElementById('registerError');
        if (errorDisplay) {
            errorDisplay.innerHTML = `<div style="color: #00FF88;"><i class="bi bi-check-circle me-2"></i>${message}</div>`;
            errorDisplay.style.display = 'block';
        }
    }
});

// Modal event handlers
document.addEventListener('DOMContentLoaded', function() {
    const registerModal = document.getElementById('registerModal');
    if (registerModal) {
        // Reset form when modal is hidden
        registerModal.addEventListener('hidden.bs.modal', function() {
            const form = document.getElementById('ajaxRegisterForm');
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
                const errorDisplay = document.getElementById('registerError');
                if (errorDisplay) {
                    errorDisplay.style.display = 'none';
                }
            }
        });
        
        // Focus first input when modal is shown
        registerModal.addEventListener('shown.bs.modal', function() {
            const firstInput = registerModal.querySelector('.form-control-3d');
            if (firstInput) {
                firstInput.focus();
            }
        });
    }
});

// Global function to show register modal
function showRegisterModal() {
    const modal = new bootstrap.Modal(document.getElementById('registerModal'));
    modal.show();
}

// Global function to show login modal (for switching between modals)
function showLoginModal() {
    const registerModal = bootstrap.Modal.getInstance(document.getElementById('registerModal'));
    if (registerModal) {
        registerModal.hide();
    }
    
    // Show login modal if it exists
    const loginModal = document.getElementById('loginModal');
    if (loginModal) {
        const modal = new bootstrap.Modal(loginModal);
        modal.show();
    }
}

// Handle Google login (shared function)
function handleGoogleLogin(event) {
    // Close the modal before redirecting to Google
    const registerModal = bootstrap.Modal.getInstance(document.getElementById('registerModal'));
    if (registerModal) {
        registerModal.hide();
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

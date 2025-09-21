// OTP Verification Modal JavaScript
class OTPVerification {
    constructor() {
        this.otpInputs = [];
        this.currentIndex = 0;
        this.countdownTimer = null;
        this.countdownSeconds = 60;
        this.userEmail = '';
        this.registrationData = null;
        this.init();
    }

    init() {
        this.setupOTPInputs();
        this.setupEventListeners();
    }

    setupOTPInputs() {
        this.otpInputs = document.querySelectorAll('.otp-input');
        
        this.otpInputs.forEach((input, index) => {
            input.addEventListener('input', (e) => this.handleInput(e, index));
            input.addEventListener('keydown', (e) => this.handleKeyDown(e, index));
            input.addEventListener('paste', (e) => this.handlePaste(e));
        });
    }

    setupEventListeners() {
        // Form submission
        const form = document.getElementById('otpVerificationForm');
        if (form) {
            form.addEventListener('submit', (e) => {
                e.preventDefault();
                this.verifyOTP();
            });
        }

        // Resend OTP button
        const resendBtn = document.getElementById('resendOTPBtn');
        if (resendBtn) {
            resendBtn.addEventListener('click', () => {
                this.resendOTP();
            });
        }

        // Modal events
        const modal = document.getElementById('otpVerificationModal');
        if (modal) {
            modal.addEventListener('hidden.bs.modal', () => {
                this.resetModal();
            });
        }
    }

    handleInput(e, index) {
        const value = e.target.value;
        
        // Only allow numbers
        if (!/^\d*$/.test(value)) {
            e.target.value = '';
            return;
        }

        if (value.length === 1) {
            e.target.classList.add('filled');
            this.moveToNext(index);
        } else if (value.length === 0) {
            e.target.classList.remove('filled');
        }

        this.updateCompleteOTP();
    }

    handleKeyDown(e, index) {
        // Handle backspace
        if (e.key === 'Backspace' && e.target.value === '') {
            this.moveToPrevious(index);
        }
        
        // Handle arrow keys
        if (e.key === 'ArrowLeft') {
            this.moveToPrevious(index);
        } else if (e.key === 'ArrowRight') {
            this.moveToNext(index);
        }
    }

    handlePaste(e) {
        e.preventDefault();
        const pastedData = e.clipboardData.getData('text').replace(/\D/g, '').slice(0, 6);
        
        if (pastedData.length === 6) {
            pastedData.split('').forEach((digit, index) => {
                if (this.otpInputs[index]) {
                    this.otpInputs[index].value = digit;
                    this.otpInputs[index].classList.add('filled');
                }
            });
            this.updateCompleteOTP();
            this.otpInputs[5].focus();
        }
    }

    moveToNext(currentIndex) {
        if (currentIndex < this.otpInputs.length - 1) {
            this.otpInputs[currentIndex + 1].focus();
        }
    }

    moveToPrevious(currentIndex) {
        if (currentIndex > 0) {
            this.otpInputs[currentIndex - 1].focus();
        }
    }

    updateCompleteOTP() {
        const completeOTP = Array.from(this.otpInputs)
            .map(input => input.value)
            .join('');
        
        const completeOTPInput = document.getElementById('completeOTP');
        if (completeOTPInput) completeOTPInput.value = completeOTP;
    }

    showModal(email, registrationData) {
        this.userEmail = email;
        this.registrationData = registrationData;
        
        // Update subtitle with email
        document.querySelector('.otp-subtitle-3d').textContent = 
            `Enter the 6-digit code sent to ${email}`;
        
        // Show modal
        const modal = new bootstrap.Modal(document.getElementById('otpVerificationModal'));
        modal.show();
        
        // Focus first input
        setTimeout(() => {
            this.otpInputs[0].focus();
        }, 500);
        
        // Start countdown
        this.startCountdown();
    }

    startCountdown() {
        this.countdownSeconds = 60;
        const countdownElement = document.getElementById('countdown');
        const resendBtn = document.getElementById('resendOTPBtn');
        
        if (!countdownElement || !resendBtn) return;
        
        resendBtn.disabled = true;
        resendBtn.textContent = 'Resend Code';
        
        this.countdownTimer = setInterval(() => {
            this.countdownSeconds--;
            countdownElement.textContent = this.countdownSeconds;
            
            if (this.countdownSeconds <= 0) {
                clearInterval(this.countdownTimer);
                resendBtn.disabled = false;
                document.querySelector('.otp-timer').style.display = 'none';
            }
        }, 1000);
    }

    async resendOTP() {
        try {
            const token = $('input[name="__RequestVerificationToken"]').val();
            
            const response = await fetch('/Account/SendRegistrationOTP', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    email: this.userEmail,
                    purpose: 'Registration'
                    // __RequestVerificationToken: token  // Temporarily disabled for testing
                })
            });

            const result = await response.json();
            
            if (result.success) {
                this.showSuccess('New OTP sent successfully!');
                this.startCountdown();
                document.querySelector('.otp-timer').style.display = 'block';
            } else {
                this.showError(result.message || 'Failed to resend OTP. Please try again.');
            }
        } catch (error) {
            console.error('Error resending OTP:', error);
            this.showError('Network error. Please check your connection and try again.');
        }
    }

    async verifyOTP() {
        const otpCode = Array.from(this.otpInputs).map(input => input.value).join('');
        
        if (otpCode.length !== 6) {
            this.showError('Please enter a complete 6-digit OTP code.');
            return;
        }

        // Disable submit button
        const submitBtn = document.querySelector('.btn-verify-otp');
        if (submitBtn) {
            submitBtn.disabled = true;
            submitBtn.innerHTML = '<span class="btn-content-3d"><i class="bi bi-hourglass-split me-2"></i><span class="btn-text">Verifying...</span></span>';
        }

        try {
            console.log('Verifying OTP:', {
                email: this.userEmail,
                code: otpCode,
                purpose: 'Registration'
            });

            // Step 1: Verify OTP first
            const verifyResponse = await fetch('/Account/VerifyRegistrationOTP', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    email: this.userEmail,
                    code: otpCode,
                    purpose: 'Registration'
                })
            });

            console.log('OTP Verification Response Status:', verifyResponse.status);

            if (!verifyResponse.ok) {
                throw new Error(`HTTP error! status: ${verifyResponse.status}`);
            }

            const responseText = await verifyResponse.text();
            console.log('OTP Verification Response Text:', responseText);

            let verifyResult;
            try {
                verifyResult = JSON.parse(responseText);
            } catch (parseError) {
                console.error('Failed to parse JSON response:', parseError);
                console.error('Raw response:', responseText);
                throw new Error('Invalid JSON response from server');
            }

            console.log('OTP Verification Result:', verifyResult);

            if (!verifyResult.success) {
                this.showError(verifyResult.message || 'Invalid OTP. Please try again.');
                this.highlightError();
                return;
            }

            // Step 2: If OTP is valid, proceed with registration
            await this.completeRegistration();

        } catch (error) {
            console.error('Error verifying OTP:', error);
            this.showError(`An error occurred during OTP verification: ${error.message}`);
            this.highlightError();
        } finally {
            // Re-enable submit button
            if (submitBtn) {
                submitBtn.disabled = false;
                submitBtn.innerHTML = '<span class="btn-content-3d"><i class="bi bi-check-circle-fill me-2"></i><span class="btn-text">Verify & Register</span></span><div class="btn-glow-3d"></div><div class="btn-ripple-3d"></div>';
            }
        }
    }

    async completeRegistration() {
        try {
            console.log('Completing registration for:', this.userEmail);
            
            console.log('Registration data being sent:', this.registrationData);
            
            const formData = new FormData();
            formData.append('FullName', this.registrationData.FullName);
            formData.append('Email', this.registrationData.Email);
            formData.append('Password', this.registrationData.Password);
            formData.append('ConfirmPassword', this.registrationData.ConfirmPassword);
            
            // Debug: Log FormData contents
            console.log('FormData contents:');
            for (let [key, value] of formData.entries()) {
                console.log(`${key}: ${value}`);
            }
            
            // CSRF token removed for testing
            // const token = $('input[name="__RequestVerificationToken"]').val();

            const response = await fetch('/Account/RegisterCustomerWithOTPAjax', {
                method: 'POST',
                headers: {
                    // 'RequestVerificationToken': token  // Temporarily disabled for testing
                },
                body: formData
            });

            console.log('Registration Response Status:', response.status);

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const responseText = await response.text();
            console.log('Registration Response Text:', responseText);

            let result;
            try {
                result = JSON.parse(responseText);
            } catch (parseError) {
                console.error('Failed to parse JSON response:', parseError);
                console.error('Raw response:', responseText);
                
                // If it's not JSON, it might be HTML error page
                if (responseText.includes('<html') || responseText.includes('<!DOCTYPE')) {
                    console.error('Server returned HTML instead of JSON - likely an error page');
                    throw new Error('Server error - returned HTML page instead of JSON');
                }
                
                throw new Error('Invalid JSON response from server');
            }

            console.log('Registration Result:', result);

            if (result.success) {
                this.showSuccess('Registration successful! Welcome to Car Rental System!');
                
                // Close modal and reload page
                setTimeout(() => {
                    $('#otpVerificationModal').modal('hide');
                    location.reload();
                }, 1500);
            } else {
                this.showError(result.errors ? result.errors.join('<br>') : 'Registration failed. Please try again.');
            }
        } catch (error) {
            console.error('Error completing registration:', error);
            this.showError(`Registration failed: ${error.message}`);
        }
    }

    highlightError() {
        this.otpInputs.forEach(input => {
            input.classList.add('error');
        });
        
        // Remove error class after animation
        setTimeout(() => {
            this.otpInputs.forEach(input => {
                input.classList.remove('error');
            });
        }, 500);
    }

    showError(message) {
        const errorDiv = document.getElementById('otpError');
        if (errorDiv) {
            errorDiv.innerHTML = message;
            errorDiv.style.display = 'block';
        }
        
        // Auto-hide after 5 seconds
        setTimeout(() => {
            if (errorDiv) errorDiv.style.display = 'none';
        }, 5000);
    }

    showSuccess(message) {
        // You can implement a success toast here
        if (typeof showToast === 'function') {
            showToast(message, 'success');
        }
    }

    resetModal() {
        // Clear all inputs
        this.otpInputs.forEach(input => {
            input.value = '';
            input.classList.remove('filled', 'error');
        });
        
        // Clear hidden field
        const completeOTPInput = document.getElementById('completeOTP');
        if (completeOTPInput) completeOTPInput.value = '';
        
        // Hide error message
        const errorDiv = document.getElementById('otpError');
        if (errorDiv) errorDiv.style.display = 'none';
        
        // Clear timer
        if (this.countdownTimer) {
            clearInterval(this.countdownTimer);
        }
        
        // Reset countdown
        const countdownEl = document.getElementById('countdown');
        if (countdownEl) countdownEl.textContent = '60';
        
        const resendBtn = document.getElementById('resendOTPBtn');
        if (resendBtn) resendBtn.disabled = true;
        
        const timerEl = document.getElementById('otpTimer');
        if (timerEl) timerEl.style.display = 'block';
        
        // Reset data
        this.userEmail = '';
        this.registrationData = null;
        this.countdownSeconds = 60;
    }
}

// Initialize OTP verification when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    window.otpVerification = new OTPVerification();
});

// Function to show OTP modal (called from registration form)
function showOTPVerificationModal(email, registrationData) {
    if (window.otpVerification) {
        window.otpVerification.showModal(email, registrationData);
    }
}

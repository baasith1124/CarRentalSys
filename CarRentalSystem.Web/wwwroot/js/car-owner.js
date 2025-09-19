// Car Owner Portal JavaScript

document.addEventListener('DOMContentLoaded', function() {
    // Initialize sidebar toggle
    initializeSidebar();
    
    // Initialize tooltips
    initializeTooltips();
    
    // Initialize form validations
    initializeFormValidations();
    
    // Initialize file uploads
    initializeFileUploads();
});

// Sidebar functionality
function initializeSidebar() {
    const sidebarToggle = document.querySelector('.sidebar-toggle-mobile');
    const sidebar = document.querySelector('.car-owner-sidebar');
    
    if (sidebarToggle && sidebar) {
        sidebarToggle.addEventListener('click', function() {
            sidebar.classList.toggle('show');
        });
        
        // Close sidebar when clicking outside on mobile
        document.addEventListener('click', function(e) {
            if (window.innerWidth < 992) {
                if (!sidebar.contains(e.target) && !sidebarToggle.contains(e.target)) {
                    sidebar.classList.remove('show');
                }
            }
        });
    }
}

// Initialize Bootstrap tooltips
function initializeTooltips() {
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
}

// Form validations
function initializeFormValidations() {
    // Car registration form validation
    const carForm = document.querySelector('form[action*="RegisterCar"]');
    if (carForm) {
        carForm.addEventListener('submit', function(e) {
            if (!validateCarForm()) {
                e.preventDefault();
            }
        });
    }
    
    // Car edit form validation
    const editForm = document.querySelector('form[action*="EditCar"]');
    if (editForm) {
        editForm.addEventListener('submit', function(e) {
            if (!validateCarForm()) {
                e.preventDefault();
            }
        });
    }
}

// Validate car form
function validateCarForm() {
    let isValid = true;
    
    // Clear previous errors
    clearFormErrors();
    
    // Validate required fields
    const requiredFields = ['Name', 'Model', 'AvailableFrom', 'AvailableTo', 'RatePerDay'];
    requiredFields.forEach(fieldName => {
        const field = document.querySelector(`[name="${fieldName}"]`);
        if (field && !field.value.trim()) {
            showFieldError(field, `${fieldName} is required`);
            isValid = false;
        }
    });
    
    // Validate date range
    const fromDate = document.querySelector('[name="AvailableFrom"]');
    const toDate = document.querySelector('[name="AvailableTo"]');
    
    if (fromDate && toDate && fromDate.value && toDate.value) {
        const from = new Date(fromDate.value);
        const to = new Date(toDate.value);
        
        if (from >= to) {
            showFieldError(toDate, 'Available To date must be after Available From date');
            isValid = false;
        }
        
        if (from < new Date()) {
            showFieldError(fromDate, 'Available From date cannot be in the past');
            isValid = false;
        }
    }
    
    // Validate rate
    const rateField = document.querySelector('[name="RatePerDay"]');
    if (rateField && rateField.value) {
        const rate = parseFloat(rateField.value);
        if (rate < 1 || rate > 10000) {
            showFieldError(rateField, 'Rate must be between $1 and $10,000');
            isValid = false;
        }
    }
    
    // Validate year
    const yearField = document.querySelector('[name="Year"]');
    if (yearField && yearField.value) {
        const year = parseInt(yearField.value);
        if (year < 1990 || year > 2025) {
            showFieldError(yearField, 'Year must be between 1990 and 2025');
            isValid = false;
        }
    }
    
    return isValid;
}

// Show field error
function showFieldError(field, message) {
    const errorDiv = document.createElement('div');
    errorDiv.className = 'text-danger field-error';
    errorDiv.textContent = message;
    
    field.classList.add('is-invalid');
    field.parentNode.appendChild(errorDiv);
}

// Clear form errors
function clearFormErrors() {
    const errorElements = document.querySelectorAll('.field-error');
    errorElements.forEach(el => el.remove());
    
    const invalidFields = document.querySelectorAll('.is-invalid');
    invalidFields.forEach(field => field.classList.remove('is-invalid'));
}

// File upload functionality
function initializeFileUploads() {
    // Car image preview
    const carImageInput = document.querySelector('input[name="CarImage"]');
    if (carImageInput) {
        carImageInput.addEventListener('change', function(e) {
            previewImage(e.target, 'carImagePreview');
        });
    }
    
    // Document previews
    const documentInputs = document.querySelectorAll('input[name="Documents"]');
    documentInputs.forEach(input => {
        input.addEventListener('change', function(e) {
            previewDocuments(e.target.files, 'documentsPreview');
        });
    });
}

// Preview uploaded image
function previewImage(input, previewId) {
    const preview = document.getElementById(previewId);
    if (!preview) return;
    
    if (input.files && input.files[0]) {
        const reader = new FileReader();
        
        reader.onload = function(e) {
            preview.src = e.target.result;
            preview.style.display = 'block';
        };
        
        reader.readAsDataURL(input.files[0]);
    }
}

// Preview uploaded documents
function previewDocuments(files, previewId) {
    const preview = document.getElementById(previewId);
    if (!preview) return;
    
    preview.innerHTML = '';
    
    Array.from(files).forEach((file, index) => {
        const fileItem = document.createElement('div');
        fileItem.className = 'file-preview-item';
        fileItem.innerHTML = `
            <div class="file-info">
                <i class="bi bi-file-earmark"></i>
                <span>${file.name}</span>
                <small>(${(file.size / 1024 / 1024).toFixed(2)} MB)</small>
            </div>
        `;
        preview.appendChild(fileItem);
    });
}

// Utility functions
function showLoading(element) {
    element.classList.add('loading');
    const originalText = element.textContent;
    element.innerHTML = '<span class="spinner"></span> Loading...';
    return originalText;
}

function hideLoading(element, originalText) {
    element.classList.remove('loading');
    element.textContent = originalText;
}

function showNotification(message, type = 'success') {
    const alertClass = type === 'success' ? 'alert-success' : 'alert-danger';
    const icon = type === 'success' ? 'bi-check-circle' : 'bi-exclamation-triangle';
    
    const notification = document.createElement('div');
    notification.className = `alert ${alertClass} alert-dismissible fade show`;
    notification.innerHTML = `
        <i class="${icon} me-2"></i>
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    
    const container = document.querySelector('.car-owner-content');
    if (container) {
        container.insertBefore(notification, container.firstChild);
        
        // Auto-dismiss after 5 seconds
        setTimeout(() => {
            if (notification.parentNode) {
                notification.remove();
            }
        }, 5000);
    }
}

// AJAX form submission
function submitFormAjax(form, successCallback, errorCallback) {
    const formData = new FormData(form);
    const submitButton = form.querySelector('button[type="submit"]');
    const originalText = showLoading(submitButton);
    
    fetch(form.action, {
        method: form.method,
        body: formData,
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        }
    })
    .then(response => {
        if (response.ok) {
            return response.json();
        }
        throw new Error('Network response was not ok');
    })
    .then(data => {
        hideLoading(submitButton, originalText);
        if (data.success) {
            showNotification(data.message || 'Operation completed successfully');
            if (successCallback) successCallback(data);
        } else {
            showNotification(data.message || 'Operation failed', 'error');
            if (errorCallback) errorCallback(data);
        }
    })
    .catch(error => {
        hideLoading(submitButton, originalText);
        showNotification('An error occurred. Please try again.', 'error');
        if (errorCallback) errorCallback(error);
    });
}

// Format currency
function formatCurrency(amount) {
    return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD'
    }).format(amount);
}

// Format date
function formatDate(dateString) {
    return new Date(dateString).toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric'
    });
}

// Calculate earnings
function calculateEarnings(bookings) {
    return bookings
        .filter(booking => booking.PaymentStatus === 'Paid')
        .reduce((total, booking) => total + booking.TotalCost, 0);
}

// Export functions for global use
window.CarOwner = {
    showLoading,
    hideLoading,
    showNotification,
    submitFormAjax,
    formatCurrency,
    formatDate,
    calculateEarnings
};

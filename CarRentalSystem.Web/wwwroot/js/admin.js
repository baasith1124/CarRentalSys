// Admin Panel JavaScript

document.addEventListener('DOMContentLoaded', function() {
    // Initialize sidebar toggle
    initializeSidebar();
    
    // Initialize tooltips
    initializeTooltips();
    
    // Initialize auto-refresh
    initializeAutoRefresh();
    
    // Initialize form confirmations
    initializeConfirmations();
});

// Sidebar functionality
function initializeSidebar() {
    const sidebarToggle = document.querySelector('.sidebar-toggle-mobile');
    const sidebar = document.querySelector('.admin-sidebar');
    
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

// Auto-refresh functionality
function initializeAutoRefresh() {
    // Auto-refresh dashboard every 30 seconds
    if (window.location.pathname.includes('/Admin') && window.location.pathname.endsWith('/Admin')) {
        setInterval(function() {
            refreshDashboardData();
        }, 30000);
    }
}

// Refresh dashboard data
function refreshDashboardData() {
    fetch('/Admin/GetDashboardData', {
        method: 'GET',
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        }
    })
    .then(response => response.json())
    .then(data => {
        updateDashboardStats(data);
    })
    .catch(error => {
        console.log('Auto-refresh failed:', error);
    });
}

// Update dashboard statistics
function updateDashboardStats(data) {
    // Update pending cars count
    const pendingCarsElement = document.querySelector('.stats-card.pending h3');
    if (pendingCarsElement && data.pendingCars !== undefined) {
        pendingCarsElement.textContent = data.pendingCars;
    }
    
    // Update pending KYC count
    const pendingKYCElement = document.querySelector('.stats-card.kyc h3');
    if (pendingKYCElement && data.pendingKYC !== undefined) {
        pendingKYCElement.textContent = data.pendingKYC;
    }
    
    // Update total bookings
    const bookingsElement = document.querySelector('.stats-card.bookings h3');
    if (bookingsElement && data.totalBookings !== undefined) {
        bookingsElement.textContent = data.totalBookings;
    }
    
    // Update total customers
    const customersElement = document.querySelector('.stats-card.customers h3');
    if (customersElement && data.totalCustomers !== undefined) {
        customersElement.textContent = data.totalCustomers;
    }
}

// Form confirmation dialogs
function initializeConfirmations() {
    // Car approval/rejection confirmations
    const approveButtons = document.querySelectorAll('form[action*="ApproveCar"] button');
    const rejectButtons = document.querySelectorAll('form[action*="RejectCar"] button');
    
    approveButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            if (!confirm('Are you sure you want to approve this car?')) {
                e.preventDefault();
            }
        });
    });
    
    rejectButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            if (!confirm('Are you sure you want to reject this car?')) {
                e.preventDefault();
            }
        });
    });
    
    // KYC approval/rejection confirmations
    const approveKYCButtons = document.querySelectorAll('form[action*="ApproveKYC"] button');
    const rejectKYCButtons = document.querySelectorAll('form[action*="RejectKYC"] button');
    
    approveKYCButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            if (!confirm('Are you sure you want to approve this KYC document?')) {
                e.preventDefault();
            }
        });
    });
    
    rejectKYCButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            if (!confirm('Are you sure you want to reject this KYC document?')) {
                e.preventDefault();
            }
        });
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
    
    const container = document.querySelector('.admin-content');
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

// Search functionality
function initializeSearch() {
    const searchInputs = document.querySelectorAll('.search-input');
    
    searchInputs.forEach(input => {
        input.addEventListener('input', function() {
            const searchTerm = this.value.toLowerCase();
            const targetSelector = this.dataset.target;
            const targetElements = document.querySelectorAll(targetSelector);
            
            targetElements.forEach(element => {
                const text = element.textContent.toLowerCase();
                if (text.includes(searchTerm)) {
                    element.style.display = '';
                } else {
                    element.style.display = 'none';
                }
            });
        });
    });
}

// Export functions for global use
window.AdminPanel = {
    showLoading,
    hideLoading,
    showNotification,
    submitFormAjax,
    refreshDashboardData
};

// User Dashboard JavaScript functionality

document.addEventListener('DOMContentLoaded', function() {
    // Initialize sidebar toggle functionality
    initializeSidebarToggle();
    
    // Initialize dashboard widgets
    initializeDashboardWidgets();
    
    // Initialize notifications
    initializeNotifications();
});

function initializeSidebarToggle() {
    const sidebarToggle = document.querySelector('.sidebar-toggle-mobile');
    const sidebar = document.querySelector('#userSidebar');
    
    if (sidebarToggle && sidebar) {
        sidebarToggle.addEventListener('click', function() {
            sidebar.classList.toggle('show');
        });
    }
    
    // Close sidebar when clicking outside on mobile
    document.addEventListener('click', function(event) {
        if (window.innerWidth < 992) {
            if (!sidebar.contains(event.target) && !sidebarToggle.contains(event.target)) {
                sidebar.classList.remove('show');
            }
        }
    });
}

function initializeDashboardWidgets() {
    // Initialize any dashboard-specific widgets
    const statsCards = document.querySelectorAll('.stats-card');
    
    statsCards.forEach(card => {
        card.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-2px)';
            this.style.transition = 'transform 0.2s ease';
        });
        
        card.addEventListener('mouseleave', function() {
            this.style.transform = 'translateY(0)';
        });
    });
    
    // Initialize quick action buttons
    const quickActionBtns = document.querySelectorAll('.quick-action-btn');
    
    quickActionBtns.forEach(btn => {
        btn.addEventListener('mouseenter', function() {
            this.style.transform = 'scale(1.02)';
            this.style.transition = 'transform 0.2s ease';
        });
        
        btn.addEventListener('mouseleave', function() {
            this.style.transform = 'scale(1)';
        });
    });
}

function initializeNotifications() {
    // Auto-hide notifications after 5 seconds
    const alerts = document.querySelectorAll('.alert');
    
    alerts.forEach(alert => {
        setTimeout(() => {
            if (alert.classList.contains('show')) {
                alert.classList.remove('show');
                alert.classList.add('fade');
            }
        }, 5000);
    });
}

// Utility function to refresh dashboard data
function refreshDashboard() {
    // You can implement AJAX refresh here
    location.reload();
}

// Function to show loading state
function showLoading(element) {
    if (element) {
        element.innerHTML = '<i class="bi bi-hourglass-split me-2"></i>Loading...';
        element.disabled = true;
    }
}

// Function to hide loading state
function hideLoading(element, originalText) {
    if (element) {
        element.innerHTML = originalText;
        element.disabled = false;
    }
}

// Function to format currency
function formatCurrency(amount) {
    return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD'
    }).format(amount);
}

// Function to format date
function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric'
    });
}

// Export functions for global use
window.dashboardUtils = {
    refreshDashboard,
    showLoading,
    hideLoading,
    formatCurrency,
    formatDate
};

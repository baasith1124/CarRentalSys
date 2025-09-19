// Enhanced User Dashboard Mobile JavaScript - Smooth Animations and Interactivity

document.addEventListener('DOMContentLoaded', function() {
    initializeUserDashboardMobile();
});

function initializeUserDashboardMobile() {
    // Initialize all mobile features
    initializeMobileSidebar();
    initializeTouchInteractions();
    initializeMobileViewport();
    initializeMobileGestures();
    initializeMobileAnimations();
    initializeMobileNotifications();
}

// Mobile Sidebar Toggle
function initializeMobileSidebar() {
    const sidebar = document.querySelector('.user-sidebar');
    const toggleButton = document.querySelector('.sidebar-toggle-mobile');
    const overlay = document.createElement('div');
    
    // Create overlay
    overlay.className = 'sidebar-overlay';
    overlay.style.cssText = `
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background: rgba(0, 0, 0, 0.5);
        z-index: 1040;
        display: none;
        backdrop-filter: blur(5px);
        opacity: 0;
        transition: opacity 0.3s ease;
    `;
    document.body.appendChild(overlay);
    
    // Toggle sidebar
    function toggleSidebar() {
        const isOpen = sidebar.classList.contains('show');
        
        if (isOpen) {
            sidebar.classList.remove('show');
            overlay.style.display = 'none';
            overlay.style.opacity = '0';
            document.body.style.overflow = '';
        } else {
            sidebar.classList.add('show');
            overlay.style.display = 'block';
            setTimeout(() => {
                overlay.style.opacity = '1';
            }, 10);
            document.body.style.overflow = 'hidden';
        }
    }
    
    // Event listeners
    if (toggleButton) {
        toggleButton.addEventListener('click', function(e) {
            e.preventDefault();
            e.stopPropagation();
            toggleSidebar();
        });
    }
    
    // Close on overlay click
    overlay.addEventListener('click', toggleSidebar);
    
    // Close on escape key
    document.addEventListener('keydown', function(e) {
        if (e.key === 'Escape' && sidebar.classList.contains('show')) {
            toggleSidebar();
        }
    });
    
    // Close on window resize to desktop
    window.addEventListener('resize', function() {
        if (window.innerWidth > 768 && sidebar.classList.contains('show')) {
            toggleSidebar();
        }
    });
}

// Touch Interactions
function initializeTouchInteractions() {
    // Add touch-friendly classes
    if ('ontouchstart' in window) {
        document.body.classList.add('touch-device');
    }
    
    // Prevent zoom on double tap for buttons
    let lastTouchEnd = 0;
    document.addEventListener('touchend', function(event) {
        const now = (new Date()).getTime();
        if (now - lastTouchEnd <= 300) {
            event.preventDefault();
        }
        lastTouchEnd = now;
    }, false);
    
    // Add touch feedback
    document.querySelectorAll('.enhanced-nav-link, .quick-action-card, .metric-card, .chart-card').forEach(element => {
        element.addEventListener('touchstart', function() {
            this.classList.add('touch-active');
        });
        
        element.addEventListener('touchend', function() {
            setTimeout(() => {
                this.classList.remove('touch-active');
            }, 150);
        });
    });
}

// Mobile Viewport Handling
function initializeMobileViewport() {
    // Set viewport meta tag for mobile
    let viewport = document.querySelector('meta[name="viewport"]');
    if (!viewport) {
        viewport = document.createElement('meta');
        viewport.name = 'viewport';
        document.head.appendChild(viewport);
    }
    viewport.content = 'width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no';
    
    // Handle orientation change
    window.addEventListener('orientationchange', function() {
        setTimeout(() => {
            // Recalculate layout after orientation change
            window.dispatchEvent(new Event('resize'));
        }, 100);
    });
}

// Mobile Gestures
function initializeMobileGestures() {
    let startX = 0;
    let startY = 0;
    let isDragging = false;
    
    // Swipe to close sidebar
    document.addEventListener('touchstart', function(e) {
        startX = e.touches[0].clientX;
        startY = e.touches[0].clientY;
        isDragging = false;
    });
    
    document.addEventListener('touchmove', function(e) {
        if (!isDragging) {
            const deltaX = Math.abs(e.touches[0].clientX - startX);
            const deltaY = Math.abs(e.touches[0].clientY - startY);
            
            if (deltaX > deltaY && deltaX > 10) {
                isDragging = true;
            }
        }
    });
    
    document.addEventListener('touchend', function(e) {
        if (isDragging) {
            const deltaX = e.changedTouches[0].clientX - startX;
            const sidebar = document.querySelector('.user-sidebar');
            
            // Swipe right to open sidebar (from left edge)
            if (startX < 50 && deltaX > 100 && !sidebar.classList.contains('show')) {
                sidebar.classList.add('show');
                document.querySelector('.sidebar-overlay').style.display = 'block';
                document.body.style.overflow = 'hidden';
            }
            // Swipe left to close sidebar
            else if (deltaX < -100 && sidebar.classList.contains('show')) {
                sidebar.classList.remove('show');
                document.querySelector('.sidebar-overlay').style.display = 'none';
                document.body.style.overflow = '';
            }
        }
    });
}

// Mobile Animations
function initializeMobileAnimations() {
    // Animate cards on scroll
    const cards = document.querySelectorAll('.metric-card, .chart-card, .booking-item, .quick-action-card');
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.animationDelay = '0s';
                entry.target.classList.add('animate-in');
            }
        });
    }, { threshold: 0.1 });
    
    cards.forEach(card => observer.observe(card));
    
    // Add hover effects for desktop
    if (window.innerWidth > 768) {
        cards.forEach(card => {
            card.addEventListener('mouseenter', function() {
                this.style.transform = 'translateY(-5px) scale(1.02)';
            });
            
            card.addEventListener('mouseleave', function() {
                this.style.transform = 'translateY(0) scale(1)';
            });
        });
    }
}

// Mobile Notifications
function initializeMobileNotifications() {
    // Enhanced notification system
    window.showMobileNotification = function(message, type = 'success', duration = 5000) {
        const notification = document.createElement('div');
        notification.className = `mobile-notification ${type}`;
        notification.innerHTML = `
            <div class="notification-content">
                <i class="bi bi-${type === 'success' ? 'check-circle' : 'exclamation-triangle'}"></i>
                <span>${message}</span>
            </div>
            <button class="notification-close" onclick="this.parentElement.remove()">
                <i class="bi bi-x"></i>
            </button>
        `;
        
        // Style the notification
        notification.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            background: var(--glass-bg-dark);
            backdrop-filter: var(--glass-backdrop-strong);
            border: 2px solid var(--glass-border);
            border-radius: 1rem;
            padding: 1rem;
            color: var(--text-primary);
            z-index: 9999;
            max-width: 300px;
            box-shadow: var(--glass-shadow-colored);
            transform: translateX(100%);
            transition: transform 0.3s ease;
        `;
        
        document.body.appendChild(notification);
        
        // Animate in
        setTimeout(() => {
            notification.style.transform = 'translateX(0)';
        }, 100);
        
        // Auto remove
        setTimeout(() => {
            notification.style.transform = 'translateX(100%)';
            setTimeout(() => {
                if (notification.parentElement) {
                    notification.remove();
                }
            }, 300);
        }, duration);
    };
}

// Mobile-specific utility functions
function isMobile() {
    return window.innerWidth <= 768;
}

function isTouchDevice() {
    return 'ontouchstart' in window || navigator.maxTouchPoints > 0;
}

// Enhanced scroll handling
function initializeScrollEffects() {
    let lastScrollTop = 0;
    const header = document.querySelector('.dashboard-header');
    
    window.addEventListener('scroll', function() {
        const scrollTop = window.pageYOffset || document.documentElement.scrollTop;
        
        if (scrollTop > lastScrollTop && scrollTop > 100) {
            // Scrolling down
            if (header) {
                header.style.transform = 'translateY(-100%)';
            }
        } else {
            // Scrolling up
            if (header) {
                header.style.transform = 'translateY(0)';
            }
        }
        
        lastScrollTop = scrollTop;
    });
}

// Initialize scroll effects
document.addEventListener('DOMContentLoaded', function() {
    initializeScrollEffects();
});

// Mobile chart responsiveness
function initializeMobileCharts() {
    const charts = document.querySelectorAll('canvas');
    
    charts.forEach(chart => {
        const resizeObserver = new ResizeObserver(entries => {
            entries.forEach(entry => {
                const chartInstance = Chart.getChart(chart);
                if (chartInstance) {
                    chartInstance.resize();
                }
            });
        });
        
        resizeObserver.observe(chart);
    });
}

// Initialize mobile charts
document.addEventListener('DOMContentLoaded', function() {
    initializeMobileCharts();
});

// Mobile performance optimizations
function initializePerformanceOptimizations() {
    // Lazy load images
    const images = document.querySelectorAll('img[data-src]');
    const imageObserver = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const img = entry.target;
                img.src = img.dataset.src;
                img.classList.remove('lazy');
                imageObserver.unobserve(img);
            }
        });
    });
    
    images.forEach(img => imageObserver.observe(img));
    
    // Debounce scroll events
    let scrollTimeout;
    window.addEventListener('scroll', function() {
        if (scrollTimeout) {
            clearTimeout(scrollTimeout);
        }
        scrollTimeout = setTimeout(function() {
            // Handle scroll events here
        }, 16); // ~60fps
    });
}

// Initialize performance optimizations
document.addEventListener('DOMContentLoaded', function() {
    initializePerformanceOptimizations();
});

// Export functions for global use
window.showMobileNotification = showMobileNotification;
window.isMobile = isMobile;
window.isTouchDevice = isTouchDevice;

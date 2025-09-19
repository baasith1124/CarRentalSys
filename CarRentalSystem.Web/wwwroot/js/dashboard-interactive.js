// Enhanced Interactive Dashboard JavaScript

class InteractiveDashboard {
    constructor() {
        this.isInitialized = false;
        this.updateInterval = null;
        this.animationObserver = null;
        this.particles = null;
        
        // Bind methods
        this.init = this.init.bind(this);
        this.initParticles = this.initParticles.bind(this);
        this.initLiveClock = this.initLiveClock.bind(this);
        this.initCounterAnimations = this.initCounterAnimations.bind(this);
        this.initProgressRings = this.initProgressRings.bind(this);
        this.initInteractiveElements = this.initInteractiveElements.bind(this);
        this.initRealTimeUpdates = this.initRealTimeUpdates.bind(this);
        this.initNotificationSystem = this.initNotificationSystem.bind(this);
        this.initActivityTimeline = this.initActivityTimeline.bind(this);
        this.initWeatherWidget = this.initWeatherWidget.bind(this);
        this.initTooltips = this.initTooltips.bind(this);
    }

    async init() {
        if (this.isInitialized) return;
        
        try {
            console.log('🚀 Initializing Interactive Dashboard...');
            
            // Initialize core features
            this.initParticles();
            this.initLiveClock();
            this.initCounterAnimations();
            this.initProgressRings();
            this.initInteractiveElements();
            this.initTooltips();
            this.initActivityTimeline();
            this.initWeatherWidget();
            this.initRealTimeUpdates();
            this.initNotificationSystem();
            
            // Initialize external libraries
            await this.initTiltEffects();
            
            this.isInitialized = true;
            console.log('✅ Interactive Dashboard initialized successfully!');
            
            // Show welcome notification
            this.showNotification('Dashboard loaded successfully!', 'success', 3000);
            
        } catch (error) {
            console.error('❌ Failed to initialize dashboard:', error);
            this.showNotification('Some features may not work properly', 'warning', 5000);
        }
    }

    initParticles() {
        const container = document.getElementById('particles-bg');
        if (!container) return;

        // Create simple CSS-based particle animation
        for (let i = 0; i < 20; i++) {
            const particle = document.createElement('div');
            particle.style.cssText = `
                position: absolute;
                width: 4px;
                height: 4px;
                background: radial-gradient(circle, rgba(102, 126, 234, 0.6) 0%, transparent 70%);
                border-radius: 50%;
                left: ${Math.random() * 100}%;
                top: ${Math.random() * 100}%;
                animation: float ${3 + Math.random() * 4}s ease-in-out infinite;
                animation-delay: ${Math.random() * 2}s;
            `;
            container.appendChild(particle);
        }
    }

    initLiveClock() {
        const updateClock = () => {
            const now = new Date();
            const timeString = now.toLocaleTimeString([], {
                hour: '2-digit', 
                minute: '2-digit',
                second: '2-digit'
            });
            const clockElement = document.getElementById('live-clock');
            if (clockElement) {
                clockElement.textContent = timeString;
            }
        };
        
        updateClock();
        setInterval(updateClock, 1000);
    }

    initCounterAnimations() {
        const counters = document.querySelectorAll('.counter');
        const observerOptions = {
            threshold: 0.7,
            rootMargin: '0px 0px -50px 0px'
        };
        
        this.animationObserver = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting && !entry.target.classList.contains('animated')) {
                    this.animateCounter(entry.target);
                    entry.target.classList.add('animated');
                }
            });
        }, observerOptions);
        
        counters.forEach(counter => {
            this.animationObserver.observe(counter);
        });
    }

    animateCounter(element) {
        const target = parseInt(element.dataset.target) || 0;
        const duration = 2000;
        const startTime = performance.now();
        
        const animate = (currentTime) => {
            const elapsed = currentTime - startTime;
            const progress = Math.min(elapsed / duration, 1);
            
            // Easing function for smooth animation
            const easeOutCubic = 1 - Math.pow(1 - progress, 3);
            const current = Math.floor(target * easeOutCubic);
            
            element.textContent = current.toLocaleString();
            
            if (progress < 1) {
                requestAnimationFrame(animate);
            } else {
                element.textContent = target.toLocaleString();
            }
        };
        
        requestAnimationFrame(animate);
    }

    initProgressRings() {
        const rings = document.querySelectorAll('.progress-ring-fill');
        
        rings.forEach((ring, index) => {
            const radius = ring.r.baseVal.value;
            const circumference = radius * 2 * Math.PI;
            const progress = Math.min((index + 1) * 25, 100);
            const offset = circumference - (progress / 100) * circumference;
            
            ring.style.strokeDasharray = circumference;
            ring.style.strokeDashoffset = circumference;
            
            setTimeout(() => {
                ring.style.strokeDashoffset = offset;
                ring.style.transition = 'stroke-dashoffset 2s cubic-bezier(0.4, 0, 0.2, 1)';
            }, 500 + index * 200);
        });
    }

    async initTiltEffects() {
        // Check if VanillaTilt is available
        if (typeof VanillaTilt !== 'undefined') {
            VanillaTilt.init(document.querySelectorAll('[data-tilt]'), {
                max: 15,
                speed: 1000,
                glare: true,
                'max-glare': 0.2,
                scale: 1.02
            });
        } else {
            console.warn('VanillaTilt library not found. Tilt effects disabled.');
        }
    }

    initInteractiveElements() {
        // Refresh buttons
        this.initRefreshButtons();
        
        // Interactive cards
        this.initInteractiveCards();
        
        // Quick actions
        this.initQuickActions();
        
        // Booking items
        this.initBookingItems();
    }

    initRefreshButtons() {
        const refreshButtons = [
            { id: 'refresh-actions', message: 'Quick actions refreshed' },
            { id: 'refresh-bookings', message: 'Bookings refreshed' }
        ];
        
        refreshButtons.forEach(({ id, message }) => {
            const button = document.getElementById(id);
            if (button) {
                button.addEventListener('click', (e) => {
                    e.preventDefault();
                    this.performRefresh(button, message);
                });
            }
        });
    }

    performRefresh(button, message) {
        const originalHTML = button.innerHTML;
        button.innerHTML = '<i class="bi bi-arrow-clockwise spin"></i>';
        button.disabled = true;
        
        // Simulate async operation
        setTimeout(() => {
            button.innerHTML = originalHTML;
            button.disabled = false;
            this.showNotification(message, 'success', 3000);
        }, 1500);
    }

    initInteractiveCards() {
        const cards = document.querySelectorAll('.interactive-card, .interactive-hover-card');
        
        cards.forEach(card => {
            // Add ripple effect on click
            card.addEventListener('click', (e) => {
                this.createRippleEffect(e, card);
            });
            
            // Enhanced hover effects
            card.addEventListener('mouseenter', () => {
                this.addCardGlow(card);
            });
            
            card.addEventListener('mouseleave', () => {
                this.removeCardGlow(card);
            });
        });
    }

    createRippleEffect(event, element) {
        const ripple = document.createElement('div');
        const rect = element.getBoundingClientRect();
        const size = Math.max(rect.width, rect.height);
        const x = event.clientX - rect.left - size / 2;
        const y = event.clientY - rect.top - size / 2;
        
        ripple.style.cssText = `
            position: absolute;
            width: ${size}px;
            height: ${size}px;
            border-radius: 50%;
            background: rgba(255, 255, 255, 0.6);
            transform: scale(0);
            animation: ripple 0.6s linear;
            left: ${x}px;
            top: ${y}px;
            pointer-events: none;
            z-index: 1000;
        `;
        
        element.style.position = 'relative';
        element.style.overflow = 'hidden';
        element.appendChild(ripple);
        
        setTimeout(() => {
            ripple.remove();
        }, 600);
    }

    addCardGlow(card) {
        card.style.boxShadow = '0 0 20px rgba(102, 126, 234, 0.3)';
    }

    removeCardGlow(card) {
        card.style.boxShadow = '';
    }

    initQuickActions() {
        const quickActions = document.querySelectorAll('.quick-action-btn');
        
        quickActions.forEach(action => {
            action.addEventListener('mouseenter', () => {
                const effect = action.querySelector('.quick-action-effect');
                if (effect) {
                    effect.style.left = '100%';
                }
            });
            
            action.addEventListener('mouseleave', () => {
                const effect = action.querySelector('.quick-action-effect');
                if (effect) {
                    setTimeout(() => {
                        effect.style.left = '-100%';
                    }, 300);
                }
            });
        });
    }

    initBookingItems() {
        const bookingItems = document.querySelectorAll('.booking-item');
        
        bookingItems.forEach(item => {
            // Add progress animation based on booking status
            const progressLine = item.querySelector('.booking-progress');
            const status = progressLine?.dataset.status;
            
            if (progressLine && status) {
                let width = '0%';
                switch (status.toLowerCase()) {
                    case 'confirmed': width = '100%'; break;
                    case 'pending': width = '30%'; break;
                    case 'in-progress': width = '70%'; break;
                    default: width = '10%';
                }
                
                setTimeout(() => {
                    progressLine.style.width = width;
                }, 500);
            }
            
            // Add click handler for details
            item.addEventListener('click', (e) => {
                if (!e.target.closest('.booking-actions')) {
                    const bookingId = item.dataset.bookingId;
                    if (bookingId) {
                        this.viewBookingDetails(bookingId);
                    }
                }
            });
        });
    }

    initActivityTimeline() {
        // Sample activity data - in a real app, this would come from the server
        const activities = [
            { 
                type: 'booking', 
                title: 'New booking created', 
                desc: 'Toyota Camry for Dec 15-20', 
                time: '2 hours ago', 
                icon: 'bi-calendar-plus' 
            },
            { 
                type: 'payment', 
                title: 'Payment successful', 
                desc: '$250.00 processed', 
                time: '1 day ago', 
                icon: 'bi-credit-card' 
            },
            { 
                type: 'profile', 
                title: 'Profile updated', 
                desc: 'Contact information changed', 
                time: '3 days ago', 
                icon: 'bi-person-check' 
            },
            { 
                type: 'booking', 
                title: 'Booking completed', 
                desc: 'Honda Civic rental finished', 
                time: '1 week ago', 
                icon: 'bi-check-circle' 
            }
        ];
        
        const timeline = document.getElementById('activity-timeline');
        if (timeline) {
            timeline.innerHTML = activities.map((activity, index) => `
                <div class="timeline-item" style="animation-delay: ${index * 0.2}s">
                    <div class="timeline-marker ${activity.type}">
                        <i class="${activity.icon}"></i>
                    </div>
                    <div class="timeline-content">
                        <h6>${activity.title}</h6>
                        <p>${activity.desc}</p>
                        <small class="text-muted">${activity.time}</small>
                    </div>
                </div>
            `).join('');
            
            // Animate timeline items
            const timelineItems = timeline.querySelectorAll('.timeline-item');
            timelineItems.forEach((item, index) => {
                item.style.opacity = '0';
                item.style.transform = 'translateX(-20px)';
                
                setTimeout(() => {
                    item.style.transition = 'all 0.5s ease';
                    item.style.opacity = '1';
                    item.style.transform = 'translateX(0)';
                }, index * 200);
            });
        }
    }

    initWeatherWidget() {
        // Simulate weather data - in a real app, this would come from a weather API
        const weatherData = {
            temp: Math.floor(Math.random() * 15) + 15, // 15-30°C
            condition: 'sunny',
            description: 'Perfect for driving!'
        };
        
        const weatherInfo = document.getElementById('weather-info');
        if (weatherInfo) {
            const tempElement = weatherInfo.querySelector('.weather-temp');
            const descElement = weatherInfo.querySelector('.weather-desc');
            
            if (tempElement) tempElement.textContent = `${weatherData.temp}°C`;
            if (descElement) descElement.textContent = weatherData.description;
            
            // Update weather every 10 minutes
            setInterval(() => {
                this.updateWeather(weatherInfo);
            }, 600000);
        }
    }

    updateWeather(weatherElement) {
        const temps = [18, 22, 25, 28, 20, 24];
        const descriptions = [
            'Perfect for driving!',
            'Great weather today!',
            'Sunny and warm!',
            'Clear skies ahead!',
            'Beautiful day!'
        ];
        
        const temp = temps[Math.floor(Math.random() * temps.length)];
        const desc = descriptions[Math.floor(Math.random() * descriptions.length)];
        
        const tempElement = weatherElement.querySelector('.weather-temp');
        const descElement = weatherElement.querySelector('.weather-desc');
        
        if (tempElement) tempElement.textContent = `${temp}°C`;
        if (descElement) descElement.textContent = desc;
    }

    initRealTimeUpdates() {
        // Update time-remaining displays
        this.updateTimeRemaining();
        
        // Set up periodic updates
        this.updateInterval = setInterval(() => {
            this.updateTimeRemaining();
            this.updateOnlineStatus();
        }, 30000); // Every 30 seconds
        
        // Simulate live data updates
        setTimeout(() => {
            this.simulateDataUpdate();
        }, 5000);
    }

    updateTimeRemaining() {
        document.querySelectorAll('.time-remaining').forEach(element => {
            const pickupDate = new Date(element.dataset.pickup);
            const now = new Date();
            const diff = pickupDate - now;
            
            if (diff > 0) {
                const days = Math.floor(diff / (1000 * 60 * 60 * 24));
                const hours = Math.floor((diff % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
                
                if (days > 0) {
                    element.textContent = `${days} days, ${hours} hours to pickup`;
                } else if (hours > 0) {
                    element.textContent = `${hours} hours to pickup`;
                } else {
                    const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
                    element.textContent = `${minutes} minutes to pickup`;
                }
                
                element.className = 'time-remaining text-success';
            } else {
                element.textContent = 'Pickup time passed';
                element.className = 'time-remaining text-warning';
            }
        });
    }

    updateOnlineStatus() {
        const onlineIndicators = document.querySelectorAll('.online-indicator');
        onlineIndicators.forEach(indicator => {
            indicator.style.background = navigator.onLine ? '#28a745' : '#dc3545';
        });
    }

    simulateDataUpdate() {
        // Simulate new notification or data update
        if (Math.random() > 0.7) {
            const messages = [
                'New booking inquiry received',
                'Payment reminder sent',
                'Profile view increased',
                'New promotional offer available'
            ];
            
            const message = messages[Math.floor(Math.random() * messages.length)];
            this.showNotification(message, 'info', 4000);
        }
    }

    initNotificationSystem() {
        // Create notification container if it doesn't exist
        let container = document.getElementById('notification-container');
        if (!container) {
            container = document.createElement('div');
            container.id = 'notification-container';
            container.className = 'position-fixed top-0 end-0 p-3';
            container.style.zIndex = '9999';
            document.body.appendChild(container);
        }
        
        // Check for notifications every minute
        setInterval(() => {
            this.checkNotifications();
        }, 60000);
    }

    checkNotifications() {
        // Simulate notification check
        if (Math.random() > 0.95) {
            this.showNotification('System update available', 'info', 6000);
        }
    }

    showNotification(message, type = 'info', duration = 5000) {
        const container = document.getElementById('notification-container');
        if (!container) return;
        
        const notificationId = 'notification-' + Date.now();
        const toast = document.createElement('div');
        toast.id = notificationId;
        toast.className = `toast align-items-center text-white bg-${type} border-0 show mb-2`;
        toast.style.minWidth = '300px';
        
        const iconMap = {
            success: 'bi-check-circle-fill',
            error: 'bi-exclamation-triangle-fill',
            warning: 'bi-exclamation-triangle-fill',
            info: 'bi-info-circle-fill'
        };
        
        toast.innerHTML = `
            <div class="d-flex">
                <div class="toast-body d-flex align-items-center">
                    <i class="${iconMap[type] || iconMap.info} me-2"></i>
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" onclick="this.closest('.toast').remove()"></button>
            </div>
        `;
        
        // Add entrance animation
        toast.style.transform = 'translateX(100%)';
        toast.style.transition = 'transform 0.3s ease';
        
        container.appendChild(toast);
        
        // Trigger entrance animation
        setTimeout(() => {
            toast.style.transform = 'translateX(0)';
        }, 10);
        
        // Auto-remove after duration
        setTimeout(() => {
            if (toast.parentNode) {
                toast.style.transform = 'translateX(100%)';
                setTimeout(() => {
                    toast.remove();
                }, 300);
            }
        }, duration);
    }

    initTooltips() {
        // Initialize Bootstrap tooltips if available
        if (typeof bootstrap !== 'undefined' && bootstrap.Tooltip) {
            const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle=\"tooltip\"]'));
            tooltipTriggerList.map(function (tooltipTriggerEl) {
                return new bootstrap.Tooltip(tooltipTriggerEl);
            });
        }
    }

    // Public methods for external use
    viewBookingDetails(bookingId) {
        this.showNotification('Loading booking details...', 'info', 2000);
        setTimeout(() => {
            window.location.href = `/Dashboard/Bookings/${bookingId}`;
        }, 500);
    }

    cancelBooking(bookingId) {
        if (confirm('Are you sure you want to cancel this booking?')) {
            this.showNotification('Booking cancellation initiated...', 'warning', 3000);
            // Implement actual cancellation logic here
            setTimeout(() => {
                this.showNotification('Booking cancelled successfully', 'success', 4000);
            }, 2000);
        }
    }

    refreshData() {
        this.showNotification('Refreshing dashboard data...', 'info', 2000);
        // Implement actual data refresh logic
        setTimeout(() => {
            this.showNotification('Dashboard data updated', 'success', 3000);
        }, 1500);
    }

    destroy() {
        // Clean up resources
        if (this.updateInterval) {
            clearInterval(this.updateInterval);
        }
        
        if (this.animationObserver) {
            this.animationObserver.disconnect();
        }
        
        this.isInitialized = false;
        console.log('🧹 Interactive Dashboard destroyed');
    }
}

// Global instance
let dashboardInstance = null;

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    dashboardInstance = new InteractiveDashboard();
    dashboardInstance.init();
});

// Global functions for external use
window.viewBookingDetails = function(bookingId) {
    if (dashboardInstance) {
        dashboardInstance.viewBookingDetails(bookingId);
    }
};

window.cancelBooking = function(bookingId) {
    if (dashboardInstance) {
        dashboardInstance.cancelBooking(bookingId);
    }
};

window.refreshDashboard = function() {
    if (dashboardInstance) {
        dashboardInstance.refreshData();
    }
};

// Clean up on page unload
window.addEventListener('beforeunload', function() {
    if (dashboardInstance) {
        dashboardInstance.destroy();
    }
});

// Add CSS animation for ripple effect
const style = document.createElement('style');
style.textContent = `
    @keyframes ripple {
        to {
            transform: scale(4);
            opacity: 0;
        }
    }
`;
document.head.appendChild(style);
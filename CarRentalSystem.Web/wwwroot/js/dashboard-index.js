// Dashboard Index Page JavaScript
// Depends on: window.totalSpent, window.totalBookings, window.activeBookings (set from Razor view)

document.addEventListener('DOMContentLoaded', function() {
    // Initialize chart type switcher
    initChartTypeSwitcher();
    
    // Initialize dashboard with all interactive features
    initializeDashboard();
});

// Chart type switcher
function initChartTypeSwitcher() {
    const chartTypeBtns = document.querySelectorAll('input[name="chartType"]');
    const chartCanvases = {
        'trendChart': 'bookingTrendChart',
        'carTypeChart': 'carTypeChart', 
        'radarChart': 'preferenceRadarChart'
    };
    
    chartTypeBtns.forEach(btn => {
        btn.addEventListener('change', function() {
            if (this.checked) {
                // Hide all charts
                Object.values(chartCanvases).forEach(canvasId => {
                    const canvas = document.getElementById(canvasId);
                    if (canvas) canvas.style.display = 'none';
                });
                
                // Show selected chart
                const selectedCanvas = document.getElementById(chartCanvases[this.id]);
                if (selectedCanvas) {
                    selectedCanvas.style.display = 'block';
                    
                    // Trigger chart animation
                    const container = selectedCanvas.parentElement;
                    container.style.transform = 'scale(0.95)';
                    container.style.transition = 'transform 0.3s ease';
                    setTimeout(() => {
                        container.style.transform = 'scale(1)';
                    }, 100);
                }
            }
        });
    });
}

// Enhanced Dashboard JavaScript with Interactive Features
function initializeDashboard() {
    // Initialize all interactive features
    initLiveClock();
    initCounterAnimations();
    initProgressRings();
    initTiltEffects();
    initCustomerCharts();
    initActivityTimeline();
    initRealTimeUpdates();
    initNotificationSystem();
    initInteractiveElements();
    
    // Initialize time remaining
    updateTimeRemaining();
}

// Live clock implementation
function initLiveClock() {
    function updateClock() {
        const now = new Date();
        const timeString = now.toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'});
        const clockElement = document.getElementById('live-clock');
        if (clockElement) {
            clockElement.textContent = timeString;
        }
    }
    updateClock();
    setInterval(updateClock, 1000);
}

// Counter animations for stats cards
function initCounterAnimations() {
    const counters = document.querySelectorAll('.counter');
    const observerOptions = {
        threshold: 0.7
    };
    
    const counterObserver = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                animateCounter(entry.target);
                counterObserver.unobserve(entry.target);
            }
        });
    }, observerOptions);
    
    counters.forEach(counter => {
        counterObserver.observe(counter);
    });
}

function animateCounter(element) {
    const target = parseInt(element.dataset.target) || 0;
    let current = 0;
    const increment = target / 100;
    const timer = setInterval(() => {
        current += increment;
        if (current >= target) {
            current = target;
            clearInterval(timer);
        }
        element.textContent = Math.floor(current);
    }, 20);
}

// Progress ring animations
function initProgressRings() {
    const rings = document.querySelectorAll('.progress-ring-fill');
    rings.forEach((ring, index) => {
        const radius = ring.r.baseVal.value;
        const circumference = radius * 2 * Math.PI;
        const progress = (index + 1) * 25; // Different progress for each ring
        const offset = circumference - (progress / 100) * circumference;
        
        ring.style.strokeDasharray = circumference;
        ring.style.strokeDashoffset = circumference;
        
        setTimeout(() => {
            ring.style.strokeDashoffset = offset;
            ring.style.transition = 'stroke-dashoffset 2s ease-in-out';
        }, 500 + index * 200);
    });
}

// Tilt effects for interactive cards
function initTiltEffects() {
    if (typeof VanillaTilt !== 'undefined') {
        VanillaTilt.init(document.querySelectorAll('[data-tilt]'), {
            max: 15,
            speed: 1000,
            glare: true,
            'max-glare': 0.2
        });
    }
}

// Enhanced customer charts with interactive features
function initCustomerCharts() {
    const spendingCtx = document.getElementById('spendingChart');
    if (spendingCtx && typeof window.totalSpent !== 'undefined') {
        const totalSpent = window.totalSpent;
        const carRentals = totalSpent * 0.8;
        const serviceFees = totalSpent * 0.12;
        const insurance = totalSpent * 0.08;
        
        new Chart(spendingCtx, {
            type: 'doughnut',
            data: {
                labels: ['Car Rentals', 'Service Fees', 'Insurance'],
                datasets: [{
                    data: [carRentals, serviceFees, insurance],
                    backgroundColor: [
                        'rgba(52, 152, 219, 0.8)',
                        'rgba(46, 204, 113, 0.8)',
                        'rgba(241, 196, 15, 0.8)'
                    ],
                    borderColor: [
                        'rgba(52, 152, 219, 1)',
                        'rgba(46, 204, 113, 1)',
                        'rgba(241, 196, 15, 1)'
                    ],
                    borderWidth: 3,
                    hoverOffset: 15,
                    cutout: '60%'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                animation: {
                    animateRotate: true,
                    duration: 2000
                },
                plugins: {
                    legend: {
                        position: 'bottom',
                        cornerRadius: 10,
                        callbacks: {
                            label: function(context) {
                                return context.label + ': $' + context.parsed.toFixed(2);
                            }
                        }
                    }
                },
                elements: {
                    arc: {
                        borderRadius: 8
                    }
                },
                interaction: {
                    intersect: false
                }
            }
        });
    }
}

// Activity timeline
function initActivityTimeline() {
    // Sample activity data - in real app, this would come from the server
    const activities = [
        { type: 'booking', title: 'New booking created', desc: 'Toyota Camry for Dec 15-20', time: '2 hours ago', icon: 'bi-calendar-plus' },
        { type: 'payment', title: 'Payment successful', desc: '$250.00 processed', time: '1 day ago', icon: 'bi-credit-card' },
        { type: 'profile', title: 'Profile updated', desc: 'Contact information changed', time: '3 days ago', icon: 'bi-person-check' },
        { type: 'booking', title: 'Booking completed', desc: 'Honda Civic rental finished', time: '1 week ago', icon: 'bi-check-circle' }
    ];
    
    const timeline = document.getElementById('activity-timeline');
    if (timeline) {
        timeline.innerHTML = activities.map(activity => `
            <div class="timeline-item">
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
    }
}

// Real-time updates simulation
function initRealTimeUpdates() {
    // Simulate real-time data updates every 30 seconds
    setInterval(() => {
        updateTimeRemaining();
        // Add subtle notification for updates
        showToast('Data refreshed', 'info', 2000);
    }, 30000);
}

function updateTimeRemaining() {
    document.querySelectorAll('.time-remaining').forEach(element => {
        const pickupDate = new Date(element.dataset.pickup);
        const now = new Date();
        const diff = pickupDate - now;
        
        if (diff > 0) {
            const days = Math.floor(diff / (1000 * 60 * 60 * 24));
            const hours = Math.floor((diff % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
            
            if (days > 0) {
                element.textContent = `${days} days, ${hours} hours to pickup`;
            } else {
                element.textContent = `${hours} hours to pickup`;
            }
        } else {
            element.textContent = 'Pickup time passed';
        }
    });
}

// Notification system
function initNotificationSystem() {
    // Check for new notifications every minute
    setInterval(checkNotifications, 60000);
}

function checkNotifications() {
    // Simulate notification check
    if (Math.random() > 0.9) {
        showToast('New activity detected', 'info');
    }
}

function showToast(message, type = 'info', duration = 5000) {
    const container = document.getElementById('notification-container');
    if (!container) return;
    
    const toast = document.createElement('div');
    toast.className = `toast align-items-center text-white bg-${type} border-0 show`;
    toast.innerHTML = `
        <div class="d-flex">
            <div class="toast-body">${message}</div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
        </div>
    `;
    
    container.appendChild(toast);
    
    setTimeout(() => {
        toast.remove();
    }, duration);
}

// Interactive elements
function initInteractiveElements() {
    // Refresh buttons
    document.getElementById('refresh-actions')?.addEventListener('click', function() {
        this.innerHTML = '<i class="bi bi-arrow-clockwise spin"></i>';
        setTimeout(() => {
            this.innerHTML = '<i class="bi bi-arrow-clockwise"></i>';
            showToast('Quick actions refreshed', 'success', 3000);
        }, 1000);
    });
    
    document.getElementById('refresh-bookings')?.addEventListener('click', function() {
        this.innerHTML = '<i class="bi bi-arrow-clockwise spin"></i>';
        setTimeout(() => {
            this.innerHTML = '<i class="bi bi-arrow-clockwise"></i>';
            showToast('Bookings refreshed', 'success', 3000);
        }, 1000);
    });
}

// Global functions for booking actions
window.viewBookingDetails = function(bookingId) {
    window.location.href = `/Dashboard/Bookings/${bookingId}`;
};

window.cancelBooking = function(bookingId) {
    if (confirm('Are you sure you want to cancel this booking?')) {
        showToast('Booking cancellation initiated', 'warning');
        // Implement actual cancellation logic
    }
};
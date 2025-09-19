// 3D Dashboard functionality for Admin Panel

// Initialize 3D effects when page loads
document.addEventListener('DOMContentLoaded', function() {
    init3DEffects();
    initCharts();
    initInteractiveElements();
});

// Initialize 3D visual effects
function init3DEffects() {
    // Apply 3D classes to stats cards
    const statsCards = document.querySelectorAll('.stats-card');
    statsCards.forEach(card => {
        card.classList.add('stats-card-3d');
        
        // Add floating animation to alternate cards
        if (Math.random() > 0.5) {
            card.classList.add('floating-3d');
        }
        
        // Upgrade icons to 3D
        const icon = card.querySelector('.stats-icon');
        if (icon) {
            icon.classList.add('stats-icon-3d');
        }
    });

    // Apply 3D effects to chart containers
    const chartContainers = document.querySelectorAll('.card');
    chartContainers.forEach(container => {
        if (container.querySelector('canvas') || container.classList.contains('chart-card')) {
            container.classList.add('chart-container-3d');
        }
    });

    // Enhance buttons with 3D effects
    const buttons = document.querySelectorAll('.btn-primary, .btn-success, .btn-warning');
    buttons.forEach(btn => {
        btn.classList.add('btn-3d');
    });
}

// Initialize charts with enhanced 3D styling
function initCharts() {
    // Revenue Chart (Doughnut)
    const revenueCtx = document.getElementById('revenueChart');
    if (revenueCtx) {
        new Chart(revenueCtx, {
            type: 'doughnut',
            data: {
                labels: ['Car Rentals', 'Service Fees', 'Late Fees', 'Insurance'],
                datasets: [{
                    data: [65, 20, 10, 5],
                    backgroundColor: [
                        'rgba(52, 152, 219, 0.8)',
                        'rgba(46, 204, 113, 0.8)',
                        'rgba(241, 196, 15, 0.8)',
                        'rgba(231, 76, 60, 0.8)'
                    ],
                    borderColor: [
                        'rgba(52, 152, 219, 1)',
                        'rgba(46, 204, 113, 1)',
                        'rgba(241, 196, 15, 1)',
                        'rgba(231, 76, 60, 1)'
                    ],
                    borderWidth: 3,
                    hoverOffset: 15
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            padding: 20,
                            usePointStyle: true,
                            font: {
                                size: 12,
                                weight: 'bold'
                            }
                        }
                    },
                    tooltip: {
                        backgroundColor: 'rgba(0, 0, 0, 0.8)',
                        titleColor: 'white',
                        bodyColor: 'white',
                        borderColor: 'rgba(255, 255, 255, 0.2)',
                        borderWidth: 1,
                        cornerRadius: 10,
                        displayColors: true
                    }
                },
                elements: {
                    arc: {
                        borderRadius: 5
                    }
                }
            }
        });
    }

    // Bookings Trend Chart (Line)
    const bookingsTrendCtx = document.getElementById('bookingsTrendChart');
    if (bookingsTrendCtx) {
        new Chart(bookingsTrendCtx, {
            type: 'line',
            data: {
                labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
                datasets: [{
                    label: 'Bookings',
                    data: [65, 59, 80, 81, 56, 95],
                    fill: true,
                    backgroundColor: 'rgba(52, 152, 219, 0.1)',
                    borderColor: 'rgba(52, 152, 219, 1)',
                    borderWidth: 3,
                    pointBackgroundColor: 'rgba(52, 152, 219, 1)',
                    pointBorderColor: 'white',
                    pointBorderWidth: 3,
                    pointRadius: 6,
                    pointHoverRadius: 8,
                    tension: 0.4
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: false
                    },
                    tooltip: {
                        backgroundColor: 'rgba(0, 0, 0, 0.8)',
                        titleColor: 'white',
                        bodyColor: 'white',
                        borderColor: 'rgba(255, 255, 255, 0.2)',
                        borderWidth: 1,
                        cornerRadius: 10
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        grid: {
                            color: 'rgba(0, 0, 0, 0.1)'
                        },
                        ticks: {
                            font: {
                                weight: 'bold'
                            }
                        }
                    },
                    x: {
                        grid: {
                            color: 'rgba(0, 0, 0, 0.1)'
                        },
                        ticks: {
                            font: {
                                weight: 'bold'
                            }
                        }
                    }
                },
                elements: {
                    point: {
                        hoverBackgroundColor: 'rgba(52, 152, 219, 1)',
                        hoverBorderColor: 'white'
                    }
                }
            }
        });
    }

    // Car Status Chart (Bar)
    const carStatusCtx = document.getElementById('carStatusChart');
    if (carStatusCtx) {
        new Chart(carStatusCtx, {
            type: 'bar',
            data: {
                labels: ['Approved', 'Pending', 'Rejected'],
                datasets: [{
                    label: 'Cars',
                    data: [120, 25, 8],
                    backgroundColor: [
                        'rgba(46, 204, 113, 0.8)',
                        'rgba(241, 196, 15, 0.8)',
                        'rgba(231, 76, 60, 0.8)'
                    ],
                    borderColor: [
                        'rgba(46, 204, 113, 1)',
                        'rgba(241, 196, 15, 1)',
                        'rgba(231, 76, 60, 1)'
                    ],
                    borderWidth: 2,
                    borderRadius: 8
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: false
                    },
                    tooltip: {
                        backgroundColor: 'rgba(0, 0, 0, 0.8)',
                        titleColor: 'white',
                        bodyColor: 'white',
                        borderColor: 'rgba(255, 255, 255, 0.2)',
                        borderWidth: 1,
                        cornerRadius: 10
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        grid: {
                            color: 'rgba(0, 0, 0, 0.1)'
                        },
                        ticks: {
                            font: {
                                weight: 'bold'
                            }
                        }
                    },
                    x: {
                        grid: {
                            display: false
                        },
                        ticks: {
                            font: {
                                weight: 'bold'
                            }
                        }
                    }
                }
            }
        });
    }
}

// Initialize interactive 3D elements
function initInteractiveElements() {
    // Add hover effects to cards
    const cards = document.querySelectorAll('.stats-card-3d, .chart-container-3d');
    cards.forEach(card => {
        card.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-10px) rotateX(5deg) rotateY(2deg)';
        });
        
        card.addEventListener('mouseleave', function() {
            this.style.transform = 'translateY(0) rotateX(0) rotateY(0)';
        });
    });

    // Add ripple effect to buttons
    const buttons = document.querySelectorAll('.btn-3d');
    buttons.forEach(button => {
        button.addEventListener('click', function(e) {
            createRipple(e, this);
        });
    });

    // Initialize parallax scroll effects
    window.addEventListener('scroll', function() {
        const scrolled = window.pageYOffset;
        const parallaxElements = document.querySelectorAll('.floating-3d');
        
        parallaxElements.forEach(element => {
            const speed = 0.5;
            element.style.transform = `translateY(${scrolled * speed}px)`;
        });
    });
}

// Create ripple effect for buttons
function createRipple(event, button) {
    const circle = document.createElement('span');
    const diameter = Math.max(button.clientWidth, button.clientHeight);
    const radius = diameter / 2;

    circle.style.width = circle.style.height = `${diameter}px`;
    circle.style.left = `${event.clientX - button.offsetLeft - radius}px`;
    circle.style.top = `${event.clientY - button.offsetTop - radius}px`;
    circle.classList.add('ripple');

    const ripple = button.getElementsByClassName('ripple')[0];
    if (ripple) {
        ripple.remove();
    }

    button.appendChild(circle);
}

// Add ripple CSS dynamically
const rippleStyle = document.createElement('style');
rippleStyle.textContent = `
    .ripple {
        position: absolute;
        border-radius: 50%;
        background: rgba(255, 255, 255, 0.6);
        transform: scale(0);
        animation: ripple-animation 0.6s linear;
        pointer-events: none;
    }
    
    @keyframes ripple-animation {
        to {
            transform: scale(4);
            opacity: 0;
        }
    }
`;
document.head.appendChild(rippleStyle);

// Animated counter for stats
function animateCounter(element, target, duration = 2000) {
    let start = 0;
    const increment = target / (duration / 16);
    
    const timer = setInterval(() => {
        start += increment;
        element.textContent = Math.floor(start);
        
        if (start >= target) {
            element.textContent = target;
            clearInterval(timer);
        }
    }, 16);
}

// Initialize counters on page load
window.addEventListener('load', function() {
    const counters = document.querySelectorAll('.stats-content h3');
    counters.forEach(counter => {
        const target = parseInt(counter.textContent.replace(/[^0-9]/g, ''));
        if (!isNaN(target)) {
            counter.textContent = '0';
            setTimeout(() => animateCounter(counter, target), 500);
        }
    });
});

// Progressive enhancement for older browsers
if (!window.CSS || !CSS.supports('backdrop-filter', 'blur(10px)')) {
    document.documentElement.classList.add('no-backdrop-filter');
}

// Preload Chart.js if not already loaded
if (typeof Chart === 'undefined') {
    const script = document.createElement('script');
    script.src = 'https://cdn.jsdelivr.net/npm/chart.js';
    script.onload = function() {
        initCharts();
    };
    document.head.appendChild(script);
}
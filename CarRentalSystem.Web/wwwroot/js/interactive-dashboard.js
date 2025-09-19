/**
 * 🚗 Interactive User Dashboard - Enhanced 3D Effects & Animations
 * ==================================================================
 */

class InteractiveDashboard {
    constructor() {
        this.init();
        this.setupEventListeners();
        this.startAnimations();
        this.setupParticleEffects();
        this.setupChartAnimations();
    }

    init() {
        console.log('🎪 Interactive Dashboard Initialized');
        
        // Initialize dashboard components
        this.initializeMetrics();
        this.initializeCharts();
        this.initializeSidebar();
        this.initializeMobileSupport();
        
        // Add loading animation
        this.showLoadingAnimation();
        
        // Setup intersection observer for animations
        this.setupIntersectionObserver();
    }

    setupEventListeners() {
        // Sidebar navigation
        document.querySelectorAll('.enhanced-nav-link').forEach(link => {
            link.addEventListener('click', (e) => {
                this.handleNavClick(e);
            });
            
            link.addEventListener('mouseenter', (e) => {
                this.handleNavHover(e);
            });
        });

        // Metric cards
        document.querySelectorAll('.metric-card').forEach(card => {
            card.addEventListener('click', (e) => {
                this.handleMetricClick(e);
            });
            
            card.addEventListener('mouseenter', (e) => {
                this.handleMetricHover(e);
            });
        });

        // Quick action cards
        document.querySelectorAll('.quick-action-card').forEach(card => {
            card.addEventListener('click', (e) => {
                this.handleQuickActionClick(e);
            });
        });

        // Booking items
        document.querySelectorAll('.booking-item').forEach(item => {
            item.addEventListener('click', (e) => {
                this.handleBookingClick(e);
            });
        });

        // Chart controls
        document.querySelectorAll('.chart-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                this.handleChartControlClick(e);
            });
        });

        // Mobile sidebar toggle
        const mobileToggle = document.querySelector('.sidebar-toggle-mobile');
        if (mobileToggle) {
            mobileToggle.addEventListener('click', () => {
                this.toggleMobileSidebar();
            });
        }

        // Window resize
        window.addEventListener('resize', () => {
            this.handleResize();
        });

        // Scroll effects
        window.addEventListener('scroll', () => {
            this.handleScroll();
        });
    }

    initializeMetrics() {
        const metrics = document.querySelectorAll('.metric-card');
        metrics.forEach((metric, index) => {
            // Add staggered animation delay
            metric.style.animationDelay = `${index * 0.1}s`;
            metric.classList.add('animate-fade-in');
            
            // Animate numbers on load
            this.animateNumbers(metric);
        });
    }

    initializeCharts() {
        // Initialize Chart.js charts with enhanced styling
        this.setupBookingTrendsChart();
        this.setupSpendingAnalyticsChart();
    }

    initializeSidebar() {
        // Add interactive effects to sidebar
        const sidebar = document.querySelector('.user-sidebar');
        if (sidebar) {
            // Add hover effects
            sidebar.addEventListener('mouseenter', () => {
                sidebar.classList.add('sidebar-hover');
            });
            
            sidebar.addEventListener('mouseleave', () => {
                sidebar.classList.remove('sidebar-hover');
            });
        }

        // Initialize quick stats animation
        this.animateQuickStats();
    }

    initializeMobileSupport() {
        // Check if mobile
        if (window.innerWidth <= 768) {
            document.body.classList.add('mobile-view');
            this.setupMobileGestures();
        }
    }

    setupParticleEffects() {
        // Create floating particles
        this.createFloatingParticles();
        
        // Create background dots
        this.createBackgroundDots();
    }

    createFloatingParticles() {
        const container = document.querySelector('.dashboard-3d-container');
        if (!container) return;

        // Create particle container
        const particleContainer = document.createElement('div');
        particleContainer.className = 'floating-particles';
        particleContainer.style.cssText = `
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            pointer-events: none;
            z-index: 1;
        `;

        // Create particles
        for (let i = 0; i < 20; i++) {
            const particle = document.createElement('div');
            particle.className = 'floating-particle';
            particle.style.cssText = `
                position: absolute;
                width: 4px;
                height: 4px;
                background: linear-gradient(135deg, #2323FF, #4A4AFF);
                border-radius: 50%;
                opacity: 0.6;
                animation: floatParticle ${5 + Math.random() * 10}s ease-in-out infinite;
                left: ${Math.random() * 100}%;
                top: ${Math.random() * 100}%;
                animation-delay: ${Math.random() * 5}s;
            `;
            particleContainer.appendChild(particle);
        }

        container.appendChild(particleContainer);

        // Add CSS animation
        const style = document.createElement('style');
        style.textContent = `
            @keyframes floatParticle {
                0%, 100% {
                    transform: translateY(0px) translateX(0px);
                    opacity: 0.6;
                }
                25% {
                    transform: translateY(-20px) translateX(10px);
                    opacity: 0.8;
                }
                50% {
                    transform: translateY(-10px) translateX(-10px);
                    opacity: 0.4;
                }
                75% {
                    transform: translateY(-30px) translateX(5px);
                    opacity: 0.7;
                }
            }
        `;
        document.head.appendChild(style);
    }

    createBackgroundDots() {
        const container = document.querySelector('.dashboard-3d-container');
        if (!container) return;

        const dotsContainer = document.createElement('div');
        dotsContainer.className = 'background-dots';
        dotsContainer.style.cssText = `
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            pointer-events: none;
            z-index: 0;
            background-image: 
                radial-gradient(circle at 20% 30%, rgba(35, 35, 255, 0.1) 1px, transparent 1px),
                radial-gradient(circle at 80% 70%, rgba(240, 147, 251, 0.1) 1px, transparent 1px);
            background-size: 50px 50px, 80px 80px;
            animation: dotsMove 20s linear infinite;
        `;

        container.appendChild(dotsContainer);

        // Add CSS animation
        const style = document.createElement('style');
        style.textContent = `
            @keyframes dotsMove {
                0% { transform: translateX(0) translateY(0); }
                100% { transform: translateX(-50px) translateY(-50px); }
            }
        `;
        document.head.appendChild(style);
    }

    setupChartAnimations() {
        // Enhanced chart animations
        this.setupBookingTrendsChart();
        this.setupSpendingAnalyticsChart();
    }

    setupBookingTrendsChart() {
        const ctx = document.getElementById('bookingTrendsChart');
        if (!ctx) return;

        const chart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul'],
                datasets: [{
                    label: 'Bookings',
                    data: [12, 19, 3, 5, 2, 3, 8],
                    backgroundColor: 'rgba(35, 35, 255, 0.8)',
                    borderColor: '#2323FF',
                    borderWidth: 2,
                    borderRadius: 8,
                    borderSkipped: false,
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        labels: {
                            color: '#FFFFFF',
                            font: {
                                family: 'Inter',
                                size: 14,
                                weight: '600'
                            }
                        }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        grid: {
                            color: 'rgba(255, 255, 255, 0.1)'
                        },
                        ticks: {
                            color: '#FFFFFF',
                            font: {
                                family: 'Inter',
                                size: 12
                            }
                        }
                    },
                    x: {
                        grid: {
                            color: 'rgba(255, 255, 255, 0.1)'
                        },
                        ticks: {
                            color: '#FFFFFF',
                            font: {
                                family: 'Inter',
                                size: 12
                            }
                        }
                    }
                },
                animation: {
                    duration: 2000,
                    easing: 'easeInOutQuart'
                }
            }
        });
    }

    setupSpendingAnalyticsChart() {
        const ctx = document.getElementById('spendingAnalyticsChart');
        if (!ctx) return;

        const chart = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: ['Car Rental', 'Insurance', 'Fuel', 'Maintenance', 'Other'],
                datasets: [{
                    data: [40, 20, 15, 15, 10],
                    backgroundColor: [
                        'rgba(35, 35, 255, 0.8)',
                        'rgba(240, 147, 251, 0.8)',
                        'rgba(0, 212, 255, 0.8)',
                        'rgba(236, 72, 153, 0.8)',
                        'rgba(245, 158, 11, 0.8)'
                    ],
                    borderColor: [
                        '#2323FF',
                        '#F093FB',
                        '#00D4FF',
                        '#EC4899',
                        '#F59E0B'
                    ],
                    borderWidth: 3,
                    hoverOffset: 10
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            color: '#FFFFFF',
                            font: {
                                family: 'Inter',
                                size: 12,
                                weight: '500'
                            },
                            padding: 20,
                            usePointStyle: true
                        }
                    }
                },
                animation: {
                    duration: 2000,
                    easing: 'easeInOutQuart'
                }
            }
        });
    }

    animateNumbers(element) {
        const numberElement = element.querySelector('.metric-number');
        if (!numberElement) return;

        const targetNumber = parseInt(numberElement.textContent);
        const duration = 2000;
        const startTime = performance.now();

        const animate = (currentTime) => {
            const elapsed = currentTime - startTime;
            const progress = Math.min(elapsed / duration, 1);
            
            // Easing function
            const easeOutQuart = 1 - Math.pow(1 - progress, 4);
            const currentNumber = Math.floor(easeOutQuart * targetNumber);
            
            numberElement.textContent = currentNumber;
            
            if (progress < 1) {
                requestAnimationFrame(animate);
            } else {
                numberElement.textContent = targetNumber;
            }
        };

        numberElement.textContent = '0';
        requestAnimationFrame(animate);
    }

    animateQuickStats() {
        const stats = document.querySelectorAll('.quick-stat-item');
        stats.forEach((stat, index) => {
            setTimeout(() => {
                stat.classList.add('animate-fade-in');
            }, index * 200);
        });
    }

    setupIntersectionObserver() {
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('animate-in');
                }
            });
        }, {
            threshold: 0.1,
            rootMargin: '0px 0px -50px 0px'
        });

        // Observe elements for animation
        document.querySelectorAll('.metric-card, .chart-card, .booking-item, .quick-action-card').forEach(el => {
            observer.observe(el);
        });
    }

    handleNavClick(e) {
        // Add click animation
        e.target.closest('.enhanced-nav-link').classList.add('nav-clicked');
        setTimeout(() => {
            e.target.closest('.enhanced-nav-link').classList.remove('nav-clicked');
        }, 300);

        // Add ripple effect
        this.createRippleEffect(e);
    }

    handleNavHover(e) {
        // Add hover sound effect (if desired)
        // this.playHoverSound();
    }

    handleMetricClick(e) {
        const card = e.target.closest('.metric-card');
        card.classList.add('metric-clicked');
        
        // Add glow effect
        card.style.boxShadow = '0 0 50px rgba(35, 35, 255, 0.6)';
        
        setTimeout(() => {
            card.classList.remove('metric-clicked');
            card.style.boxShadow = '';
        }, 500);

        // Show detailed view (if implemented)
        // this.showMetricDetails(card);
    }

    handleMetricHover(e) {
        const card = e.target.closest('.metric-card');
        const icon = card.querySelector('.metric-icon');
        
        if (icon) {
            icon.style.transform = 'scale(1.1) rotate(10deg)';
        }
    }

    handleQuickActionClick(e) {
        const card = e.target.closest('.quick-action-card');
        
        // Add click animation
        card.classList.add('action-clicked');
        setTimeout(() => {
            card.classList.remove('action-clicked');
        }, 300);

        // Create ripple effect
        this.createRippleEffect(e);
    }

    handleBookingClick(e) {
        const item = e.target.closest('.booking-item');
        
        // Add click animation
        item.classList.add('booking-clicked');
        setTimeout(() => {
            item.classList.remove('booking-clicked');
        }, 300);

        // Show booking details (if implemented)
        // this.showBookingDetails(item);
    }

    handleChartControlClick(e) {
        const btn = e.target.closest('.chart-btn');
        
        // Remove active class from siblings
        btn.parentNode.querySelectorAll('.chart-btn').forEach(b => {
            b.classList.remove('active');
        });
        
        // Add active class to clicked button
        btn.classList.add('active');

        // Add click animation
        btn.style.transform = 'scale(0.95)';
        setTimeout(() => {
            btn.style.transform = '';
        }, 150);

        // Update chart data (if implemented)
        // this.updateChartData(btn.dataset.period);
    }

    toggleMobileSidebar() {
        const sidebar = document.querySelector('.user-sidebar');
        sidebar.classList.toggle('show');
        
        // Add overlay
        if (sidebar.classList.contains('show')) {
            this.createMobileOverlay();
        } else {
            this.removeMobileOverlay();
        }
    }

    createMobileOverlay() {
        const overlay = document.createElement('div');
        overlay.className = 'mobile-sidebar-overlay';
        overlay.style.cssText = `
            position: fixed;
            top: 0;
            left: 0;
            width: 100vw;
            height: 100vh;
            background: rgba(0, 0, 0, 0.5);
            backdrop-filter: blur(5px);
            z-index: 1040;
            opacity: 0;
            transition: opacity 0.3s ease;
        `;

        overlay.addEventListener('click', () => {
            this.toggleMobileSidebar();
        });

        document.body.appendChild(overlay);
        
        // Fade in
        requestAnimationFrame(() => {
            overlay.style.opacity = '1';
        });
    }

    removeMobileOverlay() {
        const overlay = document.querySelector('.mobile-sidebar-overlay');
        if (overlay) {
            overlay.style.opacity = '0';
            setTimeout(() => {
                overlay.remove();
            }, 300);
        }
    }

    setupMobileGestures() {
        // Add swipe gestures for mobile
        let startX = 0;
        let startY = 0;

        document.addEventListener('touchstart', (e) => {
            startX = e.touches[0].clientX;
            startY = e.touches[0].clientY;
        });

        document.addEventListener('touchend', (e) => {
            const endX = e.changedTouches[0].clientX;
            const endY = e.changedTouches[0].clientY;
            const diffX = startX - endX;
            const diffY = startY - endY;

            // Swipe left to open sidebar
            if (diffX > 50 && Math.abs(diffY) < 100) {
                this.toggleMobileSidebar();
            }
        });
    }

    handleResize() {
        // Update mobile state
        if (window.innerWidth <= 768) {
            document.body.classList.add('mobile-view');
        } else {
            document.body.classList.remove('mobile-view');
            this.removeMobileOverlay();
            document.querySelector('.user-sidebar').classList.remove('show');
        }

        // Resize charts
        this.resizeCharts();
    }

    handleScroll() {
        // Parallax effects
        const scrolled = window.pageYOffset;
        const parallaxElements = document.querySelectorAll('.parallax-element');
        
        parallaxElements.forEach(element => {
            const speed = element.dataset.speed || 0.5;
            element.style.transform = `translateY(${scrolled * speed}px)`;
        });
    }

    createRippleEffect(e) {
        const button = e.target.closest('button, .enhanced-nav-link, .quick-action-card, .metric-card');
        if (!button) return;

        const ripple = document.createElement('span');
        const rect = button.getBoundingClientRect();
        const size = Math.max(rect.width, rect.height);
        const x = e.clientX - rect.left - size / 2;
        const y = e.clientY - rect.top - size / 2;

        ripple.style.cssText = `
            position: absolute;
            width: ${size}px;
            height: ${size}px;
            left: ${x}px;
            top: ${y}px;
            background: rgba(255, 255, 255, 0.3);
            border-radius: 50%;
            transform: scale(0);
            animation: ripple 0.6s ease-out;
            pointer-events: none;
            z-index: 1000;
        `;

        button.style.position = 'relative';
        button.style.overflow = 'hidden';
        button.appendChild(ripple);

        // Add ripple animation
        const style = document.createElement('style');
        style.textContent = `
            @keyframes ripple {
                to {
                    transform: scale(2);
                    opacity: 0;
                }
            }
        `;
        document.head.appendChild(style);

        setTimeout(() => {
            ripple.remove();
        }, 600);
    }

    showLoadingAnimation() {
        // Show loading animation on page load
        const loader = document.createElement('div');
        loader.className = 'dashboard-loader';
        loader.style.cssText = `
            position: fixed;
            top: 0;
            left: 0;
            width: 100vw;
            height: 100vh;
            background: linear-gradient(135deg, #0A0A0A 0%, #1A1A1A 50%, #2A2A2A 100%);
            display: flex;
            align-items: center;
            justify-content: center;
            z-index: 9999;
            opacity: 1;
            transition: opacity 0.5s ease;
        `;

        loader.innerHTML = `
            <div class="loader-content">
                <div class="loader-icon">
                    <i class="bi bi-car-front-fill"></i>
                </div>
                <div class="loader-text">Loading Dashboard...</div>
            </div>
        `;

        // Add loader styles
        const style = document.createElement('style');
        style.textContent = `
            .loader-content {
                text-align: center;
                color: #FFFFFF;
            }
            .loader-icon {
                font-size: 4rem;
                color: #2323FF;
                margin-bottom: 1rem;
                animation: pulse 2s ease-in-out infinite;
            }
            .loader-text {
                font-size: 1.5rem;
                font-weight: 600;
                font-family: 'Inter', sans-serif;
            }
        `;
        document.head.appendChild(style);

        document.body.appendChild(loader);

        // Hide loader after page load
        window.addEventListener('load', () => {
            setTimeout(() => {
                loader.style.opacity = '0';
                setTimeout(() => {
                    loader.remove();
                }, 500);
            }, 1000);
        });
    }

    startAnimations() {
        // Start continuous animations
        this.startFloatingAnimations();
        this.startGlowAnimations();
    }

    startFloatingAnimations() {
        // Add floating animation to elements
        const floatingElements = document.querySelectorAll('.metric-card, .chart-card');
        floatingElements.forEach((element, index) => {
            element.style.animationDelay = `${index * 0.2}s`;
            element.classList.add('animate-float');
        });
    }

    startGlowAnimations() {
        // Add glow animation to interactive elements
        const glowElements = document.querySelectorAll('.sidebar-brand, .metric-icon, .action-icon');
        glowElements.forEach((element, index) => {
            element.style.animationDelay = `${index * 0.3}s`;
            element.classList.add('glow-effect');
        });
    }

    resizeCharts() {
        // Resize charts on window resize
        if (window.Chart) {
            Chart.helpers.each(Chart.instances, (chart) => {
                chart.resize();
            });
        }
    }
}

// Initialize dashboard when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    new InteractiveDashboard();
});

// Add CSS animations
const dashboardStyles = document.createElement('style');
dashboardStyles.textContent = `
    .nav-clicked {
        transform: scale(0.95) !important;
        transition: transform 0.1s ease !important;
    }
    
    .metric-clicked {
        transform: scale(0.98) !important;
        transition: transform 0.2s ease !important;
    }
    
    .action-clicked {
        transform: scale(0.95) !important;
        transition: transform 0.1s ease !important;
    }
    
    .booking-clicked {
        transform: translateX(5px) scale(0.98) !important;
        transition: transform 0.2s ease !important;
    }
    
    .sidebar-hover {
        box-shadow: 0 0 50px rgba(35, 35, 255, 0.3) !important;
    }
    
    .mobile-view .user-sidebar {
        transform: translateX(-100%);
    }
    
    .mobile-view .user-sidebar.show {
        transform: translateX(0);
    }
    
    .animate-fade-in {
        animation: fadeInScale 0.8s ease-out forwards;
    }
    
    .animate-in {
        animation: slideInUp 0.6s ease-out forwards;
    }
    
    .animate-float {
        animation: float 3s ease-in-out infinite;
    }
    
    .glow-effect {
        animation: glow 2s ease-in-out infinite alternate;
    }
    
    @keyframes fadeInScale {
        from {
            opacity: 0;
            transform: scale(0.9);
        }
        to {
            opacity: 1;
            transform: scale(1);
        }
    }
    
    @keyframes slideInUp {
        from {
            opacity: 0;
            transform: translateY(30px);
        }
        to {
            opacity: 1;
            transform: translateY(0);
        }
    }
    
    @keyframes float {
        0%, 100% {
            transform: translateY(0px);
        }
        50% {
            transform: translateY(-10px);
        }
    }
    
    @keyframes glow {
        0% {
            filter: brightness(1);
        }
        100% {
            filter: brightness(1.2);
        }
    }
    
    @keyframes pulse {
        0%, 100% {
            transform: scale(1);
        }
        50% {
            transform: scale(1.05);
        }
    }
`;

document.head.appendChild(dashboardStyles);

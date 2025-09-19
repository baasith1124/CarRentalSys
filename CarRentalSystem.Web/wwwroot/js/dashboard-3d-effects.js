// 3D Dashboard Effects JavaScript
document.addEventListener('DOMContentLoaded', function() {
    // Initialize 3D scene elements
    initializeDashboardScene3D();
    
    // Initialize enhanced animations
    initializeDashboardAnimations();
    
    // Parallax scrolling effect
    setupParallaxScrolling();
    
    // Add floating animation to existing elements
    addFloatingAnimations();
});

function initializeDashboardScene3D() {
    // Create animated dots
    createDashboardAnimatedDots();
    
    // Create floating particles
    createDashboardFloatingParticles();
    
    // Create floating 3D elements
    createDashboardFloating3DElements();
}

function createDashboardAnimatedDots() {
    const container = document.querySelector('.dashboard-animated-dots-container');
    if (!container) return;
    
    for (let i = 0; i < 25; i++) {
        const dot = document.createElement('div');
        dot.className = 'dashboard-floating-dot';
        dot.style.left = Math.random() * 100 + '%';
        dot.style.top = Math.random() * 100 + '%';
        dot.style.width = Math.random() * 8 + 4 + 'px';
        dot.style.height = dot.style.width;
        dot.style.animationDelay = Math.random() * 8 + 's';
        dot.style.animationDuration = (Math.random() * 4 + 6) + 's';
        container.appendChild(dot);
    }
}

function createDashboardFloatingParticles() {
    const container = document.querySelector('.dashboard-particles-3d');
    if (!container) return;
    
    for (let i = 0; i < 20; i++) {
        const particle = document.createElement('div');
        particle.className = 'dashboard-particle-3d';
        particle.style.left = Math.random() * 100 + '%';
        particle.style.top = Math.random() * 100 + '%';
        particle.style.animationDelay = Math.random() * 6 + 's';
        particle.style.animationDuration = (Math.random() * 3 + 4) + 's';
        container.appendChild(particle);
    }
}

function createDashboardFloating3DElements() {
    const container = document.querySelector('.dashboard-floating-3d-container');
    if (!container) return;
    
    for (let i = 0; i < 10; i++) {
        const element = document.createElement('div');
        element.className = 'dashboard-floating-element-3d';
        element.style.left = Math.random() * 100 + '%';
        element.style.top = Math.random() * 100 + '%';
        element.style.width = Math.random() * 60 + 40 + 'px';
        element.style.height = Math.random() * 60 + 40 + 'px';
        element.style.animationDelay = Math.random() * 10 + 's';
        element.style.animationDuration = (Math.random() * 5 + 8) + 's';
        container.appendChild(element);
    }
}

function setupParallaxScrolling() {
    const parallaxLayers = document.querySelectorAll('.parallax-layer');
    
    if (parallaxLayers.length > 0) {
        window.addEventListener('scroll', () => {
            const scrolled = window.pageYOffset;
            const rate = scrolled * -0.5;
            
            parallaxLayers.forEach((layer, index) => {
                const speed = (index + 1) * 0.1;
                layer.style.transform = `translateY(${rate * speed}px)`;
            });
        });
    }
}

function initializeDashboardAnimations() {
    // Animate cards on scroll
    const cards = document.querySelectorAll('.stats-card, .quick-actions-card, .booking-item, .welcome-header');
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.animationDelay = '0s';
                entry.target.classList.add('animate-in');
            }
        });
    }, { threshold: 0.1 });
    
    cards.forEach(card => observer.observe(card));
}

function addFloatingAnimations() {
    // Add floating class to existing elements
    const floatingElements = document.querySelectorAll('.floating-element');
    floatingElements.forEach(element => {
        element.style.animation = 'floatingAnimation 3s ease-in-out infinite';
    });
}

// Add CSS for floating animation
const style = document.createElement('style');
style.textContent = `
    @keyframes floatingAnimation {
        0%, 100% { transform: translateY(0px); }
        50% { transform: translateY(-10px); }
    }
    
    .animate-in {
        animation: slideInUp 0.6s ease-out forwards;
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
`;
document.head.appendChild(style);

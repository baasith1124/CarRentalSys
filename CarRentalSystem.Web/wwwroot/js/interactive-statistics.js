// Interactive Statistics with Counter Animation and 3D Effects
document.addEventListener('DOMContentLoaded', function() {
    console.log('Interactive Statistics script loaded');
    initializeInteractiveStatistics();
});

function initializeInteractiveStatistics() {
    const statItems = document.querySelectorAll('.interactive-stat');
    
    if (statItems.length === 0) {
        console.log('No statistics items found');
        return;
    }
    
    console.log('Found', statItems.length, 'statistics items');
    
    // Initialize intersection observer for scroll animations
    const observerOptions = {
        threshold: 0.3,
        rootMargin: '0px 0px -50px 0px'
    };
    
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const statItem = entry.target;
                animateStatItem(statItem);
                observer.unobserve(statItem);
            }
        });
    }, observerOptions);
    
    // Observe all stat items
    statItems.forEach((item, index) => {
        observer.observe(item);
        
        // Add click interaction
        item.addEventListener('click', function() {
            handleStatClick(this);
        });
        
        // Add hover sound effect (optional)
        item.addEventListener('mouseenter', function() {
            addHoverEffect(this);
        });
        
        item.addEventListener('mouseleave', function() {
            removeHoverEffect(this);
        });
    });
    
    console.log('Interactive statistics initialized');
}

function animateStatItem(statItem) {
    const delay = parseInt(statItem.dataset.delay) || 0;
    const targetValue = parseInt(statItem.dataset.stat);
    const suffix = statItem.dataset.suffix || '';
    const counterElement = statItem.querySelector('.counter-animate');
    
    if (!counterElement) {
        console.warn('Counter element not found for stat item');
        return;
    }
    
    // Add loading animation
    statItem.classList.add('loading');
    
    setTimeout(() => {
        // Remove loading animation
        statItem.classList.remove('loading');
        
        // Add slide-in animation
        statItem.classList.add('animate-in');
        
        // Start counter animation
        animateCounter(counterElement, targetValue, suffix, 2000);
        
        // Add glow effect
        setTimeout(() => {
            statItem.classList.add('animate-counter');
        }, 500);
        
    }, delay);
}

function animateCounter(element, targetValue, suffix, duration) {
    const startValue = 0;
    const startTime = performance.now();
    
    function updateCounter(currentTime) {
        const elapsed = currentTime - startTime;
        const progress = Math.min(elapsed / duration, 1);
        
        // Easing function for smooth animation
        const easeOutQuart = 1 - Math.pow(1 - progress, 4);
        const currentValue = Math.floor(startValue + (targetValue - startValue) * easeOutQuart);
        
        element.textContent = currentValue + suffix;
        
        if (progress < 1) {
            requestAnimationFrame(updateCounter);
        } else {
            // Final value
            element.textContent = targetValue + suffix;
            element.classList.add('counter-animate');
        }
    }
    
    requestAnimationFrame(updateCounter);
}

function handleStatClick(statItem) {
    // Add click animation
    statItem.style.transform = 'translateY(-5px) scale(0.95)';
    
    setTimeout(() => {
        statItem.style.transform = '';
    }, 150);
    
    // Show detailed information (optional)
    showStatDetails(statItem);
    
    // Add ripple effect
    createRippleEffect(statItem);
}

function showStatDetails(statItem) {
    const statLabel = statItem.querySelector('.stat-label-3d span').textContent;
    const statValue = statItem.querySelector('.stat-number-3d').textContent;
    
    // Remove any existing tooltip
    const existingTooltip = statItem.querySelector('.stat-tooltip');
    if (existingTooltip) {
        existingTooltip.remove();
    }
    
    // Create a temporary tooltip or modal
    const tooltip = document.createElement('div');
    tooltip.className = 'stat-tooltip';
    tooltip.innerHTML = `
        <div class="stat-tooltip-content">
            <h4>${statLabel}</h4>
            <p>Current Value: ${statValue}</p>
            <p>Click to learn more about this statistic</p>
        </div>
    `;
    
    // Style the tooltip
    tooltip.style.cssText = `
        position: absolute;
        top: -120px;
        left: 50%;
        transform: translateX(-50%);
        background: rgba(0, 0, 0, 0.95);
        color: white;
        padding: 1.2rem 1.5rem;
        border-radius: 12px;
        backdrop-filter: blur(15px);
        border: 1px solid rgba(59, 130, 246, 0.3);
        z-index: 1000;
        opacity: 0;
        transition: all 0.3s ease;
        pointer-events: none;
        white-space: nowrap;
        box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
        min-width: 200px;
        text-align: center;
    `;
    
    statItem.style.position = 'relative';
    statItem.appendChild(tooltip);
    
    // Show tooltip with animation
    setTimeout(() => {
        tooltip.style.opacity = '1';
        tooltip.style.transform = 'translateX(-50%) translateY(-5px)';
    }, 10);
    
    // Hide tooltip after 4 seconds
    setTimeout(() => {
        tooltip.style.opacity = '0';
        tooltip.style.transform = 'translateX(-50%) translateY(5px)';
        setTimeout(() => {
            if (tooltip.parentNode) {
                tooltip.parentNode.removeChild(tooltip);
            }
        }, 300);
    }, 4000);
}

function createRippleEffect(element) {
    const ripple = document.createElement('div');
    ripple.className = 'stat-ripple';
    
    ripple.style.cssText = `
        position: absolute;
        border-radius: 50%;
        background: rgba(59, 130, 246, 0.4);
        transform: scale(0);
        animation: ripple 0.6s linear;
        pointer-events: none;
        top: 50%;
        left: 50%;
        width: 120px;
        height: 120px;
        margin-left: -60px;
        margin-top: -60px;
        z-index: 1;
    `;
    
    element.appendChild(ripple);
    
    setTimeout(() => {
        if (ripple.parentNode) {
            ripple.parentNode.removeChild(ripple);
        }
    }, 600);
}

function addHoverEffect(statItem) {
    // Add subtle vibration effect
    statItem.style.animation = 'statHover 0.3s ease-in-out';
    
    // Add particle effect (optional)
    createHoverParticles(statItem);
}

function removeHoverEffect(statItem) {
    statItem.style.animation = '';
}

function createHoverParticles(statItem) {
    const particleCount = 5;
    
    for (let i = 0; i < particleCount; i++) {
        const particle = document.createElement('div');
        particle.className = 'stat-particle';
        
        const size = Math.random() * 4 + 2;
        const x = Math.random() * 100;
        const y = Math.random() * 100;
        
        particle.style.cssText = `
            position: absolute;
            width: ${size}px;
            height: ${size}px;
            background: var(--primary-blue-light);
            border-radius: 50%;
            top: ${y}%;
            left: ${x}%;
            animation: particleFloat 1s ease-out forwards;
            pointer-events: none;
            z-index: 10;
        `;
        
        statItem.appendChild(particle);
        
        setTimeout(() => {
            if (particle.parentNode) {
                particle.parentNode.removeChild(particle);
            }
        }, 1000);
    }
}

// Add CSS animations dynamically
function addDynamicStyles() {
    const style = document.createElement('style');
    style.textContent = `
        @keyframes ripple {
            to {
                transform: scale(4);
                opacity: 0;
            }
        }
        
        @keyframes statHover {
            0%, 100% { transform: translateY(-10px) rotateX(5deg) rotateY(5deg); }
            50% { transform: translateY(-12px) rotateX(7deg) rotateY(7deg); }
        }
        
        @keyframes particleFloat {
            0% {
                opacity: 1;
                transform: translateY(0) scale(1);
            }
            100% {
                opacity: 0;
                transform: translateY(-50px) scale(0);
            }
        }
        
        .stat-tooltip-content h4 {
            margin: 0 0 0.5rem 0;
            color: var(--primary-blue-light);
            font-size: 1.1rem;
        }
        
        .stat-tooltip-content p {
            margin: 0.25rem 0;
            font-size: 0.9rem;
            color: rgba(255, 255, 255, 0.8);
        }
    `;
    document.head.appendChild(style);
}

// Initialize dynamic styles
addDynamicStyles();

// Manual trigger function for testing
window.triggerStatAnimation = function() {
    const statItems = document.querySelectorAll('.interactive-stat');
    statItems.forEach((item, index) => {
        setTimeout(() => {
            animateStatItem(item);
        }, index * 200);
    });
};

// Reset statistics function
window.resetStatistics = function() {
    const statItems = document.querySelectorAll('.interactive-stat');
    statItems.forEach(item => {
        const counterElement = item.querySelector('.counter-animate');
        if (counterElement) {
            counterElement.textContent = '0';
        }
        item.classList.remove('animate-in', 'animate-counter', 'loading');
    });
};

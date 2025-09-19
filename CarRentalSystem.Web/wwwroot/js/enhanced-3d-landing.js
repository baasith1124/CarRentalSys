// Enhanced 3D Landing Page Interactions
// ================================

document.addEventListener('DOMContentLoaded', function() {
    initializeCarParticles();
    initializeScrollAnimations();
    initializeParallaxEffects();
    initializeFloatingElements();
    initializeProgressiveLoading();
    initializeWhiteDots();
    initializeNavbarEffects();
    initializeCarCardAnimations();
});

// Car Particles Animation
function initializeCarParticles() {
    const carParticlesContainer = document.querySelector('.car-particles-container');
    if (!carParticlesContainer) return;

    // Create additional dynamic car particles
    const carIcons = [
        'bi-car-front-fill',
        'bi-truck',
        'bi-car-front',
        'bi-truck-front',
        'bi-bus-front',
        'bi-bicycle'
    ];

    const colors = [
        'var(--primary-blue-light)',
        'var(--accent-cyan)',
        'var(--accent-pink)',
        'var(--accent-orange)',
        'var(--success-green)',
        'var(--secondary-purple)'
    ];

    // Add random car particles periodically
    setInterval(() => {
        if (document.hidden) return; // Don't animate when tab is not active
        
        const particle = document.createElement('i');
        const randomIcon = carIcons[Math.floor(Math.random() * carIcons.length)];
        const randomColor = colors[Math.floor(Math.random() * colors.length)];
        
        particle.className = `car-particle bi ${randomIcon}`;
        particle.style.color = randomColor;
        particle.style.top = Math.random() * 80 + '%';
        particle.style.left = '-80px';
        particle.style.fontSize = (Math.random() * 1.5 + 1.5) + 'rem';
        particle.style.animationDuration = (Math.random() * 10 + 15) + 's';
        particle.style.animationDelay = '0s';
        
        carParticlesContainer.appendChild(particle);
        
        // Remove particle after animation
        setTimeout(() => {
            if (particle.parentNode) {
                particle.parentNode.removeChild(particle);
            }
        }, 25000);
    }, 3000);
}

// Enhanced Scroll Animations
function initializeScrollAnimations() {
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('revealed');
                
                // Add staggered animation for child elements
                const childElements = entry.target.querySelectorAll('.stat-item-3d, .step-3d, .testimonial-card-3d, .card-3d-interactive');
                childElements.forEach((child, index) => {
                    setTimeout(() => {
                        child.style.animationDelay = `${index * 0.1}s`;
                        child.classList.add('revealed');
                    }, index * 100);
                });
            }
        });
    }, observerOptions);

    // Observe all scroll reveal elements
    document.querySelectorAll('.scroll-reveal-3d').forEach(element => {
        observer.observe(element);
    });
}

// Parallax Effects
function initializeParallaxEffects() {
    let ticking = false;

    function updateParallax() {
        const scrolled = window.pageYOffset;
        const parallaxLayers = document.querySelectorAll('.parallax-layer');
        
        parallaxLayers.forEach((layer, index) => {
            const speed = 0.5 + (index * 0.1);
            const yPos = -(scrolled * speed);
            layer.style.transform = `translateY(${yPos}px)`;
        });

        // Parallax for floating elements
        const floatingElements = document.querySelectorAll('.floating-element-3d');
        floatingElements.forEach((element, index) => {
            const speed = 0.2 + (index * 0.05);
            const yPos = scrolled * speed;
            const xPos = Math.sin(scrolled * 0.001 + index) * 20;
            element.style.transform = `translateX(${xPos}px) translateY(${yPos}px)`;
        });

        ticking = false;
    }

    function requestTick() {
        if (!ticking) {
            requestAnimationFrame(updateParallax);
            ticking = true;
        }
    }

    window.addEventListener('scroll', requestTick, { passive: true });
}

// Dynamic Floating Elements
function initializeFloatingElements() {
    const floatingContainer = document.querySelector('.floating-3d-container');
    if (!floatingContainer) return;

    // Create dynamic floating geometric shapes
    const shapes = [
        { class: 'cube-3d', icon: null },
        { class: 'sphere-3d', icon: null },
        { class: 'pyramid-3d', icon: null },
        { class: 'diamond-3d', icon: null }
    ];

    shapes.forEach((shape, index) => {
        const element = document.createElement('div');
        element.className = `floating-element-3d floating-element-${index + 1}`;
        
        const shapeDiv = document.createElement('div');
        shapeDiv.className = `shape-3d ${shape.class}`;
        
        element.appendChild(shapeDiv);
        floatingContainer.appendChild(element);
    });
}

// Progressive Loading Animation
function initializeProgressiveLoading() {
    // Add loading states
    document.body.classList.add('loading');
    
    // Simulate progressive loading
    setTimeout(() => {
        document.body.classList.remove('loading');
        document.body.classList.add('loaded');
        
        // Trigger hero animations
        const heroContent = document.querySelector('.hero-content-3d');
        if (heroContent) {
            heroContent.style.opacity = '1';
            heroContent.style.transform = 'translateZ(0) rotateX(0deg)';
        }
    }, 500);
}

// Mouse Movement 3D Effects
document.addEventListener('mousemove', function(e) {
    if (window.innerWidth < 768) return; // Skip on mobile
    
    const mouseX = (e.clientX / window.innerWidth) * 2 - 1;
    const mouseY = (e.clientY / window.innerHeight) * 2 - 1;
    
    // Apply subtle 3D rotation to cards based on mouse position
    const cards = document.querySelectorAll('.card-3d-interactive, .step-3d, .testimonial-card-3d');
    cards.forEach(card => {
        const rect = card.getBoundingClientRect();
        const cardCenterX = rect.left + rect.width / 2;
        const cardCenterY = rect.top + rect.height / 2;
        
        const distanceX = (e.clientX - cardCenterX) / rect.width;
        const distanceY = (e.clientY - cardCenterY) / rect.height;
        
        if (Math.abs(distanceX) < 1 && Math.abs(distanceY) < 1) {
            const rotateX = distanceY * 5;
            const rotateY = distanceX * 5;
            
            card.style.transform = `rotateX(${rotateX}deg) rotateY(${rotateY}deg) translateZ(10px)`;
        }
    });
});

// Performance Optimization
function optimizePerformance() {
    // Disable animations on low-end devices
    if (navigator.hardwareConcurrency < 4) {
        document.body.classList.add('reduced-animations');
    }
    
    // Pause animations when tab is not visible
    document.addEventListener('visibilitychange', function() {
        if (document.hidden) {
            document.body.classList.add('paused-animations');
        } else {
            document.body.classList.remove('paused-animations');
        }
    });
}

// Initialize performance optimizations
optimizePerformance();

// Smooth scroll for navigation links
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        e.preventDefault();
        const target = document.querySelector(this.getAttribute('href'));
        if (target) {
            target.scrollIntoView({
                behavior: 'smooth',
                block: 'start'
            });
        }
    });
});

// Enhanced Live Chat Function
function openLiveChat() {
    // Create a modal or integrate with actual chat service
    const chatModal = document.createElement('div');
    chatModal.className = 'chat-modal-3d';
    chatModal.innerHTML = `
        <div class="chat-container-3d">
            <div class="chat-header-3d">
                <h4>Live Chat Support</h4>
                <button class="close-chat" onclick="closeLiveChat()">&times;</button>
            </div>
            <div class="chat-body-3d">
                <p>Hello! How can we help you today?</p>
                <p class="text-muted">This is a demo chat. Integration with live chat service coming soon!</p>
            </div>
            <div class="chat-input-3d">
                <input type="text" placeholder="Type your message..." class="form-control-3d">
                <button class="btn-3d-interactive btn-primary-3d">Send</button>
            </div>
        </div>
    `;
    
    document.body.appendChild(chatModal);
    
    // Add styles for chat modal
    const chatStyles = document.createElement('style');
    chatStyles.textContent = `
        .chat-modal-3d {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0, 0, 0, 0.5);
            display: flex;
            align-items: center;
            justify-content: center;
            z-index: 9999;
            backdrop-filter: blur(10px);
        }
        .chat-container-3d {
            background: var(--glass-bg-light);
            border: 1px solid var(--glass-border-light);
            border-radius: var(--radius-xl);
            width: 400px;
            max-width: 90vw;
            backdrop-filter: var(--glass-backdrop);
            box-shadow: var(--shadow-3d);
        }
        .chat-header-3d {
            padding: var(--spacing-lg);
            border-bottom: 1px solid var(--glass-border);
            display: flex;
            justify-content: space-between;
            align-items: center;
        }
        .chat-body-3d {
            padding: var(--spacing-lg);
            max-height: 300px;
            overflow-y: auto;
        }
        .chat-input-3d {
            padding: var(--spacing-lg);
            border-top: 1px solid var(--glass-border);
            display: flex;
            gap: var(--spacing-sm);
        }
        .close-chat {
            background: none;
            border: none;
            color: var(--text-primary);
            font-size: 1.5rem;
            cursor: pointer;
        }
    `;
    document.head.appendChild(chatStyles);
}

function closeLiveChat() {
    const chatModal = document.querySelector('.chat-modal-3d');
    if (chatModal) {
        chatModal.remove();
    }
}

// Export functions for global use
window.openLiveChat = openLiveChat;
window.closeLiveChat = closeLiveChat;

// White Dots Animation
function initializeWhiteDots() {
    const dotsContainer = document.querySelector('.animated-dots-container');
    if (!dotsContainer) return;

    // Create dynamic floating dots
    function createFloatingDot() {
        const dot = document.createElement('div');
        dot.className = 'floating-dot';
        
        // Random positioning
        dot.style.left = Math.random() * 100 + '%';
        dot.style.animationDelay = Math.random() * 15 + 's';
        dot.style.animationDuration = (Math.random() * 10 + 15) + 's';
        
        // Random size variation
        const size = Math.random() * 3 + 2; // 2-5px
        dot.style.width = size + 'px';
        dot.style.height = size + 'px';
        
        dotsContainer.appendChild(dot);
        
        // Remove dot after animation
        setTimeout(() => {
            if (dot.parentNode) {
                dot.parentNode.removeChild(dot);
            }
        }, 25000);
    }

    // Create dots periodically
    setInterval(() => {
        if (document.hidden) return; // Don't create when tab is not active
        createFloatingDot();
    }, 2000);

    // Create initial dots
    for (let i = 0; i < 10; i++) {
        setTimeout(() => createFloatingDot(), i * 200);
    }
}

// Navbar Effects
function initializeNavbarEffects() {
    const navbar = document.querySelector('.modern-nav');
    if (!navbar) return;

    // Navbar scroll effect
    let lastScrollTop = 0;
    window.addEventListener('scroll', function() {
        const scrollTop = window.pageYOffset || document.documentElement.scrollTop;
        
        if (scrollTop > 100) {
            navbar.classList.add('scrolled');
        } else {
            navbar.classList.remove('scrolled');
        }
        
        // Hide/show navbar on scroll
        if (scrollTop > lastScrollTop && scrollTop > 200) {
            navbar.style.transform = 'translateY(-100%)';
        } else {
            navbar.style.transform = 'translateY(0)';
        }
        
        lastScrollTop = scrollTop;
    }, { passive: true });

    // Add hover effects to navigation items
    const navItems = document.querySelectorAll('.nav-hover-3d');
    navItems.forEach(item => {
        item.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-2px) scale(1.05)';
        });
        
        item.addEventListener('mouseleave', function() {
            this.style.transform = 'translateY(0) scale(1)';
        });
    });

    // Brand icon interaction
    const brandIcon = document.querySelector('.brand-icon-3d');
    if (brandIcon) {
        brandIcon.addEventListener('click', function() {
            this.style.animation = 'none';
            setTimeout(() => {
                this.style.animation = 'brandFloat 4s ease-in-out infinite';
            }, 100);
        });
    }
}

// Enhanced Car Card Animations
function initializeCarCardAnimations() {
    const carCards = document.querySelectorAll('.car-card-enhanced-3d');
    
    carCards.forEach((card, index) => {
        // Staggered entrance animation
        card.style.opacity = '0';
        card.style.transform = 'translateY(50px)';
        
        setTimeout(() => {
            card.style.transition = 'all 0.6s cubic-bezier(0.175, 0.885, 0.32, 1.275)';
            card.style.opacity = '1';
            card.style.transform = 'translateY(0)';
        }, index * 150);
        
        // Enhanced hover interactions
        card.addEventListener('mouseenter', function() {
            // Animate action buttons
            const actionButtons = this.querySelectorAll('.action-btn');
            actionButtons.forEach((btn, i) => {
                setTimeout(() => {
                    btn.style.transform = 'translateY(0)';
                    btn.style.opacity = '1';
                }, i * 100);
            });
            
            // Animate feature items
            const featureItems = this.querySelectorAll('.feature-item');
            featureItems.forEach((item, i) => {
                setTimeout(() => {
                    item.style.transform = 'translateY(-2px) scale(1.02)';
                }, i * 50);
            });
        });
        
        card.addEventListener('mouseleave', function() {
            // Reset animations
            const actionButtons = this.querySelectorAll('.action-btn');
            actionButtons.forEach(btn => {
                btn.style.transform = 'translateY(10px)';
                btn.style.opacity = '0';
            });
            
            const featureItems = this.querySelectorAll('.feature-item');
            featureItems.forEach(item => {
                item.style.transform = 'translateY(0) scale(1)';
            });
        });
        
        // Add ripple effect to view details button
        const viewBtn = card.querySelector('.btn-view-details-3d');
        if (viewBtn) {
            viewBtn.addEventListener('click', function(e) {
                const ripple = document.createElement('div');
                ripple.style.cssText = `
                    position: absolute;
                    border-radius: 50%;
                    background: rgba(255, 255, 255, 0.3);
                    transform: scale(0);
                    animation: ripple 0.6s linear;
                    pointer-events: none;
                `;
                
                const rect = this.getBoundingClientRect();
                const size = Math.max(rect.width, rect.height);
                const x = e.clientX - rect.left - size / 2;
                const y = e.clientY - rect.top - size / 2;
                
                ripple.style.width = ripple.style.height = size + 'px';
                ripple.style.left = x + 'px';
                ripple.style.top = y + 'px';
                
                this.appendChild(ripple);
                
                setTimeout(() => {
                    ripple.remove();
                }, 600);
            });
        }
    });
    
    // Add CSS for ripple animation
    if (!document.getElementById('ripple-styles')) {
        const style = document.createElement('style');
        style.id = 'ripple-styles';
        style.textContent = `
            @keyframes ripple {
                to {
                    transform: scale(4);
                    opacity: 0;
                }
            }
        `;
        document.head.appendChild(style);
    }
    
    // Initialize favorite functionality
    const favoriteButtons = document.querySelectorAll('.favorite-btn');
    favoriteButtons.forEach(btn => {
        btn.addEventListener('click', function(e) {
            e.preventDefault();
            e.stopPropagation();
            
            const icon = this.querySelector('i');
            if (icon.classList.contains('bi-heart')) {
                icon.classList.remove('bi-heart');
                icon.classList.add('bi-heart-fill');
                this.style.color = '#ec4899';
                
                // Add animation
                this.style.transform = 'scale(1.2)';
                setTimeout(() => {
                    this.style.transform = 'scale(1)';
                }, 200);
            } else {
                icon.classList.remove('bi-heart-fill');
                icon.classList.add('bi-heart');
                this.style.color = '';
            }
        });
    });
    
    // Initialize share functionality
    const shareButtons = document.querySelectorAll('.share-btn');
    shareButtons.forEach(btn => {
        btn.addEventListener('click', function(e) {
            e.preventDefault();
            e.stopPropagation();
            
            // Simple share functionality
            if (navigator.share) {
                const carCard = this.closest('.car-card-enhanced-3d');
                const carTitle = carCard.querySelector('.car-title-link').textContent;
                const carUrl = carCard.querySelector('.car-title-link').href;
                
                navigator.share({
                    title: `Check out this ${carTitle}`,
                    text: `Amazing car rental option: ${carTitle}`,
                    url: carUrl
                });
            } else {
                // Fallback: copy URL to clipboard
                const carUrl = this.closest('.car-card-enhanced-3d').querySelector('.car-title-link').href;
                navigator.clipboard.writeText(carUrl).then(() => {
                    // Show feedback
                    const originalText = this.innerHTML;
                    this.innerHTML = '<i class="bi bi-check"></i>';
                    this.style.color = '#10b981';
                    
                    setTimeout(() => {
                        this.innerHTML = originalText;
                        this.style.color = '';
                    }, 2000);
                });
            }
        });
    });
}
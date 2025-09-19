// Enhanced 3D Animations using Anime.js for Car Rental System
class Anime3DEnhanced {
    constructor() {
        this.isInitialized = false;
        this.init();
    }

    init() {
        if (typeof anime === 'undefined') {
            console.warn('Anime.js library not loaded');
            return;
        }
        
        this.setupHeroAnimations();
        this.setupCardAnimations();
        this.setupScrollAnimations();
        this.setupMicroInteractions();
        this.setupParticleEnhancements();
        this.isInitialized = true;
    }

    // Hero Section Animations
    setupHeroAnimations() {
        // Hero title animation with stagger effect
        anime({
            targets: '.hero-title',
            opacity: [0, 1],
            translateY: [100, 0],
            scale: [0.8, 1],
            duration: 1500,
            easing: 'spring(1, 80, 10, 0)',
            delay: 500
        });

        // Hero subtitle with wave effect
        anime({
            targets: '.hero-subtitle',
            opacity: [0, 1],
            translateY: [50, 0],
            duration: 1200,
            easing: 'easeOutExpo',
            delay: 800
        });

        // Hero description with typewriter effect
        this.typewriterEffect('.hero-description', 1000);

        // Animate floating elements with physics
        anime({
            targets: '.floating-element',
            translateY: [
                { value: -20, duration: 2000 },
                { value: 20, duration: 2000 }
            ],
            rotate: [
                { value: '1turn', duration: 4000 }
            ],
            scale: [
                { value: 1.1, duration: 1000 },
                { value: 1, duration: 1000 }
            ],
            loop: true,
            direction: 'alternate',
            easing: 'easeInOutSine',
            delay: (el, i) => i * 200
        });

        // Search form animation
        anime({
            targets: '.search-form-3d',
            opacity: [0, 1],
            translateY: [80, 0],
            scale: [0.9, 1],
            duration: 1000,
            easing: 'easeOutBack',
            delay: 1200
        });
    }

    // Card Animations
    setupCardAnimations() {
        // Car card entrance animations
        const carCards = document.querySelectorAll('.car-card-enhanced-3d');
        
        anime({
            targets: carCards,
            opacity: [0, 1],
            translateY: [100, 0],
            rotateX: [45, 0],
            duration: 1000,
            easing: 'easeOutExpo',
            delay: anime.stagger(200, {start: 300})
        });

        // Benefit cards wave animation
        const benefitCards = document.querySelectorAll('.benefit-card-3d');
        
        anime({
            targets: benefitCards,
            opacity: [0, 1],
            translateY: [60, 0],
            rotateY: [15, 0],
            scale: [0.8, 1],
            duration: 800,
            easing: 'easeOutBack',
            delay: anime.stagger(150, {start: 500})
        });

        // Testimonial cards flip animation
        const testimonialCards = document.querySelectorAll('.testimonial-card-3d');
        
        anime({
            targets: testimonialCards,
            opacity: [0, 1],
            rotateY: [90, 0],
            duration: 1000,
            easing: 'easeOutExpo',
            delay: anime.stagger(200, {start: 800})
        });
    }

    // Scroll-triggered Animations
    setupScrollAnimations() {
        // Stats counter animation
        this.animateCounters();

        // Section reveal animations
        const sections = document.querySelectorAll('section');
        const observerOptions = {
            threshold: 0.1,
            rootMargin: '0px 0px -100px 0px'
        };

        const sectionObserver = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    this.animateSection(entry.target);
                }
            });
        }, observerOptions);

        sections.forEach(section => {
            sectionObserver.observe(section);
        });

        // Parallax scrolling with anime.js
        this.setupParallaxScrolling();
    }

    // Micro Interactions
    setupMicroInteractions() {
        // Button hover animations
        this.setupButtonAnimations();
        
        // Icon animations
        this.setupIconAnimations();
        
        // Form field focus animations
        this.setupFormAnimations();
        
        // Navigation hover effects
        this.setupNavigationAnimations();
    }

    // Enhanced Particle System Integration
    setupParticleEnhancements() {
        // Animate particle entry
        if (window.particleSystem) {
            anime({
                targets: window.particleSystem.particles,
                opacity: [0, 1],
                scale: [0, 1],
                duration: 2000,
                easing: 'easeOutExpo',
                delay: anime.stagger(50, {start: 1000})
            });
        }
    }

    // Utility Functions
    typewriterEffect(selector, delay = 0) {
        const element = document.querySelector(selector);
        if (!element) return;

        const text = element.textContent;
        element.textContent = '';
        element.style.opacity = '1';

        anime({
            targets: element,
            opacity: [0, 1],
            duration: 500,
            delay: delay,
            complete: () => {
                let i = 0;
                const typeInterval = setInterval(() => {
                    element.textContent += text.charAt(i);
                    i++;
                    if (i >= text.length) {
                        clearInterval(typeInterval);
                    }
                }, 30);
            }
        });
    }

    animateCounters() {
        const counters = document.querySelectorAll('.stat-number');
        
        counters.forEach(counter => {
            const target = parseInt(counter.textContent.replace(/[^\d]/g, ''));
            const obj = { count: 0 };
            
            anime({
                targets: obj,
                count: target,
                duration: 2000,
                easing: 'easeOutExpo',
                update: () => {
                    const currentValue = Math.floor(obj.count);
                    const originalText = counter.textContent;
                    const suffix = originalText.replace(/[\d,]/g, '');
                    counter.textContent = currentValue.toLocaleString() + suffix;
                }
            });
        });
    }

    animateSection(section) {
        const elements = section.querySelectorAll('.scroll-reveal');
        
        anime({
            targets: elements,
            opacity: [0, 1],
            translateY: [30, 0],
            duration: 800,
            easing: 'easeOutExpo',
            delay: anime.stagger(100)
        });
    }

    setupParallaxScrolling() {
        window.addEventListener('scroll', () => {
            const scrolled = window.pageYOffset;
            const parallaxElements = document.querySelectorAll('[data-parallax]');
            
            parallaxElements.forEach(element => {
                const speed = element.dataset.parallax || 0.5;
                const yPos = -(scrolled * speed);
                
                anime.set(element, {
                    translateY: yPos
                });
            });
        });
    }

    setupButtonAnimations() {
        const buttons = document.querySelectorAll('.btn-3d, .btn-view-details-3d, .action-btn');
        
        buttons.forEach(button => {
            button.addEventListener('mouseenter', () => {
                anime({
                    targets: button,
                    scale: 1.05,
                    rotateY: 5,
                    duration: 300,
                    easing: 'easeOutQuad'
                });
                
                // Animate button content
                const content = button.querySelector('.btn-content, i');
                if (content) {
                    anime({
                        targets: content,
                        translateX: 3,
                        duration: 300,
                        easing: 'easeOutQuad'
                    });
                }
            });
            
            button.addEventListener('mouseleave', () => {
                anime({
                    targets: button,
                    scale: 1,
                    rotateY: 0,
                    duration: 300,
                    easing: 'easeOutQuad'
                });
                
                const content = button.querySelector('.btn-content, i');
                if (content) {
                    anime({
                        targets: content,
                        translateX: 0,
                        duration: 300,
                        easing: 'easeOutQuad'
                    });
                }
            });
            
            button.addEventListener('click', () => {
                anime({
                    targets: button,
                    scale: [1, 0.95, 1],
                    duration: 200,
                    easing: 'easeInOutQuad'
                });
            });
        });
    }

    setupIconAnimations() {
        const icons = document.querySelectorAll('.feature-icon-3d, .benefit-icon-3d, .contact-icon-3d');
        
        icons.forEach(icon => {
            icon.addEventListener('mouseenter', () => {
                anime({
                    targets: icon,
                    rotateY: '1turn',
                    scale: 1.1,
                    duration: 600,
                    easing: 'easeOutBack'
                });
            });
        });
    }

    setupFormAnimations() {
        const formFields = document.querySelectorAll('.form-control-3d, input, textarea');
        
        formFields.forEach(field => {
            field.addEventListener('focus', () => {
                anime({
                    targets: field,
                    scale: 1.02,
                    borderWidth: '2px',
                    duration: 300,
                    easing: 'easeOutQuad'
                });
                
                // Animate label if exists
                const label = field.previousElementSibling;
                if (label && label.tagName === 'LABEL') {
                    anime({
                        targets: label,
                        translateY: -5,
                        scale: 0.9,
                        color: '#667eea',
                        duration: 300,
                        easing: 'easeOutQuad'
                    });
                }
            });
            
            field.addEventListener('blur', () => {
                anime({
                    targets: field,
                    scale: 1,
                    borderWidth: '1px',
                    duration: 300,
                    easing: 'easeOutQuad'
                });
                
                const label = field.previousElementSibling;
                if (label && label.tagName === 'LABEL' && !field.value) {
                    anime({
                        targets: label,
                        translateY: 0,
                        scale: 1,
                        color: '#6b7280',
                        duration: 300,
                        easing: 'easeOutQuad'
                    });
                }
            });
        });
    }

    setupNavigationAnimations() {
        const navItems = document.querySelectorAll('.nav-hover-3d');
        
        navItems.forEach(item => {
            item.addEventListener('mouseenter', () => {
                anime({
                    targets: item,
                    translateY: -2,
                    scale: 1.05,
                    duration: 300,
                    easing: 'easeOutQuad'
                });
                
                // Animate icon if exists
                const icon = item.querySelector('i');
                if (icon) {
                    anime({
                        targets: icon,
                        rotate: '1turn',
                        duration: 500,
                        easing: 'easeOutBack'
                    });
                }
            });
            
            item.addEventListener('mouseleave', () => {
                anime({
                    targets: item,
                    translateY: 0,
                    scale: 1,
                    duration: 300,
                    easing: 'easeOutQuad'
                });
            });
        });
    }

    // Car Card Specific Animations
    setupCarCardHoverEffects() {
        const carCards = document.querySelectorAll('.car-card-enhanced-3d');
        
        carCards.forEach(card => {
            card.addEventListener('mouseenter', () => {
                // Main card animation
                anime({
                    targets: card,
                    translateY: -15,
                    rotateX: 5,
                    rotateY: 5,
                    scale: 1.02,
                    duration: 400,
                    easing: 'easeOutQuad'
                });
                
                // Feature items animation
                const features = card.querySelectorAll('.feature-item');
                anime({
                    targets: features,
                    scale: 1.05,
                    duration: 300,
                    delay: anime.stagger(50),
                    easing: 'easeOutQuad'
                });
                
                // Price tag animation
                const priceTag = card.querySelector('.price-tag-3d');
                if (priceTag) {
                    anime({
                        targets: priceTag,
                        scale: 1.1,
                        rotateZ: 5,
                        duration: 300,
                        easing: 'easeOutBack'
                    });
                }
            });
            
            card.addEventListener('mouseleave', () => {
                anime({
                    targets: card,
                    translateY: 0,
                    rotateX: 0,
                    rotateY: 0,
                    scale: 1,
                    duration: 400,
                    easing: 'easeOutQuad'
                });
                
                const features = card.querySelectorAll('.feature-item');
                anime({
                    targets: features,
                    scale: 1,
                    duration: 300,
                    easing: 'easeOutQuad'
                });
                
                const priceTag = card.querySelector('.price-tag-3d');
                if (priceTag) {
                    anime({
                        targets: priceTag,
                        scale: 1,
                        rotateZ: 0,
                        duration: 300,
                        easing: 'easeOutQuad'
                    });
                }
            });
        });
    }

    // Page Transition Effects
    setupPageTransitions() {
        // Animate page load
        anime({
            targets: 'body',
            opacity: [0, 1],
            duration: 1000,
            easing: 'easeOutExpo'
        });
        
        // Loading animation for dynamic content
        this.showLoadingAnimation();
    }

    showLoadingAnimation() {
        const loadingElements = document.querySelectorAll('[data-loading]');
        
        loadingElements.forEach(element => {
            anime({
                targets: element,
                opacity: [0.3, 1],
                scale: [0.98, 1],
                duration: 1000,
                direction: 'alternate',
                loop: true,
                easing: 'easeInOutSine'
            });
        });
    }

    // Performance optimization
    pauseAnimations() {
        anime.running.forEach(animation => {
            animation.pause();
        });
    }

    resumeAnimations() {
        anime.running.forEach(animation => {
            animation.play();
        });
    }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    window.anime3DEnhanced = new Anime3DEnhanced();
    
    // Setup car card hover effects after initialization
    setTimeout(() => {
        window.anime3DEnhanced.setupCarCardHoverEffects();
        window.anime3DEnhanced.setupPageTransitions();
    }, 500);
});

// Pause animations when page is not visible for performance
document.addEventListener('visibilitychange', () => {
    if (window.anime3DEnhanced) {
        if (document.hidden) {
            window.anime3DEnhanced.pauseAnimations();
        } else {
            window.anime3DEnhanced.resumeAnimations();
        }
    }
});
// Modern 3D Landing Page JavaScript - ChainGPT Style
class Modern3DLanding {
    constructor() {
        this.isMouseInside = false;
        this.mouseX = 0;
        this.mouseY = 0;
        this.parallaxElements = [];
        this.scrollElements = [];
        this.init();
    }

    init() {
        this.setupParallax();
        this.setupScrollAnimations();
        this.setupMouseInteractions();
        this.setupPerformanceOptimizations();
        this.startAnimationLoop();
        console.log('Modern 3D Landing initialized');
    }

    // Parallax Scrolling System
    setupParallax() {
        // Find all parallax layers
        const layers = document.querySelectorAll('.parallax-layer');
        layers.forEach((layer, index) => {
            this.parallaxElements.push({
                element: layer,
                speed: (index + 1) * 0.2,
                offset: 0
            });
        });

        // Floating elements parallax
        const floatingElements = document.querySelectorAll('.floating-element');
        floatingElements.forEach((element, index) => {
            this.parallaxElements.push({
                element: element,
                speed: 0.1 + (index * 0.05),
                offset: 0,
                isFloating: true
            });
        });

        // Geometric shapes parallax
        const shapes = document.querySelectorAll('.geometric-shape');
        shapes.forEach((shape, index) => {
            this.parallaxElements.push({
                element: shape,
                speed: 0.15 + (index * 0.08),
                offset: 0,
                isShape: true
            });
        });

        // Bind scroll event with throttling
        this.bindScrollEvent();
    }

    bindScrollEvent() {
        let ticking = false;
        
        const updateParallax = () => {
            const scrollTop = window.pageYOffset;
            const windowHeight = window.innerHeight;
            
            this.parallaxElements.forEach(item => {
                const { element, speed, isFloating, isShape } = item;
                const elementTop = element.getBoundingClientRect().top + scrollTop;
                const elementHeight = element.offsetHeight;
                
                // Calculate if element is in viewport
                if (elementTop < scrollTop + windowHeight && elementTop + elementHeight > scrollTop) {
                    const yPos = -(scrollTop * speed);
                    
                    if (isFloating) {
                        // Add complex movement for floating elements
                        const rotation = scrollTop * 0.01;
                        const oscillation = Math.sin(scrollTop * 0.002) * 10;
                        element.style.transform = `translate3d(${oscillation}px, ${yPos}px, 0) rotate(${rotation}deg)`;
                    } else if (isShape) {
                        // 3D rotation for geometric shapes
                        const rotateX = scrollTop * 0.02;
                        const rotateY = scrollTop * 0.03;
                        element.style.transform = `translate3d(0, ${yPos}px, 0) rotateX(${rotateX}deg) rotateY(${rotateY}deg)`;
                    } else {
                        element.style.transform = `translate3d(0, ${yPos}px, 0)`;
                    }
                }
            });
            
            ticking = false;
        };

        const requestTick = () => {
            if (!ticking) {
                requestAnimationFrame(updateParallax);
                ticking = true;
            }
        };

        window.addEventListener('scroll', requestTick, { passive: true });
        
        // Initial call
        updateParallax();
    }

    // Scroll-triggered Animations
    setupScrollAnimations() {
        const observerOptions = {
            threshold: 0.1,
            rootMargin: '0px 0px -50px 0px'
        };

        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('revealed');
                    
                    // Add staggered animation for children
                    const children = entry.target.querySelectorAll('.feature-card-3d, .stat-card-3d');
                    children.forEach((child, index) => {
                        setTimeout(() => {
                            child.style.animationDelay = `${index * 0.1}s`;
                            child.classList.add('revealed');
                        }, index * 100);
                    });
                }
            });
        }, observerOptions);

        // Observe all scroll reveal elements
        const scrollRevealElements = document.querySelectorAll('.scroll-reveal, .scroll-reveal-left, .scroll-reveal-right');
        scrollRevealElements.forEach(el => {
            observer.observe(el);
        });

        // Counter animation for stats
        this.setupCounterAnimations();
    }

    setupCounterAnimations() {
        const counters = document.querySelectorAll('.stat-number');
        const counterObserver = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    this.animateCounter(entry.target);
                    counterObserver.unobserve(entry.target);
                }
            });
        }, { threshold: 0.5 });

        counters.forEach(counter => {
            counterObserver.observe(counter);
        });
    }

    animateCounter(element) {
        const text = element.textContent;
        const number = parseInt(text.replace(/[^\d]/g, ''));
        const suffix = text.replace(/[\d]/g, '');
        const duration = 2000;
        const increment = number / (duration / 16);
        let current = 0;

        const timer = setInterval(() => {
            current += increment;
            if (current >= number) {
                current = number;
                clearInterval(timer);
            }
            element.textContent = Math.floor(current) + suffix;
        }, 16);
    }

    // Mouse Interactions and 3D Effects
    setupMouseInteractions() {
        const hero = document.querySelector('.hero-section');
        const car3D = document.querySelector('.car-3d-display');
        const features = document.querySelectorAll('.feature-card-3d');
        const buttons = document.querySelectorAll('.btn-3d');

        if (hero) {
            hero.addEventListener('mousemove', (e) => {
                this.handleHeroMouseMove(e, hero);
            });

            hero.addEventListener('mouseenter', () => {
                this.isMouseInside = true;
            });

            hero.addEventListener('mouseleave', () => {
                this.isMouseInside = false;
                this.resetHeroElements();
            });
        }

        // Enhanced 3D car interaction
        if (car3D) {
            this.setup3DCarInteraction(car3D);
        }

        // Feature cards magnetic effect
        features.forEach(card => {
            this.setupMagneticEffect(card);
        });

        // Button hover effects
        buttons.forEach(button => {
            this.setupButtonEffects(button);
        });
    }

    handleHeroMouseMove(e, hero) {
        const rect = hero.getBoundingClientRect();
        const centerX = rect.width / 2;
        const centerY = rect.height / 2;
        
        this.mouseX = (e.clientX - rect.left - centerX) / centerX;
        this.mouseY = (e.clientY - rect.top - centerY) / centerY;

        // Apply subtle parallax to hero elements
        const title = hero.querySelector('.hero-title');
        const subtitle = hero.querySelector('.hero-subtitle');
        const description = hero.querySelector('.hero-description');

        if (title) {
            const titleMove = this.mouseX * 10;
            title.style.transform = `translateX(${titleMove}px) translateZ(0)`;
        }

        if (subtitle) {
            const subtitleMove = this.mouseX * 5;
            subtitle.style.transform = `translateX(${subtitleMove}px) translateZ(20px)`;
        }

        if (description) {
            const descMove = this.mouseX * 3;
            description.style.transform = `translateX(${descMove}px) translateZ(30px)`;
        }

        // Move floating elements based on mouse
        const floatingElements = hero.querySelectorAll('.floating-element');
        floatingElements.forEach((element, index) => {
            const multiplier = (index + 1) * 0.02;
            const x = this.mouseX * multiplier * 50;
            const y = this.mouseY * multiplier * 30;
            element.style.transform += ` translate(${x}px, ${y}px)`;
        });
    }

    resetHeroElements() {
        const hero = document.querySelector('.hero-section');
        if (!hero) return;

        const elements = hero.querySelectorAll('.hero-title, .hero-subtitle, .hero-description');
        elements.forEach(el => {
            el.style.transform = '';
        });
    }

    setup3DCarInteraction(car3D) {
        let isHovering = false;
        
        car3D.addEventListener('mouseenter', () => {
            isHovering = true;
            car3D.style.transition = 'transform 0.3s ease';
        });

        car3D.addEventListener('mouseleave', () => {
            isHovering = false;
            car3D.style.transform = 'rotateX(10deg) rotateY(-5deg)';
        });

        car3D.addEventListener('mousemove', (e) => {
            if (!isHovering) return;

            const rect = car3D.getBoundingClientRect();
            const centerX = rect.left + rect.width / 2;
            const centerY = rect.top + rect.height / 2;
            
            const rotateX = (e.clientY - centerY) / 10;
            const rotateY = (e.clientX - centerX) / 10;
            
            car3D.style.transform = `rotateX(${10 - rotateX}deg) rotateY(${-5 + rotateY}deg) scale(1.05)`;
        });
    }

    setupMagneticEffect(element) {
        element.addEventListener('mousemove', (e) => {
            const rect = element.getBoundingClientRect();
            const x = e.clientX - rect.left - rect.width / 2;
            const y = e.clientY - rect.top - rect.height / 2;
            
            const moveX = x * 0.15;
            const moveY = y * 0.15;
            
            element.style.transform = `translate(${moveX}px, ${moveY}px) rotateX(${-y * 0.05}deg) rotateY(${x * 0.05}deg)`;
        });

        element.addEventListener('mouseleave', () => {
            element.style.transform = '';
        });
    }

    setupButtonEffects(button) {
        button.addEventListener('mouseenter', () => {
            const ripple = document.createElement('div');
            ripple.className = 'button-ripple';
            ripple.style.cssText = `
                position: absolute;
                border-radius: 50%;
                background: rgba(255,255,255,0.3);
                transform: scale(0);
                animation: ripple 0.6s linear;
                pointer-events: none;
            `;
            
            button.appendChild(ripple);
            
            setTimeout(() => {
                ripple.remove();
            }, 600);
        });
    }

    // Performance Optimizations
    setupPerformanceOptimizations() {
        // Enable GPU acceleration for key elements
        const acceleratedElements = document.querySelectorAll('.hero-content, .car-3d-display, .feature-card-3d, .stat-card-3d');
        acceleratedElements.forEach(el => {
            el.classList.add('gpu-accelerated');
        });

        // Lazy load images
        this.setupLazyLoading();

        // Preload critical animations
        this.preloadAnimations();
    }

    setupLazyLoading() {
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
    }

    preloadAnimations() {
        // Preload animation keyframes
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
    }

    // Animation Loop for Smooth Performance
    startAnimationLoop() {
        const animate = () => {
            this.updateFloatingElements();
            this.updateParticleSystem();
            requestAnimationFrame(animate);
        };
        
        animate();
    }

    updateFloatingElements() {
        const time = Date.now() * 0.001;
        const floatingElements = document.querySelectorAll('.floating-element');
        
        floatingElements.forEach((element, index) => {
            const speed = 0.5 + index * 0.1;
            const amplitude = 10 + index * 5;
            const y = Math.sin(time * speed) * amplitude;
            const rotation = Math.cos(time * speed * 0.5) * 5;
            
            if (!this.isMouseInside) {
                element.style.transform = `translateY(${y}px) rotate(${rotation}deg)`;
            }
        });
    }

    updateParticleSystem() {
        // Simple particle system for background
        const shapes = document.querySelectorAll('.geometric-shape');
        const time = Date.now() * 0.0005;
        
        shapes.forEach((shape, index) => {
            const speed = 0.3 + index * 0.1;
            const rotateX = Math.sin(time * speed) * 15;
            const rotateY = Math.cos(time * speed * 0.7) * 20;
            const scale = 1 + Math.sin(time * speed * 2) * 0.1;
            
            shape.style.transform = `rotateX(${rotateX}deg) rotateY(${rotateY}deg) scale(${scale})`;
        });
    }

    // Public Methods
    destroy() {
        // Clean up event listeners and observers
        window.removeEventListener('scroll', this.scrollHandler);
        window.removeEventListener('mousemove', this.mouseHandler);
    }

    pause() {
        this.isPaused = true;
    }

    resume() {
        this.isPaused = false;
    }
}

// Utility Functions
const Utils = {
    // Smooth scroll to element
    smoothScrollTo(target, duration = 1000) {
        const targetElement = typeof target === 'string' ? document.querySelector(target) : target;
        if (!targetElement) return;

        const targetPosition = targetElement.getBoundingClientRect().top + window.pageYOffset;
        const startPosition = window.pageYOffset;
        const distance = targetPosition - startPosition;
        let startTime = null;

        const animation = (currentTime) => {
            if (startTime === null) startTime = currentTime;
            const timeElapsed = currentTime - startTime;
            const run = this.easeInOutQuad(timeElapsed, startPosition, distance, duration);
            window.scrollTo(0, run);
            if (timeElapsed < duration) requestAnimationFrame(animation);
        };

        requestAnimationFrame(animation);
    },

    // Easing function
    easeInOutQuad(t, b, c, d) {
        t /= d / 2;
        if (t < 1) return c / 2 * t * t + b;
        t--;
        return -c / 2 * (t * (t - 2) - 1) + b;
    },

    // Throttle function
    throttle(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    },

    // Debounce function
    debounce(func, wait, immediate) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                timeout = null;
                if (!immediate) func(...args);
            };
            const callNow = immediate && !timeout;
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
            if (callNow) func(...args);
        };
    }
};

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    // Check if we're on the landing page
    if (document.querySelector('.hero-section')) {
        window.modern3DLanding = new Modern3DLanding();
        
        // Setup smooth scrolling for navigation links
        document.querySelectorAll('a[href^="#"]').forEach(anchor => {
            anchor.addEventListener('click', function (e) {
                e.preventDefault();
                const target = document.querySelector(this.getAttribute('href'));
                if (target) {
                    Utils.smoothScrollTo(target, 800);
                }
            });
        });
    }
});

// Handle page visibility changes for performance
document.addEventListener('visibilitychange', () => {
    if (window.modern3DLanding) {
        if (document.hidden) {
            window.modern3DLanding.pause();
        } else {
            window.modern3DLanding.resume();
        }
    }
});

// Export for potential module use
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { Modern3DLanding, Utils };
}
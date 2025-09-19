/**
 * 3D Interactive Landing Page Controller
 * Advanced parallax scrolling, floating effects, and interactive 3D elements
 */

class InteractiveLanding3D {
    constructor() {
        this.isInitialized = false;
        this.particles = [];
        this.scrollPosition = 0;
        this.mousePosition = { x: 0, y: 0 };
        this.parallaxElements = [];
        this.floatingElements = [];
        
        this.init();
    }
    
    init() {
        if (this.isInitialized) return;
        
        console.log('🚀 Initializing 3D Interactive Landing System');
        
        // Wait for DOM to be ready
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', () => this.setupAllEffects());
        } else {
            this.setupAllEffects();
        }
        
        this.isInitialized = true;
    }
    
    setupAllEffects() {
        this.setupParallaxScrolling();
        this.setupFloatingElements();
        this.setupInteractiveParticles();
        this.setupScrollReveal();
        this.setupMouseInteraction();
        this.setup3DCards();
        this.setupPerformanceOptimizations();
        
        console.log('✅ 3D Interactive System Initialized');
    }
    
    // Parallax Scrolling System
    setupParallaxScrolling() {
        this.parallaxElements = document.querySelectorAll('.parallax-layer');
        
        // Optimized scroll handler with requestAnimationFrame
        let ticking = false;
        
        const updateParallax = () => {
            const scrolled = window.pageYOffset;
            const rate = scrolled * -0.5;
            
            this.parallaxElements.forEach((element, index) => {
                const speed = 0.2 + (index * 0.1);
                const yPos = -(scrolled * speed);
                element.style.transform = `translate3d(0, ${yPos}px, 0)`;
            });
            
            // Update floating elements based on scroll
            this.updateFloatingElementsOnScroll(scrolled);
            
            ticking = false;
        };
        
        const requestParallaxUpdate = () => {
            if (!ticking) {
                requestAnimationFrame(updateParallax);
                ticking = true;
            }
        };
        
        window.addEventListener('scroll', requestParallaxUpdate, { passive: true });
        
        console.log('🌊 Parallax scrolling initialized');
    }
    
    // 3D Floating Elements System
    setupFloatingElements() {
        this.createFloatingElements();
        this.animateFloatingElements();
        
        console.log('🎈 Floating elements initialized');
    }
    
    createFloatingElements() {
        const container = document.querySelector('.floating-3d-container');
        if (!container) {
            const newContainer = document.createElement('div');
            newContainer.className = 'floating-3d-container';
            document.querySelector('.hero-3d').appendChild(newContainer);
        }
        
        const shapes = [
            { type: 'cube', class: 'cube-3d' },
            { type: 'sphere', class: 'sphere-3d' },
            { type: 'pyramid', class: 'pyramid-3d' },
            { type: 'diamond', class: 'diamond-3d' }
        ];
        
        const positions = [
            { top: '15%', left: '10%' },
            { top: '25%', right: '15%' },
            { top: '60%', left: '5%' },
            { bottom: '20%', right: '10%' }
        ];
        
        shapes.forEach((shape, index) => {
            const element = document.createElement('div');
            element.className = `floating-element-3d shape-3d ${shape.class}`;
            
            // Set position
            Object.keys(positions[index]).forEach(key => {
                element.style[key] = positions[index][key];
            });
            
            // Add to container
            document.querySelector('.floating-3d-container').appendChild(element);
            this.floatingElements.push(element);
        });
    }
    
    animateFloatingElements() {
        const animateElement = (element, index) => {
            const baseDelay = index * 2000;
            const amplitude = 30 + (index * 10);
            const frequency = 0.001 + (index * 0.0005);
            
            const animate = (timestamp) => {
                const offset = timestamp + baseDelay;
                const y = Math.sin(offset * frequency) * amplitude;
                const x = Math.cos(offset * frequency * 0.5) * (amplitude * 0.5);
                const rotation = (timestamp * 0.05) % 360;
                
                element.style.transform = `translate3d(${x}px, ${y}px, 0) rotateY(${rotation}deg)`;
                
                requestAnimationFrame(animate);
            };
            
            requestAnimationFrame(animate);
        };
        
        this.floatingElements.forEach(animateElement);
    }
    
    updateFloatingElementsOnScroll(scrollPosition) {
        this.floatingElements.forEach((element, index) => {
            const speed = 0.1 + (index * 0.05);
            const yOffset = scrollPosition * speed;
            const currentTransform = element.style.transform;
            
            // Preserve existing transforms and add scroll offset
            if (currentTransform.includes('translate3d')) {
                const newTransform = currentTransform.replace(
                    /translate3d\([^)]+\)/,
                    `translate3d(0, ${-yOffset}px, 0)`
                );
                element.style.transform = newTransform;
            }
        });
    }
    
    // Interactive Particles System
    setupInteractiveParticles() {
        this.createParticleSystem();
        this.animateParticles();
        
        console.log('✨ Interactive particles initialized');
    }
    
    createParticleSystem() {
        const particleContainer = document.createElement('div');
        particleContainer.className = 'particles-3d';
        document.body.appendChild(particleContainer);
        
        // Create initial particles
        for (let i = 0; i < 50; i++) {
            this.createParticle(particleContainer);
        }
        
        // Continuously create particles
        setInterval(() => {
            if (this.particles.length < 100) {
                this.createParticle(particleContainer);
            }
        }, 200);
    }
    
    createParticle(container) {
        const particle = document.createElement('div');
        particle.className = 'particle-3d';
        
        // Random starting position
        particle.style.left = Math.random() * 100 + '%';
        particle.style.animationDuration = (Math.random() * 10 + 10) + 's';
        particle.style.animationDelay = Math.random() * 2 + 's';
        
        // Random size and opacity
        const size = Math.random() * 4 + 2;
        particle.style.width = size + 'px';
        particle.style.height = size + 'px';
        particle.style.opacity = Math.random() * 0.6 + 0.2;
        
        container.appendChild(particle);
        this.particles.push(particle);
        
        // Remove particle after animation
        setTimeout(() => {
            if (particle.parentNode) {
                particle.parentNode.removeChild(particle);
                this.particles = this.particles.filter(p => p !== particle);
            }
        }, 20000);
    }
    
    animateParticles() {
        // Particles are animated via CSS, but we can add mouse interaction
        document.addEventListener('mousemove', (e) => {
            this.mousePosition.x = (e.clientX / window.innerWidth) * 100;
            this.mousePosition.y = (e.clientY / window.innerHeight) * 100;
            
            // Influence nearby particles
            this.particles.forEach(particle => {
                const rect = particle.getBoundingClientRect();
                const particleX = rect.left + rect.width / 2;
                const particleY = rect.top + rect.height / 2;
                
                const distance = Math.sqrt(
                    Math.pow(e.clientX - particleX, 2) + 
                    Math.pow(e.clientY - particleY, 2)
                );
                
                if (distance < 100) {
                    const force = (100 - distance) / 100;
                    const angle = Math.atan2(e.clientY - particleY, e.clientX - particleX);
                    const pushX = Math.cos(angle) * force * 20;
                    const pushY = Math.sin(angle) * force * 20;
                    
                    particle.style.transform = `translate(${pushX}px, ${pushY}px)`;
                    
                    setTimeout(() => {
                        particle.style.transform = '';
                    }, 1000);
                }
            });
        });
    }
    
    // Scroll Reveal Animation System
    setupScrollReveal() {
        const observerOptions = {
            threshold: 0.1,
            rootMargin: '0px 0px -50px 0px'
        };
        
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('revealed');
                    
                    // Add staggered animation delay for child elements
                    const children = entry.target.querySelectorAll('.stat-item-3d, .feature-card-3d, .card-3d-interactive');
                    children.forEach((child, index) => {
                        setTimeout(() => {
                            child.style.animationDelay = (index * 0.1) + 's';
                            child.classList.add('revealed');
                        }, index * 100);
                    });
                }
            });
        }, observerOptions);
        
        // Observe all elements with scroll-reveal class
        document.querySelectorAll('.scroll-reveal-3d').forEach(el => {
            observer.observe(el);
        });
        
        console.log('👁️ Scroll reveal animations initialized');
    }
    
    // Mouse Interaction System
    setupMouseInteraction() {
        // 3D tilt effect for cards
        document.querySelectorAll('.card-3d-interactive').forEach(card => {
            card.addEventListener('mousemove', (e) => {
                const rect = card.getBoundingClientRect();
                const centerX = rect.left + rect.width / 2;
                const centerY = rect.top + rect.height / 2;
                
                const deltaX = (e.clientX - centerX) / (rect.width / 2);
                const deltaY = (e.clientY - centerY) / (rect.height / 2);
                
                const rotateX = deltaY * -10;
                const rotateY = deltaX * 10;
                
                card.style.transform = `perspective(1000px) rotateX(${rotateX}deg) rotateY(${rotateY}deg) translateZ(20px)`;
            });
            
            card.addEventListener('mouseleave', () => {
                card.style.transform = '';
            });
        });
        
        // Hero section mouse parallax
        const hero = document.querySelector('.hero-3d');
        if (hero) {
            hero.addEventListener('mousemove', (e) => {
                const centerX = window.innerWidth / 2;
                const centerY = window.innerHeight / 2;
                
                const deltaX = (e.clientX - centerX) / centerX;
                const deltaY = (e.clientY - centerY) / centerY;
                
                // Apply subtle parallax to hero content
                const heroContent = document.querySelector('.hero-content-3d');
                if (heroContent) {
                    heroContent.style.transform = `translate3d(${deltaX * 10}px, ${deltaY * 10}px, 0)`;
                }
                
                // Apply stronger parallax to floating elements
                this.floatingElements.forEach((element, index) => {
                    const intensity = (index + 1) * 5;
                    element.style.transform += ` translate3d(${deltaX * intensity}px, ${deltaY * intensity}px, 0)`;
                });
            });
        }
        
        console.log('🖱️ Mouse interactions initialized');
    }
    
    // 3D Cards System
    setup3DCards() {
        // Enhanced hover effects for 3D cards
        document.querySelectorAll('.card-3d-interactive').forEach(card => {
            // Add ripple effect on click
            card.addEventListener('click', (e) => {
                const ripple = document.createElement('div');
                ripple.style.position = 'absolute';
                ripple.style.borderRadius = '50%';
                ripple.style.background = 'rgba(255, 255, 255, 0.3)';
                ripple.style.transform = 'scale(0)';
                ripple.style.animation = 'rippleEffect 0.6s linear';
                ripple.style.left = (e.clientX - card.offsetLeft) + 'px';
                ripple.style.top = (e.clientY - card.offsetTop) + 'px';
                
                card.appendChild(ripple);
                
                setTimeout(() => {
                    ripple.remove();
                }, 600);
            });
        });
        
        // Add CSS for ripple effect
        const style = document.createElement('style');
        style.textContent = `
            @keyframes rippleEffect {
                to {
                    transform: scale(4);
                    opacity: 0;
                }
            }
        `;
        document.head.appendChild(style);
        
        console.log('🃏 3D cards system initialized');
    }
    
    // Performance Optimizations
    setupPerformanceOptimizations() {
        // GPU acceleration for better performance
        document.querySelectorAll('.floating-element-3d, .card-3d-interactive, .parallax-layer').forEach(element => {
            element.classList.add('gpu-accelerated');
        });
        
        // Reduce animations on low-end devices
        if (navigator.hardwareConcurrency && navigator.hardwareConcurrency < 4) {
            document.documentElement.style.setProperty('--animation-duration', '0.5s');
        }
        
        // Pause animations when page is not visible
        document.addEventListener('visibilitychange', () => {
            if (document.hidden) {
                document.querySelectorAll('.floating-element-3d').forEach(el => {
                    el.style.animationPlayState = 'paused';
                });
            } else {
                document.querySelectorAll('.floating-element-3d').forEach(el => {
                    el.style.animationPlayState = 'running';
                });
            }
        });
        
        console.log('⚡ Performance optimizations applied');
    }
    
    // Utility Methods
    
    // Method to add new floating element dynamically
    addFloatingElement(type = 'cube', position = {}) {
        const element = document.createElement('div');
        element.className = `floating-element-3d shape-3d ${type}-3d`;
        
        // Set position
        element.style.top = position.top || Math.random() * 80 + 10 + '%';
        element.style.left = position.left || Math.random() * 80 + 10 + '%';
        
        document.querySelector('.floating-3d-container').appendChild(element);
        this.floatingElements.push(element);
        
        // Start animation for new element
        this.animateFloatingElements();
    }
    
    // Method to trigger special effects
    triggerSpecialEffect(type) {
        switch (type) {
            case 'explosion':
                this.createParticleExplosion();
                break;
            case 'wave':
                this.createWaveEffect();
                break;
            default:
                console.log('Unknown effect type:', type);
        }
    }
    
    createParticleExplosion() {
        const center = {
            x: window.innerWidth / 2,
            y: window.innerHeight / 2
        };
        
        for (let i = 0; i < 20; i++) {
            setTimeout(() => {
                this.createParticle(document.querySelector('.particles-3d'));
            }, i * 50);
        }
    }
    
    createWaveEffect() {
        document.querySelectorAll('.card-3d-interactive').forEach((card, index) => {
            setTimeout(() => {
                card.style.transform = 'translateY(-20px) rotateX(10deg)';
                setTimeout(() => {
                    card.style.transform = '';
                }, 300);
            }, index * 100);
        });
    }
    
    // Cleanup method
    destroy() {
        // Remove event listeners and cleanup
        this.particles.forEach(particle => {
            if (particle.parentNode) {
                particle.parentNode.removeChild(particle);
            }
        });
        
        this.particles = [];
        this.floatingElements = [];
        this.isInitialized = false;
        
        console.log('🧹 3D Interactive System cleaned up');
    }
}

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    window.interactiveLanding3D = new InteractiveLanding3D();
});

// Smooth scrolling for anchor links
document.addEventListener('DOMContentLoaded', function() {
    const anchorLinks = document.querySelectorAll('a[href^="#"]');
    anchorLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            e.preventDefault();
            const targetId = this.getAttribute('href');
            const targetElement = document.querySelector(targetId);
            if (targetElement) {
                const offsetTop = targetElement.offsetTop - 80;
                window.scrollTo({
                    top: offsetTop,
                    behavior: 'smooth'
                });
            }
        });
    });
});

// Export for global access
window.InteractiveLanding3D = InteractiveLanding3D;
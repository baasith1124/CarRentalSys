// Enhanced 3D Interactive Sidebar JavaScript

class Interactive3DSidebar {
    constructor() {
        this.isInitialized = false;
        this.sidebarElement = null;
        this.navLinks = [];
        this.profileCard = null;
        this.quickStats = [];
        
        // Bind methods
        this.init = this.init.bind(this);
        this.initInteractiveElements = this.initInteractiveElements.bind(this);
        this.initTooltips = this.initTooltips.bind(this);
        this.initAnimations = this.initAnimations.bind(this);
        this.initSidebarEffects = this.initSidebarEffects.bind(this);
        this.updateNotificationBadges = this.updateNotificationBadges.bind(this);
    }

    init() {
        if (this.isInitialized) return;
        
        console.log('🎯 Initializing 3D Interactive Sidebar...');
        
        this.sidebarElement = document.getElementById('userSidebar');
        if (!this.sidebarElement) {
            console.warn('Sidebar element not found');
            return;
        }
        
        this.navLinks = this.sidebarElement.querySelectorAll('.enhanced-nav-link');
        this.profileCard = this.sidebarElement.querySelector('.sidebar-profile-card');
        this.quickStats = this.sidebarElement.querySelectorAll('.quick-stat-item');
        
        this.initInteractiveElements();
        this.initTooltips();
        this.initAnimations();
        this.initSidebarEffects();
        this.initProgressAnimations();
        this.setupEventListeners();
        
        // Update badges periodically
        this.updateNotificationBadges();
        setInterval(this.updateNotificationBadges, 30000);
        
        this.isInitialized = true;
        console.log('✅ 3D Interactive Sidebar initialized successfully!');
    }

    initInteractiveElements() {
        // Enhanced nav link interactions
        this.navLinks.forEach((link, index) => {
            // Add entrance animation delay
            link.style.animationDelay = `${index * 0.1}s`;
            link.classList.add('nav-entrance');
            
            // 3D hover effects
            link.addEventListener('mouseenter', (e) => {
                this.handleNavHover(e.target, true);
            });
            
            link.addEventListener('mouseleave', (e) => {
                this.handleNavHover(e.target, false);
            });
            
            // Click effects
            link.addEventListener('click', (e) => {
                this.handleNavClick(e.target);
            });
            
            // Focus for accessibility
            link.addEventListener('focus', (e) => {
                this.handleNavFocus(e.target, true);
            });
            
            link.addEventListener('blur', (e) => {
                this.handleNavFocus(e.target, false);
            });
        });
        
        // Profile card interactions
        if (this.profileCard) {
            this.profileCard.addEventListener('click', () => {
                this.handleProfileClick();
            });
            
            this.profileCard.addEventListener('mouseenter', () => {
                this.handleProfileHover(true);
            });
            
            this.profileCard.addEventListener('mouseleave', () => {
                this.handleProfileHover(false);
            });
        }
        
        // Quick stats interactions
        this.quickStats.forEach((stat, index) => {
            stat.addEventListener('click', () => {
                this.handleQuickStatClick(stat, index);
            });
        });
    }

    handleNavHover(element, isHover) {
        const icon3D = element.querySelector('.nav-icon-3d');
        const navText = element.querySelector('.nav-text');
        const badge = element.querySelector('.nav-badge');
        
        if (isHover) {
            // 3D transform effect
            if (icon3D) {
                icon3D.style.transform = 'rotateY(15deg) scale(1.1)';
            }
            
            // Text glow effect
            if (navText) {
                navText.style.textShadow = '0 0 10px rgba(255, 255, 255, 0.5)';
            }
            
            // Badge bounce
            if (badge) {
                badge.style.transform = 'scale(1.1) rotate(5deg)';
            }
            
            // Sound effect (optional)
            this.playHoverSound();
            
        } else {
            // Reset transforms
            if (icon3D) {
                icon3D.style.transform = '';
            }
            
            if (navText) {
                navText.style.textShadow = '';
            }
            
            if (badge) {
                badge.style.transform = '';
            }
        }
    }

    handleNavFocus(element, isFocused) {
        if (isFocused) {
            element.style.outline = '2px solid #3498db';
            element.style.outlineOffset = '2px';
        } else {
            element.style.outline = '';
            element.style.outlineOffset = '';
        }
    }

    handleNavClick(element) {
        // Ripple effect
        this.createRippleEffect(element);
        
        // Update active state
        this.navLinks.forEach(link => link.classList.remove('active'));
        element.classList.add('active');
        
        // Vibration feedback on mobile
        if ('vibrate' in navigator) {
            navigator.vibrate(50);
        }
        
        // Analytics tracking
        this.trackNavigation(element);
    }

    createRippleEffect(element) {
        const rect = element.getBoundingClientRect();
        const ripple = document.createElement('div');
        
        ripple.style.cssText = `
            position: absolute;
            width: 100px;
            height: 100px;
            border-radius: 50%;
            background: rgba(52, 152, 219, 0.3);
            transform: scale(0);
            animation: rippleEffect 0.6s ease-out;
            pointer-events: none;
            top: 50%;
            left: 50%;
            transform-origin: center;
            z-index: 1000;
        `;
        
        element.style.position = 'relative';
        element.appendChild(ripple);
        
        setTimeout(() => {
            ripple.remove();
        }, 600);
    }

    handleProfileClick() {
        // Profile modal or navigation
        console.log('Profile clicked - opening profile modal...');
        
        // Create profile modal
        this.showProfileModal();
        
        // Animation feedback
        const avatar = this.profileCard.querySelector('.avatar-container');
        if (avatar) {
            avatar.style.transform = 'scale(0.95) rotateY(180deg)';
            setTimeout(() => {
                avatar.style.transform = '';
            }, 300);
        }
    }

    handleProfileHover(isHover) {
        const avatar = this.profileCard.querySelector('.avatar-container');
        const stats = this.profileCard.querySelectorAll('.stat-item');
        
        if (isHover) {
            if (avatar) {
                avatar.style.transform = 'scale(1.05) rotateY(10deg)';
            }
            
            stats.forEach((stat, index) => {
                setTimeout(() => {
                    stat.style.transform = 'translateY(-3px) scale(1.05)';
                }, index * 100);
            });
        } else {
            if (avatar) {
                avatar.style.transform = '';
            }
            
            stats.forEach(stat => {
                stat.style.transform = '';
            });
        }
    }

    handleQuickStatClick(statElement, index) {
        // Animate clicked stat
        statElement.style.transform = 'scale(0.95)';
        setTimeout(() => {
            statElement.style.transform = '';
        }, 150);
        
        // Show detailed info
        this.showStatDetail(index);
    }

    showStatDetail(index) {
        const statTitles = ['Miles Driven', 'CO2 Saved'];
        const statDetails = [
            'You\'ve driven 2,450 miles across 12 trips. Keep up the great work!',
            'You\'ve saved 45kg of CO2 emissions by using our car sharing service!'
        ];
        
        if (window.dashboardInstance) {
            window.dashboardInstance.showNotification(
                `${statTitles[index]}: ${statDetails[index]}`,
                'info',
                5000
            );
        }
    }

    initTooltips() {
        // Enhanced tooltips with 3D positioning
        this.navLinks.forEach(link => {
            const tooltip = link.getAttribute('data-tooltip');
            if (tooltip) {
                link.addEventListener('mouseenter', (e) => {
                    this.showTooltip(e.target, tooltip);
                });
                
                link.addEventListener('mouseleave', (e) => {
                    this.hideTooltip(e.target);
                });
            }
        });
    }

    showTooltip(element, text) {
        // Create tooltip if it doesn't exist
        let tooltip = element.querySelector('.sidebar-tooltip');
        if (!tooltip) {
            tooltip = document.createElement('div');
            tooltip.className = 'sidebar-tooltip';
            tooltip.style.cssText = `
                position: absolute;
                left: 100%;
                top: 50%;
                transform: translateY(-50%) translateX(10px) scale(0.8);
                background: linear-gradient(135deg, rgba(0, 0, 0, 0.9), rgba(30, 30, 30, 0.9));
                color: white;
                padding: 0.75rem 1rem;
                border-radius: 8px;
                font-size: 0.85rem;
                white-space: nowrap;
                z-index: 1000;
                opacity: 0;
                pointer-events: none;
                backdrop-filter: blur(10px);
                border: 1px solid rgba(255, 255, 255, 0.1);
                box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3);
                transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
            `;
            tooltip.textContent = text;
            element.appendChild(tooltip);
        }
        
        // Animate tooltip appearance
        setTimeout(() => {
            tooltip.style.opacity = '1';
            tooltip.style.transform = 'translateY(-50%) translateX(10px) scale(1)';
        }, 50);
    }

    hideTooltip(element) {
        const tooltip = element.querySelector('.sidebar-tooltip');
        if (tooltip) {
            tooltip.style.opacity = '0';
            tooltip.style.transform = 'translateY(-50%) translateX(10px) scale(0.8)';
            
            setTimeout(() => {
                tooltip.remove();
            }, 300);
        }
    }

    initAnimations() {
        // Staggered entrance animations
        const observerOptions = {
            threshold: 0.1,
            rootMargin: '0px 0px -50px 0px'
        };
        
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    this.animateElementEntrance(entry.target);
                }
            });
        }, observerOptions);
        
        // Observe nav links
        this.navLinks.forEach((link, index) => {
            link.style.opacity = '0';
            link.style.transform = 'translateX(-30px)';
            observer.observe(link);
        });
        
        // Profile card animation
        if (this.profileCard) {
            this.profileCard.style.opacity = '0';
            this.profileCard.style.transform = 'translateY(20px)';
            observer.observe(this.profileCard);
        }
    }

    animateElementEntrance(element) {
        const delay = Array.from(element.parentNode.children).indexOf(element) * 100;
        
        setTimeout(() => {
            element.style.transition = 'all 0.6s cubic-bezier(0.4, 0, 0.2, 1)';
            element.style.opacity = '1';
            element.style.transform = 'translateX(0) translateY(0)';
        }, delay);
    }

    initSidebarEffects() {
        // Parallax effect on scroll
        let ticking = false;
        
        const updateParallax = () => {
            const scrolled = window.pageYOffset;
            const parallax = this.sidebarElement.querySelector('.brand-icon-3d');
            
            if (parallax) {
                const speed = scrolled * 0.1;
                parallax.style.transform = `translateY(${speed}px) rotateY(${speed * 0.1}deg)`;
            }
            
            ticking = false;
        };
        
        const requestParallax = () => {
            if (!ticking) {
                requestAnimationFrame(updateParallax);
                ticking = true;
            }
        };
        
        window.addEventListener('scroll', requestParallax);
        
        // Interactive brand effects
        const interactiveBrand = this.sidebarElement.querySelector('.interactive-brand');
        if (interactiveBrand) {
            interactiveBrand.addEventListener('click', () => {
                this.triggerBrandAnimation();
            });
        }
    }

    triggerBrandAnimation() {
        const brand = this.sidebarElement.querySelector('.interactive-brand');
        const icon = brand.querySelector('.brand-icon-3d i');
        
        // 360-degree rotation
        icon.style.transition = 'transform 1s ease';
        icon.style.transform = 'rotateY(360deg) scale(1.2)';
        
        setTimeout(() => {
            icon.style.transform = '';
        }, 1000);
        
        // Particle burst effect
        this.createParticleBurst(brand);
    }

    createParticleBurst(element) {
        for (let i = 0; i < 12; i++) {
            const particle = document.createElement('div');
            particle.style.cssText = `
                position: absolute;
                width: 4px;
                height: 4px;
                background: #3498db;
                border-radius: 50%;
                top: 50%;
                left: 50%;
                pointer-events: none;
                z-index: 1000;
            `;
            
            const angle = (i / 12) * Math.PI * 2;
            const distance = 50;
            const x = Math.cos(angle) * distance;
            const y = Math.sin(angle) * distance;
            
            element.appendChild(particle);
            
            particle.animate([
                { transform: 'translate(-50%, -50%) scale(1)', opacity: 1 },
                { transform: `translate(${x}px, ${y}px) scale(0)`, opacity: 0 }
            ], {
                duration: 800,
                easing: 'cubic-bezier(0.4, 0, 0.2, 1)'
            }).onfinish = () => particle.remove();
        }
    }

    initProgressAnimations() {
        // Animate progress bars
        const progressBars = this.sidebarElement.querySelectorAll('.progress-fill');
        progressBars.forEach(bar => {
            const targetWidth = bar.style.width;
            bar.style.width = '0%';
            
            setTimeout(() => {
                bar.style.transition = 'width 2s cubic-bezier(0.4, 0, 0.2, 1)';
                bar.style.width = targetWidth;
            }, 1000);
        });
        
        // Animate completion rings
        const completionRings = this.sidebarElement.querySelectorAll('.completion-ring circle:last-child');
        completionRings.forEach(ring => {
            const dashArray = ring.getAttribute('stroke-dasharray');
            const dashOffset = ring.getAttribute('stroke-dashoffset');
            
            ring.style.strokeDashoffset = dashArray;
            
            setTimeout(() => {
                ring.style.transition = 'stroke-dashoffset 2s cubic-bezier(0.4, 0, 0.2, 1)';
                ring.style.strokeDashoffset = dashOffset;
            }, 1500);
        });
    }

    updateNotificationBadges() {
        // Simulate real-time badge updates
        const badges = this.sidebarElement.querySelectorAll('.nav-badge');
        
        badges.forEach(badge => {
            if (badge.classList.contains('notification-badge')) {
                // Simulate new notifications
                if (Math.random() > 0.8) {
                    const currentCount = parseInt(badge.textContent) || 0;
                    badge.textContent = currentCount + 1;
                    
                    // Flash animation
                    badge.style.animation = 'none';
                    setTimeout(() => {
                        badge.style.animation = 'notificationPulse 1.5s infinite';
                    }, 10);
                }
            }
        });
    }

    setupEventListeners() {
        // Keyboard navigation
        document.addEventListener('keydown', (e) => {
            if (e.ctrlKey && e.shiftKey && e.key === 'S') {
                e.preventDefault();
                this.toggleSidebarFocus();
            }
        });
        
        // Mobile touch gestures
        let touchStartX = 0;
        
        document.addEventListener('touchstart', (e) => {
            touchStartX = e.touches[0].clientX;
        });
        
        document.addEventListener('touchend', (e) => {
            const touchEndX = e.changedTouches[0].clientX;
            const diff = touchStartX - touchEndX;
            
            if (Math.abs(diff) > 50) {
                if (diff > 0) {
                    // Swipe left - close sidebar
                    this.closeSidebar();
                } else {
                    // Swipe right - open sidebar
                    this.openSidebar();
                }
            }
        });
    }

    toggleSidebarFocus() {
        const firstNavLink = this.navLinks[0];
        if (firstNavLink) {
            firstNavLink.focus();
        }
    }

    openSidebar() {
        if (window.innerWidth < 992) {
            this.sidebarElement.classList.add('show');
        }
    }

    closeSidebar() {
        if (window.innerWidth < 992) {
            this.sidebarElement.classList.remove('show');
        }
    }

    showProfileModal() {
        // Create enhanced profile modal
        const modal = document.createElement('div');
        modal.className = 'modal fade profile-modal';
        modal.innerHTML = `
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header gradient-header">
                        <h5 class="modal-title">
                            <i class="bi bi-person-circle me-2"></i>
                            Profile Overview
                        </h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <div class="profile-summary">
                            <div class="profile-avatar-large">
                                <i class="bi bi-person-circle"></i>
                            </div>
                            <div class="profile-details">
                                <h4>${document.querySelector('.profile-name')?.textContent || 'User'}</h4>
                                <p class="text-muted">Premium Member since 2024</p>
                                <div class="profile-metrics">
                                    <div class="metric">
                                        <span class="value">4.8</span>
                                        <span class="label">Rating</span>
                                    </div>
                                    <div class="metric">
                                        <span class="value">12</span>
                                        <span class="label">Trips</span>
                                    </div>
                                    <div class="metric">
                                        <span class="value">2,450</span>
                                        <span class="label">Miles</span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-primary" onclick="window.location.href='/Profile'">
                            Edit Profile
                        </button>
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                            Close
                        </button>
                    </div>
                </div>
            </div>
        `;
        
        document.body.appendChild(modal);
        const bootstrapModal = new bootstrap.Modal(modal);
        bootstrapModal.show();
        
        modal.addEventListener('hidden.bs.modal', () => {
            modal.remove();
        });
    }

    playHoverSound() {
        // Optional: play subtle sound effect
        if ('AudioContext' in window) {
            try {
                const audioContext = new AudioContext();
                const oscillator = audioContext.createOscillator();
                const gainNode = audioContext.createGain();
                
                oscillator.connect(gainNode);
                gainNode.connect(audioContext.destination);
                
                oscillator.frequency.setValueAtTime(800, audioContext.currentTime);
                gainNode.gain.setValueAtTime(0.01, audioContext.currentTime);
                gainNode.gain.exponentialRampToValueAtTime(0.001, audioContext.currentTime + 0.1);
                
                oscillator.start(audioContext.currentTime);
                oscillator.stop(audioContext.currentTime + 0.1);
            } catch (e) {
                // Ignore audio errors
            }
        }
    }

    trackNavigation(element) {
        // Analytics tracking
        const navText = element.querySelector('.nav-text')?.textContent;
        console.log(`Navigation: ${navText}`);
        
        // Send to analytics service
        if (window.gtag) {
            window.gtag('event', 'sidebar_navigation', {
                'navigation_item': navText,
                'custom_parameter': 'sidebar_3d'
            });
        }
    }

    destroy() {
        // Clean up event listeners and resources
        this.isInitialized = false;
        console.log('🧹 3D Interactive Sidebar destroyed');
    }
}

// Global instance
let sidebar3D = null;

// Initialize after DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    setTimeout(() => {
        sidebar3D = new Interactive3DSidebar();
        sidebar3D.init();
    }, 500);
});

// Clean up on page unload
window.addEventListener('beforeunload', function() {
    if (sidebar3D) {
        sidebar3D.destroy();
    }
});

// Add CSS animation for ripple effect
const rippleStyle = document.createElement('style');
rippleStyle.textContent = `
    @keyframes rippleEffect {
        to {
            transform: translate(-50%, -50%) scale(4);
            opacity: 0;
        }
    }
    
    .nav-entrance {
        animation: navEntrance 0.6s cubic-bezier(0.4, 0, 0.2, 1) forwards;
    }
    
    @keyframes navEntrance {
        from {
            opacity: 0;
            transform: translateX(-30px);
        }
        to {
            opacity: 1;
            transform: translateX(0);
        }
    }
`;
document.head.appendChild(rippleStyle);

// Export for global use
window.Interactive3DSidebar = Interactive3DSidebar;
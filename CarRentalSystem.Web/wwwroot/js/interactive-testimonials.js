// Interactive Testimonials JavaScript
document.addEventListener('DOMContentLoaded', function() {
    initializeInteractiveTestimonials();
});

function initializeInteractiveTestimonials() {
    const testimonials = document.querySelectorAll('.interactive-testimonial');
    
    if (testimonials.length === 0) {
        return;
    }
    
    // Set up intersection observer for scroll animations
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };
    
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0) rotateX(0)';
            }
        });
    }, observerOptions);
    
    // Observe each testimonial
    testimonials.forEach(testimonial => {
        observer.observe(testimonial);
        
        // Add hover effects
        addHoverEffects(testimonial);
        
        // Add click interactions
        addClickInteractions(testimonial);
        
        // Add action button handlers
        addActionButtonHandlers(testimonial);
    });
    
    // Add floating elements
    addFloatingElements();
}

function addHoverEffects(testimonial) {
    testimonial.addEventListener('mouseenter', function() {
        // Add particle effects
        createParticleEffect(this);
        
        // Add floating animation
        this.style.animation = 'testimonialFloat 2s ease-in-out infinite';
        
        // Track interaction
        trackTestimonialInteraction(this, 'hover');
    });
    
    testimonial.addEventListener('mouseleave', function() {
        // Remove floating animation
        this.style.animation = '';
        
        // Stop particle effects
        stopParticleEffects(this);
    });
}

function addClickInteractions(testimonial) {
    testimonial.addEventListener('click', function(e) {
        // Don't trigger if clicking action buttons
        if (e.target.closest('.testimonial-action-btn')) {
            return;
        }
        
        // Create ripple effect
        createRippleEffect(this, e);
        
        // Show testimonial details
        showTestimonialDetails(this);
        
        // Track interaction
        trackTestimonialInteraction(this, 'click');
    });
}

function addActionButtonHandlers(testimonial) {
    const shareBtn = testimonial.querySelector('.testimonial-action-btn[title="Share Testimonial"]');
    const likeBtn = testimonial.querySelector('.testimonial-action-btn[title="Like Testimonial"]');
    
    if (shareBtn) {
        shareBtn.addEventListener('click', function(e) {
            e.stopPropagation();
            handleShareTestimonial(testimonial);
        });
    }
    
    if (likeBtn) {
        likeBtn.addEventListener('click', function(e) {
            e.stopPropagation();
            handleLikeTestimonial(likeBtn);
        });
    }
}

function createParticleEffect(testimonial) {
    const particles = [];
    const particleCount = 8;
    
    for (let i = 0; i < particleCount; i++) {
        const particle = document.createElement('div');
        particle.className = 'testimonial-particle';
        particle.style.cssText = `
            position: absolute;
            width: 4px;
            height: 4px;
            background: #3B82F6;
            border-radius: 50%;
            pointer-events: none;
            z-index: 1;
            animation: testimonialParticleFloat 2s ease-out forwards;
        `;
        
        // Random position around the testimonial
        const angle = (i / particleCount) * Math.PI * 2;
        const distance = 60 + Math.random() * 40;
        const x = Math.cos(angle) * distance;
        const y = Math.sin(angle) * distance;
        
        particle.style.left = `calc(50% + ${x}px)`;
        particle.style.top = `calc(50% + ${y}px)`;
        particle.style.animationDelay = `${i * 0.1}s`;
        
        testimonial.appendChild(particle);
        particles.push(particle);
        
        // Remove particle after animation
        setTimeout(() => {
            if (particle.parentNode) {
                particle.parentNode.removeChild(particle);
            }
        }, 2000);
    }
    
    // Store particles for cleanup
    testimonial._particles = particles;
}

function stopParticleEffects(testimonial) {
    if (testimonial._particles) {
        testimonial._particles.forEach(particle => {
            if (particle.parentNode) {
                particle.parentNode.removeChild(particle);
            }
        });
        testimonial._particles = [];
    }
}

function createRippleEffect(testimonial, event) {
    const ripple = document.createElement('div');
    ripple.className = 'testimonial-ripple';
    
    const rect = testimonial.getBoundingClientRect();
    const size = Math.max(rect.width, rect.height);
    const x = event.clientX - rect.left - size / 2;
    const y = event.clientY - rect.top - size / 2;
    
    ripple.style.cssText = `
        position: absolute;
        width: ${size}px;
        height: ${size}px;
        left: ${x}px;
        top: ${y}px;
        background: rgba(59, 130, 246, 0.4);
        border-radius: 50%;
        transform: scale(0);
        animation: testimonialRipple 0.6s linear;
        pointer-events: none;
        z-index: 1;
    `;
    
    testimonial.appendChild(ripple);
    
    // Remove ripple after animation
    setTimeout(() => {
        if (ripple.parentNode) {
            ripple.parentNode.removeChild(ripple);
        }
    }, 600);
}

function showTestimonialDetails(testimonial) {
    const author = testimonial.querySelector('.testimonial-author').textContent;
    const role = testimonial.querySelector('.testimonial-role').textContent;
    const quote = testimonial.querySelector('.testimonial-quote').textContent;
    
    // Create tooltip
    const tooltip = document.createElement('div');
    tooltip.className = 'testimonial-tooltip';
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
        box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
        min-width: 200px;
        text-align: center;
        z-index: 1000;
        opacity: 0;
        transition: opacity 0.3s ease;
        pointer-events: none;
    `;
    
    tooltip.innerHTML = `
        <div style="font-weight: 700; margin-bottom: 0.5rem; color: #3B82F6;">${author}</div>
        <div style="font-size: 0.9rem; color: rgba(255, 255, 255, 0.8); margin-bottom: 0.75rem;">${role}</div>
        <div style="font-size: 0.85rem; color: rgba(255, 255, 255, 0.9); font-style: italic;">"${quote}"</div>
    `;
    
    testimonial.appendChild(tooltip);
    
    // Show tooltip
    setTimeout(() => {
        tooltip.style.opacity = '1';
    }, 10);
    
    // Hide tooltip after 4 seconds
    setTimeout(() => {
        tooltip.style.opacity = '0';
        setTimeout(() => {
            if (tooltip.parentNode) {
                tooltip.parentNode.removeChild(tooltip);
            }
        }, 300);
    }, 4000);
}

function handleShareTestimonial(testimonial) {
    const author = testimonial.querySelector('.testimonial-author').textContent;
    const quote = testimonial.querySelector('.testimonial-quote').textContent;
    
    const shareText = `"${quote}" - ${author}`;
    
    if (navigator.share) {
        navigator.share({
            title: 'Customer Testimonial',
            text: shareText,
            url: window.location.href
        }).catch(err => {
            console.log('Error sharing:', err);
            fallbackShare(shareText);
        });
    } else {
        fallbackShare(shareText);
    }
    
    // Track share
    trackTestimonialInteraction(testimonial, 'share');
}

function fallbackShare(text) {
    // Copy to clipboard
    navigator.clipboard.writeText(text).then(() => {
        showToast('Testimonial copied to clipboard!', 'success');
    }).catch(() => {
        // Fallback: show in alert
        alert(`Share this testimonial:\n\n${text}`);
    });
}

function handleLikeTestimonial(likeBtn) {
    const isLiked = likeBtn.classList.contains('liked');
    
    if (isLiked) {
        likeBtn.classList.remove('liked');
        likeBtn.style.color = 'rgba(255, 255, 255, 0.8)';
        likeBtn.style.background = 'rgba(255, 255, 255, 0.1)';
        showToast('Testimonial unliked', 'info');
    } else {
        likeBtn.classList.add('liked');
        likeBtn.style.color = '#EF4444';
        likeBtn.style.background = 'rgba(239, 68, 68, 0.2)';
        likeBtn.style.borderColor = '#EF4444';
        showToast('Testimonial liked!', 'success');
        
        // Add heart animation
        likeBtn.style.animation = 'testimonialHeartBeat 0.6s ease-in-out';
        setTimeout(() => {
            likeBtn.style.animation = '';
        }, 600);
    }
    
    // Track like
    const testimonial = likeBtn.closest('.interactive-testimonial');
    trackTestimonialInteraction(testimonial, 'like');
}

function addFloatingElements() {
    const testimonialsSection = document.getElementById('testimonials');
    if (!testimonialsSection) return;
    
    // Add floating elements
    for (let i = 0; i < 3; i++) {
        const element = document.createElement('div');
        element.className = 'floating-testimonial-element';
        element.innerHTML = '<i class="bi bi-quote"></i>';
        element.style.cssText = `
            position: absolute;
            font-size: 2rem;
            color: rgba(59, 130, 246, 0.1);
            pointer-events: none;
            z-index: 0;
            animation: testimonialFloat 8s ease-in-out infinite;
            animation-delay: ${i * 2}s;
        `;
        
        // Random positioning
        const positions = [
            { top: '10%', left: '5%' },
            { top: '60%', right: '10%' },
            { bottom: '20%', left: '15%' }
        ];
        
        Object.assign(element.style, positions[i]);
        testimonialsSection.appendChild(element);
    }
}

function trackTestimonialInteraction(testimonial, action) {
    const testimonialId = testimonial.getAttribute('data-testimonial');
    const author = testimonial.querySelector('.testimonial-author').textContent;
    
    // Store in localStorage for analytics
    const interactions = JSON.parse(localStorage.getItem('testimonialInteractions') || '[]');
    interactions.push({
        testimonialId,
        author,
        action,
        timestamp: new Date().toISOString()
    });
    
    // Keep only last 50 interactions
    if (interactions.length > 50) {
        interactions.splice(0, interactions.length - 50);
    }
    
    localStorage.setItem('testimonialInteractions', JSON.stringify(interactions));
}

// Add CSS animations dynamically
const style = document.createElement('style');
style.textContent = `
    @keyframes testimonialParticleFloat {
        0% {
            opacity: 1;
            transform: scale(0) translateY(0);
        }
        50% {
            opacity: 0.8;
            transform: scale(1) translateY(-20px);
        }
        100% {
            opacity: 0;
            transform: scale(0) translateY(-40px);
        }
    }
    
    @keyframes testimonialHeartBeat {
        0% { transform: scale(1); }
        25% { transform: scale(1.2); }
        50% { transform: scale(1); }
        75% { transform: scale(1.1); }
        100% { transform: scale(1); }
    }
    
    .testimonial-action-btn.liked {
        color: #EF4444 !important;
        background: rgba(239, 68, 68, 0.2) !important;
        border-color: #EF4444 !important;
    }
`;
document.head.appendChild(style);

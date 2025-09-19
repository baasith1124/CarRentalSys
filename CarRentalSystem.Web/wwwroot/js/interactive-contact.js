// Interactive Contact Section JavaScript
document.addEventListener('DOMContentLoaded', function() {
    initializeInteractiveContact();
});

function initializeInteractiveContact() {
    const contactCards = document.querySelectorAll('.interactive-contact-card');
    
    if (contactCards.length === 0) {
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
    
    // Observe each contact card
    contactCards.forEach(card => {
        observer.observe(card);
        
        // Add hover effects
        addHoverEffects(card);
        
        // Add click interactions
        addClickInteractions(card);
        
        // Add action button handlers
        addActionButtonHandlers(card);
    });
    
    // Add floating elements
    addFloatingElements();
    
    // Initialize live chat functionality
    initializeLiveChat();
}

function addHoverEffects(contactCard) {
    contactCard.addEventListener('mouseenter', function() {
        // Add particle effects
        createParticleEffect(this);
        
        // Add floating animation
        this.style.animation = 'contactFloat 2s ease-in-out infinite';
        
        // Track interaction
        trackContactInteraction(this, 'hover');
    });
    
    contactCard.addEventListener('mouseleave', function() {
        // Remove floating animation
        this.style.animation = '';
        
        // Stop particle effects
        stopParticleEffects(this);
    });
}

function addClickInteractions(contactCard) {
    contactCard.addEventListener('click', function(e) {
        // Don't trigger if clicking buttons or links
        if (e.target.closest('.btn-contact-3d') || e.target.closest('.contact-action-btn')) {
            return;
        }
        
        // Create ripple effect
        createRippleEffect(this, e);
        
        // Show contact details
        showContactDetails(this);
        
        // Track interaction
        trackContactInteraction(this, 'click');
    });
}

function addActionButtonHandlers(contactCard) {
    const copyBtn = contactCard.querySelector('.contact-action-btn[title*="Copy"]');
    const infoBtn = contactCard.querySelector('.contact-action-btn[title*="Info"]');
    
    if (copyBtn) {
        copyBtn.addEventListener('click', function(e) {
            e.stopPropagation();
            handleCopyAction(contactCard, this);
        });
    }
    
    if (infoBtn) {
        infoBtn.addEventListener('click', function(e) {
            e.stopPropagation();
            handleInfoAction(contactCard, this);
        });
    }
}

function createParticleEffect(contactCard) {
    const particles = [];
    const particleCount = 6;
    
    for (let i = 0; i < particleCount; i++) {
        const particle = document.createElement('div');
        particle.className = 'contact-particle';
        particle.style.cssText = `
            position: absolute;
            width: 4px;
            height: 4px;
            background: #3B82F6;
            border-radius: 50%;
            pointer-events: none;
            z-index: 1;
            animation: contactParticleFloat 2s ease-out forwards;
        `;
        
        // Random position around the contact card
        const angle = (i / particleCount) * Math.PI * 2;
        const distance = 50 + Math.random() * 30;
        const x = Math.cos(angle) * distance;
        const y = Math.sin(angle) * distance;
        
        particle.style.left = `calc(50% + ${x}px)`;
        particle.style.top = `calc(50% + ${y}px)`;
        particle.style.animationDelay = `${i * 0.1}s`;
        
        contactCard.appendChild(particle);
        particles.push(particle);
        
        // Remove particle after animation
        setTimeout(() => {
            if (particle.parentNode) {
                particle.parentNode.removeChild(particle);
            }
        }, 2000);
    }
    
    // Store particles for cleanup
    contactCard._particles = particles;
}

function stopParticleEffects(contactCard) {
    if (contactCard._particles) {
        contactCard._particles.forEach(particle => {
            if (particle.parentNode) {
                particle.parentNode.removeChild(particle);
            }
        });
        contactCard._particles = [];
    }
}

function createRippleEffect(contactCard, event) {
    const ripple = document.createElement('div');
    ripple.className = 'contact-ripple';
    
    const rect = contactCard.getBoundingClientRect();
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
        animation: contactRipple 0.6s linear;
        pointer-events: none;
        z-index: 1;
    `;
    
    contactCard.appendChild(ripple);
    
    // Remove ripple after animation
    setTimeout(() => {
        if (ripple.parentNode) {
            ripple.parentNode.removeChild(ripple);
        }
    }, 600);
}

function showContactDetails(contactCard) {
    const contactType = contactCard.getAttribute('data-contact');
    const title = contactCard.querySelector('.contact-title-3d').textContent;
    const description = contactCard.querySelector('.contact-description-3d').textContent;
    const statusText = contactCard.querySelector('.status-text').textContent;
    
    // Create tooltip
    const tooltip = document.createElement('div');
    tooltip.className = 'contact-tooltip';
    tooltip.style.cssText = `
        position: absolute;
        top: -140px;
        left: 50%;
        transform: translateX(-50%);
        background: rgba(0, 0, 0, 0.95);
        color: white;
        padding: 1.2rem 1.5rem;
        border-radius: 12px;
        backdrop-filter: blur(15px);
        border: 1px solid rgba(59, 130, 246, 0.3);
        box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
        min-width: 220px;
        text-align: center;
        z-index: 1000;
        opacity: 0;
        transition: opacity 0.3s ease;
        pointer-events: none;
    `;
    
    let contactInfo = '';
    switch(contactType) {
        case 'call':
            contactInfo = 'Phone: +94 77 123 4567<br>Available 24/7';
            break;
        case 'email':
            contactInfo = 'Email: support@carrentalsystem.com<br>Response within 2 hours';
            break;
        case 'chat':
            contactInfo = 'Live Chat Support<br>Online now - Instant response';
            break;
    }
    
    tooltip.innerHTML = `
        <div style="font-weight: 700; margin-bottom: 0.5rem; color: #3B82F6;">${title}</div>
        <div style="font-size: 0.9rem; color: rgba(255, 255, 255, 0.8); margin-bottom: 0.75rem;">${description}</div>
        <div style="font-size: 0.85rem; color: rgba(255, 255, 255, 0.9);">${contactInfo}</div>
    `;
    
    contactCard.appendChild(tooltip);
    
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

function handleCopyAction(contactCard, copyBtn) {
    const contactType = contactCard.getAttribute('data-contact');
    let textToCopy = '';
    let successMessage = '';
    
    switch(contactType) {
        case 'call':
            textToCopy = '+94771234567';
            successMessage = 'Phone number copied to clipboard!';
            break;
        case 'email':
            textToCopy = 'abaasith18@gmail.com'; // Support email from your config
            successMessage = 'Email address copied to clipboard!';
            break;
        case 'chat':
            textToCopy = 'Live Chat Support - Car Rental System';
            successMessage = 'Chat info copied to clipboard!';
            break;
    }
    
    // Copy to clipboard
    navigator.clipboard.writeText(textToCopy).then(() => {
        showToast(successMessage, 'success');
        
        // Add visual feedback
        copyBtn.style.background = '#10B981';
        copyBtn.style.color = 'white';
        copyBtn.innerHTML = '<i class="bi bi-check"></i>';
        
        setTimeout(() => {
            copyBtn.style.background = '';
            copyBtn.style.color = '';
            copyBtn.innerHTML = '<i class="bi bi-clipboard"></i>';
        }, 2000);
        
    }).catch(() => {
        showToast('Failed to copy to clipboard', 'error');
    });
    
    // Track interaction
    trackContactInteraction(contactCard, 'copy');
}

function handleInfoAction(contactCard, infoBtn) {
    const contactType = contactCard.getAttribute('data-contact');
    let infoMessage = '';
    
    switch(contactType) {
        case 'chat':
            infoMessage = 'WhatsApp Chat Features:\n• Instant response via WhatsApp\n• Available 24/7\n• Send images and documents\n• Voice messages supported\n• Direct WhatsApp conversation';
            break;
        default:
            infoMessage = 'Click the main button to contact us directly!';
    }
    
    showToast(infoMessage, 'info');
    
    // Track interaction
    trackContactInteraction(contactCard, 'info');
}

function initializeLiveChat() {
    // WhatsApp redirect is now handled directly in HTML
    // This function is kept for compatibility but does nothing
}

function addFloatingElements() {
    const contactSection = document.getElementById('contact');
    if (!contactSection) return;
    
    // Add floating elements
    for (let i = 0; i < 3; i++) {
        const element = document.createElement('div');
        element.className = 'floating-contact-element';
        const icons = ['<i class="bi bi-telephone"></i>', '<i class="bi bi-envelope"></i>', '<i class="bi bi-chat-dots"></i>'];
        element.innerHTML = icons[i];
        element.style.cssText = `
            position: absolute;
            font-size: 2rem;
            color: rgba(59, 130, 246, 0.1);
            pointer-events: none;
            z-index: 0;
            animation: contactFloat 8s ease-in-out infinite;
            animation-delay: ${i * 2}s;
        `;
        
        // Random positioning
        const positions = [
            { top: '10%', left: '5%' },
            { top: '60%', right: '10%' },
            { bottom: '20%', left: '15%' }
        ];
        
        Object.assign(element.style, positions[i]);
        contactSection.appendChild(element);
    }
}

function trackContactInteraction(contactCard, action) {
    const contactType = contactCard.getAttribute('data-contact');
    const title = contactCard.querySelector('.contact-title-3d').textContent;
    
    // Store in localStorage for analytics
    const interactions = JSON.parse(localStorage.getItem('contactInteractions') || '[]');
    interactions.push({
        contactType,
        title,
        action,
        timestamp: new Date().toISOString()
    });
    
    // Keep only last 50 interactions
    if (interactions.length > 50) {
        interactions.splice(0, interactions.length - 50);
    }
    
    localStorage.setItem('contactInteractions', JSON.stringify(interactions));
}

// Add CSS animations dynamically
const style = document.createElement('style');
style.textContent = `
    @keyframes contactParticleFloat {
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
`;
document.head.appendChild(style);

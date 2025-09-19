// Interactive How It Works Section
document.addEventListener('DOMContentLoaded', function() {
    console.log('Initializing Interactive How It Works...');
    
    initializeInteractiveHowItWorks();
    createFloatingElements();
    initializeStepAnimations();
    initializeStepInteractions();
    initializeProgressTracking();
});

function initializeInteractiveHowItWorks() {
    const howItWorksSection = document.getElementById('how-it-works');
    if (!howItWorksSection) {
        console.warn('How It Works section not found');
        return;
    }
    
    console.log('How It Works section found, initializing...');
    
    // Add glow effects to all step cards
    const stepCards = howItWorksSection.querySelectorAll('.step-3d');
    stepCards.forEach((card, index) => {
        // Add glow effect div
        const glowEffect = document.createElement('div');
        glowEffect.className = 'step-glow-effect';
        card.appendChild(glowEffect);
        
        // Add progress line (except for last card)
        if (index < stepCards.length - 1) {
            const progressLine = document.createElement('div');
            progressLine.className = 'step-progress-line';
            card.appendChild(progressLine);
        }
        
        // Add step counter
        const stepCounter = document.createElement('div');
        stepCounter.className = 'step-counter';
        stepCounter.textContent = index + 1;
        card.appendChild(stepCounter);
        
        // Add data attributes for tracking
        card.setAttribute('data-step', index + 1);
        card.setAttribute('data-interactive', 'true');
    });
    
    console.log(`Enhanced ${stepCards.length} step cards`);
}

function createFloatingElements() {
    const howItWorksSection = document.getElementById('how-it-works');
    if (!howItWorksSection) return;
    
    // Create floating car icons
    const floatingContainer = document.createElement('div');
    floatingContainer.className = 'floating-elements-container';
    floatingContainer.style.cssText = `
        position: absolute;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        pointer-events: none;
        z-index: 0;
        overflow: hidden;
    `;
    
    const carIcons = ['bi-car-front', 'bi-truck', 'bi-car-front-fill', 'bi-truck-front'];
    
    for (let i = 0; i < 6; i++) {
        const floatingElement = document.createElement('i');
        floatingElement.className = `floating-element bi ${carIcons[i % carIcons.length]}`;
        floatingElement.style.cssText = `
            position: absolute;
            font-size: 2rem;
            color: rgba(59, 130, 246, 0.1);
            top: ${Math.random() * 100}%;
            left: ${Math.random() * 100}%;
            animation-delay: ${Math.random() * 6}s;
            animation-duration: ${6 + Math.random() * 4}s;
        `;
        floatingContainer.appendChild(floatingElement);
    }
    
    howItWorksSection.appendChild(floatingContainer);
    console.log('Created floating elements');
}

function initializeStepAnimations() {
    const stepCards = document.querySelectorAll('#how-it-works .step-3d');
    
    // Intersection Observer for scroll animations
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('animate-in');
                console.log(`Step ${entry.target.dataset.step} animated in`);
            }
        });
    }, {
        threshold: 0.2,
        rootMargin: '0px 0px -50px 0px'
    });
    
    stepCards.forEach(card => {
        observer.observe(card);
    });
    
    console.log('Step animations initialized');
}

function initializeStepInteractions() {
    const stepCards = document.querySelectorAll('#how-it-works .step-3d');
    
    stepCards.forEach((card, index) => {
        // Click interaction
        card.addEventListener('click', function(e) {
            e.preventDefault();
            handleStepClick(card, index);
        });
        
        // Hover interactions
        card.addEventListener('mouseenter', function() {
            handleStepHover(card, index, true);
        });
        
        card.addEventListener('mouseleave', function() {
            handleStepHover(card, index, false);
        });
        
        // Touch interactions for mobile
        card.addEventListener('touchstart', function(e) {
            e.preventDefault();
            handleStepClick(card, index);
        });
    });
    
    console.log('Step interactions initialized');
}

function handleStepClick(stepCard, index) {
    console.log(`Step ${index + 1} clicked`);
    
    // Create ripple effect
    createStepRipple(stepCard);
    
    // Show detailed tooltip
    showStepDetails(stepCard, index);
    
    // Add active state
    stepCard.classList.add('active');
    
    // Remove active state from other cards
    const allSteps = document.querySelectorAll('#how-it-works .step-3d');
    allSteps.forEach((step, i) => {
        if (i !== index) {
            step.classList.remove('active');
        }
    });
    
    // Auto-remove active state after 3 seconds
    setTimeout(() => {
        stepCard.classList.remove('active');
    }, 3000);
}

function handleStepHover(stepCard, index, isEntering) {
    if (isEntering) {
        console.log(`Hovering over step ${index + 1}`);
        
        // Add particle effects
        addHoverParticles(stepCard);
        
        // Animate connected elements
        animateConnectedElements(stepCard, index);
    } else {
        // Clean up hover effects
        removeHoverParticles(stepCard);
    }
}

function createStepRipple(stepCard) {
    const ripple = document.createElement('div');
    ripple.className = 'step-ripple';
    
    ripple.style.cssText = `
        position: absolute;
        border-radius: 50%;
        background: rgba(59, 130, 246, 0.4);
        transform: scale(0);
        animation: stepRipple 0.6s linear;
        pointer-events: none;
        top: 50%;
        left: 50%;
        width: 120px;
        height: 120px;
        margin-left: -60px;
        margin-top: -60px;
        z-index: 1;
    `;
    
    stepCard.appendChild(ripple);
    
    setTimeout(() => {
        if (ripple.parentNode) {
            ripple.parentNode.removeChild(ripple);
        }
    }, 600);
}

function showStepDetails(stepCard, index) {
    const stepTitle = stepCard.querySelector('.step-title').textContent;
    const stepDescription = stepCard.querySelector('.step-description').textContent;
    
    // Remove any existing tooltip
    const existingTooltip = stepCard.querySelector('.step-tooltip');
    if (existingTooltip) {
        existingTooltip.remove();
    }
    
    // Create detailed tooltip
    const tooltip = document.createElement('div');
    tooltip.className = 'step-tooltip';
    tooltip.innerHTML = `
        <div class="step-tooltip-content">
            <h4>${stepTitle}</h4>
            <p>Step ${index + 1} of 4</p>
            <p>${stepDescription}</p>
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
        min-width: 250px;
        text-align: center;
    `;
    
    stepCard.style.position = 'relative';
    stepCard.appendChild(tooltip);
    
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

function addHoverParticles(stepCard) {
    // Create floating particles around the card
    for (let i = 0; i < 5; i++) {
        const particle = document.createElement('div');
        particle.className = 'hover-particle';
        particle.style.cssText = `
            position: absolute;
            width: 4px;
            height: 4px;
            background: rgba(59, 130, 246, 0.6);
            border-radius: 50%;
            pointer-events: none;
            z-index: 1;
            top: ${Math.random() * 100}%;
            left: ${Math.random() * 100}%;
            animation: particleFloat 2s ease-in-out infinite;
            animation-delay: ${Math.random() * 2}s;
        `;
        
        stepCard.appendChild(particle);
        
        // Remove particle after animation
        setTimeout(() => {
            if (particle.parentNode) {
                particle.parentNode.removeChild(particle);
            }
        }, 2000);
    }
}

function removeHoverParticles(stepCard) {
    const particles = stepCard.querySelectorAll('.hover-particle');
    particles.forEach(particle => {
        if (particle.parentNode) {
            particle.parentNode.removeChild(particle);
        }
    });
}

function animateConnectedElements(stepCard, index) {
    // Animate progress lines
    const progressLine = stepCard.querySelector('.step-progress-line');
    if (progressLine) {
        progressLine.style.transform = 'translateY(-50%) scaleY(2)';
        progressLine.style.opacity = '1';
        progressLine.style.boxShadow = '0 0 15px rgba(59, 130, 246, 0.4)';
    }
    
    // Animate step counter
    const stepCounter = stepCard.querySelector('.step-counter');
    if (stepCounter) {
        stepCounter.style.opacity = '1';
        stepCounter.style.transform = 'scale(1)';
    }
}

function initializeProgressTracking() {
    // Track user interaction with steps
    let stepInteractions = {
        step1: 0,
        step2: 0,
        step3: 0,
        step4: 0
    };
    
    const stepCards = document.querySelectorAll('#how-it-works .step-3d');
    
    stepCards.forEach((card, index) => {
        card.addEventListener('click', function() {
            stepInteractions[`step${index + 1}`]++;
            console.log(`Step ${index + 1} interaction count:`, stepInteractions[`step${index + 1}`]);
            
            // Store in localStorage for analytics
            localStorage.setItem('howItWorksInteractions', JSON.stringify(stepInteractions));
        });
    });
    
    // Load previous interactions
    const savedInteractions = localStorage.getItem('howItWorksInteractions');
    if (savedInteractions) {
        stepInteractions = JSON.parse(savedInteractions);
        console.log('Loaded previous step interactions:', stepInteractions);
    }
}

// Add CSS for particle animation
const style = document.createElement('style');
style.textContent = `
    @keyframes particleFloat {
        0%, 100% {
            transform: translateY(0px) scale(1);
            opacity: 0.6;
        }
        50% {
            transform: translateY(-20px) scale(1.2);
            opacity: 1;
        }
    }
    
    .hover-particle {
        animation: particleFloat 2s ease-in-out infinite;
    }
`;
document.head.appendChild(style);

console.log('Interactive How It Works JavaScript loaded successfully');

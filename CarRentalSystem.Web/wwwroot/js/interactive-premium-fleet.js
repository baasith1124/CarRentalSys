// Interactive Premium Fleet Section
document.addEventListener('DOMContentLoaded', function() {
    console.log('Initializing Interactive Premium Fleet...');
    
    initializeInteractiveFleet();
    createFloatingCarElements();
    initializeCarCardAnimations();
    initializeCarCardInteractions();
    initializeFleetAnalytics();
});

function initializeInteractiveFleet() {
    const fleetSection = document.getElementById('cars');
    if (!fleetSection) {
        console.warn('Premium Fleet section not found');
        return;
    }
    
    console.log('Premium Fleet section found, initializing...');
    
    // Add glow effects to all car cards
    const carCards = fleetSection.querySelectorAll('.car-card-enhanced-3d');
    carCards.forEach((card, index) => {
        // Add glow effect div
        const glowEffect = document.createElement('div');
        glowEffect.className = 'card-glow-effect';
        card.appendChild(glowEffect);
        
        // Add data attributes for tracking
        card.setAttribute('data-car-index', index);
        card.setAttribute('data-interactive', 'true');
        
        // Add car brand detection
        const carTitle = card.querySelector('.car-title');
        if (carTitle) {
            const brand = extractCarBrand(carTitle.textContent);
            card.setAttribute('data-car-brand', brand);
        }
    });
    
    console.log(`Enhanced ${carCards.length} car cards`);
}

function extractCarBrand(carName) {
    const brands = ['Toyota', 'Honda', 'Nissan', 'Ford', 'BMW', 'Mercedes', 'Audi', 'Volkswagen', 'Hyundai', 'Kia', 'Mazda', 'Subaru', 'Lexus', 'Infiniti', 'Acura', 'Chevrolet', 'Dodge', 'Jeep', 'Ram', 'GMC', 'Cadillac', 'Lincoln', 'Buick', 'Chrysler', 'Mitsubishi', 'Suzuki', 'Isuzu', 'Citroen', 'Peugeot', 'Renault', 'Fiat', 'Alfa Romeo', 'Maserati', 'Ferrari', 'Lamborghini', 'Porsche', 'Bentley', 'Rolls-Royce', 'Aston Martin', 'McLaren', 'Bugatti', 'Koenigsegg'];
    
    for (const brand of brands) {
        if (carName.toLowerCase().includes(brand.toLowerCase())) {
            return brand;
        }
    }
    return 'Unknown';
}

function createFloatingCarElements() {
    const fleetSection = document.getElementById('cars');
    if (!fleetSection) return;
    
    // Create floating car icons
    const floatingContainer = document.createElement('div');
    floatingContainer.className = 'floating-cars-container';
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
    
    const carIcons = ['bi-car-front', 'bi-truck', 'bi-car-front-fill', 'bi-truck-front', 'bi-bus-front', 'bi-motorcycle'];
    
    for (let i = 0; i < 8; i++) {
        const floatingElement = document.createElement('i');
        floatingElement.className = `floating-car-element bi ${carIcons[i % carIcons.length]}`;
        floatingElement.style.cssText = `
            position: absolute;
            font-size: 2.5rem;
            color: rgba(59, 130, 246, 0.1);
            top: ${Math.random() * 100}%;
            left: ${Math.random() * 100}%;
            animation-delay: ${Math.random() * 8}s;
            animation-duration: ${8 + Math.random() * 4}s;
        `;
        floatingContainer.appendChild(floatingElement);
    }
    
    fleetSection.appendChild(floatingContainer);
    console.log('Created floating car elements');
}

function initializeCarCardAnimations() {
    const carCards = document.querySelectorAll('#cars .car-card-enhanced-3d');
    
    // Intersection Observer for scroll animations
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('animate-in');
                console.log(`Car card ${entry.target.dataset.carIndex} animated in`);
            }
        });
    }, {
        threshold: 0.2,
        rootMargin: '0px 0px -50px 0px'
    });
    
    carCards.forEach(card => {
        observer.observe(card);
    });
    
    console.log('Car card animations initialized');
}

function initializeCarCardInteractions() {
    const carCards = document.querySelectorAll('#cars .car-card-enhanced-3d');
    
    carCards.forEach((card, index) => {
        // Click interaction
        card.addEventListener('click', function(e) {
            // Don't trigger if clicking on action buttons
            if (e.target.closest('.action-btn') || e.target.closest('.btn-view-details-3d') || e.target.closest('.quick-action-btn')) {
                return;
            }
            
            e.preventDefault();
            handleCarCardClick(card, index);
        });
        
        // Hover interactions
        card.addEventListener('mouseenter', function() {
            handleCarCardHover(card, index, true);
        });
        
        card.addEventListener('mouseleave', function() {
            handleCarCardHover(card, index, false);
        });
        
        // Touch interactions for mobile
        card.addEventListener('touchstart', function(e) {
            e.preventDefault();
            handleCarCardClick(card, index);
        });
        
        // Initialize action buttons
        initializeActionButtons(card, index);
    });
    
    console.log('Car card interactions initialized');
}

function handleCarCardClick(card, index) {
    console.log(`Car card ${index + 1} clicked`);
    
    // Create ripple effect
    createCarCardRipple(card);
    
    // Show car details tooltip
    showCarDetails(card, index);
    
    // Add active state
    card.classList.add('active');
    
    // Remove active state from other cards
    const allCards = document.querySelectorAll('#cars .car-card-enhanced-3d');
    allCards.forEach((carCard, i) => {
        if (i !== index) {
            carCard.classList.remove('active');
        }
    });
    
    // Auto-remove active state after 3 seconds
    setTimeout(() => {
        card.classList.remove('active');
    }, 3000);
}

function handleCarCardHover(card, index, isEntering) {
    if (isEntering) {
        console.log(`Hovering over car card ${index + 1}`);
        
        // Add particle effects
        addCarHoverParticles(card);
        
        // Animate connected elements
        animateCarConnectedElements(card, index);
        
        // Play hover sound effect (optional)
        playHoverSound();
    } else {
        // Clean up hover effects
        removeCarHoverParticles(card);
    }
}

function createCarCardRipple(card) {
    const ripple = document.createElement('div');
    ripple.className = 'card-ripple';
    
    ripple.style.cssText = `
        position: absolute;
        border-radius: 50%;
        background: rgba(59, 130, 246, 0.4);
        transform: scale(0);
        animation: cardRipple 0.6s linear;
        pointer-events: none;
        top: 50%;
        left: 50%;
        width: 120px;
        height: 120px;
        margin-left: -60px;
        margin-top: -60px;
        z-index: 1;
    `;
    
    card.appendChild(ripple);
    
    setTimeout(() => {
        if (ripple.parentNode) {
            ripple.parentNode.removeChild(ripple);
        }
    }, 600);
}

function showCarDetails(card, index) {
    const carTitle = card.querySelector('.car-title').textContent;
    const carModel = card.querySelector('.car-model').textContent;
    const carBrand = card.getAttribute('data-car-brand') || 'Unknown';
    
    // Remove any existing tooltip
    const existingTooltip = card.querySelector('.car-tooltip');
    if (existingTooltip) {
        existingTooltip.remove();
    }
    
    // Create detailed tooltip
    const tooltip = document.createElement('div');
    tooltip.className = 'car-tooltip';
    tooltip.innerHTML = `
        <div class="car-tooltip-content">
            <h4>${carTitle}</h4>
            <p><strong>Brand:</strong> ${carBrand}</p>
            <p><strong>Model:</strong> ${carModel}</p>
            <p>Click to view full details and booking options</p>
        </div>
    `;
    
    // Style the tooltip
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
        z-index: 1000;
        opacity: 0;
        transition: all 0.3s ease;
        pointer-events: none;
        white-space: nowrap;
        box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
        min-width: 250px;
        text-align: center;
    `;
    
    card.style.position = 'relative';
    card.appendChild(tooltip);
    
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

function addCarHoverParticles(card) {
    // Create floating particles around the card
    for (let i = 0; i < 6; i++) {
        const particle = document.createElement('div');
        particle.className = 'car-hover-particle';
        particle.style.cssText = `
            position: absolute;
            width: 6px;
            height: 6px;
            background: rgba(59, 130, 246, 0.6);
            border-radius: 50%;
            pointer-events: none;
            z-index: 1;
            top: ${Math.random() * 100}%;
            left: ${Math.random() * 100}%;
            animation: carParticleFloat 3s ease-in-out infinite;
            animation-delay: ${Math.random() * 3}s;
        `;
        
        card.appendChild(particle);
        
        // Remove particle after animation
        setTimeout(() => {
            if (particle.parentNode) {
                particle.parentNode.removeChild(particle);
            }
        }, 3000);
    }
}

function removeCarHoverParticles(card) {
    const particles = card.querySelectorAll('.car-hover-particle');
    particles.forEach(particle => {
        if (particle.parentNode) {
            particle.parentNode.removeChild(particle);
        }
    });
}

function animateCarConnectedElements(card, index) {
    // Animate status badge
    const statusBadge = card.querySelector('.status-badge-3d');
    if (statusBadge) {
        statusBadge.style.transform = 'scale(1.1) rotateY(10deg)';
        statusBadge.style.boxShadow = '0 6px 20px rgba(16, 185, 129, 0.5)';
    }
    
    // Animate feature items
    const featureItems = card.querySelectorAll('.feature-item');
    featureItems.forEach((item, i) => {
        setTimeout(() => {
            item.style.transform = 'translateY(-2px)';
            item.style.boxShadow = '0 4px 15px rgba(59, 130, 246, 0.2)';
        }, i * 100);
    });
}

function initializeActionButtons(card, index) {
    // Favorite button
    const favoriteBtn = card.querySelector('.favorite-btn');
    if (favoriteBtn) {
        favoriteBtn.addEventListener('click', function(e) {
            e.stopPropagation();
            handleFavoriteAction(card, index);
        });
    }
    
    // Share button
    const shareBtn = card.querySelector('.share-btn');
    if (shareBtn) {
        shareBtn.addEventListener('click', function(e) {
            e.stopPropagation();
            handleShareAction(card, index);
        });
    }
    
    // View details button
    const viewBtn = card.querySelector('.view-btn');
    if (viewBtn) {
        viewBtn.addEventListener('click', function(e) {
            e.stopPropagation();
            handleViewDetailsAction(card, index);
        });
    }
    
    // Quick action buttons
    const quickActionBtns = card.querySelectorAll('.quick-action-btn');
    quickActionBtns.forEach((btn, i) => {
        btn.addEventListener('click', function(e) {
            e.stopPropagation();
            handleQuickAction(card, index, i);
        });
    });
}

function handleFavoriteAction(card, index) {
    console.log(`Favorite action for car ${index + 1}`);
    
    const favoriteBtn = card.querySelector('.favorite-btn');
    const icon = favoriteBtn.querySelector('i');
    
    // Toggle favorite state
    if (icon.classList.contains('bi-heart')) {
        icon.classList.remove('bi-heart');
        icon.classList.add('bi-heart-fill');
        favoriteBtn.style.color = '#EF4444';
        showToast('Added to favorites!', 'success');
    } else {
        icon.classList.remove('bi-heart-fill');
        icon.classList.add('bi-heart');
        favoriteBtn.style.color = '';
        showToast('Removed from favorites!', 'info');
    }
    
    // Create heart animation
    createHeartAnimation(favoriteBtn);
}

function handleShareAction(card, index) {
    console.log(`Share action for car ${index + 1}`);
    
    const carTitle = card.querySelector('.car-title').textContent;
    const shareUrl = window.location.href;
    const shareText = `Check out this amazing car: ${carTitle}`;
    
    if (navigator.share) {
        navigator.share({
            title: carTitle,
            text: shareText,
            url: shareUrl
        });
    } else {
        // Fallback: copy to clipboard
        navigator.clipboard.writeText(`${shareText} - ${shareUrl}`).then(() => {
            showToast('Link copied to clipboard!', 'success');
        });
    }
}

function handleViewDetailsAction(card, index) {
    console.log(`View details action for car ${index + 1}`);
    
    // Add loading state
    const viewBtn = card.querySelector('.view-btn');
    const originalContent = viewBtn.innerHTML;
    viewBtn.innerHTML = '<i class="bi bi-hourglass-split"></i>';
    viewBtn.style.pointerEvents = 'none';
    
    // Simulate loading
    setTimeout(() => {
        viewBtn.innerHTML = originalContent;
        viewBtn.style.pointerEvents = '';
        // Navigate to details page
        const detailsUrl = viewBtn.href;
        window.location.href = detailsUrl;
    }, 500);
}

function handleQuickAction(card, index, actionIndex) {
    const actions = ['Compare', 'Calculate Price'];
    console.log(`${actions[actionIndex]} action for car ${index + 1}`);
    
    showToast(`${actions[actionIndex]} feature coming soon!`, 'info');
}

function createHeartAnimation(button) {
    const heart = document.createElement('div');
    heart.innerHTML = '❤️';
    heart.style.cssText = `
        position: absolute;
        top: 50%;
        left: 50%;
        transform: translate(-50%, -50%);
        font-size: 1.5rem;
        pointer-events: none;
        z-index: 10;
        animation: heartFloat 1s ease-out forwards;
    `;
    
    button.style.position = 'relative';
    button.appendChild(heart);
    
    setTimeout(() => {
        if (heart.parentNode) {
            heart.parentNode.removeChild(heart);
        }
    }, 1000);
}

function playHoverSound() {
    // Optional: Add subtle hover sound effect
    // This would require audio files and proper implementation
    console.log('Hover sound effect (optional)');
}

function initializeFleetAnalytics() {
    // Track user interaction with car cards
    let fleetInteractions = {
        totalViews: 0,
        favorites: 0,
        shares: 0,
        detailsViews: 0
    };
    
    const carCards = document.querySelectorAll('#cars .car-card-enhanced-3d');
    
    carCards.forEach((card, index) => {
        // Track card views
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    fleetInteractions.totalViews++;
                    console.log(`Car ${index + 1} viewed. Total views: ${fleetInteractions.totalViews}`);
                }
            });
        }, { threshold: 0.5 });
        
        observer.observe(card);
    });
    
    // Store in localStorage for analytics
    localStorage.setItem('fleetInteractions', JSON.stringify(fleetInteractions));
    
    // Load previous interactions
    const savedInteractions = localStorage.getItem('fleetInteractions');
    if (savedInteractions) {
        fleetInteractions = JSON.parse(savedInteractions);
        console.log('Loaded previous fleet interactions:', fleetInteractions);
    }
}

// Add CSS for particle animation
const style = document.createElement('style');
style.textContent = `
    @keyframes carParticleFloat {
        0%, 100% {
            transform: translateY(0px) scale(1);
            opacity: 0.6;
        }
        50% {
            transform: translateY(-25px) scale(1.3);
            opacity: 1;
        }
    }
    
    @keyframes heartFloat {
        0% {
            transform: translate(-50%, -50%) scale(0);
            opacity: 1;
        }
        50% {
            transform: translate(-50%, -70px) scale(1.2);
            opacity: 1;
        }
        100% {
            transform: translate(-50%, -100px) scale(0);
            opacity: 0;
        }
    }
    
    .car-hover-particle {
        animation: carParticleFloat 3s ease-in-out infinite;
    }
`;
document.head.appendChild(style);

console.log('Interactive Premium Fleet JavaScript loaded successfully');

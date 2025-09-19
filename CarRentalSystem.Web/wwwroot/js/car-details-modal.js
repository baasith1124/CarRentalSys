// Car Details Modal JavaScript

// Initialize modal when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    initializeCarDetailsModal();
});

function initializeCarDetailsModal() {
    const modalElement = document.getElementById('carDetailsModal');
    if (!modalElement) {
        return;
    }
    
    // Initialize Bootstrap modal
    window.carDetailsModal = new bootstrap.Modal(modalElement, {
        backdrop: 'static',
        keyboard: false
    });
    
    // Add event listeners for modal buttons
    const favoriteBtn = modalElement.querySelector('.favorite-btn');
    const shareBtn = modalElement.querySelector('.share-btn');
    
    if (favoriteBtn) {
        favoriteBtn.addEventListener('click', handleModalFavoriteClick);
    }
    
    if (shareBtn) {
        shareBtn.addEventListener('click', handleModalShareClick);
    }
    
    // Add modal event listeners
    modalElement.addEventListener('hidden.bs.modal', function() {
        modalElement.querySelector('.car-details-modal-3d').classList.remove('show');
    });
}

function showCarDetailsModal(carData) {
    if (!window.carDetailsModal) {
        initializeCarDetailsModal();
        
        if (!window.carDetailsModal) {
            return;
        }
    }
    
    // Populate modal with car data
    populateModalData(carData);
    
    // Show modal
    window.carDetailsModal.show();
}

function populateModalData(carData) {
    // Basic car information
    const nameElement = document.getElementById('modalCarName');
    const modelElement = document.getElementById('modalCarModel');
    const imageElement = document.getElementById('modalCarImage');
    
    if (nameElement) nameElement.textContent = carData.name || 'Car Details';
    if (modelElement) modelElement.textContent = carData.model || 'Car Model';
    if (imageElement) imageElement.src = carData.imageUrl || '/images/cars/civic.png';
    
    // Car features
    const seatsElement = document.getElementById('modalCarSeats');
    const transmissionElement = document.getElementById('modalCarTransmission');
    const fuelElement = document.getElementById('modalCarFuelType');
    const speedElement = document.getElementById('modalCarTopSpeed');
    
    if (seatsElement) seatsElement.textContent = carData.seats || '5';
    if (transmissionElement) transmissionElement.textContent = carData.transmission || 'Auto';
    if (fuelElement) fuelElement.textContent = carData.fuelType || 'Petrol';
    if (speedElement) speedElement.textContent = carData.topSpeed || '180';
    
    // Availability dates
    const availableFromElement = document.getElementById('modalCarAvailableFrom');
    const availableToElement = document.getElementById('modalCarAvailableTo');
    
    if (availableFromElement) availableFromElement.textContent = formatDate(carData.availableFrom);
    if (availableToElement) availableToElement.textContent = formatDate(carData.availableTo);
    
}

function formatDate(dateString) {
    if (!dateString) return 'N/A';
    
    try {
        const date = new Date(dateString);
        return date.toLocaleDateString('en-US', {
            year: 'numeric',
            month: 'short',
            day: 'numeric'
        });
    } catch (error) {
        console.error('Error formatting date:', error);
        return dateString;
    }
}

function handleModalFavoriteClick(e) {
    e.preventDefault();
    // Add favorite functionality here
}

function handleModalShareClick(e) {
    e.preventDefault();
    // Add share functionality here
}


// Global function to show car details modal
window.showCarDetailsModal = showCarDetailsModal;

// Add click event listener for view details buttons
document.addEventListener('click', function(e) {
    // Check for view details buttons
    const viewBtn = e.target.closest('.view-btn');
    const viewDetailsBtn = e.target.closest('.btn-view-details-3d');
    
    if (viewBtn || viewDetailsBtn) {
        e.preventDefault();
        e.stopPropagation();
        
        const button = viewBtn || viewDetailsBtn;
        
        // Extract car data from data attributes
        const carData = {
            carId: button.getAttribute('data-car-id'),
            name: button.getAttribute('data-car-name'),
            model: button.getAttribute('data-car-model'),
            imageUrl: button.getAttribute('data-car-image'),
            seats: button.getAttribute('data-car-seats'),
            transmission: button.getAttribute('data-car-transmission'),
            fuelType: button.getAttribute('data-car-fuel'),
            topSpeed: button.getAttribute('data-car-speed'),
            availableFrom: button.getAttribute('data-car-available-from'),
            availableTo: button.getAttribute('data-car-available-to')
        };
        
        // Show modal with car data
        showCarDetailsModal(carData);
    }
});
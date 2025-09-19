// DateTime Validation and Styling for Car Rental Search Form
document.addEventListener('DOMContentLoaded', function() {
    console.log('DateTime validation script loaded');
    initializeDateTimeValidation();
    initializeTooltips();
});

function initializeTooltips() {
    // Initialize Bootstrap tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl, {
            delay: { show: 500, hide: 100 },
            trigger: 'hover focus'
        });
    });
    console.log('Tooltips initialized:', tooltipList.length);
}

function initializeDateTimeValidation() {
    console.log('Initializing datetime validation...');
    
    const pickupInput = document.getElementById('pickupDateTime');
    const dropInput = document.getElementById('dropDateTime');
    
    if (!pickupInput || !dropInput) {
        console.error('DateTime inputs not found');
        return;
    }
    
    // Set minimum datetime (current time + 3 hours)
    const now = new Date();
    const minDateTime = new Date(now.getTime() + (3 * 60 * 60 * 1000)); // 3 hours from now
    const minDateTimeString = formatDateTimeForInput(minDateTime);
    
    console.log('Minimum datetime set to:', minDateTimeString);
    
    // Set minimum values for both inputs
    pickupInput.min = minDateTimeString;
    dropInput.min = minDateTimeString;
    
    // Add event listeners
    pickupInput.addEventListener('change', function() {
        validatePickupDateTime();
        updateDropDateTimeMin();
    });
    
    dropInput.addEventListener('change', function() {
        validateDropDateTime();
    });
    
    // Add focus/blur events for styling
    pickupInput.addEventListener('focus', function() {
        this.classList.remove('datetime-error');
        removeErrorMessage(this);
    });
    
    dropInput.addEventListener('focus', function() {
        this.classList.remove('datetime-error');
        removeErrorMessage(this);
    });
    
    console.log('DateTime validation initialized successfully');
}

function formatDateTimeForInput(date) {
    // Format date for datetime-local input (YYYY-MM-DDTHH:MM)
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    
    return `${year}-${month}-${day}T${hours}:${minutes}`;
}

function validatePickupDateTime() {
    const pickupInput = document.getElementById('pickupDateTime');
    const pickupValue = pickupInput.value;
    
    if (!pickupValue) {
        removeSuccessState(pickupInput);
        return;
    }
    
    const pickupDate = new Date(pickupValue);
    const now = new Date();
    const minDateTime = new Date(now.getTime() + (3 * 60 * 60 * 1000)); // 3 hours from now
    
    console.log('Validating pickup datetime:', pickupValue);
    console.log('Pickup date:', pickupDate);
    console.log('Minimum allowed:', minDateTime);
    
    if (pickupDate < minDateTime) {
        showDateTimeError(pickupInput, 'Please select a pickup time at least 3 hours from now');
        return;
    }
    
    // Valid datetime
    setSuccessState(pickupInput);
    console.log('Pickup datetime is valid');
}

function validateDropDateTime() {
    const pickupInput = document.getElementById('pickupDateTime');
    const dropInput = document.getElementById('dropDateTime');
    const dropValue = dropInput.value;
    
    if (!dropValue) {
        removeSuccessState(dropInput);
        return;
    }
    
    const dropDate = new Date(dropValue);
    const now = new Date();
    const minDateTime = new Date(now.getTime() + (3 * 60 * 60 * 1000)); // 3 hours from now
    
    console.log('Validating drop datetime:', dropValue);
    console.log('Drop date:', dropDate);
    console.log('Minimum allowed:', minDateTime);
    
    // Check if drop time is at least 3 hours from now
    if (dropDate < minDateTime) {
        showDateTimeError(dropInput, 'Please select a drop time at least 3 hours from now');
        return;
    }
    
    // Check if pickup time is set and drop time is after pickup time
    if (pickupInput.value) {
        const pickupDate = new Date(pickupInput.value);
        const minDropTime = new Date(pickupDate.getTime() + (1 * 60 * 60 * 1000)); // 1 hour after pickup
        
        if (dropDate < minDropTime) {
            showDateTimeError(dropInput, 'Drop time must be at least 1 hour after pickup time');
            return;
        }
    }
    
    // Valid datetime
    setSuccessState(dropInput);
    console.log('Drop datetime is valid');
}

function updateDropDateTimeMin() {
    const pickupInput = document.getElementById('pickupDateTime');
    const dropInput = document.getElementById('dropDateTime');
    
    if (!pickupInput.value) {
        return;
    }
    
    const pickupDate = new Date(pickupInput.value);
    const minDropTime = new Date(pickupDate.getTime() + (1 * 60 * 60 * 1000)); // 1 hour after pickup
    const minDropTimeString = formatDateTimeForInput(minDropTime);
    
    dropInput.min = minDropTimeString;
    console.log('Updated drop datetime minimum to:', minDropTimeString);
}

function setSuccessState(input) {
    input.classList.remove('datetime-error');
    input.classList.add('datetime-selected');
    removeErrorMessage(input);
}

function removeSuccessState(input) {
    input.classList.remove('datetime-selected');
}

function showDateTimeError(input, message) {
    input.classList.add('datetime-error');
    input.classList.remove('datetime-selected');
    
    // Remove any existing error message first
    removeErrorMessage(input);
    
    // Find the field wrapper to append error message
    const fieldWrapper = input.closest('.datetime-field-wrapper') || input.parentElement;
    
    // Create new error message
    const errorElement = document.createElement('div');
    errorElement.className = 'datetime-error-message';
    errorElement.textContent = message;
    fieldWrapper.appendChild(errorElement);
    
    // Auto-remove error after 4 seconds (shorter time for better UX)
    setTimeout(() => {
        if (errorElement && errorElement.parentElement) {
            errorElement.style.opacity = '0';
            errorElement.style.transform = 'translateY(-10px)';
            setTimeout(() => {
                if (errorElement.parentElement) {
                    errorElement.remove();
                }
            }, 300);
        }
    }, 4000);
    
    // Also remove error state from input after animation
    setTimeout(() => {
        input.classList.remove('datetime-error');
    }, 500);
}

function removeErrorMessage(input) {
    const fieldWrapper = input.closest('.datetime-field-wrapper') || input.parentElement;
    const errorElement = fieldWrapper.querySelector('.datetime-error-message');
    if (errorElement) {
        errorElement.remove();
    }
}

// Form submission validation
document.addEventListener('DOMContentLoaded', function() {
    const searchForm = document.querySelector('form[asp-controller="Cars"]');
    if (searchForm) {
        searchForm.addEventListener('submit', function(e) {
            const pickupInput = document.getElementById('pickupDateTime');
            const dropInput = document.getElementById('dropDateTime');
            
            // Validate pickup datetime
            if (!validateDateTimeInput(pickupInput, 'Pickup date and time')) {
                e.preventDefault();
                return false;
            }
            
            // Validate drop datetime
            if (!validateDateTimeInput(dropInput, 'Drop date and time')) {
                e.preventDefault();
                return false;
            }
            
            // Check if drop time is after pickup time
            if (pickupInput.value && dropInput.value) {
                const pickupDate = new Date(pickupInput.value);
                const dropDate = new Date(dropInput.value);
                
                if (dropDate <= pickupDate) {
                    e.preventDefault();
                    showDateTimeError(dropInput, 'Drop time must be after pickup time');
                    return false;
                }
            }
        });
    }
});

function validateDateTimeInput(input, fieldName) {
    if (!input.value.trim()) {
        showDateTimeError(input, `Please select a ${fieldName.toLowerCase()}`);
        return false;
    }
    
    const inputDate = new Date(input.value);
    const now = new Date();
    const minDateTime = new Date(now.getTime() + (3 * 60 * 60 * 1000)); // 3 hours from now
    
    if (inputDate < minDateTime) {
        showDateTimeError(input, `Please select a ${fieldName.toLowerCase()} at least 3 hours from now`);
        return false;
    }
    
    return true;
}

// Manual validation function for debugging
window.validateAllDateTimeInputs = function() {
    console.log('=== DateTime Validation Test ===');
    
    const pickupInput = document.getElementById('pickupDateTime');
    const dropInput = document.getElementById('dropDateTime');
    
    console.log('Pickup input:', pickupInput);
    console.log('Drop input:', dropInput);
    
    if (pickupInput && dropInput) {
        console.log('Pickup value:', pickupInput.value);
        console.log('Drop value:', dropInput.value);
        
        validatePickupDateTime();
        validateDropDateTime();
        
        console.log('Validation complete');
    } else {
        console.log('❌ DateTime inputs not found');
    }
};

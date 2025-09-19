// Google Places Autocomplete for Sri Lankan Locations
console.log('Google Places Autocomplete script loaded');

document.addEventListener('DOMContentLoaded', function() {
    console.log('DOM Content Loaded - Initializing Google Places');
    
    // Test if inputs exist
    testInputs();
    
    // Initialize Google Places Autocomplete
    initializeGooglePlaces();
});

function testInputs() {
    console.log('Testing input elements...');
    
    const pickupInput = document.querySelector('input[name="PickupLocation"]');
    const dropInput = document.querySelector('input[name="DropLocation"]');
    const pickupById = document.getElementById('pickupLocation');
    const dropById = document.getElementById('dropLocation');
    
    console.log('Pickup input by name:', pickupInput);
    console.log('Drop input by name:', dropInput);
    console.log('Pickup input by ID:', pickupById);
    console.log('Drop input by ID:', dropById);
    
    // Also check all inputs on the page
    const allInputs = document.querySelectorAll('input[type="text"]');
    console.log('All text inputs on page:', allInputs);
    
    allInputs.forEach((input, index) => {
        console.log(`Input ${index}:`, {
            name: input.name,
            id: input.id,
            placeholder: input.placeholder,
            element: input
        });
    });
}

function initializeGooglePlaces() {
    console.log('Initializing Google Places...');
    
    // Check if Google Maps API is loaded
    if (typeof google === 'undefined' || !google.maps || !google.maps.places) {
        console.warn('Google Maps API not loaded. Loading now...');
        loadGoogleMapsAPI();
        return;
    }
    
    console.log('Google Maps API is already loaded');
    initializeAutocompleteFunction();
}

function loadGoogleMapsAPI() {
    console.log('Loading Google Maps API...');
    
    // Check if script is already being loaded
    if (document.querySelector('script[src*="maps.googleapis.com"]')) {
        console.log('Google Maps API script already exists');
        return;
    }
    
    // Get the API key from the server
    const script = document.createElement('script');
    script.src = `https://maps.googleapis.com/maps/api/js?key=AIzaSyBdf2UWXbcGae7v4FI0ALJwmC6JZBUWBjE&libraries=places&callback=initializeAutocomplete`;
    script.async = true;
    script.defer = true;
    script.onerror = function() {
        console.error('Failed to load Google Maps API');
        // Try alternative approach
        setTimeout(() => {
            console.log('Retrying Google Maps API initialization...');
            initializeGooglePlaces();
        }, 2000);
    };
    script.onload = function() {
        console.log('Google Maps API script loaded successfully');
    };
    document.head.appendChild(script);
    
    // Fallback: try to initialize after a delay even if callback doesn't work
    setTimeout(() => {
        if (typeof google !== 'undefined' && google.maps && google.maps.places) {
            console.log('Fallback: Google Maps API detected, initializing...');
            initializeAutocompleteFunction();
        }
    }, 3000);
}

// Global callback function for Google Maps API
window.initializeAutocomplete = function() {
    console.log('Google Maps API callback triggered');
    // Call the actual initialization function, not itself
    initializeAutocompleteFunction();
};

// Manual initialization function for debugging
window.manualInitGooglePlaces = function() {
    console.log('Manual initialization triggered');
    initializeGooglePlaces();
};

// Test function to check if everything is working
window.testGooglePlaces = function() {
    console.log('=== Google Places Test ===');
    console.log('Google object:', typeof google);
    console.log('Google Maps:', typeof google?.maps);
    console.log('Google Places:', typeof google?.maps?.places);
    
    const pickupInput = document.querySelector('input[name="PickupLocation"]');
    const dropInput = document.querySelector('input[name="DropLocation"]');
    
    console.log('Pickup input:', pickupInput);
    console.log('Drop input:', dropInput);
    
    if (typeof google !== 'undefined' && google.maps && google.maps.places) {
        console.log('✅ Google Maps API is loaded');
        if (pickupInput && dropInput) {
            console.log('✅ Input elements found');
            console.log('Ready to initialize autocomplete');
        } else {
            console.log('❌ Input elements not found');
        }
    } else {
        console.log('❌ Google Maps API not loaded');
    }
};

function initializeAutocompleteFunction() {
    console.log('Initializing autocomplete functionality...');
    
    // Sri Lanka bounds for restricting results
    const sriLankaBounds = new google.maps.LatLngBounds(
        new google.maps.LatLng(5.9167, 79.6500), // Southwest corner
        new google.maps.LatLng(9.8333, 81.8833)  // Northeast corner
    );

    // Sri Lankan cities and major areas
    const sriLankanCities = [
        'Colombo', 'Kandy', 'Galle', 'Jaffna', 'Negombo', 'Kurunegala', 'Anuradhapura',
        'Polonnaruwa', 'Trincomalee', 'Batticaloa', 'Ampara', 'Ratnapura', 'Kegalle',
        'Matale', 'Nuwara Eliya', 'Badulla', 'Monaragala', 'Hambantota', 'Matara',
        'Kalutara', 'Gampaha', 'Puttalam', 'Mannar', 'Vavuniya', 'Mullaitivu',
        'Kilinochchi', 'Dehiwala-Mount Lavinia', 'Moratuwa', 'Sri Jayawardenepura Kotte'
    ];

    // Configuration for autocomplete
    const autocompleteOptions = {
        bounds: sriLankaBounds,
        strictBounds: false, // Changed to false for better results
        types: ['geocode', 'establishment'],
        componentRestrictions: { country: 'lk' }, // Restrict to Sri Lanka
        fields: ['place_id', 'name', 'formatted_address', 'geometry', 'address_components']
    };

    console.log('Autocomplete options:', autocompleteOptions);

    // Initialize pickup location autocomplete - try multiple selectors
    let pickupInput = document.querySelector('input[name="PickupLocation"]') || 
                     document.getElementById('pickupLocation') ||
                     document.querySelector('#pickupLocation');
    console.log('Pickup input found:', pickupInput);
    
    if (pickupInput) {
        try {
            const pickupAutocomplete = new google.maps.places.Autocomplete(pickupInput, autocompleteOptions);
            console.log('Pickup autocomplete initialized successfully');
            
            pickupAutocomplete.addListener('place_changed', function() {
                const place = pickupAutocomplete.getPlace();
                console.log('Pickup place selected:', place);
                handlePlaceSelection(place, pickupInput, 'pickup');
            });

            // Add custom styling for autocomplete dropdown
            styleAutocompleteDropdown(pickupInput);
        } catch (error) {
            console.error('Error initializing pickup autocomplete:', error);
        }
    } else {
        console.error('Pickup input not found! Trying alternative selectors...');
        // Try alternative selectors
        const alternatives = [
            'input[name="PickupLocation"]',
            '#pickupLocation',
            'input[placeholder*="pickup"]',
            'input[placeholder*="Pickup"]'
        ];
        
        for (let selector of alternatives) {
            const altInput = document.querySelector(selector);
            if (altInput) {
                console.log(`Found pickup input with selector: ${selector}`, altInput);
                pickupInput = altInput;
                break;
            }
        }
    }

    // Initialize drop location autocomplete - try multiple selectors
    let dropInput = document.querySelector('input[name="DropLocation"]') || 
                   document.getElementById('dropLocation') ||
                   document.querySelector('#dropLocation');
    console.log('Drop input found:', dropInput);
    
    if (dropInput) {
        try {
            const dropAutocomplete = new google.maps.places.Autocomplete(dropInput, autocompleteOptions);
            console.log('Drop autocomplete initialized successfully');
            
            dropAutocomplete.addListener('place_changed', function() {
                const place = dropAutocomplete.getPlace();
                console.log('Drop place selected:', place);
                handlePlaceSelection(place, dropInput, 'drop');
            });

            // Add custom styling for autocomplete dropdown
            styleAutocompleteDropdown(dropInput);
        } catch (error) {
            console.error('Error initializing drop autocomplete:', error);
        }
    } else {
        console.error('Drop input not found! Trying alternative selectors...');
        // Try alternative selectors
        const alternatives = [
            'input[name="DropLocation"]',
            '#dropLocation',
            'input[placeholder*="drop"]',
            'input[placeholder*="Drop"]'
        ];
        
        for (let selector of alternatives) {
            const altInput = document.querySelector(selector);
            if (altInput) {
                console.log(`Found drop input with selector: ${selector}`, altInput);
                dropInput = altInput;
                break;
            }
        }
    }

    // Add Sri Lankan city suggestions
    addSriLankanCitySuggestions();
}

function handlePlaceSelection(place, input, type) {
    if (!place.geometry || !place.geometry.location) {
        console.warn('No details available for input: ' + place.name);
        return;
    }

    // Validate if the place is in Sri Lanka
    if (!isPlaceInSriLanka(place)) {
        showLocationError(input, 'Please select a location in Sri Lanka');
        return;
    }

    // Set the formatted address
    input.value = place.formatted_address;
    
    // Store additional place information for later use
    input.setAttribute('data-place-id', place.place_id);
    input.setAttribute('data-lat', place.geometry.location.lat());
    input.setAttribute('data-lng', place.geometry.location.lng());
    
    // Add success styling
    input.classList.add('location-selected');
    
    // Remove any existing error styling
    input.classList.remove('location-error');
    
    console.log(`${type} location selected:`, place.formatted_address);
}

function isPlaceInSriLanka(place) {
    // Check if any address component indicates Sri Lanka
    if (place.address_components) {
        for (let component of place.address_components) {
            if (component.types.includes('country') && component.short_name === 'LK') {
                return true;
            }
        }
    }
    return false;
}

function showLocationError(input, message) {
    input.classList.add('location-error');
    input.classList.remove('location-selected');
    
    // Create or update error message
    let errorElement = input.parentElement.querySelector('.location-error-message');
    if (!errorElement) {
        errorElement = document.createElement('div');
        errorElement.className = 'location-error-message';
        input.parentElement.appendChild(errorElement);
    }
    errorElement.textContent = message;
    
    // Remove error after 3 seconds
    setTimeout(() => {
        input.classList.remove('location-error');
        if (errorElement) {
            errorElement.remove();
        }
    }, 3000);
}

function styleAutocompleteDropdown(input) {
    // Style the autocomplete dropdown to match our 3D theme
    const observer = new MutationObserver(function(mutations) {
        mutations.forEach(function(mutation) {
            if (mutation.type === 'childList') {
                const dropdowns = document.querySelectorAll('.pac-container');
                dropdowns.forEach(function(dropdown) {
                    if (!dropdown.classList.contains('styled')) {
                        dropdown.classList.add('styled');
                        dropdown.style.cssText = `
                            background: var(--glass-bg-dark);
                            border: 1px solid var(--glass-border-light);
                            border-radius: var(--radius-lg);
                            backdrop-filter: var(--glass-backdrop-strong);
                            box-shadow: var(--shadow-3d-strong);
                            margin-top: 0.5rem;
                            z-index: 1000;
                        `;
                        
                        // Style individual suggestions
                        const suggestions = dropdown.querySelectorAll('.pac-item');
                        suggestions.forEach(function(suggestion) {
                            suggestion.style.cssText = `
                                color: var(--text-primary);
                                padding: 0.75rem 1rem;
                                border-bottom: 1px solid var(--glass-border);
                                transition: all 0.3s ease;
                                cursor: pointer;
                            `;
                            
                            suggestion.addEventListener('mouseenter', function() {
                                this.style.background = 'var(--glass-bg-light)';
                                this.style.color = 'var(--primary-blue-light)';
                            });
                            
                            suggestion.addEventListener('mouseleave', function() {
                                this.style.background = 'transparent';
                                this.style.color = 'var(--text-primary)';
                            });
                        });
                    }
                });
            }
        });
    });
    
    observer.observe(document.body, { childList: true, subtree: true });
}

function addSriLankanCitySuggestions() {
    const sriLankanCities = [
        'Colombo', 'Kandy', 'Galle', 'Jaffna', 'Negombo', 'Kurunegala', 'Anuradhapura',
        'Polonnaruwa', 'Trincomalee', 'Batticaloa', 'Ampara', 'Ratnapura', 'Kegalle',
        'Matale', 'Nuwara Eliya', 'Badulla', 'Monaragala', 'Hambantota', 'Matara',
        'Kalutara', 'Gampaha', 'Puttalam', 'Mannar', 'Vavuniya', 'Mullaitivu',
        'Kilinochchi', 'Dehiwala-Mount Lavinia', 'Moratuwa', 'Sri Jayawardenepura Kotte'
    ];

    // Add data attributes for common Sri Lankan cities
    const inputs = document.querySelectorAll('input[name="PickupLocation"], input[name="DropLocation"]');
    inputs.forEach(input => {
        input.setAttribute('data-sri-lankan-cities', sriLankanCities.join(','));
    });
}

// Handle form submission with location validation
document.addEventListener('DOMContentLoaded', function() {
    const searchForm = document.querySelector('form[asp-controller="Cars"]');
    if (searchForm) {
        searchForm.addEventListener('submit', function(e) {
            const pickupInput = document.querySelector('input[name="PickupLocation"]');
            const dropInput = document.querySelector('input[name="DropLocation"]');
            
            // Validate locations
            if (!validateLocationInput(pickupInput, 'Pickup location')) {
                e.preventDefault();
                return false;
            }
            
            if (!validateLocationInput(dropInput, 'Drop location')) {
                e.preventDefault();
                return false;
            }
            
            // Check if both locations are the same
            if (pickupInput.value === dropInput.value) {
                e.preventDefault();
                showLocationError(pickupInput, 'Pickup and drop locations cannot be the same');
                showLocationError(dropInput, 'Pickup and drop locations cannot be the same');
                return false;
            }
        });
    }
});

function validateLocationInput(input, fieldName) {
    if (!input.value.trim()) {
        showLocationError(input, `${fieldName} is required`);
        return false;
    }
    
    // Check if it's a valid Google Places selection
    if (!input.getAttribute('data-place-id')) {
        showLocationError(input, `Please select a valid ${fieldName.toLowerCase()} from the suggestions`);
        return false;
    }
    
    return true;
}

// Add keyboard navigation support
document.addEventListener('keydown', function(e) {
    if (e.target.matches('input[name="PickupLocation"], input[name="DropLocation"]')) {
        const dropdown = document.querySelector('.pac-container');
        if (dropdown && dropdown.style.display !== 'none') {
            const suggestions = dropdown.querySelectorAll('.pac-item');
            const activeSuggestion = dropdown.querySelector('.pac-item-selected');
            
            if (e.key === 'ArrowDown') {
                e.preventDefault();
                if (activeSuggestion) {
                    activeSuggestion.classList.remove('pac-item-selected');
                    const next = activeSuggestion.nextElementSibling;
                    if (next) {
                        next.classList.add('pac-item-selected');
                    } else {
                        suggestions[0].classList.add('pac-item-selected');
                    }
                } else {
                    suggestions[0].classList.add('pac-item-selected');
                }
            } else if (e.key === 'ArrowUp') {
                e.preventDefault();
                if (activeSuggestion) {
                    activeSuggestion.classList.remove('pac-item-selected');
                    const prev = activeSuggestion.previousElementSibling;
                    if (prev) {
                        prev.classList.add('pac-item-selected');
                    } else {
                        suggestions[suggestions.length - 1].classList.add('pac-item-selected');
                    }
                }
            } else if (e.key === 'Enter') {
                e.preventDefault();
                if (activeSuggestion) {
                    activeSuggestion.click();
                }
            }
        }
    }
});

// Route Simulation JavaScript
let map;
let directionsService;
let directionsRenderer;
let carMarker;
let animationPath = [];
let currentStep = 0;
let animationInterval;
let isAnimating = false;

// Google Maps callback function
function initUserMaps() {
    // This function is called when Google Maps API loads
    console.log('Google Maps API loaded for user dashboard');
    // The actual map initialization happens when the modal is shown
}

// Custom car icon
const carIcon = {
    url: 'data:image/svg+xml;charset=UTF-8,' + encodeURIComponent(`
        <svg width="32" height="32" viewBox="0 0 32 32" xmlns="http://www.w3.org/2000/svg">
            <path d="M8 12h16v8H8z" fill="#4285F4" stroke="#fff" stroke-width="2"/>
            <circle cx="10" cy="22" r="3" fill="#333"/>
            <circle cx="22" cy="22" r="3" fill="#333"/>
            <path d="M6 12l2-6h16l2 6" fill="#EA4335"/>
        </svg>
    `),
    scaledSize: new google.maps.Size(32, 32),
    anchor: new google.maps.Point(16, 16)
};

// Initialize the map when modal is shown
document.addEventListener('DOMContentLoaded', function() {
    const routeModal = document.getElementById('routeSimulationModal');
    
    if (routeModal) {
        routeModal.addEventListener('shown.bs.modal', function() {
            initializeMap();
        });
        
        routeModal.addEventListener('hidden.bs.modal', function() {
            resetAnimation();
        });
    }
    
    // Event listeners for animation controls
    const startBtn = document.getElementById('startAnimation');
    const resetBtn = document.getElementById('resetAnimation');
    
    if (startBtn) {
        startBtn.addEventListener('click', startCarAnimation);
    }
    
    if (resetBtn) {
        resetBtn.addEventListener('click', resetAnimation);
    }
});

        function initializeMap() {
            if (!window.bookingCoordinates) {
                console.error('Booking coordinates not found');
                return;
            }
            
            if (typeof google === 'undefined') {
                console.error('Google Maps API not loaded');
                return;
            }
    
    const { pickupLat, pickupLng, dropLat, dropLng } = window.bookingCoordinates;
    
    if (!pickupLat || !pickupLng || !dropLat || !dropLng) {
        console.error('Invalid coordinates');
        return;
    }
    
    // Initialize map
    const mapElement = document.getElementById('routeMap');
    if (!mapElement) return;
    
    map = new google.maps.Map(mapElement, {
        zoom: 12,
        center: { lat: pickupLat, lng: pickupLng },
        mapTypeId: google.maps.MapTypeId.ROADMAP
    });
    
    // Initialize directions service and renderer
    directionsService = new google.maps.DirectionsService();
    directionsRenderer = new google.maps.DirectionsRenderer({
        draggable: false,
        suppressMarkers: true // We'll add custom markers
    });
    
    directionsRenderer.setMap(map);
    
    // Get directions and draw route
    getDirections(pickupLat, pickupLng, dropLat, dropLng);
}

function getDirections(pickupLat, pickupLng, dropLat, dropLng) {
    const request = {
        origin: { lat: pickupLat, lng: pickupLng },
        destination: { lat: dropLat, lng: dropLng },
        travelMode: google.maps.TravelMode.DRIVING,
        unitSystem: google.maps.UnitSystem.METRIC
    };
    
    directionsService.route(request, function(result, status) {
        if (status === 'OK') {
            directionsRenderer.setDirections(result);
            
            // Extract path for animation
            const route = result.routes[0];
            const path = route.overview_path;
            animationPath = path;
            
            // Add custom markers
            addCustomMarkers(pickupLat, pickupLng, dropLat, dropLng);
            
            // Add car marker at pickup location
            addCarMarker(pickupLat, pickupLng);
            
            // Fit map to show entire route
            const bounds = new google.maps.LatLngBounds();
            path.forEach(point => bounds.extend(point));
            map.fitBounds(bounds);
            
        } else {
            console.error('Directions request failed: ' + status);
            // Fallback: show markers without route
            addCustomMarkers(pickupLat, pickupLng, dropLat, dropLng);
            addCarMarker(pickupLat, pickupLng);
        }
    });
}

function addCustomMarkers(pickupLat, pickupLng, dropLat, dropLng) {
    // Pickup marker
    new google.maps.Marker({
        position: { lat: pickupLat, lng: pickupLng },
        map: map,
        title: 'Pickup Location',
        icon: {
            url: 'data:image/svg+xml;charset=UTF-8,' + encodeURIComponent(`
                <svg width="24" height="24" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                    <circle cx="12" cy="12" r="10" fill="#34A853" stroke="#fff" stroke-width="2"/>
                    <path d="M12 6v6l4 2" stroke="#fff" stroke-width="2" fill="none"/>
                </svg>
            `),
            scaledSize: new google.maps.Size(24, 24),
            anchor: new google.maps.Point(12, 12)
        }
    });
    
    // Drop marker
    new google.maps.Marker({
        position: { lat: dropLat, lng: dropLng },
        map: map,
        title: 'Drop Location',
        icon: {
            url: 'data:image/svg+xml;charset=UTF-8,' + encodeURIComponent(`
                <svg width="24" height="24" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                    <circle cx="12" cy="12" r="10" fill="#EA4335" stroke="#fff" stroke-width="2"/>
                    <path d="M9 12l2 2 4-4" stroke="#fff" stroke-width="2" fill="none"/>
                </svg>
            `),
            scaledSize: new google.maps.Size(24, 24),
            anchor: new google.maps.Point(12, 12)
        }
    });
}

function addCarMarker(lat, lng) {
    carMarker = new google.maps.Marker({
        position: { lat: lat, lng: lng },
        map: map,
        title: 'Car',
        icon: carIcon
    });
}

function startCarAnimation() {
    if (isAnimating || animationPath.length === 0) return;
    
    isAnimating = true;
    currentStep = 0;
    
    const startBtn = document.getElementById('startAnimation');
    if (startBtn) {
        startBtn.disabled = true;
        startBtn.innerHTML = '<i class="bi bi-pause-fill me-2"></i>Animating...';
    }
    
    // Calculate animation duration (15 seconds total)
    const totalDuration = 15000; // 15 seconds
    const stepDuration = totalDuration / animationPath.length;
    
    animationInterval = setInterval(() => {
        if (currentStep < animationPath.length) {
            const position = animationPath[currentStep];
            carMarker.setPosition(position);
            currentStep++;
        } else {
            // Animation complete
            clearInterval(animationInterval);
            isAnimating = false;
            
            const startBtn = document.getElementById('startAnimation');
            if (startBtn) {
                startBtn.disabled = false;
                startBtn.innerHTML = '<i class="bi bi-play-fill me-2"></i>Start Animation';
            }
        }
    }, stepDuration);
}

function resetAnimation() {
    if (animationInterval) {
        clearInterval(animationInterval);
        animationInterval = null;
    }
    
    isAnimating = false;
    currentStep = 0;
    
    const startBtn = document.getElementById('startAnimation');
    if (startBtn) {
        startBtn.disabled = false;
        startBtn.innerHTML = '<i class="bi bi-play-fill me-2"></i>Start Animation';
    }
    
    // Reset car marker to pickup location
    if (carMarker && window.bookingCoordinates) {
        const { pickupLat, pickupLng } = window.bookingCoordinates;
        carMarker.setPosition({ lat: pickupLat, lng: pickupLng });
    }
}

// Dashboard Bookings JavaScript

function cancelBooking(bookingId) {
    if (confirm('Are you sure you want to cancel this booking?')) {
        // Implement cancel booking functionality
        console.log('Cancelling booking:', bookingId);
        // Add actual implementation here
        fetch(`/Dashboard/CancelBooking/${bookingId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
            }
        })
        .then(response => {
            if (response.ok) {
                location.reload();
            } else {
                alert('Failed to cancel booking. Please try again.');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert('An error occurred. Please try again.');
        });
    }
}

function downloadInvoice(bookingId) {
    // Implement download invoice functionality
    console.log('Downloading invoice for booking:', bookingId);
    window.open(`/Dashboard/DownloadInvoice/${bookingId}`, '_blank');
}

function rateBooking(bookingId) {
    // Implement rating functionality
    console.log('Rating booking:', bookingId);
    // This could open a modal or redirect to a rating page
    // For now, show a simple prompt
    const rating = prompt('Please rate this booking (1-5 stars):');
    if (rating && rating >= 1 && rating <= 5) {
        // Send rating to server
        fetch(`/Dashboard/RateBooking/${bookingId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
            },
            body: JSON.stringify({ rating: parseInt(rating) })
        })
        .then(response => {
            if (response.ok) {
                alert('Thank you for your rating!');
                location.reload();
            } else {
                alert('Failed to submit rating. Please try again.');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert('An error occurred. Please try again.');
        });
    }
}
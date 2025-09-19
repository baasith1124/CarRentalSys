// Dashboard Booking Details JavaScript

function downloadInvoice(bookingId) {
    // Implement invoice download functionality
    window.open(`/Invoice/Download/${bookingId}`, '_blank');
}

function cancelBooking(bookingId) {
    if (confirm('Are you sure you want to cancel this booking? This action cannot be undone.')) {
        // Implement booking cancellation
        fetch(`/Bookings/Cancel/${bookingId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
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
            alert('An error occurred. Please try again.');
        });
    }
}

function modifyBooking(bookingId) {
    // Implement booking modification
    window.location.href = `/Bookings/Modify/${bookingId}`;
}
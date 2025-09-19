// Payment Checkout JavaScript

// Global payment variables
let stripe;
let elements;
let cardNumberElement;
let cardExpiryElement;
let cardCvcElement;

// Initialize payment system
function initializePayment(stripePublishableKey) {
    // Initialize Stripe
    stripe = Stripe(stripePublishableKey);
    elements = stripe.elements();

    // Create card elements
    cardNumberElement = elements.create('cardNumber');
    cardExpiryElement = elements.create('cardExpiry');
    cardCvcElement = elements.create('cardCvc');

    // Mount elements
    cardNumberElement.mount('#card-number-element');
    cardExpiryElement.mount('#card-expiry-element');
    cardCvcElement.mount('#card-cvc-element');

    // Setup form handler
    setupFormSubmission();

    // Setup real-time validation
    setupValidation();
}

// Setup form submission handler
function setupFormSubmission() {
    const form = document.getElementById('payment-form');
    if (!form) return;

    form.addEventListener('submit', async (event) => {
        event.preventDefault();

        const submitButton = document.getElementById('submit-payment');
        const buttonText = document.getElementById('button-text');
        const spinner = document.getElementById('spinner');
        const cardErrors = document.getElementById('card-errors');

        // Show loading state
        submitButton.disabled = true;
        buttonText.classList.add('d-none');
        spinner.classList.remove('d-none');
        cardErrors.classList.add('d-none');

        try {
            // Get booking and amount from the global data object
            const bookingId = window.paymentCheckoutData?.bookingId;
            const amount = window.paymentCheckoutData?.amount;

            // Create payment intent
            const response = await fetch('/Payment/CreatePaymentIntent', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    bookingId: bookingId,
                    amount: amount
                })
            });

            const { clientSecret } = await response.json();

            // Confirm payment
            const { error, paymentIntent } = await stripe.confirmCardPayment(clientSecret, {
                payment_method: {
                    card: cardNumberElement,
                    billing_details: {
                        name: document.getElementById('cardholder-name').value
                    }
                }
            });

            if (error) {
                // Show error
                cardErrors.textContent = error.message;
                cardErrors.classList.remove('d-none');
            } else if (paymentIntent.status === 'succeeded') {
                // Confirm payment on server
                const confirmResponse = await fetch('/Payment/ConfirmPayment', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({
                        paymentIntentId: paymentIntent.id,
                        bookingId: bookingId,
                        amount: amount
                    })
                });

                const result = await confirmResponse.json();

                if (result.success) {
                    // Show success popup with booking ID
                    showPaymentSuccessPopup(result.bookingId, result.paymentId);
                } else {
                    throw new Error(result.error || 'Payment confirmation failed');
                }
            }
        } catch (error) {
            cardErrors.textContent = error.message;
            cardErrors.classList.remove('d-none');
        } finally {
            // Reset button state
            submitButton.disabled = false;
            buttonText.classList.remove('d-none');
            spinner.classList.add('d-none');
        }
    });
}

// Setup real-time validation
function setupValidation() {
    if (!cardNumberElement) return;

    // Handle real-time validation errors
    cardNumberElement.on('change', ({ error }) => {
        const displayError = document.getElementById('card-errors');
        if (error) {
            displayError.textContent = error.message;
            displayError.classList.remove('d-none');
        } else {
            displayError.classList.add('d-none');
        }
    });
}

// Payment success popup function
function showPaymentSuccessPopup(bookingId, paymentId) {
    // Populate the modal with booking details
    const successBookingId = document.getElementById('successBookingId');
    const successPaymentId = document.getElementById('successPaymentId');
    const successDate = document.getElementById('successDate');

    if (successBookingId) successBookingId.textContent = bookingId;
    if (successPaymentId) successPaymentId.textContent = paymentId;
    if (successDate) {
        successDate.textContent = new Date().toLocaleDateString('en-US', {
            year: 'numeric',
            month: 'short',
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    }

    // Populate car and booking details from global data
    if (window.paymentCheckoutData) {
        const successCarName = document.getElementById('successCarName');
        const successPickupDate = document.getElementById('successPickupDate');
        const successReturnDate = document.getElementById('successReturnDate');
        const successDuration = document.getElementById('successDuration');
        const successDailyRate = document.getElementById('successDailyRate');
        const successTotalAmount = document.getElementById('successTotalAmount');

        if (successCarName) successCarName.textContent = window.paymentCheckoutData.carName;
        if (successPickupDate) successPickupDate.textContent = window.paymentCheckoutData.pickupDate;
        if (successReturnDate) successReturnDate.textContent = window.paymentCheckoutData.returnDate;
        if (successDuration) successDuration.textContent = window.paymentCheckoutData.days + ' day(s)';
        if (successDailyRate) successDailyRate.textContent = '$' + window.paymentCheckoutData.dailyRate;
        if (successTotalAmount) successTotalAmount.textContent = '$' + window.paymentCheckoutData.totalAmount;
    }

    // Show the modal with enhanced backdrop
    const modal = new bootstrap.Modal(document.getElementById('paymentSuccessModal'), {
        backdrop: 'static',
        keyboard: false
    });
    modal.show();

    // Add a subtle entrance animation
    setTimeout(() => {
        const modalElement = document.querySelector('.payment-success-modal');
        if (modalElement) {
            modalElement.style.transform = 'scale(1)';
            modalElement.style.opacity = '1';
        }
    }, 100);
}

// Navigation functions
function goToDashboard() {
    // Add loading state to button
    const btn = event.target;
    const originalHtml = btn.innerHTML;
    btn.innerHTML = '<i class="bi bi-arrow-clockwise spin"></i> Loading...';
    btn.disabled = true;

    setTimeout(() => {
        window.location.href = '/Dashboard/Bookings';
    }, 500);
}

function goToHome() {
    // Add loading state to button
    const btn = event.target;
    const originalHtml = btn.innerHTML;
    btn.innerHTML = '<i class="bi bi-arrow-clockwise spin"></i> Loading...';
    btn.disabled = true;

    setTimeout(() => {
        window.location.href = '/';
    }, 500);
}

function downloadReceipt() {
    // Add loading state to button
    const btn = event.target;
    const originalHtml = btn.innerHTML;
    btn.innerHTML = '<i class="bi bi-arrow-clockwise spin"></i> Generating...';
    btn.disabled = true;

    // Simulate receipt generation (implement actual receipt download later)
    setTimeout(() => {
        // For now, show a message that receipt will be emailed
        alert('Receipt will be sent to your email address shortly.');
        btn.innerHTML = originalHtml;
        btn.disabled = false;
    }, 2000);
}

// Initialize payment data from server-side model
function setPaymentData(data) {
    window.paymentData = data;
    
    // Set form data attributes
    const form = document.getElementById('payment-form');
    if (form && data) {
        form.dataset.bookingId = data.bookingId;
        form.dataset.amount = data.amount;
    }
}

// Auto-initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    // Initialize payment system if data is available
    if (window.paymentCheckoutData && window.paymentCheckoutData.stripePublishableKey) {
        initializePayment(window.paymentCheckoutData.stripePublishableKey);
    }
    console.log('Payment checkout JavaScript loaded');
});
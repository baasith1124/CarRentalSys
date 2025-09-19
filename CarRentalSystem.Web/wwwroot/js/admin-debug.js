// Admin Panel Debug Helper
document.addEventListener('DOMContentLoaded', function() {
    console.log('=== ADMIN PANEL DEBUG ===');
    
    // Check if admin panel link is present and clickable
    const adminLink = document.getElementById('admin-panel-link');
    if (adminLink) {
        console.log('✅ Admin panel link found:', adminLink);
        console.log('Admin link href:', adminLink.href);
        console.log('Admin link text:', adminLink.textContent.trim());
        console.log('Admin link clickable:', adminLink.style.pointerEvents !== 'none');
        
        // Force the link to work with multiple methods
        adminLink.addEventListener('click', function(e) {
            console.log('🎯 Admin panel link clicked!');
            console.log('Event:', e);
            console.log('Link href:', this.href);
            
            // Force navigation if normal click doesn't work
            setTimeout(() => {
                if (window.location.pathname !== '/Admin') {
                    console.log('🔄 Forcing navigation to admin panel...');
                    window.location.href = '/Admin';
                }
            }, 100);
        });
        
        // Add backup click handler
        adminLink.addEventListener('mousedown', function(e) {
            console.log('🖱️ Mouse down on admin link');
            // Force click if needed
            if (e.button === 0) { // Left click
                setTimeout(() => {
                    window.location.href = '/Admin';
                }, 50);
            }
        });
        
        // Ensure the link is always clickable
        adminLink.style.pointerEvents = 'auto';
        adminLink.style.cursor = 'pointer';
        adminLink.style.zIndex = '9999';
        
    } else {
        console.log('❌ Admin panel link not found');
        
        // Check if user is authenticated
        console.log('User authenticated:', document.body.classList.contains('authenticated'));
        console.log('User.Identity.IsAuthenticated would be:', '@User.Identity?.IsAuthenticated');
    }
    
    // Check for any blocking elements
    const dropdown = document.querySelector('.dropdown-menu');
    if (dropdown) {
        console.log('Dropdown menu found:', dropdown);
        console.log('Dropdown z-index:', window.getComputedStyle(dropdown).zIndex);
        console.log('Dropdown pointer-events:', window.getComputedStyle(dropdown).pointerEvents);
        
        // Ensure dropdown is clickable
        dropdown.style.pointerEvents = 'auto';
        dropdown.style.zIndex = '1050';
    }
    
    // Check for any CSS that might block clicks
    const allLinks = document.querySelectorAll('a');
    allLinks.forEach(link => {
        const style = window.getComputedStyle(link);
        if (style.pointerEvents === 'none') {
            console.warn('⚠️ Link with pointer-events: none found:', link);
            link.style.pointerEvents = 'auto';
        }
    });
    
    // Check if Bootstrap dropdown is working
    const dropdownToggle = document.querySelector('.dropdown-toggle');
    if (dropdownToggle) {
        dropdownToggle.addEventListener('click', function() {
            console.log('🔄 Dropdown toggle clicked');
        });
    }
    
    // Add global click handler for admin links
    document.addEventListener('click', function(e) {
        if (e.target.closest('#admin-panel-link')) {
            console.log('🎯 Admin panel link clicked via event delegation');
            e.preventDefault();
            window.location.href = '/Admin';
        }
    });
    
    console.log('=== END ADMIN PANEL DEBUG ===');
});

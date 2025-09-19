// Theme Toggle Functionality
document.addEventListener('DOMContentLoaded', function() {
    const themeToggle = document.getElementById('theme-toggle');
    const themeIcon = document.getElementById('theme-icon');
    const themeText = document.getElementById('theme-text');
    
    // Check for saved theme preference or default to light mode
    const currentTheme = localStorage.getItem('theme') || 'light';
    document.body.classList.toggle('dark-theme', currentTheme === 'dark');
    updateThemeUI(currentTheme);
    
    themeToggle.addEventListener('click', function() {
        const isDark = document.body.classList.contains('dark-theme');
        const newTheme = isDark ? 'light' : 'dark';
        
        document.body.classList.toggle('dark-theme');
        localStorage.setItem('theme', newTheme);
        updateThemeUI(newTheme);
        
        // Trigger 3D engine theme change if available
        if (window.carRental3D) {
            window.carRental3D.toggleTheme();
        }
        
        // Add click animation
        themeToggle.style.transform = 'scale(0.95)';
        setTimeout(() => {
            themeToggle.style.transform = 'scale(1)';
        }, 150);
    });
    
    function updateThemeUI(theme) {
        if (theme === 'dark') {
            themeIcon.className = 'bi bi-sun-fill';
            themeText.textContent = 'Light Mode';
        } else {
            themeIcon.className = 'bi bi-moon-fill';
            themeText.textContent = 'Dark Mode';
        }
    }
    
    // Smooth theme transition
    document.body.style.transition = 'background-color 0.3s ease, color 0.3s ease';
});

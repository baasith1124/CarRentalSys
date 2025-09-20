// Enhanced Admin Panel JavaScript - Smooth Animations and Interactivity

document.addEventListener('DOMContentLoaded', function() {
    initializeEnhancedAdmin();
});

function initializeEnhancedAdmin() {
    // Initialize all enhanced features
    initializeSmoothScrolling();
    initializeParallaxEffects();
    initializeInteractiveElements();
    initializeFormEnhancements();
    initializeTableEnhancements();
    initializeModalEnhancements();
    initializeNotificationSystem();
    initializeThemeSwitcher();
    initializePerformanceOptimizations();
    initializeMobileFeatures();
}

// Smooth Scrolling
function initializeSmoothScrolling() {
    // Add smooth scrolling to all anchor links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });
}

// Parallax Effects
function initializeParallaxEffects() {
    const parallaxElements = document.querySelectorAll('.parallax-layer');
    
    window.addEventListener('scroll', () => {
        const scrolled = window.pageYOffset;
        const rate = scrolled * -0.5;
        
        parallaxElements.forEach(element => {
            element.style.transform = `translateY(${rate}px)`;
        });
    });
}

// Interactive Elements
function initializeInteractiveElements() {
    // Add hover effects to cards
    const cards = document.querySelectorAll('.card-3d-interactive');
    cards.forEach(card => {
        card.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-10px) scale(1.02)';
            this.style.boxShadow = '0 25px 50px rgba(31, 38, 135, 0.6)';
        });
        
        card.addEventListener('mouseleave', function() {
            this.style.transform = 'translateY(0) scale(1)';
            this.style.boxShadow = '0 8px 32px 0 rgba(31, 38, 135, 0.37)';
        });
    });
    
    // Add ripple effect to buttons
    const buttons = document.querySelectorAll('.btn-3d-interactive');
    buttons.forEach(button => {
        button.addEventListener('click', createRippleEffect);
    });
}

// Ripple Effect
function createRippleEffect(e) {
    const button = e.currentTarget;
    const ripple = document.createElement('span');
    const rect = button.getBoundingClientRect();
    const size = Math.max(rect.width, rect.height);
    const x = e.clientX - rect.left - size / 2;
    const y = e.clientY - rect.top - size / 2;
    
    ripple.style.width = ripple.style.height = size + 'px';
    ripple.style.left = x + 'px';
    ripple.style.top = y + 'px';
    ripple.classList.add('ripple');
    
    button.appendChild(ripple);
    
    setTimeout(() => {
        ripple.remove();
    }, 600);
}

// Form Enhancements
function initializeFormEnhancements() {
    const formControls = document.querySelectorAll('.form-control');
    
    formControls.forEach(control => {
        // Add floating label effect
        control.addEventListener('focus', function() {
            this.parentElement.classList.add('focused');
        });
        
        control.addEventListener('blur', function() {
            if (!this.value) {
                this.parentElement.classList.remove('focused');
            }
        });
        
        // Add input validation feedback
        control.addEventListener('input', function() {
            validateInput(this);
        });
    });
}

// Input Validation
function validateInput(input) {
    const value = input.value.trim();
    const type = input.type;
    const required = input.hasAttribute('required');
    
    // Remove existing validation classes
    input.classList.remove('is-valid', 'is-invalid');
    
    if (required && !value) {
        input.classList.add('is-invalid');
        return false;
    }
    
    if (value) {
        let isValid = true;
        
        switch (type) {
            case 'email':
                isValid = /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value);
                break;
            case 'tel':
                isValid = /^[\+]?[1-9][\d]{0,15}$/.test(value);
                break;
            case 'url':
                isValid = /^https?:\/\/.+/.test(value);
                break;
        }
        
        if (isValid) {
            input.classList.add('is-valid');
        } else {
            input.classList.add('is-invalid');
        }
        
        return isValid;
    }
    
    return true;
}

// Table Enhancements
function initializeTableEnhancements() {
    const tables = document.querySelectorAll('.table');
    
    tables.forEach(table => {
        // Add row hover effects
        const rows = table.querySelectorAll('tbody tr');
        rows.forEach(row => {
            row.addEventListener('mouseenter', function() {
                this.style.transform = 'scale(1.01)';
                this.style.backgroundColor = 'rgba(255, 255, 255, 0.05)';
            });
            
            row.addEventListener('mouseleave', function() {
                this.style.transform = 'scale(1)';
                this.style.backgroundColor = 'transparent';
            });
        });
        
        // Add sorting functionality
        const headers = table.querySelectorAll('th[data-sort]');
        headers.forEach(header => {
            header.style.cursor = 'pointer';
            header.addEventListener('click', function() {
                sortTable(table, this.dataset.sort);
            });
        });
    });
}

// Table Sorting
function sortTable(table, column) {
    const tbody = table.querySelector('tbody');
    const rows = Array.from(tbody.querySelectorAll('tr'));
    const isAscending = table.dataset.sortDirection !== 'asc';
    
    rows.sort((a, b) => {
        const aVal = a.querySelector(`td:nth-child(${column})`).textContent.trim();
        const bVal = b.querySelector(`td:nth-child(${column})`).textContent.trim();
        
        if (isAscending) {
            return aVal.localeCompare(bVal);
        } else {
            return bVal.localeCompare(aVal);
        }
    });
    
    // Clear tbody and append sorted rows
    tbody.innerHTML = '';
    rows.forEach(row => tbody.appendChild(row));
    
    // Update sort direction
    table.dataset.sortDirection = isAscending ? 'asc' : 'desc';
    
    // Update header indicators
    const headers = table.querySelectorAll('th');
    headers.forEach(header => {
        header.classList.remove('sort-asc', 'sort-desc');
    });
    
    const currentHeader = table.querySelector(`th[data-sort="${column}"]`);
    currentHeader.classList.add(isAscending ? 'sort-asc' : 'sort-desc');
}

// Modal Enhancements
function initializeModalEnhancements() {
    const modals = document.querySelectorAll('.modal');
    
    modals.forEach(modal => {
        // Add entrance animation
        modal.addEventListener('show.bs.modal', function() {
            this.style.opacity = '0';
            this.style.transform = 'scale(0.8)';
            
            setTimeout(() => {
                this.style.opacity = '1';
                this.style.transform = 'scale(1)';
            }, 10);
        });
        
        // Add exit animation
        modal.addEventListener('hide.bs.modal', function() {
            this.style.opacity = '0';
            this.style.transform = 'scale(0.8)';
        });
    });
}

// Enhanced Notification System
function initializeNotificationSystem() {
    // Create notification container if it doesn't exist
    if (!document.querySelector('.notification-container')) {
        const container = document.createElement('div');
        container.className = 'notification-container position-fixed top-0 end-0 p-3';
        container.style.zIndex = '9999';
        document.body.appendChild(container);
    }
}

// Show Enhanced Notification
function showNotification(message, type = 'info', duration = 5000) {
    const container = document.querySelector('.notification-container');
    const notification = document.createElement('div');
    
    notification.className = `toast-3d toast-${type}`;
    notification.innerHTML = `
        <div class="toast-header-3d">
            <div class="toast-icon-3d">
                <i class="bi bi-${getNotificationIcon(type)}"></i>
            </div>
            <strong class="me-auto">${getNotificationTitle(type)}</strong>
            <button type="button" class="btn-close-3d" onclick="this.parentElement.parentElement.remove()">
                <i class="bi bi-x"></i>
            </button>
        </div>
        <div class="toast-body-3d">
            ${message}
        </div>
    `;
    
    container.appendChild(notification);
    
    // Add entrance animation
    setTimeout(() => {
        notification.style.transform = 'translateX(0)';
        notification.style.opacity = '1';
    }, 10);
    
    // Auto remove
    setTimeout(() => {
        if (notification.parentElement) {
            notification.style.transform = 'translateX(100%)';
            notification.style.opacity = '0';
            setTimeout(() => {
                notification.remove();
            }, 300);
        }
    }, duration);
}

// Get notification icon
function getNotificationIcon(type) {
    const icons = {
        success: 'check-circle-fill',
        error: 'exclamation-triangle-fill',
        warning: 'exclamation-circle-fill',
        info: 'info-circle-fill'
    };
    return icons[type] || 'info-circle-fill';
}

// Get notification title
function getNotificationTitle(type) {
    const titles = {
        success: 'Success',
        error: 'Error',
        warning: 'Warning',
        info: 'Information'
    };
    return titles[type] || 'Information';
}

// Theme Switcher
function initializeThemeSwitcher() {
    // Create theme switcher button
    const themeSwitcher = document.createElement('button');
    themeSwitcher.className = 'btn btn-outline-secondary theme-switcher';
    themeSwitcher.innerHTML = '<i class="bi bi-palette"></i>';
    themeSwitcher.title = 'Switch Theme';
    
    // Add to header actions
    const headerActions = document.querySelector('.header-actions');
    if (headerActions) {
        headerActions.insertBefore(themeSwitcher, headerActions.firstChild);
    }
    
    // Theme switching functionality
    themeSwitcher.addEventListener('click', function() {
        document.body.classList.toggle('light-theme');
        const isLight = document.body.classList.contains('light-theme');
        localStorage.setItem('admin-theme', isLight ? 'light' : 'dark');
    });
    
    // Load saved theme
    const savedTheme = localStorage.getItem('admin-theme');
    if (savedTheme === 'light') {
        document.body.classList.add('light-theme');
    }
}

// Performance Optimizations
function initializePerformanceOptimizations() {
    // Lazy load images
    const images = document.querySelectorAll('img[data-src]');
    const imageObserver = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const img = entry.target;
                img.src = img.dataset.src;
                img.classList.remove('lazy');
                observer.unobserve(img);
            }
        });
    });
    
    images.forEach(img => imageObserver.observe(img));
    
    // Debounce scroll events
    let scrollTimeout;
    window.addEventListener('scroll', function() {
        clearTimeout(scrollTimeout);
        scrollTimeout = setTimeout(handleScroll, 10);
    });
}

// Handle scroll events
function handleScroll() {
    const scrolled = window.pageYOffset;
    const parallaxElements = document.querySelectorAll('.parallax-layer');
    
    parallaxElements.forEach(element => {
        const rate = scrolled * -0.5;
        element.style.transform = `translateY(${rate}px)`;
    });
}

// Enhanced Search Functionality
function initializeSearch() {
    const searchInputs = document.querySelectorAll('.search-input');
    
    searchInputs.forEach(input => {
        let searchTimeout;
        
        input.addEventListener('input', function() {
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(() => {
                performSearch(this.value, this.dataset.target);
            }, 300);
        });
    });
}

// Perform search
function performSearch(query, target) {
    const targetElement = document.querySelector(target);
    if (!targetElement) return;
    
    const items = targetElement.querySelectorAll('.searchable-item');
    
    items.forEach(item => {
        const text = item.textContent.toLowerCase();
        const matches = text.includes(query.toLowerCase());
        
        item.style.display = matches ? '' : 'none';
        item.style.opacity = matches ? '1' : '0';
        item.style.transform = matches ? 'scale(1)' : 'scale(0.95)';
    });
}

// Enhanced Form Submission
function enhanceFormSubmission(form) {
    form.addEventListener('submit', function(e) {
        e.preventDefault();
        
        const submitBtn = this.querySelector('button[type="submit"]');
        const originalText = submitBtn.innerHTML;
        
        // Show loading state
        submitBtn.innerHTML = '<i class="bi bi-hourglass-split"></i> Processing...';
        submitBtn.disabled = true;
        
        // Simulate form processing
        setTimeout(() => {
            // Reset button
            submitBtn.innerHTML = originalText;
            submitBtn.disabled = false;
            
            // Show success message
            showNotification('Form submitted successfully!', 'success');
        }, 2000);
    });
}

// Initialize all forms (except logout forms)
document.querySelectorAll('form').forEach(form => {
    // Skip logout forms to allow normal submission
    if (!form.action.includes('Logout') && !form.querySelector('button[type="submit"]')?.textContent.includes('Logout')) {
        enhanceFormSubmission(form);
    }
});

// Enhanced Data Tables
function initializeDataTables() {
    const tables = document.querySelectorAll('.data-table');
    
    tables.forEach(table => {
        // Add pagination
        addPagination(table);
        
        // Add search
        addTableSearch(table);
        
        // Add column filters
        addColumnFilters(table);
    });
}

// Add pagination to table
function addPagination(table) {
    const tbody = table.querySelector('tbody');
    const rows = tbody.querySelectorAll('tr');
    const rowsPerPage = 10;
    const totalPages = Math.ceil(rows.length / rowsPerPage);
    
    if (totalPages <= 1) return;
    
    // Create pagination container
    const pagination = document.createElement('div');
    pagination.className = 'pagination-container d-flex justify-content-center mt-3';
    
    // Create pagination buttons
    for (let i = 1; i <= totalPages; i++) {
        const button = document.createElement('button');
        button.className = 'btn btn-outline-primary mx-1';
        button.textContent = i;
        button.addEventListener('click', () => showPage(table, i, rowsPerPage));
        pagination.appendChild(button);
    }
    
    table.parentElement.appendChild(pagination);
    
    // Show first page initially
    showPage(table, 1, rowsPerPage);
}

// Show specific page
function showPage(table, page, rowsPerPage) {
    const tbody = table.querySelector('tbody');
    const rows = tbody.querySelectorAll('tr');
    const startIndex = (page - 1) * rowsPerPage;
    const endIndex = startIndex + rowsPerPage;
    
    rows.forEach((row, index) => {
        if (index >= startIndex && index < endIndex) {
            row.style.display = '';
        } else {
            row.style.display = 'none';
        }
    });
    
    // Update active page button
    const buttons = table.parentElement.querySelectorAll('.pagination-container button');
    buttons.forEach(btn => btn.classList.remove('active'));
    buttons[page - 1].classList.add('active');
}

// Add table search
function addTableSearch(table) {
    const searchContainer = document.createElement('div');
    searchContainer.className = 'table-search mb-3';
    searchContainer.innerHTML = `
        <div class="input-group">
            <span class="input-group-text"><i class="bi bi-search"></i></span>
            <input type="text" class="form-control" placeholder="Search table...">
        </div>
    `;
    
    table.parentElement.insertBefore(searchContainer, table);
    
    const searchInput = searchContainer.querySelector('input');
    searchInput.addEventListener('input', function() {
        const query = this.value.toLowerCase();
        const rows = table.querySelectorAll('tbody tr');
        
        rows.forEach(row => {
            const text = row.textContent.toLowerCase();
            row.style.display = text.includes(query) ? '' : 'none';
        });
    });
}

// Add column filters
function addColumnFilters(table) {
    const headers = table.querySelectorAll('th[data-filter]');
    
    headers.forEach(header => {
        const filterContainer = document.createElement('div');
        filterContainer.className = 'column-filter';
        filterContainer.innerHTML = `
            <select class="form-select form-select-sm">
                <option value="">All</option>
            </select>
        `;
        
        const select = filterContainer.querySelector('select');
        const columnIndex = Array.from(header.parentElement.children).indexOf(header);
        const rows = table.querySelectorAll('tbody tr');
        
        // Get unique values for this column
        const values = new Set();
        rows.forEach(row => {
            const cell = row.children[columnIndex];
            if (cell) {
                values.add(cell.textContent.trim());
            }
        });
        
        // Add options
        values.forEach(value => {
            const option = document.createElement('option');
            option.value = value;
            option.textContent = value;
            select.appendChild(option);
        });
        
        // Add filter functionality
        select.addEventListener('change', function() {
            const filterValue = this.value;
            rows.forEach(row => {
                const cell = row.children[columnIndex];
                if (cell) {
                    const cellValue = cell.textContent.trim();
                    row.style.display = (!filterValue || cellValue === filterValue) ? '' : 'none';
                }
            });
        });
        
        header.appendChild(filterContainer);
    });
}

// Mobile Features
function initializeMobileFeatures() {
    // Mobile sidebar toggle
    initializeMobileSidebar();
    
    // Touch interactions
    initializeTouchInteractions();
    
    // Mobile viewport handling
    initializeMobileViewport();
    
    // Mobile gestures
    initializeMobileGestures();
}

// Mobile Sidebar Toggle
function initializeMobileSidebar() {
    const sidebar = document.querySelector('.admin-sidebar-3d');
    const toggleButton = document.querySelector('.sidebar-toggle-mobile');
    const overlay = document.createElement('div');
    
    // Create overlay
    overlay.className = 'sidebar-overlay';
    overlay.style.cssText = `
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background: rgba(0, 0, 0, 0.5);
        z-index: 1040;
        display: none;
        backdrop-filter: blur(5px);
        opacity: 0;
        transition: opacity 0.3s ease;
    `;
    document.body.appendChild(overlay);
    
    // Toggle sidebar
    function toggleSidebar() {
        const isOpen = sidebar.classList.contains('show');
        
        if (isOpen) {
            sidebar.classList.remove('show');
            overlay.style.display = 'none';
            overlay.style.opacity = '0';
            document.body.style.overflow = '';
            // Adjust echo admin position
            adjustEchoAdminPosition(false);
        } else {
            sidebar.classList.add('show');
            overlay.style.display = 'block';
            setTimeout(() => {
                overlay.style.opacity = '1';
            }, 10);
            document.body.style.overflow = 'hidden';
            // Adjust echo admin position
            adjustEchoAdminPosition(true);
        }
    }
    
    // Adjust echo admin chat position based on sidebar state
    function adjustEchoAdminPosition(sidebarOpen) {
        const echoAdmin = document.querySelector('.echo-admin-chat');
        if (echoAdmin) {
            if (sidebarOpen && window.innerWidth <= 768) {
                echoAdmin.style.right = '20px';
                echoAdmin.style.bottom = '20px';
            } else {
                echoAdmin.style.right = '';
                echoAdmin.style.bottom = '';
            }
        }
    }
    
    // Event listeners
    if (toggleButton) {
        toggleButton.addEventListener('click', function(e) {
            e.preventDefault();
            e.stopPropagation();
            toggleSidebar();
        });
    }
    
    // Close on overlay click
    overlay.addEventListener('click', toggleSidebar);
    
    // Close on escape key
    document.addEventListener('keydown', function(e) {
        if (e.key === 'Escape' && sidebar.classList.contains('show')) {
            toggleSidebar();
        }
    });
    
    // Close on window resize to desktop
    window.addEventListener('resize', function() {
        if (window.innerWidth > 768 && sidebar.classList.contains('show')) {
            toggleSidebar();
        }
        // Adjust echo admin on resize
        adjustEchoAdminPosition(sidebar.classList.contains('show'));
    });
    
    // Initialize echo admin position
    adjustEchoAdminPosition(false);
}

// Touch Interactions
function initializeTouchInteractions() {
    // Add touch-friendly classes
    if ('ontouchstart' in window) {
        document.body.classList.add('touch-device');
    }
    
    // Prevent zoom on double tap for buttons
    let lastTouchEnd = 0;
    document.addEventListener('touchend', function(event) {
        const now = (new Date()).getTime();
        if (now - lastTouchEnd <= 300) {
            event.preventDefault();
        }
        lastTouchEnd = now;
    }, false);
    
    // Add touch feedback
    document.querySelectorAll('.btn-3d-interactive, .nav-link-3d, .card-3d-interactive').forEach(element => {
        element.addEventListener('touchstart', function() {
            this.classList.add('touch-active');
        });
        
        element.addEventListener('touchend', function() {
            setTimeout(() => {
                this.classList.remove('touch-active');
            }, 150);
        });
    });
}

// Mobile Viewport Handling
function initializeMobileViewport() {
    // Set viewport meta tag for mobile
    let viewport = document.querySelector('meta[name="viewport"]');
    if (!viewport) {
        viewport = document.createElement('meta');
        viewport.name = 'viewport';
        document.head.appendChild(viewport);
    }
    viewport.content = 'width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no';
    
    // Handle orientation change
    window.addEventListener('orientationchange', function() {
        setTimeout(() => {
            // Recalculate layout after orientation change
            window.dispatchEvent(new Event('resize'));
        }, 100);
    });
}

// Mobile Gestures
function initializeMobileGestures() {
    let startX = 0;
    let startY = 0;
    let isDragging = false;
    
    // Swipe to close sidebar
    document.addEventListener('touchstart', function(e) {
        startX = e.touches[0].clientX;
        startY = e.touches[0].clientY;
        isDragging = false;
    });
    
    document.addEventListener('touchmove', function(e) {
        if (!isDragging) {
            const deltaX = Math.abs(e.touches[0].clientX - startX);
            const deltaY = Math.abs(e.touches[0].clientY - startY);
            
            if (deltaX > deltaY && deltaX > 10) {
                isDragging = true;
            }
        }
    });
    
    document.addEventListener('touchend', function(e) {
        if (isDragging) {
            const deltaX = e.changedTouches[0].clientX - startX;
            const sidebar = document.querySelector('.admin-sidebar-3d');
            
            // Swipe right to open sidebar (from left edge)
            if (startX < 50 && deltaX > 100 && !sidebar.classList.contains('show')) {
                sidebar.classList.add('show');
                document.querySelector('.sidebar-overlay').style.display = 'block';
                document.body.style.overflow = 'hidden';
            }
            // Swipe left to close sidebar
            else if (deltaX < -100 && sidebar.classList.contains('show')) {
                sidebar.classList.remove('show');
                document.querySelector('.sidebar-overlay').style.display = 'none';
                document.body.style.overflow = '';
            }
        }
    });
}

// Mobile-specific utility functions
function isMobile() {
    return window.innerWidth <= 768;
}

function isTouchDevice() {
    return 'ontouchstart' in window || navigator.maxTouchPoints > 0;
}

// Responsive table handling
function initializeResponsiveTables() {
    const tables = document.querySelectorAll('.table-responsive');
    
    tables.forEach(table => {
        const wrapper = table;
        const tableElement = table.querySelector('table');
        
        if (tableElement && isMobile()) {
            // Add horizontal scroll indicator
            const scrollIndicator = document.createElement('div');
            scrollIndicator.className = 'scroll-indicator';
            scrollIndicator.innerHTML = '<i class="bi bi-arrow-left-right"></i> Scroll horizontally';
            scrollIndicator.style.cssText = `
                text-align: center;
                padding: 0.5rem;
                background: var(--glass-bg);
                color: var(--text-muted);
                font-size: 0.875rem;
                border-radius: 0.5rem;
                margin-bottom: 1rem;
            `;
            
            wrapper.insertBefore(scrollIndicator, tableElement);
            
            // Hide indicator after user scrolls
            let hasScrolled = false;
            wrapper.addEventListener('scroll', function() {
                if (!hasScrolled && this.scrollLeft > 0) {
                    hasScrolled = true;
                    scrollIndicator.style.opacity = '0';
                    setTimeout(() => {
                        scrollIndicator.remove();
                    }, 300);
                }
            });
        }
    });
}

// Initialize responsive tables
document.addEventListener('DOMContentLoaded', function() {
    initializeResponsiveTables();
});

// Export functions for global use
window.showNotification = showNotification;
window.initializeSearch = initializeSearch;
window.initializeDataTables = initializeDataTables;
window.isMobile = isMobile;
window.isTouchDevice = isTouchDevice;

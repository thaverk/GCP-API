/**
 * Enhanced sidebar dropdown management
 * Handles both hover and click behavior for sidebar dropdowns
 */
function initSidebarDropdowns() {
    // Cache common selectors
    const sidebar = document.getElementById('sidebar');
    const dropdownToggles = document.querySelectorAll('.sidebar .dropdown-toggle');
    const isDesktop = window.innerWidth > 768;

    // Clear any existing event handlers
    dropdownToggles.forEach(toggle => {
        toggle.removeEventListener('click', handleDropdownClick);
    });

    // Set up event handlers based on device type
    if (isDesktop) {
        // Desktop behavior: hover to show/hide
        setupDesktopBehavior();
    } else {
        // Mobile behavior: click to toggle
        setupMobileBehavior();
    }

    // Handle window resize to switch between behaviors
    window.addEventListener('resize', handleWindowResize);

    /**
     * Sets up hover-based behavior for desktop
     */
    function setupDesktopBehavior() {
        const dropdownItems = document.querySelectorAll('.sidebar .nav-item.dropdown');

        dropdownItems.forEach(item => {
            // Show dropdown on mouseenter
            item.addEventListener('mouseenter', function () {
                // Position the dropdown correctly based on sidebar state
                const dropdownMenu = this.querySelector('.dropdown-menu');
                if (dropdownMenu) {
                    if (sidebar.matches(':hover')) {
                        dropdownMenu.style.left = '250px';
                    } else {
                        dropdownMenu.style.left = '60px';
                    }
                    dropdownMenu.classList.add('show');
                }
            });

            // Hide dropdown on mouseleave
            item.addEventListener('mouseleave', function () {
                const dropdownMenu = this.querySelector('.dropdown-menu');
                if (dropdownMenu) {
                    dropdownMenu.classList.remove('show');
                }
            });

            // Prevent navigation when clicking dropdown toggle on desktop
            const toggle = item.querySelector('.dropdown-toggle');
            if (toggle) {
                toggle.addEventListener('click', function (e) {
                    e.preventDefault();
                });
            }
        });
    }

    /**
     * Sets up click-based behavior for mobile
     */
    function setupMobileBehavior() {
        dropdownToggles.forEach(toggle => {
            toggle.addEventListener('click', handleDropdownClick);
        });
    }

    /**
     * Click handler for dropdown toggles
     */
    function handleDropdownClick(e) {
        e.preventDefault();

        // Close any open dropdowns first
        document.querySelectorAll('.sidebar .dropdown-menu.show')
            .forEach(menu => {
                if (menu !== this.nextElementSibling) {
                    menu.classList.remove('show');
                }
            });

        // Toggle this dropdown
        const dropdownMenu = this.nextElementSibling;
        if (dropdownMenu && dropdownMenu.classList.contains('dropdown-menu')) {
            dropdownMenu.classList.toggle('show');

            // Position correctly based on sidebar state
            if (sidebar.matches(':hover')) {
                dropdownMenu.style.left = '250px';
            } else {
                dropdownMenu.style.left = '60px';
            }
        }
    }

    /**
     * Handles window resize to switch between mobile and desktop behaviors
     */
    function handleWindowResize() {
        const wasDesktop = isDesktop;
        const isNowDesktop = window.innerWidth > 768;

        // Only reinitialize if we've crossed the desktop/mobile threshold
        if (wasDesktop !== isNowDesktop) {
            // Close any open dropdowns
            document.querySelectorAll('.sidebar .dropdown-menu.show')
                .forEach(menu => menu.classList.remove('show'));

            // Reinitialize with appropriate behavior
            if (isNowDesktop) {
                setupDesktopBehavior();
            } else {
                setupMobileBehavior();
            }
        }
    }
}

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function () {
    initSidebarDropdowns();
});

// Monitor sidebar hover state for dropdown positioning
document.addEventListener('DOMContentLoaded', function () {
    const sidebar = document.getElementById('sidebar');

    if (sidebar) {
        sidebar.addEventListener('mouseenter', function () {
            document.querySelectorAll('.sidebar .dropdown-menu.show').forEach(menu => {
                menu.style.left = '250px';
            });
        });

        sidebar.addEventListener('mouseleave', function () {
            document.querySelectorAll('.sidebar .dropdown-menu.show').forEach(menu => {
                menu.style.left = '60px';
            });
        });
    }
});

/**
 * Setup hover behavior for all dropdowns in the application
 * This applies to both sidebar and non-sidebar dropdowns
 */
function setupGlobalDropdownBehavior() {
    // Enable hover dropdowns for desktop, leave click behavior for mobile
    if (window.innerWidth > 768) {
        $('.dropdown').on('mouseenter', function () {
            $(this).find('.dropdown-menu').addClass('show');
        }).on('mouseleave', function () {
            $(this).find('.dropdown-menu').removeClass('show');
        });
    }

    // Update behavior on window resize
    $(window).resize(function () {
        if (window.innerWidth > 768) {
            $('.dropdown').off('click');
            $('.dropdown').on('mouseenter', function () {
                $(this).find('.dropdown-menu').addClass('show');
            }).on('mouseleave', function () {
                $(this).find('.dropdown-menu').removeClass('show');
            });
        } else {
            // Revert to click behavior on mobile
            $('.dropdown').off('mouseenter mouseleave');
        }
    });
}

// Initialize all dropdown behaviors when the document is ready
$(document).ready(function () {
    setupGlobalDropdownBehavior();
});
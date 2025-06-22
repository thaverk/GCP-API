/**
 * Functions for secondary sidebar management and positioning
 */

// Sidebar items configuration
const sidebarItems = {
    'dashboard': {
        title: 'Dashboard',
        items: [
            { title: 'Overview', icon: 'fas fa-chart-line', url: '/ProgressDashboard/ProgressDashboard' },
            { title: 'Reports', icon: 'fas fa-file-alt', url: '#' }
        ]
    },
    'templates': {
        title: 'Templates',
        items: [
            { title: 'Programs', icon: 'fas fa-edit', url: '/TemplateManagement/Templates' },
            { title: 'Schemas', icon: 'fas fa-sitemap', url: '/TemplateManagement/schemaTemplate' },
            { title: 'Exercises', icon: 'fas fa-dumbbell', url: '/TemplateManagement/exerciseTemplate' }
        ]
    },
    'team': {
        title: 'Team Management',
        items: [
            { title: 'Members', icon: 'fas fa-users', url: '/Teams/MembersList' },
            { title: 'Groups', icon: 'fas fa-user-friends', url: '#' }
        ]
    }
};

/**
 * Updates positions of UI elements based on sidebar state
 */
function updateElementPositions(isPrimaryExpanded) {
    const content = document.getElementById('content');
    if (!content) return;

    // Add any positioning logic here if needed
}

/**
 * Manages secondary sidebar display
 */
function manageSecondarySidebar(key) {
    // Save the active menu in localStorage
    localStorage.setItem('activeSecondaryMenu', key);
}

/**
 * Toggles sidebar visibility
 */
function toggleSidebarVisibility(visible) {
    // Store sidebar visibility state
    if (visible !== undefined) {
        localStorage.setItem('secondarySidebarVisible', visible);
    } else {
        const currentState = localStorage.getItem('secondarySidebarVisible') === 'true';
        localStorage.setItem('secondarySidebarVisible', !currentState);
    }
}

/**
 * Sets up sidebar toggle functionality and manages sidebar visibility logic
 */
function setupSidebarToggle() {
    // Check if sidebars should be shown on this page
    const isAthleteView = document.getElementById('sidebar')?.classList.contains('d-none') || false;
    const currentView = document.body.dataset.currentView || '';
    const isAthleteUser = document.body.dataset.isAthleteUser === 'True';
    const shouldShowSidebar = !isAthleteView && !(currentView === 'ProfilePage' && isAthleteUser);

    // Add this line to ensure the content element exists and is accessible
    const content = document.getElementById('content');

    // Only continue with sidebar setup if we should show it
    if (shouldShowSidebar) {
        // Set up toggle button for secondary sidebar
        const sidebarToggle = document.getElementById('sidebar-toggle');
        const primarySidebar = document.getElementById('sidebar');

        if (sidebarToggle) {
            sidebarToggle.addEventListener('click', function (e) {
                e.preventDefault();
                toggleSidebarVisibility();
            });
        }

        // Monitor primary sidebar hover state to update positions
        if (primarySidebar) {
            primarySidebar.addEventListener('mouseenter', function () {
                updateElementPositions(true);
            });

            primarySidebar.addEventListener('mouseleave', function () {
                updateElementPositions(false);
            });
        }

        // Monitor window resize to handle responsive behavior
        window.addEventListener('resize', function () {
            const isPrimaryExpanded = primarySidebar && primarySidebar.matches(':hover');
            updateElementPositions(isPrimaryExpanded);
        });

        // Add data attributes to the relevant nav links in the primary sidebar
        const dashboardLink = document.querySelector('.nav-link i.fas.fa-home')?.closest('.nav-link');
        if (dashboardLink) dashboardLink.setAttribute('data-sidebar-trigger', 'dashboard');

        const templatesLink = document.querySelector('.nav-link i.fas.fa-layer-group')?.closest('.nav-link');
        if (templatesLink) templatesLink.setAttribute('data-sidebar-trigger', 'templates');

        const teamLink = document.querySelector('.nav-link i.fas.fa-users')?.closest('.nav-link');
        if (teamLink) teamLink.setAttribute('data-sidebar-trigger', 'team');

        // Add click handlers to all items with the data-sidebar-trigger attribute
        document.querySelectorAll('[data-sidebar-trigger]').forEach(item => {
            item.addEventListener('click', function (e) {
                e.preventDefault();
                const itemKey = this.getAttribute('data-sidebar-trigger');
                manageSecondarySidebar(itemKey);

                // Navigate to the original link after a short delay
                setTimeout(() => {
                    if (!e.ctrlKey && !e.metaKey) {  // Don't redirect if Ctrl/Cmd was pressed
                        window.location.href = this.href;
                    }
                }, 300);
            });
        });

        // Auto-open secondary sidebar if we're on a page that should show it
        const currentController = document.body.dataset.currentController || '';

        // Check for specific controllers
        if (currentController === 'TemplateManagement') {
            manageSecondarySidebar('templates');
        } else if (currentController === 'ProgressDashboard') {
            manageSecondarySidebar('dashboard');
        } else if (currentController === 'Teams') {
            manageSecondarySidebar('team');
        } else {
            // For any other non-athlete view, try to use stored menu or default to dashboard
            const storedMenu = localStorage.getItem('activeSecondaryMenu');
            if (storedMenu && sidebarItems[storedMenu]) {
                manageSecondarySidebar(storedMenu);
            } else if (sidebarItems['dashboard']) {
                // Default to dashboard for Service Provider views
                manageSecondarySidebar('dashboard');
            }
        }

        // Restore sidebar visibility state from localStorage and update positions
        const sidebarVisible = localStorage.getItem('secondarySidebarVisible');
        if (sidebarVisible === 'false') {
            toggleSidebarVisibility(false);
        } else {
            // For non-athlete views, default to showing sidebar
            toggleSidebarVisibility(true);
        }
    }

    // Always update element positions regardless of view type
    const primarySidebar = document.getElementById('sidebar');
    const isPrimaryExpanded = primarySidebar && primarySidebar.matches(':hover');
    updateElementPositions(isPrimaryExpanded);
}

// Initialize sidebar functionality when the DOM is ready
document.addEventListener('DOMContentLoaded', function () {
    setupSidebarToggle();
});
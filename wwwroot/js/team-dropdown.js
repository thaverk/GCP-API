document.addEventListener('DOMContentLoaded', function () {
    // Load selected team
    loadSelectedTeam();

    // Setup sidebar toggle button
    setupSidebarToggle();
});

// Function to handle dropdown toggle
function toggleDropdown(elementId) {
    var dropdown = document.getElementById(elementId);
    if (dropdown) {
        dropdown.classList.toggle('show');
    }
}

// Initialize Bootstrap 4 dropdowns properly
$(document).ready(function () {
    console.log("team-dropdown.js loaded");

    // Enable Bootstrap dropdowns with proper event handling
    $('.dropdown-toggle').dropdown({
        toggle: true
    });

    // Set up team dropdown change handler
    $('#teamDropdown1').on('change', function () {
        var teamId = $(this).val();
        if (teamId && teamId != 0) {
            console.log("Team dropdown changed: " + teamId);
            saveAndLoadMembers(teamId);
        }
    });

    // Handle clicks outside to close dropdowns
    $(document).on('click', function (e) {
        if (!$(e.target).closest('.dropdown').length &&
            !$(e.target).hasClass('dropdown-toggle') &&
            !$(e.target).closest('.dropdown-toggle').length) {
            $('.dropdown-menu.show').removeClass('show');
        }
    });
});

// Function to save the selected team to local storage and load members
function saveAndLoadMembers(teamId) {
    localStorage.setItem('selectedTeam', teamId);
    loadMembers(teamId);
}

// Function to load the selected team from local storage
function loadSelectedTeam() {
    var selectedTeam = localStorage.getItem('selectedTeam');
    if (selectedTeam) {
        // Update dropdown element with matching ID
        var dropdown = document.getElementById('teamDropdown1');
        if (dropdown) {
            dropdown.value = selectedTeam;
            loadMembers(selectedTeam);
        }
    }
}

// Helper function to properly initialize DataTables
function initializeDataTable(tableSelector, options) {
    options = options || {};

    // First properly destroy any existing DataTable
    if ($.fn.DataTable.isDataTable(tableSelector)) {
        $(tableSelector).DataTable().destroy();
    }

    // Remove any extra markup that might have been left behind
    const wrapperSelector = tableSelector + '_wrapper';
    if ($(wrapperSelector).length) {
        const originalTable = $(tableSelector).detach();
        $(wrapperSelector).replaceWith(originalTable);
    }

    // Initialize a fresh DataTable instance with specified options
    return $(tableSelector).DataTable({
        destroy: true,
        retrieve: false,
        responsive: true,
        language: {
            search: "_INPUT_",
            searchPlaceholder: "Search...",
            info: "Showing _START_ to _END_ of _TOTAL_ entries",
            lengthMenu: "Show _MENU_ entries"
        },
        ...options
    });
}

// Modified part of your loadMembers function
function loadMembers(teamId) {
    console.log("Loading members for team: " + teamId);

    // Show loading indicators
    $('#Members-list, #members-list, #groups-table tbody').html(
        '<div class="text-center"><div class="spinner-border" role="status"><span class="sr-only">Loading...</span></div></div>'
    );

    // Progress dashboard call
    $.ajax({
        url: '/ProgressDashboard/ProgressDashboard',
        type: 'GET',
        data: { teamId: teamId },
        success: function (data) {
            console.log('Team ID pushed to dashboard');
        },
        error: function (xhr, status, error) {
            console.error('Error pushing team ID to dashboard:', error);
        }
    });

    // Get members for the list
    $.ajax({
        url: '/Teams/GetMembersByTeamId',
        type: 'POST',
        data: { teamId: teamId },
        success: function (data) {
            // Update the content
            $('#Members-list').html(data);

            // Add debugging to check the table structure
            console.log("Checking table structure...");
            var tableExists = $('#membersTable').length;
            console.log("Table with ID 'membersTable' exists:", tableExists);

            if (!tableExists) {
                console.log("Checking for Members-table instead...");
                console.log("Table with ID 'Members-table' exists:", $('#Members-table').length);
            }

            // Modified to check both potential table IDs
            setTimeout(function () {
                // Try to initialize the table with correct ID
                if ($('#membersTable').length) {
                    // Check column count consistency
                    var headerCols = $('#membersTable thead tr th').length;
                    var bodyCols = 0;
                    if ($('#membersTable tbody tr').length > 0) {
                        bodyCols = $('#membersTable tbody tr:first td').length;
                    }
                    console.log("Header columns:", headerCols, "Body columns:", bodyCols);

                    if (headerCols === bodyCols && headerCols > 0) {
                        initializeDataTable('#membersTable');
                        console.log("membersTable initialized successfully");
                    } else {
                        console.error("Column count mismatch in membersTable: headers=" + headerCols + ", body=" + bodyCols);
                    }
                } else if ($('#Members-table').length) {
                    // Check column count consistency
                    var headerCols = $('#Members-table thead tr th').length;
                    var bodyCols = 0;
                    if ($('#Members-table tbody tr').length > 0) {
                        bodyCols = $('#Members-table tbody tr:first td').length;
                    }
                    console.log("Header columns:", headerCols, "Body columns:", bodyCols);

                    if (headerCols === bodyCols && headerCols > 0) {
                        initializeDataTable('#Members-table');
                        console.log("Members-table initialized successfully");
                    } else {
                        console.error("Column count mismatch in Members-table: headers=" + headerCols + ", body=" + bodyCols);
                    }
                } else {
                    console.error("No members table found with ID 'membersTable' or 'Members-table'");
                }
            }, 100);
        },
        error: function (xhr, status, error) {
            $('#Members-list').html('<div class="alert alert-danger">Error loading members. Please try again.</div>');
            console.error('Error loading members list:', error);
        }
    });


    // Get team members for group creation
    $.ajax({
        url: '/Teams/GetTeamMembers',
        type: 'GET',
        data: { teamId: teamId },
        success: function (data) {
            var membersList = $('#members-list');
            membersList.empty();

            if (data && data.length > 0) {
                $.each(data, function (index, member) {
                    membersList.append(
                        '<li class="list-group-item">' +
                        '<div class="form-check">' +
                        '<input class="form-check-input" type="checkbox" id="member-' + member.id + '" name="memberIds[]" value="' + member.id + '">' +
                        '<label class="form-check-label" for="member-' + member.id + '">' +
                        escapeHtml(member.name) + ' ' + escapeHtml(member.surname) + '</label>' +
                        '</div>' +
                        '</li>'
                    );
                });
            } else {
                membersList.html('<li class="list-group-item">No members available</li>');
            }
            console.log("Group member checkboxes loaded");
        },
        error: function (xhr, status, error) {
            $('#members-list').html('<div class="alert alert-danger">Error loading team members. Please try again.</div>');
            console.error('Error loading team members:', error);
        }
    });

    // Get groups for the team
    $.ajax({
        url: '/Teams/GetGroupsByTeamId',
        type: 'GET',
        data: { teamId: teamId },
        success: function (data) {
            var groupsTableBody = $('#groups-table tbody');
            groupsTableBody.empty();

            if (data && data.length > 0) {
                $.each(data, function (index, group) {
                    groupsTableBody.append(
                        '<tr>' +
                        '<td>' + escapeHtml(group.name) + '</td>' +
                        '<td>' + group.memberCount + '</td>' +
                        '<td>' +
                        '<button class="btn" onclick="viewgroup(' + group.id + ')">' +
                        '<i class="fas fa-eye" aria-hidden="true"></i></button>' +
                        '<button class="btn" onclick="deleteGroup(' + group.id + ')">' +
                        '<i class="fas fa-trash" aria-hidden="true"></i></button>' +
                        '</td>' +
                        '</tr>'
                    );
                });
            } else {
                groupsTableBody.append('<tr><td colspan="3" class="text-center">No groups available</td></tr>');
            }

            // Initialize groups table if needed
            if ($('#groups-table').length && !$.fn.DataTable.isDataTable('#groups-table')) {
                initializeDataTable('#groups-table', {
                    order: [[0, 'asc']], // Sort by first column (group name)
                    columnDefs: [
                        { orderable: false, targets: 2 } // Disable sorting on actions column
                    ]
                });
            }
            console.log("Groups table loaded");
        },
        error: function (xhr, status, error) {
            $('#groups-table tbody').html('<tr><td colspan="3" class="text-center text-danger">Error loading groups</td></tr>');
            console.error('Error loading groups:', error);
        }
    });
}

function setupSidebarToggle() {
    var sidebarToggle = document.getElementById('sidebarToggle');
    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', function () {
            document.body.classList.toggle('sidebar-toggled');
            var sidebar = document.querySelector('.sidebar');
            if (sidebar) {
                sidebar.classList.toggle('toggled');
            }
        });
    }
}

// Helper function to safely escape HTML content
function escapeHtml(unsafe) {
    if (!unsafe) return '';
    return unsafe
        .toString()
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}

// Handle form submissions with AJAX
$(document).ready(function () {
    // Handle Create Group form submission
    $('#CreateGroupForm').on('submit', function (e) {
        e.preventDefault();

        var groupName = $('#groupName').val();
        var teamId = $('#teamDropdown1').val();
        var memberIds = [];

        $('#members-list input[type="checkbox"]:checked').each(function () {
            memberIds.push($(this).val());
        });

        if (!groupName || groupName.trim() === '') {
            alert('Please enter a group name');
            return;
        }

        if (memberIds.length === 0) {
            alert('Please select at least one member for the group');
            return;
        }

        $.ajax({
            url: '/Teams/CreateGroup',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({
                groupName: groupName,
                teamId: teamId,
                memberIds: memberIds
            }),
            success: function (response) {
                $('#createGroupModal').modal('hide');
                $('#groupName').val('');
                $('#members-list input[type="checkbox"]').prop('checked', false);

                // Reload groups to show the new one
                loadMembers(teamId);

                // Show success message
                alert('Group created successfully!');
            },
            error: function (xhr, status, error) {
                console.error('Error creating group:', error);
                alert('Error creating group. Please try again.');
            }
        });
    });
});
$(document).ready(function () {
    // ===== GLOBAL VARIABLES =====
    var totalWeeks = programWeeks || 1; // Use the variable defined in the page
    var currentDayColumn;
    var weekViewMode = 1; // Default to 1 week view

    // Update the week counter in the navigation
    updateWeekCounter();

    // Create additional weeks based on the total number of weeks
    initializeWeeks();

    // Initialize drop zones for drag-drop functionality
    initDropZones();

    // Function to initialize the required number of weeks
    function initializeWeeks() {
        // The first week is already in the HTML, so start from week 2
        for (let i = 2; i <= totalWeeks; i++) {
            // Create a new week with 7 days
            var newWeekHtml = `
                        <div class="row week-row mb-4">
                            ${createWeekDays(i)}
                        </div>
                    `;

            // Append the new week before the "Add Week" button row
            $('.add-week-btn').closest('.row').before(newWeekHtml);
        }

        // Update visibility based on current view mode
        updateWeekVisibility();
    }

    // Update UI elements that display the week count
    $('.week-tabs .btn-group .btn').removeClass('btn-primary').addClass('btn-secondary');

    // Select the appropriate week button based on total weeks
    if (totalWeeks <= 1) {
        $('.week-tabs .btn-group .btn:contains("1 Week")').removeClass('btn-secondary').addClass('btn-primary');
    } else if (totalWeeks <= 2) {
        $('.week-tabs .btn-group .btn:contains("2 Week")').removeClass('btn-secondary').addClass('btn-primary');
    } else {
        $('.week-tabs .btn-group .btn:contains("4 Week")').removeClass('btn-secondary').addClass('btn-primary');
        weekViewMode = 4;
    }

    function updateDayNumbers() {
        // Assuming you have a container with week rows and day columns
        const weekContainers = document.querySelectorAll('.week-container');
        let dayCounter = 1;

        weekContainers.forEach((weekContainer, weekIndex) => {
            const dayColumns = weekContainer.querySelectorAll('.day-column');

            dayColumns.forEach((dayColumn) => {
                // Add or update day number label
                let dayNumberEl = dayColumn.querySelector('.day-number');
                if (!dayNumberEl) {
                    dayNumberEl = document.createElement('div');
                    dayNumberEl.className = 'day-number';
                    dayColumn.prepend(dayNumberEl);
                }

                dayNumberEl.textContent = `Day ${dayCounter}`;
                // Also store as data attribute for potential programmatic use
                dayColumn.setAttribute('data-day-number', dayCounter);

                dayCounter++;
            });
        });
    }
    // ===== INITIALIZATION FUNCTIONS =====

    // Initialize DataTables if available
    if ($.fn.DataTable) {
        var workoutTable = $('#existingWorkoutsTable').DataTable({
            paging: true,
            searching: true,
            info: false,
            lengthChange: false,
            pageLength: 5,
            language: {
                search: "",
                searchPlaceholder: "Search workouts..."
            }
        });

        // Use the DataTable search input
        $('#workoutSearchInput').on('keyup', function () {
            workoutTable.search(this.value).draw();
        });
    }

    // Initialize drop zones for drag-drop functionality
    initDropZones();

    // ===== UTILITY FUNCTIONS =====

    // Function to properly close modals and clean up
    function closeModal(modalId) {
        $(modalId).modal('hide');
        $('.modal-backdrop').remove();
        $('body').removeClass('modal-open').css('padding-right', '');
    }

    // Ensure modals close properly - fix for Bootstrap 4 modal issues
    $('#addWorkoutModal, #addExistingWorkoutModal, #workoutDetailsModal').on('hidden.bs.modal', function () {
        $('.modal-backdrop').remove();
        $('body').removeClass('modal-open').css('padding-right', '');
    });

    // Function to add a row to the workout creation form
    function addRow(button) {
        // Find the table and tbody - changed selector to match your HTML structure
        const table = $(button).closest('.modal-body').find('table');
        const tbody = table.find('tbody');

        // Create a unique ID for this row's schema table
        const rowId = 'row' + Date.now();
        const schemaTableId = 'schemaTable_' + rowId;

        // Create a new row with exercise-select and schema-select
        const newRow = `
                    <tr id="${rowId}" class="exercise-row">
                        <td>
                            <select class="form-select col-md-9 exercise-select">
                                <option>Select Exercise</option>
                                <option>Bench Press</option>
                                <option>Deadlift</option>
                                <option>3D BPA</option>
                            </select>
                        </td>
                        <td>
                            <select class="form-select col-md-9 schema-select" data-schema-target="#${schemaTableId}">
                                <option>Select Schema</option>
                                <option>5/3/1</option>
                                <option>5x5</option>
                                <option>2x2</option>
                            </select>
                        </td>
                    </tr>
                `;

        // Add the new rows to the table
        const mainSchemaRow = tbody.find('#schemaTable');
        if (mainSchemaRow.length > 0) {
            mainSchemaRow.before(newRow);
        } else {
            tbody.append(newRow);
        }

        // Initialize event handler for the new schema-select
        $(`#${rowId} .schema-select`).on('change', function () {
            const selectedSchema = $(this).val();
            const targetId = $(this).data('schema-target');

            if (selectedSchema && selectedSchema !== 'Select Schema' && selectedSchema !== 'Select Exercise') {
                $(targetId).collapse('show');
            } else {
                $(targetId).collapse('hide');
            }
        });
    }

    // Function to update the week counter in the navigation
    function updateWeekCounter() {
        $('.week-navigation h5').text('Week 1 of ' + totalWeeks);
    }

    // Function to create a week with 7 days
    function createWeekDays(weekNumber) {
        if ($('.week-row').length === 0) {
            // Wrap the initial week in a week-row class
            $('.col-11 > .row:first').addClass('week-row mb-4');
            // Add data-week attribute to initial day columns
            $('.col-11 > .row:first .day-column').attr('data-week', '1');
        }
        var daysHtml = '';
        for (let i = 1; i <= 7; i++) {
            daysHtml += `
                        <div class="col">
                            <div class="day-header">DAY ${i}</div>
                            <div class="day-column" data-day="${i}" data-week="${weekNumber}">
                                <button class="add-button" title="Add workout" data-toggle="modal" data-target="#addExistingWorkoutModal" data-day="${i}" data-week="${weekNumber}">
                                    <i class="fas fa-plus"></i>
                                </button>
                                <div class="workouts-container"></div>
                            </div>
                        </div>
                    `;
        }
        return daysHtml;
    }

    // Function to update which weeks are visible based on current view mode
    function updateWeekVisibility() {
        // Find all week rows (excluding the add week button row)
        const weekRows = $('.week-row');

        // Determine which week is currently shown in the navigation
        const currentWeekText = $('.week-navigation h5').text();
        const currentWeek = parseInt(currentWeekText.split(' ')[1]);

        // Calculate which weeks should be visible
        const startWeek = currentWeek;
        const endWeek = Math.min(startWeek + weekViewMode - 1, totalWeeks);

        // Hide all weeks first
        weekRows.hide();

        // Show only the weeks in the current view range
        for (let i = startWeek; i <= endWeek; i++) {
            weekRows.filter(function () {
                return $(this).find('.day-column').first().data('week') === i;
            }).show();
        }
    }

    // Function to initialize drop zones (can be called again for dynamically added elements)
    function initDropZones() {
        // Clear any existing handlers to prevent duplicates
        $(document).off('dragover', '.day-column').off('dragleave', '.day-column').off('drop', '.day-column');

        // Using event delegation for all day columns (including dynamically added ones)
        $(document).on('dragover', '.day-column', function (e) {
            e.preventDefault(); // Allow the drop
            $(this).addClass('drag-over');
        });

        $(document).on('dragleave', '.day-column', function () {
            $(this).removeClass('drag-over');
        });

        $(document).on('drop', '.day-column', function (e) {
            e.preventDefault();
            $(this).removeClass('drag-over');

            // Get the dragged element
            const draggedElement = window.draggedWorkout;
            if (draggedElement) {
                // Move the workout card to the target day column's workouts container
                $(this).find('.workouts-container').append(draggedElement);
            }
        });
    }

    // ===== SCHEMA VIEW FUNCTIONS =====

    // Function to generate the schema view
    function generateSchemaView() {
        // Create container for schema view
        const schemaViewContainer = $('<div class="schema-view-container"></div>');

        // Create row for schema cards
        const schemaCardRow = $('<div class="row"></div>');

        // Iterate through each week
        for (let weekNum = 1; weekNum <= totalWeeks; weekNum++) {
            // Get all day columns for this week
            const weekDayColumns = $(`.day-column[data-week="${weekNum}"]`);

            // Count tables (workout cards) in this week
            const workoutCount = weekDayColumns.find('.workout-card').length;

            // Only create a card if the week has workout tables
            if (workoutCount > 0) {
                // Create a card for this week
                const weekCard = `
                            <div class="col-md-4 mb-4">
                                <div class="card schema-week-card">
                                    <div class="card-header bg-primary text-white">
                                        <h5 class="mb-0">Week ${weekNum}</h5>
                                    </div>
                                    <div class="card-body">
                                        <p class="card-text"><strong>Workouts:</strong> ${workoutCount}</p>
                                        <p class="card-text"><strong>Days with exercises:</strong> ${weekDayColumns.filter(function () {
                    return $(this).find('.workout-card').length > 0;
                }).length} / 7</p>
                                        <table class="display table table-bordered table-hover">
                                            <thead>
                                                <tr class="thead-light" style="margin-top:10px;">
                                                    <th>Reps</th>
                                                    <th>%1RM</th>
                                                    <th>RPE</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td>Reps</td>
                                                    <td>PercentRM</td>
                                                    <td>RPE</td>
                                                </tr>
                                            </tbody>
                                         </table>
                                    </div>
                                </div>
                            </div>
                        `;

                // Add card to row
                schemaCardRow.append(weekCard);
            }
        }

        // Add row to container
        schemaViewContainer.append(schemaCardRow);

        // Add container after the week navigation
        $('.week-navigation').after(schemaViewContainer);

        // Initially hide schema view
        schemaViewContainer.hide();
    }

    // Function to update the schema view (use when program content changes)
    function updateSchemaView() {
        // Remove existing schema cards
        $('.schema-view-container .row').empty();

        // Regenerate cards
        for (let weekNum = 1; weekNum <= totalWeeks; weekNum++) {
            // Get all day columns for this week
            const weekDayColumns = $(`.day-column[data-week="${weekNum}"]`);

            // Count tables (workout cards) in this week
            const workoutCount = weekDayColumns.find('.workout-card').length;

            // Only create a card if the week has workout tables
            if (workoutCount > 0) {
                // Create a card for this week
                const weekCard = `
                            <div class="col-md-4 mb-4">
                                <div class="card schema-week-card">
                                    <div class="card-header bg-primary text-white">
                                        <h5 class="mb-0">Week ${weekNum}</h5>
                                    </div>
                                    <div class="card-body">
                                        <p class="card-text"><strong>Workouts:</strong> ${workoutCount}</p>
                                        <p class="card-text"><strong>Days with exercises:</strong> ${weekDayColumns.filter(function () {
                    return $(this).find('.workout-card').length > 0;
                }).length} / 7</p>
                                         <table class="display table table-bordered table-hover">
                                            <thead>
                                                <tr class="thead-light" style="margin-top:10px;">
                                                    <th>Reps</th>
                                                    <th>%1RM</th>
                                                    <th>RPE</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td>Reps</td>
                                                    <td>PercentRM</td>
                                                    <td>RPE</td>
                                                </tr>
                                            </tbody>
                                         </table>
                                    </div>
                                </div>
                            </div>
                        `;

                // Add card to row
                $('.schema-view-container .row').append(weekCard);
            }
        }
    }

    // Function to fetch workout exercises from the server
    function fetchWorkoutExercises(workoutId, callback) {
        $.ajax({
            url: '/Programs/GetWorkoutExercises',
            type: 'GET',
            data: { id: workoutId },
            success: function (response) {
                if (response && response.success) {
                    callback(response.exercises, response.schemaWeeks || 4); // Pass schema weeks if available
                } else {
                    // Fallback to sample data if the endpoint isn't implemented
                    const sampleExercises = [
                        { name: "Bench Press", schema: "5x5", schemaDetails: "5 sets × 5 reps" },
                        { name: "Push-up", schema: "3x10", schemaDetails: "3 sets × 10 reps" }
                    ];
                    callback(sampleExercises, 4); // Default to 4 weeks for sample data
                }
            },
            error: function () {
                // Fallback to sample data on error
                const sampleExercises = [
                    { name: "Bench Press", schema: "5x5", schemaDetails: "5 sets × 5 reps" },
                    { name: "Push-up", schema: "3x10", schemaDetails: "3 sets × 10 reps" }
                ];
                callback(sampleExercises, 4); // Default to 4 weeks for sample data
            }
        });
    }

    // Helper function to display workout details
    function displayWorkoutDetails(workout) {
        // Update modal title with workout name
        $('#workoutDetailsModalLabel').text('Workout Details: ' + workout.name);

        // Build the details content
        let detailsHtml = `
                    <div class="card mb-3">
                        <div class="card-header bg-primary text-white">
                            <h6 class="mb-0">${workout.name}</h6>
                        </div>
                        <div class="card-body">
                            <h6 class="mt-3">Exercises:</h6>
                            <div class="exercise-list">`;

        // Add exercises
        if (workout.exercises && workout.exercises.length > 0) {
            workout.exercises.forEach(function (exercise) {
                detailsHtml += `
                                <div class="exercise-row border-bottom py-2">
                                    <div class="row">
                                        <div class="col-6">
                                            <span class="exercise-name">${exercise.name}</span>
                                        </div>
                                        <div class="col-6">
                                            <span class="exercise-schema">${exercise.schema}</span>
                                            <span class="schema-details d-none">${exercise.schemaDetails}</span>
                                        </div>
                                    </div>
                                </div>`;
            });
        } else {
            detailsHtml += '<p class="text-muted">No exercises in this workout</p>';
        }

        detailsHtml += `
                                </div>
                            </div>
                        </div>`;

        // Update the modal content
        $('#workout-details-content').html(detailsHtml);
    }

    // Helper function to display sample workout details when API fails
    function displaySampleWorkoutDetails() {
        // Show error message
        $('#workout-details-content').html('<div class="alert alert-danger">Error loading workout details</div>');

        // Fallback to sample data
        setTimeout(function () {
            const sampleWorkout = {
                name: "Sample Workout",
                type: "Strength",
                exercises: [
                    { name: "Bench Press", schema: "5x5", schemaDetails: "5 sets × 5 reps" },
                    { name: "Push-up", schema: "3x10", schemaDetails: "3 sets × 10 reps" }
                ]
            };

            displayWorkoutDetails(sampleWorkout);
        }, 1000);
    }

    // Function to add an existing workout to the current day column
    function addExistingWorkoutToDay(workoutName, exercises, schemaWeeks = 1) {
        if (!currentDayColumn) {
            console.error("No day column selected");
            return;
        }

        // Get current day and week
        const currentDay = currentDayColumn.data('day');
        const currentWeek = currentDayColumn.data('week') || 1;

        // Create HTML for all exercise items
        var exerciseItemsHtml = '';
        exercises.forEach(function (exercise) {
            exerciseItemsHtml += `
                    <div class="exercise-item" data-exercise-id="${exercise.id || 1}" data-schema-id="${exercise.schemaId || 1}">
                        <div class="workout-card-title">${exercise.name}</div>
                        <p class="workout-card-text">Schema: ${exercise.schemaDetails || exercise.schema}</p>
                    </div>
                `;
        });

        // Create a workout card with all exercises
        var workoutCard = `
                    <div class="workout-card" draggable="true" data-schema-weeks="${schemaWeeks}">
                        <div class="workout-card-header">
                            ${workoutName}
                            <span class="remove-workout" title="Remove workout"><i class="fas fa-times"></i></span>
                        </div>
                        <div class="workout-card-body">
                            ${exerciseItemsHtml}
                        </div>
                    </div>
                `;

        // Add the workout card to the current day column
        currentDayColumn.find('.workouts-container').append(workoutCard);

        // If the schema spans multiple weeks, add this workout to subsequent weeks
        if (schemaWeeks > 1) {
            // Calculate how many additional weeks we need to add this workout to
            const weeksToAdd = Math.min(schemaWeeks - 1, totalWeeks - currentWeek);

            for (let weekOffset = 1; weekOffset <= weeksToAdd; weekOffset++) {
                const targetWeek = currentWeek + weekOffset;
                // Find the corresponding day column in the target week
                const targetDayColumn = $(`.day-column[data-day="${currentDay}"][data-week="${targetWeek}"]`);

                if (targetDayColumn.length > 0) {
                    // Create a clone of the workout card for the next week
                    const workoutCardClone = $(workoutCard).clone();
                    // Add a label to indicate it's part of a multi-week schema
                    workoutCardClone.find('.workout-card-header').append(`
                                <span class="badge badge-light ml-1" title="Week ${weekOffset + 1} of ${schemaWeeks} week schema">
                                    ${weekOffset + 1}/${schemaWeeks}
                                </span>
                            `);

                    // Add the cloned workout card to the target day column
                    targetDayColumn.find('.workouts-container').append(workoutCardClone);
                }
            }
        }

        // Ensure the add button remains above all workout cards
        $('.day-column .add-button').css('z-index', '20');
    }
    // ===== EVENT HANDLERS =====

    // Week view buttons click handler
    $('.btn-group .btn').on('click', function () {
        // Update active button state
        $('.btn-group .btn').removeClass('btn-primary').addClass('btn-secondary');
        $(this).removeClass('btn-secondary').addClass('btn-primary');

        // Get the selected view mode from button text
        const buttonText = $(this).text().trim();
        weekViewMode = parseInt(buttonText.split(' ')[0]);

        // Apply the week visibility
        updateWeekVisibility();
    });

    // Add row button click handler
    $(document).on('click', '.add-row-btn', function () {
        addRow(this);
    });

    // Listen for change on the schema-select dropdown
    $(document).on('change', '.schema-select', function () {
        // Get the selected value
        var selectedSchema = $(this).val();
        // Get the target schema table
        var targetSchemaId = $(this).data('schema-target');

        // Check if a real option is selected
        if (selectedSchema && selectedSchema !== 'Select Schema' && selectedSchema !== 'Select Exercise') {
            // Show the schema table
            $(targetSchemaId).collapse('show');
        } else {
            // Hide the table
            $(targetSchemaId).collapse('hide');
        }
    });

    // Add Week button functionality
    $('.add-week-btn, .btn-light:contains("+ Add Week")').on('click', function () {
        // Increment total weeks
        totalWeeks++;

        // Create a new week with 7 days
        var newWeekHtml = `
                    <div class="row week-row mb-4">
                        ${createWeekDays(totalWeeks)}
                    </div>
                `;

        // Append the new week before the "Add Week" button row
        $('.add-week-btn').closest('.row').before(newWeekHtml);

        // Update the week counter in the navigation
        updateWeekCounter();

        // Reinitialize drop zones for the new day columns
        initDropZones();

        // Update visibility based on current view mode
        updateWeekVisibility();
    });

    // When any add button is clicked
    $(document).on('click', '.add-button', function () {
        // Store the day column reference
        currentDayColumn = $(this).closest('.day-column');

        // Get day and week info
        var dayNumber = $(this).data('day');
        var weekNumber = $(this).data('week') || 1;

        // Update modal title to include day and week numbers
        $('#addExistingWorkoutModalLabel').text('Add Workout for Week ' + weekNumber + ', Day ' + dayNumber);
        $('#addWorkoutModalLabel').text('Add Workout for Week ' + weekNumber + ', Day ' + dayNumber);

        // Reset the form by clearing all rows except the first one
        $('.program-table tbody tr:not(:first)').remove();

        // Reset first row selections
        $('.program-table tbody tr:first .exercise-select').val('Select Exercise');
        $('.program-table tbody tr:first .schema-select').val('Select Schema');
    });

    // Add workout button in modal
    $('#addWorkoutBtn').on('click', function () {
        // Collect all exercise-schema pairs from the modal
        var exercisePairs = [];
        var isValid = true;

        // Iterate through all rows in the table
        $('.program-table tbody tr').each(function () {
            var exercise = $(this).find('.exercise-select').val();
            var schema = $(this).find('.schema-select').val();

            // Skip empty rows
            if (exercise === 'Select Exercise' && schema === 'Select Schema') {
                return true; // continue to next iteration
            }

            // Validate that both exercise and schema are selected
            if (exercise === 'Select Exercise' || schema === 'Select Schema') {
                isValid = false;
                return false; // break the loop
            }

            exercisePairs.push({
                exercise: exercise,
                schema: schema
            });
        });

        // If validation failed or no exercises were selected
        if (!isValid || exercisePairs.length === 0) {
            alert('Please select both an exercise and a schema for each row');
            return;
        }

        // Create HTML for all exercise items
        var exerciseItemsHtml = '';
        exercisePairs.forEach(function (pair) {
            // You can extract more details about the schema here
            let schemaDetails = '';
            if (pair.schema === '5x5') {
                schemaDetails = '5 sets × 5 reps';
            } else if (pair.schema === '5/3/1') {
                schemaDetails = '3 sets (5/3/1 reps)';
            } else if (pair.schema === '2x2') {
                schemaDetails = '2 sets × 2 reps';
            } else {
                schemaDetails = pair.schema;
            }

            exerciseItemsHtml += `
                        <div class="exercise-item">
                            <div class="workout-card-title">${pair.exercise}</div>
                            <p class="workout-card-text">Schema: ${schemaDetails}</p>
                        </div>
                    `;
        });

        // Create a workout card with all exercises
        var workoutCard = `
                    <div class="workout-card" draggable="true">
                        <div class="workout-card-header">
                            Custom Workout
                            <span class="remove-workout" title="Remove workout"><i class="fas fa-times"></i></span>
                        </div>
                        <div class="workout-card-body">
                            ${exerciseItemsHtml}
                        </div>
                    </div>
                `;

        // Add the workout card to the workouts-container inside the day column
        if (currentDayColumn) {
            currentDayColumn.find('.workouts-container').append(workoutCard);
            // Ensure the add button remains above all workout cards
            currentDayColumn.find('.add-button').css('z-index', '20');
        }

        // Close the modal properly
        closeModal('#addWorkoutModal');
    });

    // Handle removal of workout cards using event delegation
    $(document).on('click', '.remove-workout', function (e) {
        e.stopPropagation(); // Prevent event bubbling
        $(this).closest('.workout-card').fadeOut(300, function () {
            $(this).remove();
        });
    });

    // Handle selecting a workout from the table
    $(document).on('click', '.select-workout', function () {
        const workoutId = $(this).data('workout-id');
        const workoutName = $(this).data('workout-name');

        // Get the schema weeks (could be retrieved from server, using 4 as example)
        // In production, this should come from your backend
        const schemaWeeks = $(this).data('schema-weeks') || 4; // Default to 4 weeks if not specified

        // Fetch the workout details to display it properly
        fetchWorkoutExercises(workoutId, function (exercises) {
            // Add the workout to the current day column and propagate to subsequent weeks
            addExistingWorkoutToDay(workoutName, exercises, schemaWeeks);

            // Close the modal
            closeModal('#addExistingWorkoutModal');
        });
    });

    // Handle viewing workout details
    $(document).on('click', '.view-workout-details', function () {
        const workoutId = $(this).data('workout-id');
        // Store the ID for the select button
        $('#selectWorkoutFromDetails').data('workout-id', workoutId);

        // Get workout details and display in the modal
        fetchWorkoutDetails(workoutId);

        // Hide the existing workout modal and show the details modal
        closeModal('#addExistingWorkoutModal');
        $('#workoutDetailsModal').modal('show');
    });

    // Handle selecting a workout from the details modal
    $('#selectWorkoutFromDetails').on('click', function () {
        const workoutId = $(this).data('workout-id');
        const schemaWeeks = $(this).data('schema-weeks') || 4; // Default to 4 weeks

        // Get the workout name from the details modal title
        const workoutName = $('#workoutDetailsModalLabel').text().replace('Workout Details: ', '');

        // Get exercise data from the details
        const exercises = [];
        $('#workout-details-content .exercise-row').each(function () {
            exercises.push({
                name: $(this).find('.exercise-name').text(),
                schema: $(this).find('.exercise-schema').text(),
                schemaDetails: $(this).find('.schema-details').text()
            });
        });

        // Add the workout to the current day and subsequent weeks
        addExistingWorkoutToDay(workoutName, exercises, schemaWeeks);

        // Close the modal
        closeModal('#workoutDetailsModal');
    });

    // Drag and drop functionality
    $(document).on('dragstart', '.workout-card', function (e) {
        e.originalEvent.dataTransfer.setData('text/plain', '');
        $(this).addClass('dragging');
        // Store the source card as a global variable
        window.draggedWorkout = this;
    });

    $(document).on('dragend', '.workout-card', function () {
        $(this).removeClass('dragging');
        window.draggedWorkout = null;
    });

    // Update week navigation buttons to change visible weeks
    $('.week-navigation .btn-outline-secondary').on('click', function () {
        const isNext = $(this).find('i.fa-chevron-right').length > 0;
        const currentWeekText = $('.week-navigation h5').text();
        let currentWeek = parseInt(currentWeekText.split(' ')[1]);

        if (isNext && currentWeek + weekViewMode <= totalWeeks) {
            currentWeek++;
        } else if (!isNext && currentWeek > 1) {
            currentWeek--;
        }

        // Update the week counter
        $('.week-navigation h5').text('Week ' + currentWeek + ' of ' + totalWeeks);

        // Update visible weeks
        updateWeekVisibility();
    });

    // Schema Details View Toggle
    $('.week-navigation .btn-light:contains("View Schema Details")').on('click', function () {
        // Check if we're already in schema view mode
        const isInSchemaView = $(this).hasClass('active');

        if (isInSchemaView) {
            // Switch back to regular view
            $('.schema-view-container').hide();
            $('.col-11 > .row').show();
            $('.add-week-btn').closest('.row').show();
            $(this).removeClass('active').text('View Schema Details');
        } else {
            // Switch to schema view
            // If schema view container doesn't exist, create it
            if ($('.schema-view-container').length === 0) {
                generateSchemaView();
            } else {
                // Update the existing schema view to reflect any changes
                updateSchemaView();
            }

            // Hide regular view, show schema view
            $('.col-11 > .row').hide();
            $('.schema-view-container').show();
            $(this).addClass('active').text('Return to Program View');
        }
    });

    // Function to fetch workout details from the server
    function fetchWorkoutDetails(workoutId) {
        $.ajax({
            url: '/Programs/GetWorkoutDetails',
            type: 'GET',
            data: { id: workoutId },
            success: function (response) {
                if (response && response.success) {
                    displayWorkoutDetails(response.workout);
                } else {
                    displaySampleWorkoutDetails();
                }
            },
            error: function () {
                displaySampleWorkoutDetails();
            }
        });
    }

    // Event handler for viewing week details
    $(document).on('click', '.view-week-details', function () {
        const weekNum = $(this).data('week');

        // Switch back to program view
        $('.schema-view-container').hide();
        $('.col-11 > .row').show();
        $('.add-week-btn').closest('.row').show();
        $('.week-navigation .btn-light').removeClass('active').text('View Schema Details');

        // Show the selected week
        // First, update the week navigation text
        $('.week-navigation h5').text('Week ' + weekNum + ' of ' + totalWeeks);

        // Then apply visibility based on week view mode
        updateWeekVisibility();
    });

    $('.btn-primary:contains("Save Program")').on('click', function () {
        // Collect all workout data from the UI
        const workouts = [];

        // For each week
        for (let week = 1; week <= totalWeeks; week++) {
            // For each day column in this week
            $(`.day-column[data-week="${week}"]`).each(function () {
                const day = $(this).data('day');

                // For each workout card in this day (THIS IS KEY - process each card separately)
                $(this).find('.workout-card').each(function () {
                    // Get workout name - clean up by removing the "x" button text
                    const workoutHeader = $(this).find('.workout-card-header').clone();
                    workoutHeader.find('.remove-workout').remove();
                    const workoutName = workoutHeader.text().trim();

                    // Create a new workout entry for EACH card
                    const workout = {
                        Week: week,
                        Day: day,
                        WorkoutName: workoutName,
                        Notes: "",
                        Exercises: []
                    };

                    // Collect exercises just from THIS workout card
                    $(this).find('.exercise-item').each(function () {
                        const exerciseName = $(this).find('.workout-card-title').text().trim();
                        const schemaText = $(this).find('.workout-card-text').text().trim();

                        // Extract IDs or use defaults
                        let exerciseID = $(this).data('exercise-id') || 1;
                        let schemaID = $(this).data('schema-id') || 1;

                        workout.Exercises.push({
                            ExerciseID: exerciseID,
                            SchemaID: schemaID,
                            ExerciseName: exerciseName,
                            SchemaName: schemaText.replace('Schema: ', '')
                        });
                    });

                    // Only add if there are exercises
                    if (workout.Exercises.length > 0) {
                        workouts.push(workout);
                    }
                });
            });
        }

        // Create a form to submit the data
        const form = $('<form></form>').attr({
            'method': 'post',
            'action': saveProgramUrl // Use the URL from the page
        });

        // Add program basic info
        form.append($('<input>').attr({
            'type': 'hidden',
            'name': 'name',
            'value': programName
        }));

        form.append($('<input>').attr({
            'type': 'hidden',
            'name': 'weeks',
            'value': totalWeeks
        }));

        form.append($('<input>').attr({
            'type': 'hidden',
            'name': 'phase',
            'value': programPhase
        }));

        // Add the workout data - each one as a SEPARATE workout
        for (let i = 0; i < workouts.length; i++) {
            const workout = workouts[i];

            form.append($('<input>').attr({
                'type': 'hidden',
                'name': `workouts[${i}].Week`,
                'value': workout.Week
            }));

            form.append($('<input>').attr({
                'type': 'hidden',
                'name': `workouts[${i}].Day`,
                'value': workout.Day
            }));

            form.append($('<input>').attr({
                'type': 'hidden',
                'name': `workouts[${i}].WorkoutName`,
                'value': workout.WorkoutName
            }));

            form.append($('<input>').attr({
                'type': 'hidden',
                'name': `workouts[${i}].Notes`,
                'value': workout.Notes
            }));

            // Add exercises for this workout
            for (let j = 0; j < workout.Exercises.length; j++) {
                const exercise = workout.Exercises[j];

                form.append($('<input>').attr({
                    'type': 'hidden',
                    'name': `workouts[${i}].Exercises[${j}].ExerciseID`,
                    'value': exercise.ExerciseID
                }));

                form.append($('<input>').attr({
                    'type': 'hidden',
                    'name': `workouts[${i}].Exercises[${j}].SchemaID`,
                    'value': exercise.SchemaID
                }));

                form.append($('<input>').attr({
                    'type': 'hidden',
                    'name': `workouts[${i}].Exercises[${j}].ExerciseName`,
                    'value': exercise.ExerciseName
                }));

                form.append($('<input>').attr({
                    'type': 'hidden',
                    'name': `workouts[${i}].Exercises[${j}].SchemaName`,
                    'value': exercise.SchemaName
                }));
            }
        }

        // Add form to document and submit
        $('body').append(form);
        form.submit();
    });

    // Initialize existing program workouts if available
    if (typeof programWorkouts !== 'undefined' && programId > 0 && programWorkouts && programWorkouts.length > 0) {
        console.log("Found program workouts to display:", programWorkouts.length);

        // Create workout cards for each workout in the program
        programWorkouts.forEach(function (workout) {
            console.log("Processing workout:", workout);

            // Find the correct day column
            var weekNumber = workout.week || 1;
            var dayNumber = workout.day || 1;
            var dayColumn = $(`.day-column[data-week="${weekNumber}"][data-day="${dayNumber}"]`);

            console.log("Looking for day column:", weekNumber, dayNumber, dayColumn.length);

            if (dayColumn.length > 0) {
                // Create a basic workout name if needed
                var workoutName = workout.workoutName || "Workout";

                // Create exercise list
                var exerciseItems = "";
                if (workout.exercises && workout.exercises.length > 0) {
                    workout.exercises.forEach(function (ex) {
                        exerciseItems += `
                            <div class="exercise-item" data-exercise-id="${ex.exerciseId}" data-schema-id="${ex.schemaId}">
                                <div class="workout-card-title">${ex.exerciseName}</div>
                                <p class="workout-card-text">Schema: ${ex.schemaName || "Unknown"}</p>
                            </div>
                        `;
                    });
                }

                // Create a workout card
                var workoutCard = `
                    <div class="workout-card" draggable="true">
                        <div class="workout-card-header">
                            ${workoutName}
                            <span class="remove-workout" title="Remove workout"><i class="fas fa-times"></i></span>
                        </div>
                        <div class="workout-card-body">
                            ${exerciseItems}
                        </div>
                    </div>
                `;

                // Add the workout card to the day column
                dayColumn.find('.workouts-container').append(workoutCard);
            }
        });
    } else {
        console.log("No program workouts to display");
    }
});
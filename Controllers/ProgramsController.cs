using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhasePlayWeb.Data;
using PhasePlayWeb.Models;
using PhasePlayWeb.Models.Entities;
using System.Data;
using System.Diagnostics;

namespace PhasePlayWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProgramsController : Controller
    {
        private readonly ILogger<ProgramsController> _logger;
        private readonly ApplicationDbContext _databaseContext;

        public ProgramsController(ILogger<ProgramsController> logger, ApplicationDbContext _databaseContext)
        {
            _logger = logger;
            this._databaseContext = _databaseContext;
        }

        [HttpGet]
        public IActionResult WorkoutBuilder()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> WorkoutBuilder(ProgrammeVM vm, [FromQuery] ProgramAttributes attributes)
        {
            // Retrieve the user's email from the cookies
            var userEmail = HttpContext.Request.Cookies["UserEmail"];
            // Find the user in the database using the retrieved email
            var user = await _databaseContext.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            // Create a new program with the provided data from the view model
            var program = new Programme
            {
                Name = vm.Name,
                UserID = user.Id,
                Weeks = vm.Weeks,
            };

            // Add the new program to the database and save changes
            await _databaseContext.Programs.AddAsync(program);
            await _databaseContext.SaveChangesAsync();

            // Loop through the number of weeks and create ProgramAttributes for each week
            for (int i = 1; i < (program.Weeks) + 1; i++)
            {
                var Attributes = new ProgramAttributes
                {
                    ProgramID = program.id,
                    WeekNumber = i,
                    Programme = program,
                };
                // Add the ProgramAttributes to the database and save changes
                await _databaseContext.ProgramAttributes.AddAsync(Attributes);
                await _databaseContext.SaveChangesAsync();
            }

            // Redirect to the WarmAndMobil action, passing the created program as a parameter
            return RedirectToAction(nameof(WarmAndMobil), program);
        }

        [HttpGet]
        public async Task<IActionResult> newProgramCreation(string name, int weeks, string phase)
        {
            // Get all workouts from the database
            var allWorkouts = await _databaseContext.Workouts.ToListAsync();

            // Group workouts by Name and take the first entry from each group to get unique workouts
            var uniqueWorkouts = allWorkouts
                .GroupBy(w => w.Name)
                .Select(g => g.First())
                .ToList();

            //Pull in Exercises 
            var excercises = await _databaseContext.Excercises.ToListAsync();

            // Get only distinct schema names
            var schemas = await _databaseContext.SRSchema
                .GroupBy(s => s.name)
                .Select(g => g.First())
                .ToListAsync();
            // Store values in ViewBag for access in the view
            ViewBag.ProgramName = name;
            ViewBag.Weeks = weeks;
            ViewBag.Phase = phase;

            var tuple = new Tuple<List<Workout>, List<Excercises>, List<S_RSchema>>(uniqueWorkouts, excercises , schemas );
            return View("newProgramCreation", tuple);
        }

        [HttpGet]
        public async Task<IActionResult> GetWorkoutExercises(int id)
        {
            try
            {
                // Get the workout from the database
                var workout = await _databaseContext.Workouts
                    .Where(w => w.ID == id)
                    .Select(w => w.Name)
                    .FirstOrDefaultAsync();

                if (workout == null)
                {
                    return Json(new { success = false, message = "Workout not found" });
                }

                // Get all exercises in this workout (all entries with the same name)
                var exercises = await _databaseContext.Workouts
                    .Where(w => w.Name == workout)
                    .Include(w => w.S_RSchema)
                    .Select(w => new
                    {
                        name = w.ExcerciseName,
                        schema = w.S_RSchema.name,
                    })
                    .ToListAsync();

                return Json(new { success = true, exercises = exercises });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching workout exercises");
                return Json(new { success = false, message = "An error occurred while fetching workout exercises" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetWorkoutDetails(int id)
        {
            try
            {
                // Get the workout by ID
                var workoutName = await _databaseContext.Workouts
                    .Where(w => w.ID == id)
                    .Select(w => w.Name)
                    .FirstOrDefaultAsync();

                if (workoutName == null)
                {
                    return Json(new { success = false, message = "Workout not found" });
                }

                // Get all exercises in this workout (all entries with the same name)
                var exercises = await _databaseContext.Workouts
                    .Where(w => w.Name == workoutName)
                    .Include(w => w.S_RSchema)
                    .ToListAsync();

                // Build the response
                var result = new
                {
                    success = true,
                    name = workoutName,
                    type = exercises.FirstOrDefault()?.Type ?? "Unknown",
                    exercises = exercises.Select(e => new
                    {
                        name = e.ExcerciseName,
                        schema = e.S_RSchema?.name ?? "No Schema",
                    }).ToList()
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching workout details");
                return Json(new { success = false, message = "An error occurred while fetching workout details" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ViewProgramInCreator(int id)
        {
            try
            {
                // Start by logging to track execution
                _logger.LogInformation("Viewing program in creator, ID: {Id}", id);

                // Get the program
                var program = await _databaseContext.Programs.FirstOrDefaultAsync(p => p.id == id);
                if (program == null)
                {
                    _logger.LogWarning("Program not found, ID: {Id}", id);
                    return NotFound();
                }

                // Get all workouts from the database for the dropdown
                var allWorkouts = await _databaseContext.Workouts.ToListAsync();
                _logger.LogInformation("Found {Count} available workouts for dropdown", allWorkouts.Count);

                // Get all exercises for the dropdown
                var exercises = await _databaseContext.Excercises.ToListAsync();
                _logger.LogInformation("Found {Count} available exercises", exercises.Count);

                // Get all schemas
                var schemas = await _databaseContext.SRSchema
                    .GroupBy(s => s.name)
                    .Select(g => g.First())
                    .ToListAsync();
                _logger.LogInformation("Found {Count} unique schemas", schemas.Count);

                // Set ViewBag properties required by the view
                ViewBag.ProgramName = program.Name;
                ViewBag.Weeks = program.Weeks;
                ViewBag.Phase = "Unknown"; // Or determine from other properties if available

                // Pass the program ID
                ViewBag.ProgramId = id;

                // Get all exercise data for this program
                var paExercises = await _databaseContext.PAExercises
                    .Where(pa => pa.ProgramID == id)
                    .ToListAsync();

                _logger.LogInformation("Found {Count} PAExercises for program {Id}", paExercises.Count, id);

                if (paExercises.Count == 0)
                {
                    // If no exercises found, log warning and pass empty workout data
                    _logger.LogWarning("No PAExercises found for program {Id}", id);
                    ViewBag.ProgramWorkouts = new List<object>();
                }
                else
                {
                    // Get all exercise sets for this program with proper includes
                    var exerciseSets = new List<ExerciseSetAttrib>();
                    foreach (var pa in paExercises)
                    {
                        var sets = await _databaseContext.SetExcercises
                            .Where(es => es.PAExerciseID == pa.id)
                            .Include(es => es.S_RSchema) // Include schema information
                            .ToListAsync();

                        _logger.LogInformation("Found {Count} exercise sets for PAExercise {Id}", sets.Count, pa.id);
                        exerciseSets.AddRange(sets);
                    }

                    // First, group by Week, Day and PAExerciseID (this preserves multiple workouts per day)
                    var programWorkouts = exerciseSets
                        .Where(es => es.Day > 0 && es.PerWeek.HasValue) // Only include entries with valid Day and Week
                        .GroupBy(es => new { es.PerWeek, es.Day, es.PAExerciseID })
                        .Select(g =>
                        {
                            var paExercise = paExercises.FirstOrDefault(pa => pa.id == g.Key.PAExerciseID);

                            return new
                            {
                                Week = g.Key.PerWeek,
                                Day = g.Key.Day,
                                PAExerciseID = g.Key.PAExerciseID,
                                WorkoutName = paExercise?.name ?? "Workout",
                                Notes = paExercise?.AddNote,
                                // Group exercises within this workout
                                Exercises = g.GroupBy(es => es.ExcerciseID)
                                    .Select(eg => new
                                    {
                                        ExerciseId = eg.Key,
                                        ExerciseName = eg.First().ExcerciseName,
                                        SchemaId = eg.First().SchemaID,
                                        SchemaName = eg.First().S_RSchema?.name ?? "Unknown Schema"
                                    }).ToList()
                            };
                        }).ToList();

                    _logger.LogInformation("Generated {Count} program workouts for view", programWorkouts.Count);

                    // Log each workout for debugging
                    foreach (var workout in programWorkouts)
                    {
                        _logger.LogInformation("Workout for Week {Week}, Day {Day}, PAExerciseID {ID} has {Count} exercises",
                            workout.Week, workout.Day, workout.PAExerciseID, workout.Exercises.Count);
                    }

                    // Pass the workout data to the view
                    ViewBag.ProgramWorkouts = programWorkouts;
                }

                // Create the correct Tuple type that matches what the view expects
                var tuple = new Tuple<List<Workout>, List<Excercises>, List<S_RSchema>>(
                    allWorkouts,
                    exercises,
                    schemas
                );

                return View("newProgramCreation", tuple);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error viewing program in creator: {Message}", ex.Message);
                return View("Error", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
            }
        }

        [HttpGet]
        public async Task<IActionResult> newProgram()
        {
            var exercises = await _databaseContext.Excercises.ToListAsync();
            // Get ALL schemas, not just week 1
            var schema = await _databaseContext.SRSchema.ToListAsync();

            var tuple = new Tuple<List<Excercises>, List<S_RSchema>>(exercises, schema);
            return View(tuple);
        }

        [HttpPost]
        public async Task<IActionResult> SaveProgram(string name, int weeks, string phase, List<WorkoutDayVM> workouts)
        {
            try
            {
                // Log received data for debugging
                _logger.LogInformation($"Received program data: Name={name}, Weeks={weeks}, Phase={phase}, Workouts count={workouts?.Count ?? 0}");

                // Retrieve the user's email from the cookies
                var userEmail = HttpContext.Request.Cookies["UserEmail"];
                // Find the user in the database using the retrieved email
                var user = await _databaseContext.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

                if (user == null)
                {
                    _logger.LogWarning("User not found with email: {Email}", userEmail);
                    return BadRequest("User not found");
                }

                // 1. Basic program details
                var program = new Programme
                {
                    Name = name,
                    UserID = user.Id,
                    Weeks = weeks
                };

                // Save the program first
                await _databaseContext.Programs.AddAsync(program);
                await _databaseContext.SaveChangesAsync();

                // 2. Create attributes for each week
                var allProgramAttributes = new List<ProgramAttributes>();
                for (int i = 1; i <= program.Weeks; i++)
                {
                    var attributes = new ProgramAttributes
                    {
                        ProgramID = program.id,
                        WeekNumber = i,
                        Programme = program,
                    };
                    await _databaseContext.ProgramAttributes.AddAsync(attributes);
                    await _databaseContext.SaveChangesAsync();
                    allProgramAttributes.Add(attributes);
                }

                // 3. Process workouts for each day
                if (workouts != null && workouts.Any())
                {
                    // CRITICAL CHANGE: Use each individual workout directly - do NOT group them by week and day
                    // Remove this code that was causing the merging issue:
                    /*
                    var groupedWorkouts = workouts
                        .Where(w => w != null && w.Week > 0 && w.Day > 0)
                        .GroupBy(w => new { w.Week, w.Day })
                        .Select(g => new {
                            Week = g.Key.Week,
                            Day = g.Key.Day,
                            WorkoutName = g.First().WorkoutName ?? "Workout Day",
                            Notes = g.First().Notes,
                            Exercises = g.SelectMany(w => w.Exercises ?? new List<WorkoutExerciseVM>()).ToList()
                        })
                        .ToList();
                    */

                    // Only process valid workouts with exercises
                    var processableWorkouts = workouts
                        .Where(w => w != null && w.Week > 0 && w.Day > 0 &&
                               (w.Exercises?.Any() ?? false))
                        .ToList();

                    // Process each workout individually - each one will be a separate PAExercise
                    foreach (var workout in processableWorkouts)
                    {
                        // Skip if no exercises
                        if (workout.Exercises == null || !workout.Exercises.Any())
                        {
                            _logger.LogWarning("Skipping workout with no exercises: Week={Week}, Day={Day}",
                                workout.Week, workout.Day);
                            continue;
                        }

                        // Create a new PAExercise for EACH workout (don't reuse them by day)
                        var newWorkout = new PAExercise
                        {
                            name = workout.WorkoutName ?? "Workout",
                            AddNote = workout.Notes,
                            ProgramID = program.id,
                        };
                        await _databaseContext.PAExercises.AddAsync(newWorkout);
                        await _databaseContext.SaveChangesAsync();

                        // Get the appropriate program attributes for this week
                        var weekAttributes = allProgramAttributes.FirstOrDefault(a => a.WeekNumber == workout.Week);
                        if (weekAttributes == null)
                        {
                            _logger.LogWarning("Week attributes not found for week {Week}", workout.Week);
                            continue;
                        }

                        // Process exercises for this specific workout
                        foreach (var exercise in workout.Exercises)
                        {
                            // Skip invalid exercise entries
                            if (exercise == null || exercise.ExerciseID <= 0 || exercise.SchemaID <= 0)
                            {
                                _logger.LogWarning("Skipping invalid exercise: {Exercise}",
                                    exercise == null ? "null" : $"ExerciseID={exercise.ExerciseID}, SchemaID={exercise.SchemaID}");
                                continue;
                            }

                            // Find schema for the specific week
                            var schemaForWeek = await _databaseContext.SRSchema
                                .FirstOrDefaultAsync(s => s.id == exercise.SchemaID ||
                                                        (s.name == exercise.SchemaName && s.SetWeek == workout.Week));

                            if (schemaForWeek == null)
                            {
                                _logger.LogWarning("Schema not found for exercise {ExerciseID}, schema {SchemaID} in week {Week}",
                                    exercise.ExerciseID, exercise.SchemaID, workout.Week);

                                // Create a default ExerciseSetAttrib without schema-specific values
                                var exerciseSet = new ExerciseSetAttrib
                                {
                                    PAExerciseID = newWorkout.id,
                                    ExcerciseID = exercise.ExerciseID,
                                    SchemaID = exercise.SchemaID,
                                    ExcerciseName = exercise.ExerciseName ??
                                        await _databaseContext.Excercises
                                            .Where(x => x.Id == exercise.ExerciseID)
                                            .Select(x => x.Name)
                                            .FirstOrDefaultAsync() ?? "Unknown Exercise",
                                    reps = 5, // Default values
                                    RM = 70,
                                    RPE = 7,
                                    PerWeek = workout.Week,
                                    Day = workout.Day
                                };

                                await _databaseContext.SetExcercises.AddAsync(exerciseSet);
                                continue;
                            }

                            // Get schema attributes if they exist
                            var schemaAttributes = await _databaseContext.SchemaAttributes
                                .Where(x => x.SchemaID == schemaForWeek.id)
                                .ToListAsync();

                            // If schema attributes exist, create exercise sets based on them
                            if (schemaAttributes != null && schemaAttributes.Any())
                            {
                                foreach (var attr in schemaAttributes)
                                {
                                    var exerciseSet = new ExerciseSetAttrib
                                    {
                                        PAExerciseID = newWorkout.id,
                                        ExcerciseID = exercise.ExerciseID,
                                        SchemaID = schemaForWeek.id,
                                        ExcerciseName = exercise.ExerciseName ??
                                            await _databaseContext.Excercises
                                                .Where(x => x.Id == exercise.ExerciseID)
                                                .Select(x => x.Name)
                                                .FirstOrDefaultAsync() ?? "Unknown Exercise",
                                        reps = attr.Reps,
                                        RPE = attr.RPE,
                                        RM = attr.PercentRM,
                                        Velocity = attr.Vel,
                                        PerWeek = workout.Week,
                                        Day = workout.Day
                                    };

                                    await _databaseContext.SetExcercises.AddAsync(exerciseSet);
                                }
                            }
                            else
                            {
                                // No schema attributes found, create a default exercise set
                                var exerciseSet = new ExerciseSetAttrib
                                {
                                    PAExerciseID = newWorkout.id,
                                    ExcerciseID = exercise.ExerciseID,
                                    SchemaID = schemaForWeek.id,
                                    ExcerciseName = exercise.ExerciseName ??
                                        await _databaseContext.Excercises
                                            .Where(x => x.Id == exercise.ExerciseID)
                                            .Select(x => x.Name)
                                            .FirstOrDefaultAsync() ?? "Unknown Exercise",
                                    reps = 5, // Default values
                                    RM = 70,
                                    RPE = 7,
                                    PerWeek = workout.Week,
                                    Day = workout.Day
                                };

                                await _databaseContext.SetExcercises.AddAsync(exerciseSet);
                            }
                        }

                        // Create the AttributeSet to link this PAExercise to the ProgramAttributes
                        var attributeSet = new AttributeSet
                        {
                            SetID = newWorkout.id,
                            attributeID = weekAttributes.Id,
                        };
                        await _databaseContext.AttributeSets.AddAsync(attributeSet);
                    }

                    // Save all changes at once for better performance
                    await _databaseContext.SaveChangesAsync();
                }

                // Redirect to the templates page
                return RedirectToAction("Templates", "TemplateManagement");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving program: {Message}", ex.Message);
                // Return to the program creation page with an error message
                return RedirectToAction("newProgramCreation", new { name, weeks, phase, error = "Failed to save program. Please try again." });
            }
        }


        [HttpGet]
        public async Task<IActionResult> WarmAndMobil(Programme program) //Programme program: The method takes a Programme object as a parameter, which represents the program for which we are fetching data.
        {
            //This line fetches all exercises from the database asynchronously and stores them in the excises variable.
            var excises = await _databaseContext.Excercises.ToListAsync();
            //This line fetches all ProgramAttributes associated with the given program's ID and stores them in the attributes variable.
            var attributes = await _databaseContext.ProgramAttributes.Where(x => x.ProgramID == program.id).ToListAsync();

            int[] IDs = attributes.Select(x => x.Id).ToArray(); //This line extracts the IDs of the fetched ProgramAttributes and stores them in an array called IDs.

            List<AttributeSet> AttributeSets = new List<AttributeSet>(); //This block initializes an empty list of PAExercise objects.
            foreach (var number in IDs) //It then iterates over each SetID, fetching the corresponding PAExercises from the database and adding them to the list.
            {
                AttributeSets.AddRange(await _databaseContext.AttributeSets.Where(x => x.attributeID == number).ToListAsync());
            }
            ;

            int[] SetID = AttributeSets.Select(x => x.SetID).ToArray(); //This line extracts the SetID values from the AttributeSets list and stores them in an array called SetID.

            List<PAExercise> SetSets = new List<PAExercise>();//This block initializes an empty list of PAExercise objects.
            foreach (var ID in SetID)//It then iterates over each SetID, fetching the corresponding PAExercises from the database and adding them to the list.
            {
                SetSets.AddRange(await _databaseContext.PAExercises.Where(x => x.id == ID).ToListAsync());
            }

            //This block checks if the SetSets list is empty.
            if (SetSets.Count == 0) //If it is, it fetches the ProgramAttributes again and redirects the user to the EditSets action, passing the program object as a parameter.
            {
                var attributelist = await _databaseContext.ProgramAttributes.Where(x => x.ProgramID == program.id).ToListAsync();
                return RedirectToAction(nameof(EditSets), program);
            }

            return View();
        }

        [HttpPatch]
        public async Task<IActionResult> WarmAndMobil([FromBody] Programme program, [FromQuery] ProgramAttributesVM vm, [FromQuery] SetsVM setsVM)
        {
            // Fetch all ProgramAttributes associated with the given program's ID from the database asynchronously.
            var attributes = await _databaseContext.ProgramAttributes.Where(x => x.ProgramID == program.id).ToListAsync();

            // This line seems redundant as it filters the same attributes by ProgramID again.
            var attributeid = attributes.Where(x => x.ProgramID == program.id);

            int result = 0;
            // Iterate over each attribute and sum up their IDs.
            foreach (var item in attributes)
            {
                result += item.Id;
            }

            // Redirect to the Excercises action.
            return RedirectToAction(nameof(Excercises));
        }

        //Understand
        [HttpGet]
        public async Task<IActionResult> EditSets(Programme program)
        {
            //This line fetches all ProgramAttributes associated with the given program's ID from the database asynchronously and stores them in the attributes variable.
            var attributes = await _databaseContext.ProgramAttributes.Where(x => x.ProgramID == program.id).ToListAsync();
            //This line fetches all exercises from the database asynchronously and stores them in the exercises variable.
            var exercises = await _databaseContext.Excercises.ToListAsync();
            //This line fetches all S_RSchema entries where SetWeek is equal to 1 from the database asynchronously and stores them in the schema variable.
            var schema = await _databaseContext.SRSchema.Where(x => x.SetWeek == 1).ToListAsync();
            var tuple = new Tuple<List<Excercises>, List<S_RSchema>, Programme>(new List<Excercises>(exercises), new List<S_RSchema>(schema), program); //The tuple is used to pass multiple objects to the view.

            return View(tuple);
        }
        //Understand
        [HttpPost]
        public async Task<IActionResult> EditSets([FromForm] List<SetVM> sets, [FromForm] DisplayPageVM vm)
        {
            //These lines extract the ProgramID, Name, and AddNote from the first SetVM object in the sets list.
            int id = sets[0].ProgramID;
            string name = sets[0].Name;
            string note = sets[0].AddNote;
            var PAexerciselist = new List<PAExercise>();
            foreach (var set in sets) //This line starts a loop that iterates over each SetVM object in the sets list.
            {
                //This line fetches all ProgramAttributes associated with the given program's ID from the database asynchronously and stores them in the attributes variable.
                var attributes = await _databaseContext.ProgramAttributes.Where(x => x.ProgramID == id).ToListAsync();
                //This line fetches the S_RSchema entry with the given SchemaID from the database asynchronously and stores it in the schema variable.
                var schema = await _databaseContext.SRSchema.Where(x => x.id == set.SchemaID).FirstOrDefaultAsync();
                //This line fetches all S_RSchema entries with the same name as the fetched schema and stores them in the schemas list.
                var schemas = await _databaseContext.SRSchema.Where(x => x.name == schema.name).ToListAsync();

                foreach (var attribute in attributes) //This line starts a loop that iterates over each ProgramAttributes object in the attributes list.
                {
                    //This line finds the S_RSchema entry in the schemas list that matches the WeekNumber of the current attribute.
                    var Schema = schemas.Where(x => x.SetWeek == attribute.WeekNumber).FirstOrDefault();

                    // Create a new set if it doesn't exist
                    var newSet = new PAExercise
                    {
                        name = name,
                        AddNote = note,
                        ProgramID = id,

                    };
                    await _databaseContext.PAExercises.AddAsync(newSet);
                    await _databaseContext.SaveChangesAsync();
                    PAexerciselist.Add(newSet);

                    //This block fetches all SchemaAttributes associated with the current Schema and iterates over them.
                    var schemaAttributes = await _databaseContext.SchemaAttributes.Where(x => x.SchemaID == Schema.id).ToListAsync();
                    //For each SchemaAttributes object, it fetches the corresponding exercise name and creates a new ExerciseSetAttrib object with the relevant data.
                    foreach (var attr in schemaAttributes)
                    {
                        var Name = await _databaseContext.Excercises.Where(x => x.Id == set.Exercise).FirstOrDefaultAsync();

                        var ExerciseSet = new ExerciseSetAttrib
                        {
                            PAExerciseID = newSet.id,
                            SchemaID = Schema.id,
                            ExcerciseID = set.Exercise,
                            reps = attr.Reps,
                            RPE = attr.RPE,
                            RM = attr.PercentRM,
                            Velocity = attr.Vel,
                            ExcerciseName = Name.Name,
                            PerWeek = Schema.SetWeek

                        };
                        //It then adds the new ExerciseSetAttrib to the database and saves the changes asynchronously.
                        await _databaseContext.SetExcercises.AddAsync(ExerciseSet);
                        await _databaseContext.SaveChangesAsync();

                    }

                    //This block creates a new AttributeSet object linking the new PAExercise and the current ProgramAttributes.
                    var attributeSet = new AttributeSet
                    {
                        SetID = newSet.id,
                        attributeID = attribute.Id,
                    };
                    //It then adds the new AttributeSet to the database and saves the changes asynchronously.
                    await _databaseContext.AttributeSets.AddAsync(attributeSet);
                    await _databaseContext.SaveChangesAsync();

                }
            }

            return RedirectToAction("Templates", "TemplateManagement");
        }

        [HttpGet]
        public async Task<IActionResult> EditProgram(int id)
        {
            var program = await _databaseContext.Programs.FirstOrDefaultAsync(p => p.id == id);
            if (program == null)
            {
                _logger.LogWarning("Program not found with id: {Id}", id);
                return NotFound();
            }

            var paExercise = await _databaseContext.PAExercises.Where(pa => pa.ProgramID == id).ToListAsync();
            PAExercise pA = null;
            foreach (var ex in paExercise)
            {
                if (ex.AddNote != null)
                {
                    pA = ex;
                }

            }

            var programAttributes = await _databaseContext.ProgramAttributes.Where(pA => pA.ProgramID == id).ToListAsync();

            var allExercises = await _databaseContext.Excercises.ToListAsync();

            var schemas = await _databaseContext.SRSchema.ToListAsync();
            var PAset = await _databaseContext.AttributeSets
                .Include(a => a.Sets)
                .Where(x => x.attributeID == programAttributes.FirstOrDefault().Id)
                .ToListAsync(); // Getting all the data of the weeks related to the AttributeSets

            var ExerciseSet = new List<ExerciseSetAttrib>();
            foreach (var set in PAset) //For loop create
            {
                var ExerciseSets = await _databaseContext.SetExcercises
                    .Include(x => x.S_RSchema) // Include S_RSchema data
                    .Where(x => x.PAExerciseID == set.SetID)
                    .ToListAsync();
                foreach (var Set in ExerciseSets)
                {
                    ExerciseSet.Add(Set);
                }
                ;
            };

            var exerciseSetAttribs = new List<ExerciseSetAttrib>();
            foreach (var pa in paExercise)
            {
                var exerciseSets = await _databaseContext.SetExcercises
                    .Where(es => es.PAExerciseID == pa.id)
                    .ToListAsync();
                exerciseSetAttribs.AddRange(exerciseSets);
            }

            List<Excercises> exerciseIds = new List<Excercises>();
            foreach (var exerciseSet in exerciseSetAttribs)
            {
                var ex = await _databaseContext.Excercises.Where(x => x.Id == exerciseSet.ExcerciseID).FirstOrDefaultAsync();

                if (!exerciseIds.Contains(ex))
                {
                    exerciseIds.Add(ex);
                }
            }

            var sets = await _databaseContext.SetAttributes
                .Include(sa => sa.Excercises)
                .Include(sa => sa.Sets)
                .Where(sa => sa.Sets.ProgramID == program.id)
                .ToListAsync();

            var tuple = new Tuple<List<Programme>, PAExercise, List<ProgramAttributes>, List<ExerciseSetAttrib>, List<Excercises>, List<SetAttributes>, List<S_RSchema>>(
                new List<Programme> { program }, pA,
                programAttributes, exerciseSetAttribs, allExercises, sets, schemas
            );

            return View("EditProgram", tuple);
        }

        // This method handles the POST request to save the edited program.
        [HttpPost]
        public async Task<IActionResult> SaveEditedProgram(editProgramVM model, [FromForm] List<ExerciseSetAttrib> newExercises)
        {
            // Check if the model is null or if it doesn't contain any programs.
            if (model == null || model.Programmes == null || !model.Programmes.Any())
            {
                return BadRequest("Invalid data.");
            }

            // Log what we received to help diagnose
            _logger.LogInformation("Received {Count} existing exercises in model",
                model.ExerciseSetAttribs?.Count ?? 0);
            _logger.LogInformation("Received {Count} new exercises",
                newExercises?.Count ?? 0);

            var itemsToRemove = new List<ExerciseSetAttrib>();
            var ExerciseAttributeList = model.ExerciseSetAttribs ?? new List<ExerciseSetAttrib>();

            // Process existing rows first
            if (ExerciseAttributeList != null)
            {
                foreach (var Set in ExerciseAttributeList)
                {
                    if (!itemsToRemove.Contains(Set))
                    {
                        var existing = await _databaseContext.SetExcercises
                           .AsNoTracking()
                           .Where(x => x.id == Set.id)
                           .FirstOrDefaultAsync();

                        if (existing != null)
                        {
                            // Handle exercise changes
                            if (existing.ExcerciseID != Set.ExcerciseID)
                            {
                                var newlist = await _databaseContext.SetExcercises
                                    .Where(x => x.SchemaID == existing.SchemaID &&
                                          x.ExcerciseID == existing.ExcerciseID &&
                                          x.PerWeek == Set.PerWeek &&
                                          x.PAExerciseID == existing.PAExerciseID)
                                    .ToListAsync();

                                _databaseContext.SetExcercises.RemoveRange(newlist);
                                itemsToRemove.AddRange(newlist);

                                // Get the new exercise name
                                var newExerciseName = await _databaseContext.Excercises
                                    .Where(x => x.Id == Set.ExcerciseID)
                                    .Select(y => y.Name)
                                    .FirstOrDefaultAsync();

                                // Get the schema data to recreate exercise sets
                                var schemaData = await _databaseContext.SchemaAttributes
                                    .Where(x => x.SchemaID == existing.SchemaID)
                                    .ToListAsync();

                                // Create new exercise sets with the selected exercise
                                foreach (var item in schemaData)
                                {
                                    var ExerciseSet = new ExerciseSetAttrib
                                    {
                                        PAExerciseID = existing.PAExerciseID,
                                        SchemaID = existing.SchemaID,
                                        ExcerciseID = Set.ExcerciseID,
                                        reps = item.Reps,
                                        RPE = item.RPE,
                                        RM = item.PercentRM,
                                        Velocity = item.Vel,
                                        ExcerciseName = newExerciseName,
                                        PerWeek = Set.PerWeek
                                    };
                                    await _databaseContext.SetExcercises.AddAsync(ExerciseSet);
                                }
                            }

                            // Handle schema changes 
                            if (existing.SchemaID != Set.SchemaID)
                            {
                                // Your existing schema change code...
                                var newlist = await _databaseContext.SetExcercises
                                    .Where(x => x.SchemaID == existing.SchemaID && x.ExcerciseID == existing.ExcerciseID && x.PerWeek == Set.PerWeek && x.PAExerciseID == existing.PAExerciseID)
                                    .ToListAsync();

                                _databaseContext.SetExcercises.RemoveRange(newlist);
                                var SRS = await _databaseContext.SRSchema.Where(x => x.id == Set.SchemaID).FirstOrDefaultAsync();
                                var correctSchema = await _databaseContext.SRSchema.Where(x => x.name == SRS.name && x.SetWeek == Set.PerWeek).FirstOrDefaultAsync();
                                var SchemaAttr = await _databaseContext.SchemaAttributes.Where(x => x.SchemaID == correctSchema.id).ToListAsync();

                                foreach (var item in SchemaAttr)
                                {
                                    {
                                        var ExerciseSet = new ExerciseSetAttrib
                                        {
                                            PAExerciseID = existing.PAExerciseID,
                                            SchemaID = item.SchemaID,
                                            ExcerciseID = existing.ExcerciseID,
                                            reps = item.Reps,
                                            RPE = item.RPE,
                                            RM = item.PercentRM,
                                            Velocity = item.Vel,
                                            ExcerciseName = await _databaseContext.Excercises.Where(x => x.Id == existing.ExcerciseID).Select(y => y.Name).FirstOrDefaultAsync(),
                                            PerWeek = correctSchema.SetWeek,
                                            S_RSchema = correctSchema,
                                        };
                                        await _databaseContext.SetExcercises.AddAsync(ExerciseSet);
                                    }
                                    itemsToRemove.AddRange(newlist);
                                }
                            }
                        }
                    }
                }

                // Process newly added rows
                if (newExercises != null && newExercises.Any())
                {
                    foreach (var Set in newExercises.Where(s => s.ExcerciseID > 0 && s.SchemaID > 0))
                    {
                        try
                        {
                            // Get the exercise name
                            var exerciseName = await _databaseContext.Excercises
                                .Where(x => x.Id == Set.ExcerciseID)
                                .Select(y => y.Name)
                                .FirstOrDefaultAsync();

                            if (string.IsNullOrEmpty(exerciseName))
                            {
                                _logger.LogWarning("Could not find exercise with ID {ExerciseID}", Set.ExcerciseID);
                                continue;
                            }

                            // Get schema details
                            var schema = await _databaseContext.SRSchema
                                .FirstOrDefaultAsync(x => x.id == Set.SchemaID);

                            if (schema == null)
                            {
                                _logger.LogWarning("Could not find schema with ID {SchemaID}", Set.SchemaID);
                                continue;
                            }

                            // Get the correct schema for the week
                            var correctSchema = await _databaseContext.SRSchema
                                .FirstOrDefaultAsync(x => x.name == schema.name && x.SetWeek == Set.PerWeek);

                            if (correctSchema == null)
                            {
                                _logger.LogWarning("Could not find matching schema for name {SchemaName} and week {Week}",
                                    schema.name, Set.PerWeek);
                                correctSchema = schema; // Fallback to the selected schema
                            }

                            // Get schema attributes
                            var schemaData = await _databaseContext.SchemaAttributes
                                .Where(x => x.SchemaID == correctSchema.id)
                                .ToListAsync();

                            if (schemaData.Count == 0)
                            {
                                _logger.LogWarning("No schema attributes found for schema ID {SchemaID}", correctSchema.id);
                                continue;
                            }

                            // Create exercise sets based on the schema
                            foreach (var item in schemaData)
                            {
                                var ExerciseSet = new ExerciseSetAttrib
                                {
                                    PAExerciseID = Set.PAExerciseID,
                                    SchemaID = correctSchema.id,
                                    ExcerciseID = Set.ExcerciseID,
                                    reps = item.Reps,
                                    RPE = item.RPE,
                                    RM = item.PercentRM,
                                    Velocity = item.Vel,
                                    ExcerciseName = exerciseName,
                                    PerWeek = Set.PerWeek
                                };
                                await _databaseContext.SetExcercises.AddAsync(ExerciseSet);
                                _logger.LogInformation("Added new exercise set for exercise {ExerciseName}, schema {SchemaID}, week {Week}",
                                    exerciseName, correctSchema.id, Set.PerWeek);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error processing new exercise with ID {ExerciseID}", Set.ExcerciseID);
                        }
                    }
                }

                await _databaseContext.SaveChangesAsync();
            }

            // Get the first program from the model.
            var program = model.Programmes.First();
            // Find the existing program in the database by its ID.
            var existingProgram = await _databaseContext.Programs.FirstOrDefaultAsync(p => p.id == program.id);
            // If the program is not found, return a NotFound result.
            if (existingProgram == null)
            {
                return NotFound("Program not found.");
            }

            if (program.Name != existingProgram.Name)
            {
                // Update the name of the existing program.
                existingProgram.Name = program.Name;
                // Mark the existing program as updated in the database context.
                _databaseContext.Programs.Update(existingProgram);
            }

            // Find the existing PAExercise in the database by its ID.
            var existingPAExercise = await _databaseContext.PAExercises.FirstOrDefaultAsync(pa => pa.id == model.PAExercises.id);
            // If the PAExercise is found, update its AddNote property.
            if (existingPAExercise != null && model.PAExercises.AddNote != existingPAExercise.AddNote)
            {
                existingPAExercise.AddNote = model.PAExercises.AddNote;
                // Mark the existing PAExercise as updated in the database context.
                _databaseContext.PAExercises.Update(existingPAExercise);
            }

            // Save all changes to the database.
            await _databaseContext.SaveChangesAsync();

            // Log success message
            _logger.LogInformation("Successfully saved program {ProgramID} with name {ProgramName}",
                existingProgram.id, existingProgram.Name);

            // Redirect to the Templates action in the TemplateManagement controller.
            return RedirectToAction("Templates", "TemplateManagement");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProgramme(int id)
        {
            using var transaction = await _databaseContext.Database.BeginTransactionAsync();
            try
            {
                var email = HttpContext.Request.Cookies["UserEmail"];
                var user = await _databaseContext.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    _logger.LogWarning("User not found with email: {Email}", email);
                    return Json(new { success = false, message = "User not found" });
                }

                var programme = await _databaseContext.Programs.FirstOrDefaultAsync(x => x.id == id);
                if (programme == null)
                {
                    _logger.LogWarning("Programme not found with id: {Id}", id);
                    return Json(new { success = false, message = "Programme not found" });
                }

                var PA = await _databaseContext.ProgramAttributes.Where(x => x.ProgramID == id).ToListAsync();
                var PaExercise = await _databaseContext.PAExercises.Where(x => x.ProgramID == id).ToListAsync();

                foreach (var attr in PA)
                {
                    var attributeSet = await _databaseContext.AttributeSets.Where(x => x.attributeID == attr.Id).ToListAsync();
                    _databaseContext.AttributeSets.RemoveRange(attributeSet);
                }

                foreach (var exercise in PaExercise)
                {
                    var setExercise = await _databaseContext.SetExcercises.Where(x => x.PAExerciseID == exercise.id).ToListAsync();
                    _databaseContext.SetExcercises.RemoveRange(setExercise);

                    var attributeSet = await _databaseContext.AttributeSets.Where(x => x.SetID == exercise.id).ToListAsync();
                    _databaseContext.AttributeSets.RemoveRange(attributeSet);
                }

                _databaseContext.ProgramAttributes.RemoveRange(PA);
                _databaseContext.PAExercises.RemoveRange(PaExercise);
                _databaseContext.Programs.Remove(programme);

                await _databaseContext.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Programme with id: {Id} deleted successfully", id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error deleting programme with id: {Id}", id);
                return Json(new { success = false, message = "Error deleting programme" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSets([FromBody] ProgramAttributes selectedItem)
        {
            var attributes = await _databaseContext.ProgramAttributes.Where(x => x.WeekNumber == selectedItem.WeekNumber).ToListAsync();
            var programme = attributes.Where(x => x.ProgramID == selectedItem.ProgramID).ToList();
            var excercises = await _databaseContext.Excercises.ToListAsync();

            var tuple = new Tuple<List<ProgramAttributes>, List<Excercises>>(new List<ProgramAttributes>(programme), new List<Excercises>(excercises));
            return PartialView("_SetsPartial", tuple);
        }

        [HttpGet]
        public async Task<IActionResult> MyProgrammes()
        {
            var userEmail = HttpContext.Request.Cookies["UserEmail"];
            var user = await _databaseContext.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            var programmes = await _databaseContext.Programs.Where(x => x.UserID == user.Id).ToListAsync();

            return View(programmes);
        }

        // This method handles the GET request to view a specific program by its ID.
        [HttpGet]
        public async Task<IActionResult> ProgrammeView(int id)
        {
            var program = await _databaseContext.Programs.FirstOrDefaultAsync(p => p.id == id);
            if (program == null)
            {
                _logger.LogWarning("Program not found with id: {Id}", id);
                return NotFound();
            }

            var paExercise = await _databaseContext.PAExercises.Where(pa => pa.ProgramID == id).ToListAsync();
            PAExercise pA = null;
            foreach (var ex in paExercise)
            {
                if (ex.AddNote != null)
                {
                    pA = ex;
                }
            }
            var programAttributes = await _databaseContext.ProgramAttributes.Where(pA => pA.ProgramID == id).ToListAsync();

            var exerciseSetAttribs = new List<ExerciseSetAttrib>();
            foreach (var pa in paExercise)
            {
                var exerciseSets = await _databaseContext.SetExcercises
                    .Where(es => es.PAExerciseID == pa.id)
                    .ToListAsync();
                exerciseSetAttribs.AddRange(exerciseSets);
            }

            var allExercises = await _databaseContext.Excercises.ToListAsync();
            
            var exercises = new List<Excercises>();
            foreach (var item in exerciseSetAttribs)
            {
                var checkid = await _databaseContext.Excercises.Where(x => x.Id == item.ExcerciseID).FirstOrDefaultAsync();
                
                if (checkid != null)
                {
                    if (!exercises.Contains(checkid))
                    {
                        
                        exercises.Add(checkid);
                    }
                }
            }

            var sets = await _databaseContext.SetAttributes
                .Include(sa => sa.Excercises)
                .Include(sa => sa.Sets)
                .Where(sa => sa.Sets.ProgramID == program.id)
                .ToListAsync();
            var schemas = await _databaseContext.SRSchema.ToListAsync();

            var tuple = new Tuple<Programme, PAExercise, List<ProgramAttributes>, List<ExerciseSetAttrib>, List<Excercises>, List<SetAttributes>, List<S_RSchema>>(
                 program , pA, programAttributes, exerciseSetAttribs, exercises, sets, schemas
            );

            return View("ProgrammeView", tuple);
        }


        [HttpPost]
        public async Task<IActionResult> DisplayView([FromBody] ProgramAttributes selectedItem)
        {
            var PAset = await _databaseContext.AttributeSets.Where(x => x.attributeID == selectedItem.Id).ToListAsync(); //Getting all the stuff for one week

            var ExerciseSet = new List<ExerciseSetAttrib>();
            foreach (var set in PAset) //For loop create
            {
                var ExerciseSets = await _databaseContext.SetExcercises.Where(x => x.PAExerciseID == set.SetID).ToListAsync();
                foreach (var Set in ExerciseSets)
                {
                    ExerciseSet.Add(Set);
                };
            };

            List<int> exerciseIds = new List<int>();
            foreach (var exerciseSet in ExerciseSet)
            {
                if (!exerciseIds.Contains(exerciseSet.ExcerciseID))
                {
                    exerciseIds.Add(exerciseSet.ExcerciseID);
                }
            }
            var tuple = new Tuple<List<int>, List<ExerciseSetAttrib>>(new List<int>(exerciseIds), new List<ExerciseSetAttrib>(ExerciseSet));
            // Perform your logic here
            // For example, you can fetch related data based on selectedItem.Id

            // Return a partial view with the data
            return PartialView("_OutputPartial", tuple);
        }


        [HttpGet]
        public IActionResult DisplayPage(DisplayPageVM vm)
        {

            // Your logic here
            return View(vm);
        }


        [HttpPost]
        public IActionResult GetExerciseDetails([FromBody] DisplayPageVM viewModel)
        {
            var schema = viewModel.Schemas.Where(x => x.SetWeek == viewModel.SelectedWeek).ToList();

            List<ExerciseSetAttrib> Setex = new List<ExerciseSetAttrib>();
            foreach (var item in schema)
            {

                Setex.AddRange(viewModel.SetExcercises.Where(x => x.SchemaID == item.id).ToList());
            }
            ;

            var exerciseDetails = new DisplayPageVM
            {
                Schemas = schema,
                SetExcercises = Setex,
                AddNote = viewModel.AddNote,
            };

            return PartialView("_ExerciseDetailsPartial", exerciseDetails);
        }

        [HttpGet]
        public IActionResult _ExerciseDetailsPartial()
        {
            return PartialView();
        }

        [HttpGet]
        public IActionResult _OutputPartial()
        {

            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> DisplayOutput([FromBody] ProgramAttributes selectedItem)
        {
            var attributes = await _databaseContext.ProgramAttributes.Where(x => x.WeekNumber == selectedItem.WeekNumber).ToListAsync();
            var programme = attributes.Where(x => x.ProgramID == selectedItem.ProgramID).ToList();
            var attributeset = await _databaseContext.AttributeSets.Where(x => x.attributeID == selectedItem.Id).ToListAsync();

            int[] SetID = attributeset.Select(x => x.SetID).ToArray();

            List<PAExercise> SetSets = new List<PAExercise>();
            foreach (var ID in SetID)
            {
                SetSets.AddRange(await _databaseContext.PAExercises.Where(x => x.id == ID).ToListAsync());
            };

            List<ExerciseSetAttrib> setExcercises = new List<ExerciseSetAttrib>();

            int[] excerciseID = setExcercises.Select(x => x.ExcerciseID).ToArray();
            List<Excercises> excercises = new List<Excercises>();
            foreach (var item in excerciseID)
            {
                excercises.AddRange(await _databaseContext.Excercises.Where(x => x.Id == item).ToListAsync());
            };

            var tuple = new Tuple<List<ProgramAttributes>, List<ExerciseSetAttrib>, List<Excercises>>
                (
                    programme,
                    setExcercises,
                    excercises
                );

            return PartialView(nameof(_OutputPartial), tuple);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhasePlayWeb.Data;
using PhasePlayWeb.Models;
using PhasePlayWeb.Models.Entities;
using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text.Json;

namespace PhasePlayWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExerciseController : Controller
    {

        private readonly ILogger<ExerciseController> _logger;
        private readonly ApplicationDbContext _databaseContext;
        private static readonly Random _random = new Random();
        public ExerciseController(ILogger<ExerciseController> logger, ApplicationDbContext _databaseContext)
        {
            _logger = logger;
            this._databaseContext = _databaseContext;
        }

        private int GenerateRandomId()
        {
            return _random.Next(1, int.MaxValue);
        }
        
        [HttpGet("ExerciseList")]
        public async Task<IActionResult>GetExerciseList()
        {
            // Get all exercises from the database
            var exercises = await _databaseContext.Excercises.ToListAsync();

            var serializedExercises = JsonSerializer.Serialize(exercises);
            // Return the view with the ViewModel
            return Json(serializedExercises);
        }

        [HttpGet]
        public async Task<IActionResult> workOutTemplates(int id)
        {
            var excercises = await _databaseContext.Excercises.ToListAsync();
            // Get only distinct schema names
            var schemas = await _databaseContext.SRSchema
                .GroupBy(s => s.name)
                .Select(g => g.First())
                .ToListAsync();

            // Get all workouts grouped by name
            var workouts = await _databaseContext.Workouts
                .GroupBy(w => w.Name)
                .Select(g => new
                {
                    Name = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var tuple = new Tuple<List<S_RSchema>, List<Excercises>, object>(schemas, excercises, workouts);

            return View("workOutTemplates", tuple);
        }

        [HttpGet]
        public async Task<IActionResult> GetSchemaDetails(int id)
        {
            try
            {
                // Get the schema by ID
                var schema = await _databaseContext.SRSchema.FindAsync(id);
                if (schema == null)
                {
                    return Json(new { success = false, message = "Schema not found" });
                }

                // Get all schemas with the same name (all weeks)
                var allSchemas = await _databaseContext.SRSchema
                    .Where(s => s.name == schema.name)
                    .OrderBy(s => s.SetWeek)
                    .ToListAsync();

                // Create a result object to hold all weeks and their attributes
                var result = new List<object>();

                foreach (var weekSchema in allSchemas)
                {
                    // Get schema attributes for the current week
                    var attributes = await _databaseContext.SchemaAttributes
                        .Where(a => a.SchemaID == weekSchema.id)
                        .ToListAsync();

                    // Add this week's data to the result
                    result.Add(new
                    {
                        weekNumber = weekSchema.SetWeek,
                        schemaId = weekSchema.id,
                        schemaName = weekSchema.name,
                        attributes = attributes
                    });
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching schema details");
                return Json(new { success = false, message = "An error occurred while fetching schema details" });
            }
        }



        [HttpPost]
        public async Task<IActionResult> saveWorkout([FromForm] string name, [FromForm] string type, [FromForm] List<int> exerciseIds, [FromForm] List<int> schemaIds)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return Json(new { success = false, message = "Workout name is required." });
                }

                // Ensure we have matching exercises and schemas
                if (exerciseIds.Count != schemaIds.Count)
                {
                    return Json(new { success = false, message = "Each exercise must have a corresponding schema." });
                }

                // Check if workout with this name already exists
                var existingWorkout = await _databaseContext.Workouts.FirstOrDefaultAsync(w => w.Name == name);
                if (existingWorkout != null)
                {
                    return Json(new { success = false, message = "A workout with this name already exists." });
                }

                // Create workout entries for each exercise-schema pair
                for (int i = 0; i < exerciseIds.Count; i++)
                {
                    var exerciseId = exerciseIds[i];
                    var schemaId = schemaIds[i];

                    // Get exercise and schema details
                    var exercise = await _databaseContext.Excercises.FindAsync(exerciseId);
                    var schema = await _databaseContext.SRSchema.FindAsync(schemaId);

                    if (exercise == null || schema == null)
                    {
                        continue; // Skip invalid entries
                    }

                    // Create workout entry
                    var workout = new Workout
                    {
                        Name = name,
                        ExcerciseID = exerciseId,
                        ExcerciseName = exercise.Name,
                        SchemaID = schemaId,
                        // Default values for other properties
                        PerWeek = 1, // Default value
                        reps = 0 // Default value
                    };

                    await _databaseContext.Workouts.AddAsync(workout);
                }

                await _databaseContext.SaveChangesAsync();
                return Json(new { success = true, message = "Workout saved successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving workout");
                return Json(new { success = false, message = "An error occurred while saving the workout." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetWorkoutDetails(string name)
        {
            try
            {
                // Get all exercises and schemas for this workout
                var workoutExercises = await _databaseContext.Workouts
                    .Where(w => w.Name == name)
                    .Include(w => w.S_RSchema)
                    .ToListAsync();

                if (!workoutExercises.Any())
                {
                    return Json(new { success = false, message = "Workout not found" });
                }

                // Group exercises by workout
                var result = new
                {
                    success = true,
                    name = workoutExercises.First().Name,
                    type = workoutExercises.First().Type,
                    exercises = workoutExercises.Select(w => new
                    {
                        exerciseName = w.ExcerciseName,
                        schemaName = w.S_RSchema?.name ?? "No Schema",
                        schemaId = w.SchemaID
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



        [HttpPost]
        public async Task<IActionResult> DeleteWorkout(string name)
        {
            try
            {
                var workouts = await _databaseContext.Workouts.Where(w => w.Name == name).ToListAsync();
                if (workouts.Any())
                {
                    _databaseContext.Workouts.RemoveRange(workouts);
                    await _databaseContext.SaveChangesAsync();
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Workout not found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting workout");
                return Json(new { success = false, message = "An error occurred while deleting the workout." });
            }
        }


        [HttpGet]
        public async Task<IActionResult> UploadExerciseList()
        {
            var excercises = await _databaseContext.Excercises.ToListAsync();
            return View(excercises);
        }

        [HttpPost]
        public async Task<IActionResult> UploadExercises(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Please upload a valid Excel file.");
                return RedirectToAction("UploadExerciseList");
            }

            if (file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                ModelState.AddModelError(string.Empty, "Invalid file format. Please upload an Excel file.");
                return RedirectToAction("UploadExerciseList");
            }

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    var data = new List<Excercises>();

                    using (var excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream))
                    {
                        var result = excelReader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true
                            }
                        });

                        var worksheet = result.Tables[0];

                        for (int row = 1; row < worksheet.Rows.Count; row++)
                        {
                            var name = worksheet.Rows[row][1]?.ToString() ?? string.Empty;
                            var category = worksheet.Rows[row][2]?.ToString() ?? string.Empty;
                            var bodyPart = worksheet.Rows[row][3]?.ToString() ?? string.Empty;
                            var primaryGroup = worksheet.Rows[row][4]?.ToString() ?? string.Empty;
                            var secondaryGroup1 = worksheet.Rows[row][5]?.ToString() ?? string.Empty;
                            var secondaryGroup2 = worksheet.Rows[row][6]?.ToString() ?? string.Empty;
                            var youtubeURL = worksheet.Rows[row][7]?.ToString() ?? string.Empty;

                          
                            var existingExercise = await _databaseContext.Excercises.FirstOrDefaultAsync(e => e.Name == name);

                            if (existingExercise != null)
                            {
                                existingExercise.Name = name;
                                existingExercise.Category = category;
                                existingExercise.BodyPart = bodyPart;
                                existingExercise.PrimaryGroup = primaryGroup;
                                existingExercise.SecondaryGroup1 = secondaryGroup1;
                                existingExercise.SecondaryGroup2 = secondaryGroup2;
                                existingExercise.YoutubeURL = youtubeURL;
                                _databaseContext.Excercises.Update(existingExercise);
                                await _databaseContext.SaveChangesAsync();
                            }
                            else
                            {
                                var exercise = new Excercises
                                {
                                    Name = name,
                                    Category = category,
                                    BodyPart = bodyPart,
                                    PrimaryGroup = primaryGroup,
                                    SecondaryGroup1 = secondaryGroup1,
                                    SecondaryGroup2 = secondaryGroup2,
                                    YoutubeURL = youtubeURL,
                                };
                                await _databaseContext.Excercises.AddAsync(exercise);
                               
                            }
                        }
                    }

                    await _databaseContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error uploading file: {ex.Message}");
                ModelState.AddModelError(string.Empty, "An error occurred while processing the file. Please try again.");
                return RedirectToAction("UploadExerciseList");
            }

            return RedirectToAction("UploadExerciseList");
        }



        [HttpGet]
        public IActionResult SearchExercises(string query)
        {
            var exercises = _databaseContext.Excercises
                .Where(e => e.Name.Contains(query))
                .ToList();

            return View("UploadExerciseList", exercises);
        }
        public void Delete_Exercise(int id)
        {

            var exercise = _databaseContext.Excercises.Find(id);
            if (exercise != null)
            {
                _databaseContext.Excercises.Remove(exercise);
                _databaseContext.SaveChanges();

            }
        }

        [HttpPost]
        public IActionResult DeleteExercise(int id)
        {
            try
            {
                Delete_Exercise(id);
                return Json(new { success = true });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddExercise(string name, string youtubeURL, string? category, string? bodyPart, string? primaryGroup, string? secondaryGroup1, string? secondaryGroup2)
        {

            var exercise = new Excercises
            {
                Name = name,
                Category = category,
                BodyPart = bodyPart,
                PrimaryGroup = primaryGroup,
                SecondaryGroup1 = secondaryGroup1,
                SecondaryGroup2 = secondaryGroup2,
                YoutubeURL = youtubeURL,
            };

            _databaseContext.Excercises.Add(exercise);
            await _databaseContext.SaveChangesAsync();

            return RedirectToAction("Templates", "TemplateManagement");
        }

        [HttpPost]
        public async Task<IActionResult> EditExercise(int id, string name, string category, string? bodyPart, string? primaryGroup, string? secondaryGroup1, string? secondaryGroup2, string? youtubeURL)
        {
            if (string.IsNullOrEmpty(name))
            {
                ModelState.AddModelError(string.Empty, "Exercise name is required.");
                return RedirectToAction("Templates", "TemplateManagement");
            }

            var exercise = await _databaseContext.Excercises.FindAsync(id);
            if (exercise == null)
            {
                return NotFound();
            }

            exercise.Name = name;
            exercise.Category = category;
            exercise.BodyPart = bodyPart;
            exercise.PrimaryGroup = primaryGroup;
            exercise.SecondaryGroup1 = secondaryGroup1;
            exercise.SecondaryGroup2 = secondaryGroup2;
            exercise.YoutubeURL = youtubeURL;
            _databaseContext.Excercises.Update(exercise);
            await _databaseContext.SaveChangesAsync();

            return RedirectToAction("Templates", "TemplateManagement");
        }

       

        [HttpPost]
        public IActionResult DeleteSchema(int id)
        {
            try
            {
                var schema = _databaseContext.SRSchema.Find(id);
                if (schema != null)
                {
                    _databaseContext.SRSchema.Remove(schema);
                    _databaseContext.SaveChanges();
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Schema not found." });
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error deleting schema.");
                return Json(new { success = false, message = "An error occurred while deleting the schema." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> SchemaView(int id)
        {
            var name = await _databaseContext.SRSchema.FindAsync(id);

            var schema = await _databaseContext.SRSchema.Where(x => x.name == name.name).ToListAsync();


            List<SchemaAttributes> attr = new List<SchemaAttributes>();
            foreach (var item in schema)
            {
                var attributes = await _databaseContext.SchemaAttributes.Where(x => x.SchemaID == item.id).ToListAsync();
                attr.AddRange(attributes);
            }

            var tuple = new Tuple<List<S_RSchema>, List<SchemaAttributes>, string>(new List<S_RSchema>(schema), new List<SchemaAttributes>(attr), name.name);

            return View(tuple);
        }

        [HttpGet]
        public async Task<IActionResult> EditSchema(int id)
        {
            var name = await _databaseContext.SRSchema.FindAsync(id);

            var schema = await _databaseContext.SRSchema.Where(x => x.name == name.name).ToListAsync();
           
            List<SchemaAttributes> attr = new List<SchemaAttributes>();
            foreach (var item in schema)
            {
                var attributes = await _databaseContext.SchemaAttributes.Where(x => x.SchemaID == item.id).ToListAsync();
                attr.AddRange(attributes);
            }
            var Mapping = await _databaseContext.PercentRM_Mapping.ToListAsync();

            var tuple = new Tuple<List<S_RSchema>, List<SchemaAttributes>, List<PercentRM_Mapping>>(new List<S_RSchema>(schema), new List<SchemaAttributes>(attr), new List<PercentRM_Mapping>(Mapping));

            return View("EditSchema", tuple);
        }

        [HttpPost]
        public async Task<IActionResult> EditSchema([FromBody] SchemaViewModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.SchemaName))
            {
                return Json(new { success = false, message = "Schema name is required." });
            }

            var schemaid = model.SchemaAttributes.Select(x => x.SchemaID).ToList();

            foreach (var Schema in schemaid)
            {
                var ss = await _databaseContext.SRSchema.Where(x => x.id == Schema).FirstOrDefaultAsync();
                ss.name = model.SchemaName;
                await _databaseContext.SaveChangesAsync();

            }



            foreach (var attribute in model.SchemaAttributes)
            {
                var attr = await _databaseContext.SchemaAttributes.FindAsync(attribute.id);
                if (attr == null)
                {
                    await _databaseContext.SchemaAttributes.AddAsync(attribute);
                }
                else
                {
                    attr.Reps = attribute.Reps;
                    attr.PercentRM = attribute.PercentRM;
                    attr.RPE = attribute.RPE;
                    attr.Vel = attribute.Vel;
                    attr.SchemaID = attribute.SchemaID;

                    _databaseContext.SchemaAttributes.Update(attr);
                }
            }

            // Update the schema name
            var schema = await _databaseContext.SRSchema.FindAsync(model.SchemaAttributes.First().SchemaID);
            if (schema != null)
            {
                schema.name = model.SchemaName;
                _databaseContext.SRSchema.Update(schema);
            }

            await _databaseContext.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> SaveNewSchema([FromBody] SchemaViewModel model)
        {
            try
            {
                if (model == null || model.SchemaAttributes == null || !model.SchemaAttributes.Any())
                {
                    return Json(new { success = false, message = "No schema attributes provided." });
                }

                if (string.IsNullOrEmpty(model.SchemaName))
                {
                    return Json(new { success = false, message = "Schema name is required." });
                }

                var Schemas = model.SchemaAttributes;
                var name = model.SchemaName;

                // Group by original schema ID to maintain week structure
                var weeks = Schemas
                           .GroupBy(x => x.SchemaID)
                           .ToList();

                // First create all schema records in one transaction
                var newSchemas = new Dictionary<int, S_RSchema>(); // Map original ID to new schema
                foreach (var week in weeks)
                {
                    var originalSchema = await _databaseContext.SRSchema.FindAsync(week.Key);
                    if (originalSchema == null)
                    {
                        return Json(new { success = false, message = $"Schema with ID {week.Key} not found." });
                    }

                    // Check if a schema with this name and week already exists
                    var existingSchema = await _databaseContext.SRSchema
                        .FirstOrDefaultAsync(s => s.name == name && s.SetWeek == originalSchema.SetWeek);

                    if (existingSchema != null)
                    {
                        // Use existing schema instead of creating a new one
                        newSchemas.Add(originalSchema.id, existingSchema);
                    }
                    else
                    {
                        // Only create schema records that have attributes
                        if (week.Any())
                        {
                            var newSchema = new S_RSchema
                            {
                                name = name,
                                SetWeek = originalSchema.SetWeek,
                                RM_Increase = originalSchema.RM_Increase,
                                RPE_Increase = originalSchema.RPE_Increase
                            };

                            _databaseContext.SRSchema.Add(newSchema);
                            await _databaseContext.SaveChangesAsync(); // Save to get the ID
                            newSchemas.Add(originalSchema.id, newSchema);
                        }
                    }
                }

                // Now create all attributes
                foreach (var item in model.SchemaAttributes)
                {
                    // Skip invalid items
                    if (!newSchemas.ContainsKey(item.SchemaID))
                        continue;

                    var newSchema = newSchemas[item.SchemaID];

                    // Validate the data
                    if (item.Reps <= 0)
                    {
                        return Json(new { success = false, message = "Reps must be greater than 0." });
                    }

                    if (item.PercentRM <= 0)
                    {
                        return Json(new { success = false, message = "Percent RM must be greater than 0." });
                    }

                    // Check if an identical attribute already exists for this schema
                    var existingAttr = await _databaseContext.SchemaAttributes
                        .FirstOrDefaultAsync(a =>
                            a.SchemaID == newSchema.id &&
                            a.Reps == item.Reps &&
                            Math.Abs(a.PercentRM - item.PercentRM) < 0.01 &&
                            Math.Abs(a.RPE - item.RPE) < 0.01 &&
                            a.Vel == item.Vel);

                    if (existingAttr == null)
                    {
                        var attribute = new SchemaAttributes
                        {
                            Reps = item.Reps,
                            PercentRM = item.PercentRM,
                            RPE = item.RPE,
                            Vel = item.Vel,
                            SchemaID = newSchema.id
                        };

                        _databaseContext.SchemaAttributes.Add(attribute);
                    }
                }

                await _databaseContext.SaveChangesAsync();
                return Json(new { success = true, message = "Schema saved successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SaveNewSchema");
                return Json(new { success = false, message = "An unexpected error occurred: " + ex.Message });
            }
        }


        [HttpGet]
        public IActionResult CreateSchema()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateSchema(int weeks, string name)
        {
            List<S_RSchema> Schema = new List<S_RSchema>();

             
            for (int i = 1; i < (weeks + 1); i++)
            {
                var schema = new S_RSchema
                {
                    name = name,
                    SetWeek = i
                };
                _databaseContext.SRSchema.Add(schema);
                await _databaseContext.SaveChangesAsync();
                Schema.Add(schema);

            }


            var Mapping = await _databaseContext.PercentRM_Mapping.ToListAsync();
            var tuple = new Tuple<List<S_RSchema>, List<PercentRM_Mapping>>(Schema, Mapping);


            return View(nameof(AddNewSchemaView), tuple);
        }

        [HttpGet]
        public async Task<IActionResult> AddNewSchemaView(List<S_RSchema> Schema)
        {
            var Mapping = await _databaseContext.PercentRM_Mapping.ToListAsync();

            var tuple = new Tuple<List<S_RSchema>, List<PercentRM_Mapping>>(Schema, Mapping);

            return View(tuple);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewSchema([FromBody] SchemaViewModel model)
        {
            if (model == null || model.SchemaAttributes == null || !model.SchemaAttributes.Any())
            {
                return Json(new { success = false, message = "No schema attributes provided." });
            }

            foreach (var attr in model.SchemaAttributes)
            {
                var schema = await _databaseContext.SRSchema.FindAsync(attr.SchemaID);
                if (schema == null)
                {
                    return Json(new { success = false, message = $"Schema with ID {attr.SchemaID} not found." });
                }

                _databaseContext.SchemaAttributes.Add(attr);
            }

            await _databaseContext.SaveChangesAsync();
            return Json(new { success = true });


        }

        // Other methods...
    }

}





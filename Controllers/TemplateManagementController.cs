using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhasePlayWeb.Data;
using PhasePlayWeb.Models;
using PhasePlayWeb.Models.Entities;

namespace PhasePlayWeb.Controllers
{
    public class TemplateManagementController : Controller
    {
        private readonly ILogger<TemplateManagementController> _logger;
        private readonly ApplicationDbContext _databaseContext;
        private readonly UserManager<User> _userManager;
        private static readonly Random _random = new Random();
        public TemplateManagementController(ILogger<TemplateManagementController> logger, ApplicationDbContext _databaseContext, UserManager<User> userManager)
        {
            _logger = logger;
            this._databaseContext = _databaseContext;
            _userManager = userManager;
        }

        


        [HttpGet]
        public async Task<IActionResult> Templates()
        {
            var exercises = await _databaseContext.Excercises.ToListAsync();
            var schema = await _databaseContext.SRSchema.Where(x => x.SetWeek == 1).ToListAsync();
            var userEmail = HttpContext.Request.Cookies["UserEmail"];
            var user = await _userManager.FindByEmailAsync(userEmail);
            var programmes = await _databaseContext.Programs.Where(x => x.UserID == user.Id).ToListAsync();
            var teams = await _databaseContext.Teams.Where(x => x.UserID == user.Id).ToListAsync();
            var role = await _userManager.GetRolesAsync(user);
            if (role.Contains("Staff"))
            {
                var teamIds = await _databaseContext.TeamMembers.Where(x => x.UserId == user.Id).ToListAsync();
                teams = new List<Teams>();
                foreach (var id in teamIds)
                {
                    var teamid = await _databaseContext.Teams.Where(x => x.Id == id.TeamId).FirstOrDefaultAsync();
                    teams.Add(teamid);

                }


            }
            var viewModel = new TemplateManagementVM()
            {
                Exercises = exercises,
                SetsAndRepsSchemas = schema,
                Programmes = programmes,
                Teams = teams
            };


            return View("Templates", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> exerciseTemplate()
        {
            // Get all exercises from the database
            var exercises = await _databaseContext.Excercises.ToListAsync();

            // Create a new TemplateManagementVM object with the exercises
            var viewModel = new TemplateManagementVM()
            {
                Exercises = exercises
            };

            // Return the view with the ViewModel
            return View("exerciseTemplate", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> schemaTemplate()
        {
            // Only get schemas for week 1 to avoid duplicates
            //var schema = await _databaseContext.SRSchema
            //    .Where(x => x.SetWeek == 1)
            //    .ToListAsync();

            // Alternatively, use GroupBy to get distinct schemas by name
            var schema = await _databaseContext.SRSchema
                .GroupBy(s => s.name)
                .Select(g => g.First())
                .ToListAsync();

            var viewModel = new TemplateManagementVM()
            {
                SetsAndRepsSchemas = schema
            };
            return View("schemaTemplate", viewModel);
        }

        private int GenerateRandomId()
        {
            return _random.Next(1, int.MaxValue);
        }

        [HttpGet]
        public async Task<IActionResult> GetGroupsByTeam(int teamId)
        {
            var groups = await _databaseContext.Groups.Where(g => g.TeamID == teamId).ToListAsync();
            return Json(groups);
        }


        [HttpPost]
        public async Task<IActionResult> AddProgramToGroup(int programId, int teamId, int groupId, DateTime startDate, List<string> daysOfWeek)
        {
            var attributes = await _databaseContext.ProgramAttributes.Where(x => x.ProgramID == programId).ToListAsync();
            var ProgramAttributes = new List<List<AttributeSet>>();
            int TotalSessions = 0;
            DateTime StartDate;

            foreach (var attribute in attributes)
            {
                var Paset = await _databaseContext.AttributeSets.Where(x => x.attributeID == attribute.Id).ToListAsync();
                ProgramAttributes.Add(Paset);
            }

            var PAset = new List<ExerciseSetAttrib>();

            foreach (var attributeList in ProgramAttributes)
            {
                foreach (var attribute in attributeList)
                {
                    var ExerciseSets = await _databaseContext.SetExcercises.Where(x => x.PAExerciseID == attribute.SetID).ToListAsync();

                    foreach (var Set in ExerciseSets)
                    {
                        PAset.Add(Set);
                    }
                }
            }

            var groupProgramme = new GroupProgramme
            {
                id = GenerateRandomId(),
                programmeId = programId,
                GroupID = groupId,
                DateAdded = DateTime.Now
            };

            using (var transaction = await _databaseContext.Database.BeginTransactionAsync())
            {
                try
                {
                    await _databaseContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[GroupProgrammes] ON");
                    await _databaseContext.AddAsync(groupProgramme);
                    await _databaseContext.SaveChangesAsync();
                    await _databaseContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[GroupProgrammes] OFF");
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            var program = await _databaseContext.Programs.Where(x => x.id == programId).FirstOrDefaultAsync();

            var Members = new List<User>();
            var userEmail = HttpContext.Request.Cookies["UserEmail"];
            var user = await _databaseContext.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (program == null)
            {
                return NotFound("Program not found");
            }

            var initialStartDate = startDate;
            var days = new List<int>();

            foreach (var day in daysOfWeek)
            {
                switch (day)
                {
                    case "MO":
                        days.Add(0);
                        break;
                    case "TU":
                        days.Add(1);
                        break;
                    case "WE":
                        days.Add(2);
                        break;
                    case "TH":
                        days.Add(3);
                        break;
                    case "FR":
                        days.Add(4);
                        break;
                    case "SA":
                        days.Add(5);
                        break;
                    case "SU":
                        days.Add(6);
                        break;
                }
            }

            for (int i = 1; i <= program.Weeks; i++)
            {
                var weekStartDate = initialStartDate.AddDays(7 * (i - 1));

                foreach (var day in days)
                {
                    var programs = PAset.Where(x => x.PerWeek == i).ToList();
                    var count = programs.Count;
                    int programcounter = 0;

                    // Create events with IsAllDay set to true and use date only
                    var eventDate = weekStartDate.AddDays(day).Date;
                    var newEvent = new Event
                    {
                        StartTime = eventDate,
                        EndTime = eventDate,
                        IsAllDay = true,
                        Subject = program.Name,
                        DateAdded = groupProgramme.DateAdded,
                    };

                    await _databaseContext.Events.AddAsync(newEvent);
                    await _databaseContext.SaveChangesAsync();

                    var GroupSession = new GroupSession
                    {
                        EventID = newEvent.Id,
                        GroupProgrammeID = groupProgramme.id,
                        Event = newEvent
                    };

                    await _databaseContext.GroupSession.AddAsync(GroupSession);
                    await _databaseContext.SaveChangesAsync();

                    foreach (var gram in programs)
                    {
                        StartDate = eventDate;

                        var SessionSet = new SessionSet
                        {
                            GroupSessionID = GroupSession.id,
                            ExerciseSetAttribID = gram.id,
                            PAExerciseID = gram.PAExerciseID,
                            SetNumber = programcounter,
                            PerWeek = gram.PerWeek,
                            DateAssigned = newEvent.StartTime,
                            ProgrammeID = programId,
                            ExerciseSetAttrib = gram
                        };

                        await _databaseContext.SessionSet.AddAsync(SessionSet);

                        programcounter++;
                        if (programcounter == count)
                        {
                            TotalSessions += programcounter;
                            programcounter = 0;
                            break;
                        }
                    }
                }
            }

            TotalSessions = 0;
            await _databaseContext.SaveChangesAsync();

            return RedirectToAction(nameof(Templates));
        }

    }
}

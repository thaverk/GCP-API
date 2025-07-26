using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhasePlayWeb.Data;
using PhasePlayWeb.Models;
using PhasePlayWeb.Models.Entities;
using Syncfusion.EJ2.Linq;
using Syncfusion.EJ2.PivotView;
using System;

namespace PhasePlayWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AthleteViewsController : Controller
    {
        private readonly ApplicationDbContext _databaseContext;
        private readonly UserManager<User> _userManager;
        private static readonly Random _random = new Random();
        public AthleteViewsController(ApplicationDbContext _databaseContext, UserManager<User> userManager)
        {
            this._databaseContext = _databaseContext;
            _userManager = userManager;
        }

        private int GenerateRandomId()
        {
            return _random.Next(1, int.MaxValue);
        }


        [HttpGet]
        public async Task<IActionResult> AthleteDashboard(int page = 1)
        {
            var email = HttpContext.Request.Cookies["UserEmail"];
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // If user is signing in for the first time, redirect to body weight
            if (user.FirstSignIn == true)
            {
                return RedirectToAction(nameof(BodyWeight));
            }

            ViewData["UserName"] = user?.Name;

            // --- Get events for calendar ---
            var sessions = await _databaseContext.Sessions.Where(x => x.UserId == user.Id).ToListAsync();
            var events = new List<Event>();
            var eventId = new List<SessionDate>();

            foreach (var session in sessions)
            {
                var eventid = await _databaseContext.SessionDates.Where(x => x.SessionID == session.id).FirstOrDefaultAsync();
                if (eventid != null)
                {
                    eventId.Add(eventid);
                }
            }

            foreach (var id in eventId)
            {
                var ev = await _databaseContext.Events.Where(x => x.Id == id.EventID).FirstOrDefaultAsync();
                if (ev != null)
                {
                    events.Add(ev);
                }
            }

            // Get the event IDs from groups and sessions
            var mygroups = await _databaseContext.GroupMembers.Where(x => x.UserId == user.Id).Select(y => y.GroupId).ToListAsync();
            var groupProgrammes = await _databaseContext.GroupProgrammes.Where(x => mygroups.Contains(x.GroupID)).ToListAsync();
            var GroupSessions = await _databaseContext.GroupSession.Where(x => groupProgrammes.Select(y => y.id).Contains(x.GroupProgrammeID)).ToListAsync();
            var groupEvents = await _databaseContext.Events.Where(x => GroupSessions.Select(y => y.EventID).Contains(x.Id)).ToListAsync();

            // Add the events from groups to our events list
            events.AddRange(groupEvents);

            // Mark events as completed
            foreach (var ev in events)
            {
                // Check if this event is completed based on SessionHistory
                var sessionDate = await _databaseContext.SessionDates.FirstOrDefaultAsync(sd => sd.EventID == ev.Id);
                if (sessionDate != null)
                {
                    var completedSession = await _databaseContext.SessionHistory
                        .AnyAsync(sh => sh.GroupSessionID == sessionDate.SessionID && sh.UserId == user.Id);

                    // Add IsCompleted property to Event object
                    ev.IsCompleted = completedSession;
                }
                else
                {
                    // Check group sessions
                    var groupSession = await _databaseContext.GroupSession.FirstOrDefaultAsync(gs => gs.EventID == ev.Id);
                    if (groupSession != null)
                    {
                        var completedGroupSession = await _databaseContext.SessionHistory
                            .AnyAsync(sh => sh.GroupSessionID == groupSession.id && sh.UserId == user.Id);

                        ev.IsCompleted = completedGroupSession;
                    }
                    else
                    {
                        ev.IsCompleted = false;
                    }
                }
            }

            // --- Begin MyPrograms functionality ---
            var groups = new List<Groups>();
            foreach (var group in await _databaseContext.GroupMembers.Where(x => x.UserId == user.Id).ToListAsync())
            {
                var gr = await _databaseContext.Groups.Where(x => x.Id == group.GroupId).FirstOrDefaultAsync();
                if (gr != null)
                {
                    groups.Add(gr);
                }
            }

            var Groupprogrammes = new List<List<GroupProgramme>>();
            foreach (var id in groups)
            {
                var a = await _databaseContext.GroupProgrammes.Where(x => x.GroupID == id.Id).ToListAsync();
                if (a != null)
                {
                    Groupprogrammes.Add(a);
                }
            }

            var combinedGroupProgrammes = Groupprogrammes.SelectMany(gp => gp).ToList();
            var PastPrograms = new List<PastProgramView>();
            var MissedSessions = new List<PastProgramView>();
            var programmes = new List<AthleteViewProgrammeView>();
            var sessionHistory = await _databaseContext.SessionHistory.Where(x => x.UserId == user.Id).ToListAsync();

            foreach (var i in combinedGroupProgrammes)
            {
                var paExercise = await _databaseContext.PAExercises
                    .FirstOrDefaultAsync(x => x.ProgramID == i.programmeId);

                var program = await _databaseContext.Programs.Where(x => x.id == i.programmeId).FirstOrDefaultAsync();
                // Rename this variable to avoid the scope conflict
                var groupSessions = await _databaseContext.GroupSession
                    .Where(x => x.GroupProgrammeID == i.id)
                    .ToListAsync();

                if (groupSessions != null)
                {
                    foreach (var sess in groupSessions)
                    {
                        var fortoday = await _databaseContext.Events.Where(x => x.Id == sess.EventID).Select(y => y.StartTime).FirstOrDefaultAsync();
                        var IFToday = fortoday.Date;
                        var history = await _databaseContext.SessionHistory.Where(x => x.GroupSessionID == sess.id & x.UserId == user.Id).FirstOrDefaultAsync();
                        var ExistingMissed = await _databaseContext.MissedSessions.Where(x => x.GroupSessionID == sess.id & x.UserId == user.Id).FirstOrDefaultAsync();

                        if (IFToday < DateTime.Today.Date & history == null & ExistingMissed == null)
                        {
                            var missed = new MissedSessions
                            {
                                UserId = user.Id,
                                GroupProgrammeId = i.id,
                                GroupSessionID = sess.id,
                                ProgramID = i.programmeId,
                            };

                            await _databaseContext.MissedSessions.AddAsync(missed);
                            await _databaseContext.SaveChangesAsync();
                        }
                        else if (history != null & ExistingMissed != null)
                        {
                            _databaseContext.MissedSessions.Remove(ExistingMissed);
                            await _databaseContext.SaveChangesAsync();
                        }
                    }
                }

                var missedSession = await _databaseContext.MissedSessions.Where(x => x.UserId == user.Id).ToListAsync();

                var Sessions = await _databaseContext.GroupSession
                    .Where(x => x.GroupProgrammeID == i.id)
                    .ToListAsync();

                var SessionHistory = await _databaseContext.SessionHistory.Where(x => x.UserId == user.Id & x.GroupProgrammeId == i.id).ToListAsync();

                // Update to use groupSessions instead of sessions
                groupSessions = Sessions.Where(x => !missedSession.Select(ms => ms.GroupSessionID).Contains(x.id) & !sessionHistory.Select(ms => ms.GroupSessionID).Contains(x.id)).OrderBy(y => y.id).ToList();
                var Sessionsets = await _databaseContext.SessionSet.Where(x => groupSessions.Select(y => y.id).Contains(x.GroupSessionID)).ToListAsync();

                Sessionsets = Sessionsets.Where(x => !missedSession.Select(ms => ms.GroupSessionID).Contains(x.GroupSessionID) & !sessionHistory.Select(ms => ms.GroupSessionID).Contains(x.GroupSessionID)).ToList();
                var sessionset = Sessionsets.FirstOrDefault();
                var session = groupSessions.FirstOrDefault();

                var sessionDates = await _databaseContext.SessionHistory
                    .Where(x => x.ProgramID == i.programmeId & x.UserId == user.Id & x.GroupProgrammeId == i.id)
                    .OrderBy(x => x.DateAssigned)
                    .GroupBy(e => new { e.DateAssigned })
                    .Select(g => g.First())
                    .ToListAsync();

                if (sessionDates.Count != 0)
                {
                    foreach (var date in sessionDates)
                    {
                        var pastProgram = new PastProgramView
                        {
                            ProgramName = await _databaseContext.Programs.Where(x => x.id == i.programmeId).Select(x => x.Name).FirstOrDefaultAsync(),
                            DateAssigned = date.DateAssigned,
                            Phase = await _databaseContext.PAExercises.Where(x => x.id == date.PAExerciseID).Select(x => x.name).FirstOrDefaultAsync(),
                            Week = date.PerWeek,
                            DateCompleted = date.DateCompleted,
                            ProgramId = i.programmeId,
                            UserID = user.Id,
                            GroupID = i.id,
                            GroupSessionID = date.GroupSessionID,
                            SessionSetID = date.SessionSetID,
                        };
                        PastPrograms.Add(pastProgram);
                    }
                }

                if (program != null & groupSessions.Count != 0 & session != null & sessionset != null)
                {
                    AthleteViewProgrammeView athleteViewProgrammeView = new AthleteViewProgrammeView
                    {
                        programmeId = i.programmeId,
                        Name = program.Name,
                        Weeks = program.Weeks,
                        OccursOn = program.OccursOn,
                        DateAdded = i.DateAdded,
                        NextSessionDate = await _databaseContext.Events.Where(x => x.Id == session.EventID).Select(s => s.StartTime).FirstOrDefaultAsync(),
                        WeekOfProgramme = sessionset.PerWeek ?? 0,
                        GroupProgrammeId = i.id,
                        PAExerciseName = paExercise.name,
                        GroupSessionID = session.id
                    };
                    programmes.Add(athleteViewProgrammeView);
                }
            }

            var missedSessions = await _databaseContext.MissedSessions
               .Where(x => x.UserId == user.Id)
               .OrderBy(x => x.GroupSessionID)
               .GroupBy(x => x.GroupProgrammeId)
               .Select(g => g.First())
               .ToListAsync();

            if (missedSessions != null)
            {
                foreach (var missed in missedSessions)
                {
                    var groupprogramme = await _databaseContext.GroupProgrammes.Where(x => x.id == missed.GroupProgrammeId).FirstOrDefaultAsync();
                    var session = await _databaseContext.GroupSession.Where(x => x.id == missed.GroupSessionID).FirstOrDefaultAsync();
                    var paExercise = await _databaseContext.PAExercises.Where(x => x.ProgramID == groupprogramme.programmeId).FirstOrDefaultAsync();
                    var SessionSets = await _databaseContext.SessionSet.Where(x => x.GroupSessionID == session.id).ToListAsync();

                    if (SessionSets.Count > 0)
                    {
                        var missedProgram = new PastProgramView
                        {
                            ProgramName = await _databaseContext.Programs.Where(x => x.id == groupprogramme.programmeId).Select(x => x.Name).FirstOrDefaultAsync(),
                            DateAssigned = await _databaseContext.Events.Where(x => x.Id == session.EventID).Select(x => x.StartTime.Date).FirstOrDefaultAsync(),
                            Phase = await _databaseContext.PAExercises.Where(x => x.id == paExercise.id).Select(x => x.name).FirstOrDefaultAsync(),
                            Week = SessionSets[0].PerWeek,
                            DateCompleted = DateTime.Now,
                            ProgramId = groupprogramme.programmeId,
                            UserID = user.Id,
                            GroupID = groupprogramme.GroupID,
                            GroupProgrammeID = groupprogramme.id,
                            GroupSessionID = missed.GroupSessionID
                        };
                        MissedSessions.Add(missedProgram);
                    }
                }
            }

            // Set the current page in ViewData
            ViewData["CurrentPage"] = page;


            var sortedProgrammes = programmes.OrderBy(x => x.NextSessionDate).ToList();

            // Create the tuple that will be passed to the view
            var programsTuple = new Tuple<List<AthleteViewProgrammeView>, List<PastProgramView>, List<SessionHistory>, List<PastProgramView>>(
                sortedProgrammes, PastPrograms, sessionHistory, MissedSessions);

            // Create a model that contains both the events and the programs tuple
            var dashboardModel = (events, programsTuple);

            return View(dashboardModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetExerciseAnalytics(int exerciseId, string period = "week")
        {
            var email = HttpContext.Request.Cookies["UserEmail"];
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Json(new { error = "User not found" });
            }

            // Get exercise data
            var exercise = await _databaseContext.Excercises.FirstOrDefaultAsync(e => e.Id == exerciseId);
            if (exercise == null)
            {
                return Json(new { error = "Exercise not found" });
            }

            // Define the filter date based on the period
            DateTime filterDate = DateTime.Now;
            switch (period?.ToLower())
            {
                case "day":
                    filterDate = DateTime.Now.AddDays(-1);
                    break;
                case "week":
                    filterDate = DateTime.Now.AddDays(-7);
                    break;
                case "month":
                    filterDate = DateTime.Now.AddDays(-30);
                    break;
                default:
                    filterDate = DateTime.Now.AddDays(-7); // Default to week
                    break;
            }

            // Get history data for this exercise with time period filter
            var history = await _databaseContext.SessionHistory
                .Where(x => x.ExcerciseID == exerciseId &&
                           x.UserId == user.Id)
                .OrderBy(x => x.DateCompleted)
                .ToListAsync();

            // Calculate global stats (these are always calculated on all data)
            var allTimeMaxWeight = history.Any() ? history.Max(x => x.WeightUsed) : 0;
            var allTimeAvgWeight = history.Any() ? history.Average(x => x.WeightUsed) : 0;
            var allTimeSessions = history.Count;
            var lastUsed = history.Any() ? history.Max(x => x.DateCompleted)?.ToString("dd/MM/yy") : "N/A";

            // Filter data for chart based on the period
            var filteredHistory = history.Where(x =>
                x.DateCompleted.HasValue &&
                x.DateCompleted >= filterDate).ToList();

            // Create filtered progress data for chart
            var progressData = filteredHistory
                .Where(x => x.DateCompleted.HasValue && x.WeightUsed.HasValue)
                .Select(x => new {
                    x = x.DateCompleted.Value.ToString("yyyy-MM-ddTHH:mm:ss"), // Include time to support day view
                    y = x.WeightUsed.Value
                })
                .ToList();

            // Calculate period-specific stats
            var periodMaxWeight = filteredHistory.Any() ? filteredHistory.Max(x => x.WeightUsed) : 0;
            var periodAvgWeight = filteredHistory.Any() ? filteredHistory.Average(x => x.WeightUsed) : 0;
            var periodSessions = filteredHistory.Count;

            return Json(new
            {
                exerciseName = exercise.Name,
                maxWeight = allTimeMaxWeight?.ToString("0.##") ?? "0",
                avgWeight = allTimeAvgWeight?.ToString("0.##") ?? "0",
                sessions = allTimeSessions,
                lastUsed = lastUsed,
                // Add period-specific stats
                periodMaxWeight = periodMaxWeight?.ToString("0.##") ?? "0",
                periodAvgWeight = periodAvgWeight?.ToString("0.##") ?? "0",
                periodSessions = periodSessions,
                period = period,
                progressData = progressData
            });
        }


        [HttpGet]
        public IActionResult BodyWeight()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EventDetail(int id)
        {
            var email = HttpContext.Request.Cookies["UserEmail"];
            var user = await _userManager.FindByEmailAsync(email);

            // Find the event by id
            var eventDetails = await _databaseContext.Events.FirstOrDefaultAsync(e => e.Id == id);
            if (eventDetails == null)
            {
                return NotFound();
            }

            // Find the session date that links to this event
            var sessionDate = await _databaseContext.SessionDates.FirstOrDefaultAsync(sd => sd.EventID == id);
            if (sessionDate == null)
            {
                // Check for group sessions linked to this event
                var groupSession = await _databaseContext.GroupSession.FirstOrDefaultAsync(gs => gs.EventID == id);
                if (groupSession == null)
                {
                    return RedirectToAction("AthleteDashboard");
                }

                // Find the group program details
                var groupProgram = await _databaseContext.GroupProgrammes.FirstOrDefaultAsync(gp => gp.id == groupSession.GroupProgrammeID);
                if (groupProgram == null)
                {
                    return RedirectToAction("AthleteDashboard");
                }

                // Redirect to ProgramView with the required parameters
                return RedirectToAction("ProgramView", new
                {
                    programid = groupProgram.programmeId,
                    GroupProgramID = groupProgram.id,
                    GroupSessionID = groupSession.id,
                    IscatchUp = false
                });
            }

            // Handle regular sessions if not a group session
            var session = await _databaseContext.Sessions.FirstOrDefaultAsync(s => s.id == sessionDate.SessionID);
            if (session == null)
            {
                return RedirectToAction("AthleteDashboard");
            }

            // Redirect to appropriate view with session details
            return RedirectToAction("ProgramView", new
            {
                programid = session.ProgramID,
                GroupProgramID = session.GroupProgrammeId,
                GroupSessionID = sessionDate.SessionID,
                IscatchUp = false
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBodyWeight(float bodyWeight, float height, string gender)
        {
            var email = HttpContext.Request.Cookies["UserEmail"];
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                user.BodyWeight = bodyWeight;
                user.Height = height;
                user.Gender = gender;
                user.FirstSignIn = false;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(AthleteDashboard));

                }
            }
            return BadRequest("Failed to update body weight.");
        }

        private async Task<IActionResult> ProgramCheck()
        {
            var email = HttpContext.Request.Cookies["UserEmail"];
            if (email == null)
            {
                // Log the error or handle it appropriately
                return View("Error", new ErrorViewModel { RequestId = "Email not found in cookies" });
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Log the error or handle it appropriately
                return View("Error", new ErrorViewModel { RequestId = "User not found" });
            }

            DateTime DateAdded;
            var groupid = await _databaseContext.GroupMembers.Where(x => x.UserId == user.Id).ToListAsync();
            var groups = new List<Groups>();

            foreach (var group in groupid)
            {
                var gr = await _databaseContext.Groups.Where(x => x.Id == group.GroupId).FirstOrDefaultAsync();
                if (gr != null)
                {
                    groups.Add(gr);
                }
            }

            var Groupprogrammes = new List<List<GroupProgramme>>();
            foreach (var id in groups)
            {
                var a = await _databaseContext.GroupProgrammes.Where(x => x.GroupID == id.Id).ToListAsync();
                if (a != null)
                {
                    Groupprogrammes.Add(a);

                }
            }
            var combinedGroupProgrammes = Groupprogrammes.SelectMany(gp => gp).ToList();


            var programmes = new List<AthleteViewProgrammeView>();
            foreach (var i in combinedGroupProgrammes)
            {
                var program = await _databaseContext.Programs.Where(x => x.id == i.programmeId).FirstOrDefaultAsync();

                var sessions = await _databaseContext.Sessions
                .Where(x => x.ProgramID == i.programmeId & x.UserId == user.Id & x.GroupProgrammeId == i.id)
                .OrderBy(x => x.DateAssigned)
                .ToListAsync();
                var session = sessions.FirstOrDefault();





            }

            return View();

        }

        [HttpGet]
        public async Task<IActionResult> ProgramView(int programid, int GroupProgramID, int GroupSessionID, bool IscatchUp)
        {
            var email = HttpContext.Request.Cookies["UserEmail"];
            var user = await _userManager.FindByEmailAsync(email);

            // Get the program name for breadcrumbs
            var program = await _databaseContext.Programs.FirstOrDefaultAsync(p => p.id == programid);
            if (program != null)
            {
                ViewData["ProgramName"] = program.Name;
            }

            if (IscatchUp == false)
            {
                var session = await _databaseContext.GroupSession
                        .Where(x => x.id == GroupSessionID)
                        .OrderBy(s => s.EventID)
                        .FirstOrDefaultAsync();

                var sessionSets = await _databaseContext.SessionSet.Where(x => x.GroupSessionID == GroupSessionID & x.DateAssigned.Date == DateTime.Now.Date).ToListAsync();

                if (sessionSets.Count != 0)
                {
                    var paExercise = await _databaseContext.PAExercises.Where(x => x.ProgramID == programid).FirstOrDefaultAsync();
                    var setExerciseSetAttrib = sessionSets.Select(x => x.ExerciseSetAttribID).ToList();
                    var attributes = await _databaseContext.SetExcercises.Where(x => setExerciseSetAttrib.Contains(x.id)).ToListAsync();
                    var setExercises = attributes.Select(x => x.ExcerciseID).ToList();

                    List<int> exerciseIds = new List<int>();
                    foreach (var exerciseSet in setExercises)
                    {
                        if (!exerciseIds.Contains(exerciseSet))
                        {
                            exerciseIds.Add(exerciseSet);
                        }
                    }

                    var FirsdaySession = new List<Session>();

                    foreach (var sessionSet in sessionSets)
                    {
                        var exercise = await _databaseContext.SetExcercises.Where(x => x.id == sessionSet.ExerciseSetAttribID).Select(y => y.ExcerciseID).FirstOrDefaultAsync();

                        // Initialize values that will be calculated only if history exists
                        double? HighestWeight = null;
                        double? percentage = null;
                        double? e1RM = null;
                        double? recommendedweight = null;

                        // Check if there's any history for this exercise
                        var hasHistory = await _databaseContext.SessionHistory
                            .AnyAsync(x => x.UserId == user.Id && x.ExcerciseID == exercise && x.WeightUsed.HasValue);

                        if (hasHistory)
                        {
                            // Only get max weight if history exists
                            HighestWeight = await _databaseContext.SessionHistory
                                .Where(x => x.UserId == user.Id && x.ExcerciseID == exercise && x.WeightUsed.HasValue)
                                .MaxAsync(x => x.WeightUsed);

                            // Calculate metrics based on history
                            percentage = HighestWeight * sessionSet.ExerciseSetAttrib.reps * 0.033;
                            e1RM = percentage * HighestWeight;
                            recommendedweight = sessionSet.ExerciseSetAttrib.RM * HighestWeight / 100;
                        }
                        else
                        {
                            // Default values for new exercises with no history
                            HighestWeight = 0;
                            e1RM = 0;
                            recommendedweight = 0;
                        }

                        var ses = new Session
                        {
                            ExcerciseID = await _databaseContext.SetExcercises.Where(x => x.id == sessionSet.ExerciseSetAttribID).Select(y => y.ExcerciseID).FirstOrDefaultAsync(),
                            ExcerciseName = await _databaseContext.SetExcercises.Where(x => x.id == sessionSet.ExerciseSetAttribID).Select(y => y.ExcerciseName).FirstOrDefaultAsync(),
                            SchemaID = sessionSet.ExerciseSetAttrib.SchemaID,
                            PerWeek = sessionSet.PerWeek,
                            reps = sessionSet.ExerciseSetAttrib.reps,
                            RM = sessionSet.ExerciseSetAttrib.RM,
                            RPE = sessionSet.ExerciseSetAttrib.RPE,
                            Velocity = sessionSet.ExerciseSetAttrib.Velocity,
                            PAExerciseID = sessionSet.ExerciseSetAttrib.PAExerciseID,
                            DateAssigned = await _databaseContext.Events.Where(x => x.Id == session.EventID).Select(y => y.StartTime).FirstOrDefaultAsync(),
                            ProgramID = programid,
                            Completed = false,
                            GroupProgrammeId = GroupProgramID,
                            UserId = user.Id,
                            SessionSetID = sessionSet.id,
                            RecommendedWeight = recommendedweight,
                            e1RM = e1RM
                        };
                        FirsdaySession.Add(ses);
                    }

                    var tuple = new Tuple<List<Session>, List<int>, int, Session, PAExercise>(FirsdaySession, exerciseIds, GroupSessionID, FirsdaySession.FirstOrDefault(), paExercise);

                    return View(tuple);
                }
                else
                {
                    return View();
                }
            }
            else
            {
                var session = await _databaseContext.GroupSession
                        .Where(x => x.id == GroupSessionID)
                        .FirstOrDefaultAsync();

                var date = await _databaseContext.Events.Where(x => x.Id == session.EventID).Select(y => y.StartTime).FirstOrDefaultAsync();
                var sessionSets = await _databaseContext.SessionSet.Where(x => x.GroupSessionID == GroupSessionID).ToListAsync();

                var paExercise = await _databaseContext.PAExercises.Where(x => x.ProgramID == programid).FirstOrDefaultAsync();
                var setExerciseSetAttrib = sessionSets.Select(x => x.ExerciseSetAttribID).ToList();
                var attributes = await _databaseContext.SetExcercises.Where(x => setExerciseSetAttrib.Contains(x.id)).ToListAsync();
                var setExercises = attributes.Select(x => x.ExcerciseID).ToList();

                List<int> exerciseIds = new List<int>();
                foreach (var exerciseSet in setExercises)
                {
                    if (!exerciseIds.Contains(exerciseSet))
                    {
                        exerciseIds.Add(exerciseSet);
                    }
                }

                var FirsdaySession = new List<Session>();
                foreach (var sessionSet in sessionSets)
                {
                    var exercise = await _databaseContext.SetExcercises.Where(x => x.id == sessionSet.ExerciseSetAttribID).Select(y => y.ExcerciseID).FirstOrDefaultAsync();

                    // Initialize values that will be calculated only if history exists
                    double? HighestWeight = null;
                    double? percentage = null;
                    double? e1RM = null;
                    double? recommendedweight = null;

                    // Check if there's any history for this exercise
                    var hasHistory = await _databaseContext.SessionHistory
                        .AnyAsync(x => x.UserId == user.Id && x.ExcerciseID == exercise && x.WeightUsed.HasValue);

                    if (hasHistory)
                    {
                        // Only get max weight if history exists
                        HighestWeight = await _databaseContext.SessionHistory
                            .Where(x => x.UserId == user.Id && x.ExcerciseID == exercise && x.WeightUsed.HasValue)
                            .MaxAsync(x => x.WeightUsed);

                        // Calculate metrics based on history
                        percentage = HighestWeight * sessionSet.ExerciseSetAttrib.reps * 0.033;
                        e1RM = percentage * HighestWeight;
                        recommendedweight = sessionSet.ExerciseSetAttrib.RM * HighestWeight / 100;
                    }
                    else
                    {
                        // Default values for new exercises with no history
                        HighestWeight = 0;
                        e1RM = 0;
                        recommendedweight = 0;
                    }

                    var ses = new Session
                    {
                        ExcerciseID = await _databaseContext.SetExcercises.Where(x => x.id == sessionSet.ExerciseSetAttribID).Select(y => y.ExcerciseID).FirstOrDefaultAsync(),
                        ExcerciseName = await _databaseContext.SetExcercises.Where(x => x.id == sessionSet.ExerciseSetAttribID).Select(y => y.ExcerciseName).FirstOrDefaultAsync(),
                        SchemaID = sessionSet.ExerciseSetAttrib.SchemaID,
                        PerWeek = sessionSet.PerWeek,
                        reps = sessionSet.ExerciseSetAttrib.reps,
                        RM = sessionSet.ExerciseSetAttrib.RM,
                        RPE = sessionSet.ExerciseSetAttrib.RPE,
                        Velocity = sessionSet.ExerciseSetAttrib.Velocity,
                        PAExerciseID = sessionSet.ExerciseSetAttrib.PAExerciseID,
                        DateAssigned = await _databaseContext.Events.Where(x => x.Id == session.EventID).Select(y => y.StartTime).FirstOrDefaultAsync(),
                        ProgramID = programid,
                        Completed = false,
                        GroupProgrammeId = GroupProgramID,
                        UserId = user.Id,
                        SessionSetID = sessionSet.id,
                        RecommendedWeight = recommendedweight,
                        e1RM = e1RM
                    };
                    FirsdaySession.Add(ses);
                }

                var tuple = new Tuple<List<Session>, List<int>, int, Session, PAExercise>(FirsdaySession, exerciseIds, GroupSessionID, FirsdaySession.FirstOrDefault(), paExercise);

                return View(tuple);
            }
        }



        [HttpPost]
        public async Task<IActionResult> SessionUpdate(List<List<Session>> Sessions)
        {
            var sessions = Sessions.SelectMany(x => x).ToList();
            foreach (var session in sessions)
            {
                var history = new SessionHistory
                {
                    UserId = session.UserId,
                    ExcerciseID = session.ExcerciseID,
                    ExcerciseName = session.ExcerciseName,
                    SchemaID = session.SchemaID,
                    PerWeek = session.PerWeek,
                    reps = session.reps,
                    RM = session.RM,
                    RPE = session.RPE,
                    Velocity = session.Velocity,
                    PAExerciseID = session.PAExerciseID,
                    WeightUsed = session.WeightUsed,
                    DateAssigned = session.DateAssigned,
                    DateCompleted = DateTime.Now,
                    ProgramID = session.ProgramID,
                    Completed = session.Completed,
                    Reason = session.Reason,
                    GroupProgrammeId = session.GroupProgrammeId,
                    GroupSessionID = session.GroupSessionID,
                    SessionSetID = session.SessionSetID,
                    RecommendedWeight = session.RecommendedWeight,
                    id = GenerateRandomId()
                };

                using (var transaction = await _databaseContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        await _databaseContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[SessionHistory] ON");
                        await _databaseContext.SessionHistory.AddAsync(history);
                        await _databaseContext.SaveChangesAsync();
                        await _databaseContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[SessionHistory] OFF");
                        await transaction.CommitAsync();
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }

            return RedirectToAction(nameof(AthleteDashboard));
        }

        public async Task HandleProgramSessions(int programid)
        {
            bool isCompleted = false;
            var sessions = await _databaseContext.Sessions
                .Where(x => x.ProgramID == programid)
                .OrderBy(x => x.DateAssigned)
                .ToListAsync();

            var sessionDates = await _databaseContext.SessionDates
                .Where(x => x.Completed == false)
                .ToListAsync();

            var filteredSessions = sessions
                .Where(x => sessionDates.Any(y => y.SessionID == x.id))
                .ToList();

            var firstDate = filteredSessions.FirstOrDefault()?.DateAssigned;

            var firstDaySessions = filteredSessions
                .Where(s => s.DateAssigned.Date == firstDate?.Date)
                .OrderBy(s => s.id)
                .ToList();

            var (group1, group2) = DivideListEqual(firstDaySessions);

            var duplicateSessions = group2.GroupBy(x => x.id).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (duplicateSessions.Any())
            {
                foreach (var session in group2)
                {
                    var relatedSessionDates = await _databaseContext.SessionDates.Where(sd => sd.SessionID == session.id).FirstAsync();
                    var ev = _databaseContext.Events.Where(x => x.Id == relatedSessionDates.EventID);

                    _databaseContext.Events.RemoveRange(ev);
                    _databaseContext.SessionDates.Remove(relatedSessionDates);

                    // Add more related entities if needed
                }

                // Delete the sessions in group2
                _databaseContext.Sessions.RemoveRange(group2);
                await _databaseContext.SaveChangesAsync();
                Console.WriteLine("Duplicate sessions found in group2.");
                isCompleted = true;
            }


        }


        static (List<Session> group1, List<Session> group2) DivideListEqual(List<Session> lst)
        {
            int midIndex = (lst.Count + 1) / 2;
            List<Session> group1 = lst.GetRange(0, midIndex);
            List<Session> group2 = lst.GetRange(midIndex, lst.Count - midIndex);
            return (group1, group2);
        }
        [HttpGet]
        public IActionResult AthleteProfile()
        {
            return View();
        }

        //[HttpGet]
        //public async Task<IActionResult> MissedProgramView(int programid, int GroupProgramID)
        //{
        //    var email = HttpContext.Request.Cookies["UserEmail"];
        //    var user = await _userManager.FindByEmailAsync(email);
        //    var today = DateTime.Now.Date;

        //    var sessions = await _databaseContext.MissedSessions
        //        .Where(x => x.ProgramID == programid & x.GroupProgrammeId == GroupProgramID & x.UserId == user.Id)
        //        .OrderBy(s => s.id)
        //        .ToListAsync();

        //    List<int> exerciseIds = new List<int>();
        //    foreach (var exerciseSet in sessions)
        //    {
        //        if (!exerciseIds.Contains(exerciseSet.ExcerciseID))
        //        {
        //            exerciseIds.Add(exerciseSet.ExcerciseID);
        //        }
        //    }

        //    var firstDate = sessions.FirstOrDefault();
        //    var paExercise = await _databaseContext.PAExercises
        //        .FirstOrDefaultAsync(x => x.id == firstDate.PAExerciseID);
        //    var tuple = new Tuple<List<MissedSessions>, List<int>, MissedSessions, PAExercise>(sessions, exerciseIds, firstDate, paExercise);

        //    return View(tuple);
        //}
    }
}


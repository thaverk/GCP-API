using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhasePlayWeb.Data;
using PhasePlayWeb.Models;
using PhasePlayWeb.Models.Entities;

namespace PhasePlayWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProgressDashboardController : Controller
    {
        private readonly ILogger<ProgressDashboardController> _logger;
        private readonly ApplicationDbContext _databaseContext;
        private readonly UserManager<User> _userManager;


        public ProgressDashboardController(ILogger<ProgressDashboardController> logger, ApplicationDbContext databaseContext, UserManager<User> userManager)

        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _databaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
            _userManager = userManager;
        }

        public float? HighestAverageWeightUsed { get; set; }
        public float? HighestWeightUsed { get; set; }

        [HttpGet]
        public async Task<JsonResult> GetProgressionData(
   int? teamId, int? groupId, string? athleteId, int? exerciseId, DateTime? startDate, DateTime? endDate)
        {
            var query = _databaseContext.SessionHistory.AsQueryable();

            // Define a variable to hold athlete-specific data
            IQueryable<SessionHistory> athleteQuery = null;

            // Filter by team
            if (teamId.HasValue)
            {
                var teamMemberIds = await _databaseContext.TeamMembers
                    .Where(tm => tm.TeamId == teamId.Value)
                    .Select(tm => tm.UserId)
                    .ToListAsync();
                query = query.Where(sh => teamMemberIds.Contains(sh.UserId));
            }

            // Filter by group
            if (groupId.HasValue)
            {
                var groupMemberIds = await _databaseContext.GroupMembers
                    .Where(gm => gm.GroupId == groupId.Value)
                    .Select(gm => gm.UserId)
                    .ToListAsync();
                query = query.Where(sh => groupMemberIds.Contains(sh.UserId));
            }

            // Save the athlete query separately, but don't filter the main query by athlete
            if (!string.IsNullOrEmpty(athleteId))
            {
                athleteQuery = query.Where(sh => sh.UserId == athleteId);
            }

            // Filter by date range for both queries
            if (startDate.HasValue)
            {
                query = query.Where(sh => sh.DateCompleted >= startDate.Value);
                if (athleteQuery != null)
                    athleteQuery = athleteQuery.Where(sh => sh.DateCompleted >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(sh => sh.DateCompleted <= endDate.Value);
                if (athleteQuery != null)
                    athleteQuery = athleteQuery.Where(sh => sh.DateCompleted <= endDate.Value);
            }

            // Filter by exercise for both queries
            if (exerciseId.HasValue)
            {
                query = query.Where(sh => sh.ExcerciseID == exerciseId.Value);
                if (athleteQuery != null)
                    athleteQuery = athleteQuery.Where(sh => sh.ExcerciseID == exerciseId.Value);
            }

            // Get available exercises for the current filter
            var exercises = await query
                .Where(sh => sh.ExcerciseID != 0)
                .GroupBy(sh => new { sh.ExcerciseID, sh.ExcerciseName })
                .Select(g => new { Id = g.Key.ExcerciseID, Name = g.Key.ExcerciseName })
                .ToListAsync();

            // Prepare team/group chart data: average WeightUsed per date
            var chartData = await query
                .Where(sh => sh.WeightUsed.HasValue && sh.DateCompleted.HasValue)
                .GroupBy(sh => new { Date = sh.DateCompleted.Value.Date })
                .Select(g => new
                {
                    Date = g.Key.Date,
                    AverageWeight = g.Average(sh => sh.WeightUsed.Value)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            // Prepare athlete chart data if an athlete is selected - NOW USING MAX INSTEAD OF AVERAGE
            var athleteData = new List<object>();
            if (athleteQuery != null)
            {
                athleteData = await athleteQuery
                    .Where(sh => sh.WeightUsed.HasValue && sh.DateCompleted.HasValue)
                    .GroupBy(sh => new { Date = sh.DateCompleted.Value.Date })
                    .Select(g => new
                    {
                        Date = g.Key.Date,
                        // Change from Average to Max for athlete data
                        AverageWeight = g.Max(sh => sh.WeightUsed.Value) // Using same property name for backward compatibility
                    })
                    .OrderBy(x => x.Date)
                    .Cast<object>()
                    .ToListAsync();
            }

            return Json(new { exercises, chartData, athleteData });
        }
        [HttpGet]
        public JsonResult GetExerciseHistory(string athleteId, int exerciseId)
        {
            if (string.IsNullOrEmpty(athleteId) || exerciseId <= 0)
                return Json(new { error = "Athlete ID and Exercise ID are required" });

            try
            {
                // Get all session history records for the athlete and exercise
                var history = _databaseContext.SessionHistory
                    .Where(sh => sh.UserId == athleteId && sh.ExcerciseID == exerciseId)
                    .OrderBy(sh => sh.DateCompleted)
                    .ThenBy(sh => sh.id)
                    .Select(sh => new
                    {
                        id = sh.id,
                        dateCompleted = sh.DateCompleted,
                        weight = sh.WeightUsed,
                        reps = sh.reps,
                        sets = 1, // Each row is a set
                        rpe = sh.RPE
                    })
                    .ToList();

                // Group the history by date
                var groupedHistory = history
                    .GroupBy(h => h.dateCompleted.Value.Date)
                    .Select(g => new
                    {
                        date = g.Key,
                        sessions = g.OrderBy(s => s.id).ToList()
                    })
                    .OrderByDescending(g => g.date)
                    .ToList();

                return Json(new { success = true, history = groupedHistory });
            }
            catch (Exception ex)
            {
                return Json(new { error = $"Error fetching exercise history: {ex.Message}" });
            }
        }


        [HttpGet]
        public async Task<JsonResult> GetMembersByGroup(int groupId)
        {
            var members = await _databaseContext.GroupMembers
                .Where(gm => gm.GroupId == groupId)
                .Join(_databaseContext.Users,
                      gm => gm.UserId,
                      u => u.Id,
                      (gm, u) => new { UserId = u.Id, User = u.Name })
                .ToListAsync();

            return Json(members);
        }

        [HttpGet]
        public async Task<JsonResult> GetGroupsByTeam(int teamId)
        {
            try
            {
                _logger.LogInformation($"GetGroupsByTeam called with teamId: {teamId}");

                var groups = await _databaseContext.Groups
                    .Where(g => g.TeamID == teamId)
                    .Select(g => new { Id = g.Id, Name = g.Name })
                    .ToListAsync();

                _logger.LogInformation($"Found {groups.Count} groups for team {teamId}");

                return Json(groups);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetGroupsByTeam: {ex.Message}");
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ProgressDashboard(int teamId)
        {
            // Get the email of the currently signed-in user from the cookie
            var userEmail = HttpContext.Request.Cookies["UserEmail"];

            // Find the user with the matching email in the database
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return RedirectToAction("SignIn", "Users");
            }

            // Create an instance of the DashboardViewModel
            var progressionVM = new ProgressionVM();


            // Set the User property of the DashboardViewModel
            progressionVM.User = new UserVModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            };

            progressionVM.TeamMembers = await _databaseContext.TeamMembers
                .Where(x => x.TeamId == teamId)
                .Select(tm => new TeamMembersVM
                {
                    UserId = tm.UserId,
                    UserName = tm.UserName
                }).ToListAsync();

            // Retrieve the teams, groups, and programs associated with the user
            progressionVM.Teams = await _databaseContext.Teams
                .Where(x=>x.UserID == user.Id)
                .Select(t => new TeamViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    UserId = user.Id
                }).ToListAsync();

            // Convert Groups to GroupViewModel
            progressionVM.Groups = await _databaseContext.Groups
                .Where(x => x.TeamID == teamId)
                .Select(g => new GroupViewModel
                {
                    Id = g.Id,
                    Name = g.Name
                }).ToListAsync();

            progressionVM.Programs = await _databaseContext.Programs
             .Select(p => new ProgramViewModel
             {
                 Id = p.id,
                 Name = p.Name
             }).ToListAsync();
            // Populate the Exercises property
            progressionVM.Excercises = await _databaseContext.Excercises
                .Select(sh => new ExerciseViewModel
                {
                    Id = sh.Id,
                    Name = sh.Name
                })
                .ToListAsync();
            //var test = await _databaseContext.PlayerPhysicalTesting.ToListAsync();

            // Return the DashboardViewModel to the view
            return View(progressionVM);
        }

        [HttpGet]
        public async Task<JsonResult> GetComparisonData(
    int? teamId, int? groupId, string athleteId, int? exerciseId, DateTime? startDate, DateTime? endDate)
        {
            if (string.IsNullOrEmpty(athleteId) || !exerciseId.HasValue)
            {
                return Json(new { error = "Athlete ID and Exercise ID are required" });
            }

            var baseQuery = _databaseContext.SessionHistory.AsQueryable();

            // Apply date and exercise filters to all queries
            if (startDate.HasValue)
                baseQuery = baseQuery.Where(sh => sh.DateCompleted >= startDate.Value);
            if (endDate.HasValue)
                baseQuery = baseQuery.Where(sh => sh.DateCompleted <= endDate.Value);
            baseQuery = baseQuery.Where(sh => sh.ExcerciseID == exerciseId.Value && sh.WeightUsed.HasValue);

            // Get athlete's average
            var athleteAverage = await baseQuery
                .Where(sh => sh.UserId == athleteId)
                .AverageAsync(sh => sh.WeightUsed.Value);

            // Get team's average (if teamId is provided)
            float? teamAverage = null;
            if (teamId.HasValue)
            {
                var teamMemberIds = await _databaseContext.TeamMembers
                    .Where(tm => tm.TeamId == teamId.Value)
                    .Select(tm => tm.UserId)
                    .ToListAsync();

                teamAverage = await baseQuery
                    .Where(sh => teamMemberIds.Contains(sh.UserId))
                    .AverageAsync(sh => sh.WeightUsed.Value);
            }

            // Get group's average (if groupId is provided)
            float? groupAverage = null;
            if (groupId.HasValue)
            {
                var groupMemberIds = await _databaseContext.GroupMembers
                    .Where(gm => gm.GroupId == groupId.Value)
                    .Select(gm => gm.UserId)
                    .ToListAsync();

                groupAverage = await baseQuery
                    .Where(sh => groupMemberIds.Contains(sh.UserId))
                    .AverageAsync(sh => sh.WeightUsed.Value);
            }

            /// Calculate comparison percentages
            double? athleteToTeamPercentage = null;
            if (teamAverage.HasValue && teamAverage > 0)
            {
                athleteToTeamPercentage = Math.Round((athleteAverage / teamAverage.Value - 1) * 100, 1);
            }

            double? athleteToGroupPercentage = null;
            if (groupAverage.HasValue && groupAverage > 0)
            {
                athleteToGroupPercentage = Math.Round((athleteAverage / groupAverage.Value - 1) * 100, 1);
            }

            return Json(new
            {
                athleteAverage = Math.Round((double)athleteAverage, 2),
                teamAverage = teamAverage.HasValue ? Math.Round((double)teamAverage.Value, 2) : (double?)null,
                groupAverage = groupAverage.HasValue ? Math.Round((double)groupAverage.Value, 2) : (double?)null,
                athleteToTeamPercentage = athleteToTeamPercentage,
                athleteToGroupPercentage = athleteToGroupPercentage
            });
        }

        [HttpGet]
        public async Task<JsonResult> GetMembersByTeam(int teamId)
        {
            var members = await _databaseContext.TeamMembers
                .Where(tm => tm.TeamId == teamId)
                .Join(_databaseContext.Users,
                      tm => tm.UserId,
                      u => u.Id,
                      (tm, u) => new { UserId = u.Id, User = u.Name })
                .ToListAsync();

            return Json(members);
        }

        [HttpGet]
        public async Task<JsonResult> GetMissedSessionsData(
      int? teamId, int? groupId, string athleteId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                // Build the query to get missed sessions
                var query = _databaseContext.MissedSessions.AsQueryable();
                var totalses = new List<GroupSession>();

                // Apply user filters
                if (!string.IsNullOrEmpty(athleteId))
                {
                    // Filter by specific athlete
                    query = query.Where(ms => ms.UserId == athleteId);
                }
                else if (groupId.HasValue)
                {
                    // Filter by group members
                    var groupMemberIds = await _databaseContext.GroupMembers
                        .Where(gm => gm.GroupId == groupId.Value)
                        .Select(gm => gm.UserId)
                        .ToListAsync();

                    query = query.Where(ms => groupMemberIds.Contains(ms.UserId));
                }
                else if (teamId.HasValue)
                {
                    // Filter by team members
                    var teamMemberIds = await _databaseContext.TeamMembers
                        .Where(tm => tm.TeamId == teamId.Value)
                        .Select(tm => tm.UserId)
                        .ToListAsync();

                    query = query.Where(ms => teamMemberIds.Contains(ms.UserId)).DistinctBy(y => y.GroupSessionID);
                }

                // Get missed sessions count
                var missedCount = await query.CountAsync();

                // Get total sessions count (missed + completed)
                if (!string.IsNullOrEmpty(athleteId))
                {
                    var groupmembers = await _databaseContext.GroupMembers.Where(x => x.UserId == athleteId).ToListAsync();
                    var athleteGroupProgrammes = new List<GroupProgramme>();
                    foreach (var group in groupmembers)
                    {
                        var groupprogram = await _databaseContext.GroupProgrammes.Where(x => x.GroupID == group.Id).ToListAsync();
                        if (groupprogram != null)
                        {
                            athleteGroupProgrammes.AddRange(groupprogram);
                        }
                    }


                    // Get all group sessions for these program IDs
                    foreach (var programId in athleteGroupProgrammes)
                    {
                        var sessions = await _databaseContext.GroupSession
                            .Where(gs => gs.GroupProgrammeID == programId.programmeId)
                            .ToListAsync();

                        if (sessions != null && sessions.Count > 0)
                        {
                            totalses.AddRange(sessions);
                        }
                    }
                }
                else if (groupId.HasValue)
                {
                    // Get all group programs for this group
                    var groupProgrammes = await _databaseContext.GroupProgrammes
                        .Where(gp => gp.GroupID == groupId.Value)
                        .ToListAsync();

                    // Get all group sessions for these programs
                    foreach (var program in groupProgrammes)
                    {
                        var sessions = await _databaseContext.GroupSession
                            .Where(gs => gs.GroupProgrammeID == program.id)
                            .ToListAsync();

                        if (sessions != null && sessions.Count > 0)
                        {
                            totalses.AddRange(sessions);
                        }
                    }
                }
                else if (teamId.HasValue)
                {
                    // This is the existing logic for team-based total sessions
                    var groupprogrammes = new List<GroupProgramme>();
                    var teamMemberIds = await _databaseContext.Groups
                        .Where(tm => tm.TeamID == teamId.Value)
                        .Select(tm => tm.Id)
                        .ToListAsync();

                    foreach (var group in teamMemberIds)
                    {
                        var programmes = await _databaseContext.GroupProgrammes
                            .Where(x => x.GroupID == group)
                            .ToListAsync();

                        if (programmes != null)
                        {
                            groupprogrammes.AddRange(programmes);
                        }
                    }

                    foreach (var programe in groupprogrammes)
                    {
                        var sessions = await _databaseContext.GroupSession
                            .Where(x => x.GroupProgrammeID == programe.id)
                            .ToListAsync();

                        if (sessions != null && sessions.Count > 0)
                        {
                            totalses.AddRange(sessions);
                        }
                    }
                }

                // Remove duplicates and count total sessions
                var totalSessions = totalses.Count();

                // Get detailed information about missed sessions - join with necessary tables to get program name and user name
                // Get detailed information about missed sessions
                var missedSessions = await query
                    .Take(10) // Limit to 10 most recent missed sessions
                    .Join(_databaseContext.GroupSession,
                        ms => ms.GroupSessionID,
                        gs => gs.id,
                        (ms, gs) => new { MissedSession = ms, GroupSession = gs })
                    .Join(_databaseContext.Events,
                        combined => combined.GroupSession.EventID,
                        e => e.Id,
                        (combined, e) => new { combined.MissedSession, combined.GroupSession, Event = e })
                    .Join(_databaseContext.Users,
                        combined => combined.MissedSession.UserId,
                        u => u.Id,
                        (combined, u) => new { combined.MissedSession, combined.GroupSession, combined.Event, User = u })
                    .Join(_databaseContext.Programs,
                        combined => combined.MissedSession.ProgramID,
                        p => p.id,
                        (combined, p) => new
                        {
                            date = combined.Event.StartTime, // Use the actual event date instead of DateTime.Now
                            programName = p.Name,
                            athleteName = combined.User.Name,
                            userId = combined.User.Id,
                            programId = p.id
                        })
                    .ToListAsync();

                return Json(new
                {
                    missedCount,
                    totalSessions,
                    missedSessions
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetMissedSessionsData: {ex.Message}");
                return Json(new { error = ex.Message });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetExerciseStats(string athleteId, int exerciseId)
        {
            if (string.IsNullOrEmpty(athleteId) || exerciseId <= 0)
            {
                return Json(new { error = "Invalid parameters" });
            }

            try
            {
                // Get all session history for this athlete and exercise
                var sessionHistory = await _databaseContext.SessionHistory
                    .Where(x => x.UserId == athleteId && x.ExcerciseID == exerciseId && x.WeightUsed.HasValue)
                    .ToListAsync();

                if (!sessionHistory.Any())
                {
                    return Json(new
                    {
                        maxWeight = 0,
                        avgWeight = 0,
                        sessions = 0
                    });
                }

                // Calculate stats
                double? maxWeight = sessionHistory.Max(x => x.WeightUsed);
                double? avgWeight = sessionHistory.Average(x => x.WeightUsed);
                int sessions = sessionHistory.Count;

                return Json(new
                {
                    maxWeight = maxWeight?.ToString("0.##"),
                    avgWeight = avgWeight?.ToString("0.##"),
                    sessions = sessions
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = $"Error retrieving exercise statistics: {ex.Message}" });
            }
        }



        [HttpPost]
        public async Task<IActionResult> CalculateHighestWeight(string Userid, int GroupID, int exerciseId)
        {
            var sessionHistories = await _databaseContext.SessionHistory
                .Where(sh => sh.ExcerciseID == exerciseId)
                .ToListAsync();

            List<ProgressData> progressData = new List<ProgressData>();

            if (!string.IsNullOrEmpty(Userid))
            {
                sessionHistories = sessionHistories.Where(sh => sh.UserId == Userid).ToList();
                if (sessionHistories.Any())
                {
                    HighestWeightUsed = sessionHistories.Max(sh => sh.WeightUsed);
                    progressData = sessionHistories
                        .Select(sh => new ProgressData { DateCompleted = sh.DateCompleted.Value, HighestWeightUsed = sh.WeightUsed.Value })
                        .ToList();
                }
            }
            else
            {
                if (sessionHistories.Any())
                {
                    HighestAverageWeightUsed = sessionHistories
                        .GroupBy(sh => sh.UserId)
                        .Select(g => g.Average(sh => sh.WeightUsed.Value))
                        .Max();
                    progressData = sessionHistories
                        .GroupBy(sh => sh.UserId)
                        .Select(g => new ProgressData { DateCompleted = g.First().DateCompleted.Value, HighestWeightUsed = g.Average(sh => sh.WeightUsed.Value) })
                        .ToList();
                }
            }

            // Return the partial view with the updated chart data
            return PartialView("_ChartPartial", progressData);
        }


    }
}

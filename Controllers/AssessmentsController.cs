using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhasePlayWeb.Data;
using PhasePlayWeb.Models.Entities;

namespace PhasePlayWeb.Controllers
{
    public class AssessmentsController : Controller
    {
        private readonly ILogger<AssessmentsController> _logger;
        private readonly ApplicationDbContext _databaseContext;
        private readonly UserManager<User> _userManager;

        public AssessmentsController(ILogger<AssessmentsController> logger, ApplicationDbContext databaseContext, UserManager<User> userManager)
        {
            _logger = logger;
            _databaseContext = databaseContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> AssessmentView()
        {
            var playerPhysicalTestings = await _databaseContext.PlayerPhysicalTesting.ToListAsync();
            return View(playerPhysicalTestings);
            //return View();
        }
    }
}

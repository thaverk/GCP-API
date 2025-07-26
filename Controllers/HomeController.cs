using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhasePlayWeb.Data;
using PhasePlayWeb.Models;
using System.Diagnostics;

namespace PhasePlayWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _databaseContext;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext _databaseContext)
        {
            _logger = logger;
            this._databaseContext = _databaseContext;
        }

        public IActionResult Index()
        {

            return View();
        }

        public async Task<IActionResult> schedule()

        {
            var events = await _databaseContext.Events.ToListAsync();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

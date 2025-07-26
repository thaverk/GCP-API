// Controllers/UsersController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhasePlayWeb.Data;
using PhasePlayWeb.Models;
using PhasePlayWeb.Models.Entities;
using PhasePlayWeb.Services;
using System.Diagnostics;

namespace PhasePlayWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _databaseContext;
        private readonly IEmailSender _emailSender;

        public UsersController(ApplicationDbContext _databaseContext, IEmailSender emailSender)
        {
            this._databaseContext = _databaseContext;
            _emailSender = emailSender;
        }

        [HttpGet]
        public async Task<IActionResult> ProfilePage()
        {
            var userEmail = HttpContext.Request.Cookies["UserEmail"];
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("SignIn");
            }
            var user = await _databaseContext.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
            {
                return RedirectToAction("SignIn");
            }
            var uservm = new UserVM
            {
                Name = user.Name,
                Email = user.Email,
                Surname = user.Surname,
                BodyWeight = user.BodyWeight
            };
            return View(uservm);
        }

        [HttpPost]
        public async Task<IActionResult> SaveProfile(UserVM userVM)
        {
            if (userVM == null || string.IsNullOrEmpty(userVM.Email))
            {
                return RedirectToAction("ProfilePage");
            }
            var user = await _databaseContext.Users.FirstOrDefaultAsync(u => u.Email == userVM.Email);
            if (user == null)
            {
                return RedirectToAction("ProfilePage");
            }
            user.Name = userVM.Name ?? user.Name;
            user.Surname = userVM.Surname ?? user.Surname;
            user.BodyWeight = userVM.BodyWeight;
            await _databaseContext.SaveChangesAsync();
            return RedirectToAction("ProfilePage");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            DeleteCookie("UserEmail");
            return RedirectToAction(nameof(SignIn));
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(LoginViewModel loginViewModel)
        {
            if (loginViewModel == null || string.IsNullOrEmpty(loginViewModel.Email))
            {
                return Json(new { succeeded = false, message = "$('#FailureModal').modal('show')" });
            }
            var user = await _databaseContext.Users.FirstOrDefaultAsync(u => u.Email == loginViewModel.Email && u.Password == loginViewModel.Password);
            if (user == null)
            {
                return Json(new { succeeded = false, message = "$('#FailureModal').modal('show')" });
            }
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(30),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };
            HttpContext.Response.Cookies.Append("UserEmail", loginViewModel.Email, cookieOptions);
            return Json(new { succeeded = true, message = "window.location.href = '" + Url.Action("Index", "Home") + "'" });
        }

        public void DeleteCookie(string cookieName)
        {
            if (Request.Cookies[cookieName] != null)
            {
                HttpContext.Response.Cookies.Delete(cookieName);
            }
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] User user)
        {
            if (user == null || string.IsNullOrEmpty(user.Email))
            {
                return BadRequest(new { success = false, error = "Invalid user data" });
            }
            var existingUser = await _databaseContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser != null)
            {
                return BadRequest(new { success = false, error = "The email already exists! Please sign in or Reset your Password!" });
            }
            await _databaseContext.Users.AddAsync(user);
            await _databaseContext.SaveChangesAsync();
          
            return Ok(new { success = true });
        }
    }
}

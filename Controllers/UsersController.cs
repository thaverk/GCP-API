// Controllers/UsersController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhasePlayWeb.Data;
using PhasePlayWeb.Extensions;
using PhasePlayWeb.Models;
using PhasePlayWeb.Models.Entities;
using PhasePlayWeb.Services;
using System.Diagnostics;

namespace PhasePlayWeb.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _databaseContext;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender _emailSender;

        public UsersController(ApplicationDbContext _databaseContext, UserManager<User> userManager, SignInManager<User> signInManager, IEmailSender emailSender)
        {
            this._databaseContext = _databaseContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult EmailSent()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult EmailNotVerified()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string? Email)
        {
            if (string.IsNullOrEmpty(Email))
            {
                // Handle the case when the email is null or empty
                return RedirectToAction("ForgotPassword");
            }

            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                // Handle the case when the user is not found
                // You might want to still redirect to avoid revealing that the email doesn't exist
                return RedirectToAction("ResetPasswordSent");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
            await _emailSender.SendEmailForgotPasswordAsync(user.Email, callbackUrl);

            return RedirectToAction("ResetPasswordSent");
        }

        [HttpGet]
        public IActionResult ResetPasswordSent()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> ProfilePage()
        {
            // Get the email of the currently signed-in user from the cookie
            var userEmail = HttpContext.Request.Cookies["UserEmail"];
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("SignIn");
            }

            // Find the user with the matching email in the database
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
            // Return the user's information to the view
            return View(uservm);
        }

        [HttpPost]
        public async Task<IActionResult> SaveProfile(UserVM userVM)
        {
            if (userVM == null || string.IsNullOrEmpty(userVM.Email))
            {
                return RedirectToAction("ProfilePage");
            }

            var user = await _userManager.FindByEmailAsync(userVM.Email);
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
        public async Task<IActionResult> Logout()
        {
            await Task.CompletedTask; // Add this line to avoid the CS1998 warning
            DeleteCookie("UserEmail");

            return RedirectToAction(nameof(SignIn));
        }

        [HttpGet]
        public async Task<IActionResult> SignIn()
        {
            var token = HttpContext.Request.Cookies["UserEmail"];
            if (!string.IsNullOrEmpty(token))
            {
                var user = await _userManager.FindByEmailAsync(token);
                if (user != null)
                {
                    await _signInManager.SignInAsync(user, isPersistent: true);

                    // Refresh the cookie expiration date
                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(30), // Refresh cookie expiration
                        HttpOnly = true, // Make the cookie accessible only by the server
                        Secure = true, // Ensure the cookie is sent over HTTPS
                        SameSite = SameSiteMode.Strict // Prevent CSRF attacks
                    };

                    HttpContext.Response.Cookies.Append("UserEmail", token, cookieOptions);

                    var role = await _userManager.GetRolesAsync(user);
                    string? redirectUrl = role.Contains("Admin") ? Url.Action("ProgressDashboard", "ProgressDashboard") :
                                         role.Contains("Staff") ? Url.Action("ProgressDashboard", "ProgressDashboard") :
                                         Url.Action("AthleteDashboard", "AthleteViews");

                    if (!string.IsNullOrEmpty(redirectUrl))
                    {
                        return Redirect(redirectUrl);
                    }

                    // Fallback if no specific redirect is found
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpGet]
        public ActionResult MyAction()
        {
            return PartialView("_MyModal");
        }

        [HttpGet]
        public IActionResult CalandarDashboard()
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

            // Attempt to sign in the user
            var result = await _signInManager.PasswordSignInAsync(loginViewModel.Email, loginViewModel.Password, isPersistent: false, lockoutOnFailure: false);

            var User = await _userManager.FindByEmailAsync(loginViewModel.Email);
            if (User == null)
            {
                return Json(new { succeeded = false, message = "$('#SuccessModal').modal('show')" });
            }

            if (User.Password != loginViewModel.Password)
            {
                return Json(new { succeeded = false, message = "$('#FailureModal').modal('show')" });
            }

            if (result.Succeeded)
            {
                // Generate a token for the user
                var user = await _userManager.FindByEmailAsync(loginViewModel.Email);
                if (user == null)
                {
                    return Json(new { succeeded = false, message = "$('#FailureModal').modal('show')" });
                }

                var token = HttpContext.Request.Cookies["UserEmail"];
                DeleteCookie("UserEmail");

                if (token == null)
                {
                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(30), // Set cookie expiration
                        HttpOnly = true, // Make the cookie accessible only by the server
                        Secure = true, // Ensure the cookie is sent over HTTPS
                        SameSite = SameSiteMode.Strict // Prevent CSRF attacks
                    };

                    HttpContext.Response.Cookies.Append("UserEmail", loginViewModel.Email, cookieOptions);
                }
                // Set the cookie with the token

                var role = await _userManager.GetRolesAsync(user);
                string? redirectUrl = role.Contains("Admin") ? Url.Action("ProgressDashboard", "ProgressDashboard") :
                                     role.Contains("Staff") ? Url.Action("ProgressDashboard", "ProgressDashboard") :
                                     Url.Action("AthleteDashboard", "AthleteViews");

                return Json(new { succeeded = true, message = $"window.location.href = '{redirectUrl}'" });
            }

            return Json(new { succeeded = true, message = $"window.location.href = '{Url.Action("EmailNotVerified", "Users")}'" });
        }

        public void DeleteCookie(string cookieName)
        {
            if (Request.Cookies[cookieName] != null)
            {
                HttpContext.Response.Cookies.Delete(cookieName);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(RegisterViewModel userVM)
        {
            if (userVM == null || string.IsNullOrEmpty(userVM.Email))
            {
                ViewBag.Errors = new[] { "Invalid user data" };
                return View(userVM);
            }

            // Check if a user with the given email already exists
            var existingUser = await _userManager.FindByEmailAsync(userVM.Email);
            if (existingUser != null)
            {
                return Json(new { success = true });
            }

            var user = new User
            {
                Name = userVM.Name,
                Surname = userVM.Surname,
                Email = userVM.Email,
                Password = userVM.Password,
                UserName = userVM.Email,
            };

            var result = await _userManager.CreateAsync(user, userVM.Password);
            if (result.Succeeded)
            {
                var identityResult = await _userManager.AddToRoleAsync(user, "Admin");

                var _ConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.EmailConfirmationLink(user.Id, _ConfirmationToken, Request.Scheme);
                await _emailSender.SendEmailConfirmationAsync(user.Email, callbackUrl);

                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(30), // Set cookie expiration
                    HttpOnly = true, // Make the cookie accessible only by the server
                    Secure = true, // Ensure the cookie is sent over HTTPS
                    SameSite = SameSiteMode.Strict // Prevent CSRF attacks
                };

                HttpContext.Response.Cookies.Append("UserEmail", userVM.Email, cookieOptions);

                return Json(new { success = false });
            }
            else
            {
                // Log the errors
                foreach (var error in result.Errors)
                {
                    Debug.WriteLine($"Error Code: {error.Code}, Description: {error.Description}");
                }

                ViewBag.Errors = result.Errors.Select(e => e.Description).ToList();
                return View(userVM);
            }
        }

        [HttpGet]
        public IActionResult _MyModal()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeamModal(TeamsVM vm)
        {
            if (vm == null || string.IsNullOrEmpty(vm.TeamName))
            {
                return RedirectToAction(nameof(EmailSent));
            }

            var userEmail = HttpContext.Request.Cookies["UserEmail"];
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("SignIn");
            }

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return RedirectToAction("SignIn");
            }

            var team = new Teams
            {
                Name = vm.TeamName,
                UserID = user.Id,
            };
            await _databaseContext.Teams.AddAsync(team);
            await _databaseContext.SaveChangesAsync();

            var group1 = new Groups
            {
                Name = "Team",
                TeamID = team.Id
            };
            await _databaseContext.Groups.AddAsync(group1);
            await _databaseContext.SaveChangesAsync();

            var group2 = new Groups
            {
                Name = "Athlete",
                TeamID = team.Id
            };
            await _databaseContext.Groups.AddAsync(group2);
            await _databaseContext.SaveChangesAsync();

            var group3 = new Groups
            {
                Name = "Staff",
                TeamID = team.Id
            };

            await _databaseContext.Groups.AddAsync(group3);
            await _databaseContext.SaveChangesAsync();

            HttpContext.Response.Cookies.Delete("UserEmail");
            return RedirectToAction(nameof(EmailSent));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string? userId, string? code)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
            {
                return View("Error");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string? code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            JsonResultViewModel _JsonResultViewModel = new();
            try
            {
                if (model == null || string.IsNullOrEmpty(model.Email))
                {
                    return new JsonResult(new { IsSuccess = false, Message = "Invalid model data." });
                }

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return new JsonResult(new { IsSuccess = false, Message = "User not found." });
                }

                var result = await _userManager.ResetPasswordAsync(user, model.Code, model.NewPassword);

                if (result.Succeeded)
                {
                    user.Password = model.NewPassword;
                    await _userManager.UpdateAsync(user);
                    if (user.EmailConfirmed == false)
                    {
                        user.EmailConfirmed = true;
                        await _userManager.UpdateAsync(user);
                        await _databaseContext.SaveChangesAsync();
                    }
                    return RedirectToAction(nameof(SignIn));
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return new JsonResult(new { IsSuccess = false, Message = $"Password reset failed: {errors}" });
                }
            }
            catch (Exception ex)
            {
                _JsonResultViewModel.IsSuccess = false;
                return new JsonResult(new { IsSuccess = false, Message = ex.Message });
            }
        }
    }
}

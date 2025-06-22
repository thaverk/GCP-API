using ExcelDataReader;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PhasePlayWeb.Data;
using PhasePlayWeb.Extensions;
using PhasePlayWeb.Models;
using PhasePlayWeb.Models.Entities;
using PhasePlayWeb.Services;
using System.Data;
using System.Diagnostics;

namespace PhasePlayWeb.Controllers
{
    public class TeamsController : Controller
    {
        private readonly ApplicationDbContext _databaseContext;
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;
        private static readonly Random _random = new Random();
        public TeamsController(ApplicationDbContext _databaseContext, UserManager<User> userManager, IEmailSender emailSender)
        {
            this._databaseContext = _databaseContext;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        public async Task<IActionResult> SearchUsers(string searchQuery)
        {
             var users = await _userManager.Users
        .Where(u => u.Name.Contains(searchQuery) || u.Email.Contains(searchQuery))
        .Select(u => new { id = u.Id, name = u.Name, email = u.Email })
        .ToListAsync();

    return Json(users);
        }

        [HttpPost]
        public async Task<IActionResult> SearchAndAddMember(string selectedUser, int teamId)
        {
            // Logic to add the selected user to the team with the specified role
            var user = await _userManager.FindByIdAsync(selectedUser);
            var team = await _databaseContext.Teams.Where(x => x.Id == teamId).FirstOrDefaultAsync();
            if (user != null)
            {
                var ismember = await _databaseContext.TeamMembers.Where(x => x.TeamId == teamId & x.UserId == user.Id).FirstOrDefaultAsync();
                
                if (ismember != null)
                {
                    TempData["Alert"] = $"{user.Name} is already in Team {team.Name}.TryAgain!";
                    return RedirectToAction(nameof(MembersList));

                }
                
                var TeamMember = new TeamMembers
                {
                    Id = GenerateRandomId(),
                    TeamId = teamId,
                    UserId = user.Id,
                    UserName = user.Name // Set the required UserName property
                };

                using (var transaction = await _databaseContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        await _databaseContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[TeamMembers] ON");
                        await _databaseContext.TeamMembers.AddAsync(TeamMember);
                        await _databaseContext.SaveChangesAsync();
                        await _databaseContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[TeamMembers] OFF");
                        await transaction.CommitAsync();
                        TempData["Alert"] = $"{user.Name} has been successfully added to Team {team.Name}";
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();

                        throw;
                        
                    }

                }
            }

            else
            {
                TempData["Alert"] = "Selected User does not exist.TryAgain!";
               
            }
            return RedirectToAction(nameof(MembersList));
            // Redirect to the appropriate action
        }


        [HttpPost]
        public async Task<IActionResult> ResendVerification(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
            await _emailSender.SendEmailForgotPasswordAsync(user.Email, callbackUrl);
            return RedirectToAction(nameof(MembersList));
        }


        [HttpPost]
        public async Task<IActionResult> UploadUsers(IFormFile file, int TeamId, string role)
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
                    var data = new List<User>();

                    using (var excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream))
                    {
                        var dataSet = excelReader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true
                            }
                        });

                        var worksheet = dataSet.Tables[0];

                        for (int row = 0; row < worksheet.Rows.Count; row++)
                        {
                            var user = new User
                            {
                                Name = worksheet.Rows[row][0]?.ToString() ?? string.Empty,
                                Surname = worksheet.Rows[row][1]?.ToString() ?? string.Empty,
                                Email = worksheet.Rows[row][2]?.ToString() ?? string.Empty,
                                UserName = worksheet.Rows[row][2]?.ToString() ?? string.Empty,
                                FirstSignIn = true,
                                EmailConfirmed = false,
                                Password = "defaultPassword@1",

                            };
                            var result = await _userManager.CreateAsync(user, user.Password);

                            if (result.Succeeded)
                            {
                                data.Add(user);

                                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                                var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
                                await _emailSender.SendEmailForgotPasswordAsync(user.Email, callbackUrl);
                                await _databaseContext.SaveChangesAsync();




                                var TeamMember = new TeamMembers
                                {
                                    Id = GenerateRandomId(),
                                    TeamId = TeamId,
                                    UserId = user.Id,
                                    UserName = user.Name // Set the required UserName property
                                };
                                // Enable IDENTITY_INSERT
                                using (var transaction = await _databaseContext.Database.BeginTransactionAsync())
                                {
                                    try
                                    {
                                        await _databaseContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[TeamMembers] ON");
                                        await _databaseContext.TeamMembers.AddAsync(TeamMember);
                                        await _databaseContext.SaveChangesAsync();
                                        await _databaseContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[TeamMembers] OFF");


                                        await transaction.CommitAsync();
                                        await _userManager.AddToRoleAsync(user, role);
                                        await _databaseContext.SaveChangesAsync();

                                        //var TeamGroup = await _databaseContext.Groups.Where(x => x.Name == "Team" & x.TeamID == TeamId).FirstOrDefaultAsync();
                                        //if (TeamGroup != null)
                                        //{
                                        //    var member = new GroupMembers
                                        //    {
                                        //        GroupId = TeamGroup.Id,
                                        //        UserId = user.Id
                                        //    };
                                        //    await _databaseContext.GroupMembers.AddAsync(member);
                                        //    await _databaseContext.SaveChangesAsync();
                                        //}

                                        //if (role == "Staff")
                                        //{
                                        //    var group = await _databaseContext.Groups.Where(x => x.Name == "Staff" & x.TeamID == TeamId).FirstOrDefaultAsync();
                                        //    if (group != null)
                                        //    {
                                        //        var member = new GroupMembers
                                        //        {
                                        //            GroupId = group.Id,
                                        //            UserId = user.Id
                                        //        };
                                        //        await _databaseContext.GroupMembers.AddAsync(member);
                                        //        await _databaseContext.SaveChangesAsync();
                                        //    }
                                        //}
                                        //else if (role == "Athlete")
                                        //{
                                        //    var group = await _databaseContext.Groups.Where(x => x.Name == "Athlete" & x.TeamID == TeamId).FirstOrDefaultAsync();
                                        //    if (group != null)
                                        //    {
                                        //        var member = new GroupMembers
                                        //        {
                                        //            GroupId = group.Id,
                                        //            UserId = user.Id
                                        //        };
                                        //        await _databaseContext.GroupMembers.AddAsync(member);
                                        //        await _databaseContext.SaveChangesAsync();
                                        //    }


                                        //}
                                    }
                                    catch (Exception)
                                    {
                                        await transaction.RollbackAsync();
                                        throw;
                                    }
                                }


                            }



                        }
                    }


                    await _databaseContext.SaveChangesAsync();

                }
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework like Serilog, NLog, etc.)

                Debug.WriteLine($"Error uploading file: {ex.Message}");
                ModelState.AddModelError(string.Empty, "An error occurred while processing the file. Please try again.");
                return RedirectToAction("UploadExerciseList");
            }

            return RedirectToAction(nameof(MembersList));
        }


        [HttpGet]
        public async Task<IActionResult> ViewGroupMembers(int id)
        {
            var group = await _databaseContext.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound();
            }

            var members = await _databaseContext.GroupMembers
                .Where(gm => gm.GroupId == id)
                .Join(_databaseContext.Users,
                    gm => gm.UserId,
                    u => u.Id,
                    (gm, u) => new { User = u })
                .Select(gm => gm.User)
                .ToListAsync();

            var groupMembers = new List<User>();
            foreach (var member in members)
            {
                var user = await _userManager.FindByIdAsync(member.Id);
                if (user != null)
                {
                    groupMembers.Add(user);
                }
            }

            ViewBag.GroupName = group.Name;

            return View(groupMembers);
        }

        private int GenerateRandomId()
        {
            return _random.Next(1, int.MaxValue);
        }


        public async Task<IActionResult> AddMemberTeam(string Name, string Surname, string Email, int TeamId, string role)

        {

            // Create a new User object
            var newUser = new User
            {
                UserName = Email,
                Name = Name,
                Surname = Surname,
                Email = Email,
                Password = "defaultPassword@1",
                EmailConfirmed = false,
                FirstSignIn = true
            };

            // Add the new member to the database
            var result = await _userManager.CreateAsync(newUser, newUser.Password);

            if (result.Succeeded)
            {
                var code = await _userManager.GeneratePasswordResetTokenAsync(newUser);
                var callbackUrl = Url.ResetPasswordCallbackLink(newUser.Id, code, Request.Scheme);

                await _emailSender.SendEmailForgotPasswordAsync(newUser.Email, callbackUrl);

                var TeamMember = new TeamMembers
                {
                    Id = GenerateRandomId(),
                    TeamId = TeamId,
                    UserId = newUser.Id,
                    UserName = newUser.Name // Set the required UserName property
                };
                // Enable IDENTITY_INSERT
                using (var transaction = await _databaseContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        await _databaseContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[TeamMembers] ON");
                        await _databaseContext.TeamMembers.AddAsync(TeamMember);
                        await _databaseContext.SaveChangesAsync();
                        await _databaseContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[TeamMembers] OFF");


                        await transaction.CommitAsync();
                        await _userManager.AddToRoleAsync(newUser, role);

                        var TeamGroup = await _databaseContext.Groups.Where(x => x.Name == "Team" & x.TeamID == TeamId).FirstOrDefaultAsync();
                        if (TeamGroup != null)
                        {
                            var member = new GroupMembers
                            {
                                GroupId = TeamGroup.Id,
                                UserId = newUser.Id
                            };
                            await _databaseContext.GroupMembers.AddAsync(member);
                            await _databaseContext.SaveChangesAsync();
                        }

                        if (role == "Staff")
                        {
                            var group = await _databaseContext.Groups.Where(x => x.Name == "Staff" & x.TeamID == TeamId).FirstOrDefaultAsync();
                            if (group != null)
                            {
                                var member = new GroupMembers
                                {
                                    GroupId = group.Id,
                                    UserId = newUser.Id
                                };
                                await _databaseContext.GroupMembers.AddAsync(member);
                                await _databaseContext.SaveChangesAsync();

                            }
                        }
                        else if (role == "Athlete")
                        {
                            var group = await _databaseContext.Groups.Where(x => x.Name == "Athlete" & x.TeamID == TeamId).FirstOrDefaultAsync();
                            if (group != null)
                            {
                                var member = new GroupMembers
                                {
                                    GroupId = group.Id,
                                    UserId = newUser.Id
                                };
                                await _databaseContext.GroupMembers.AddAsync(member);
                                await _databaseContext.SaveChangesAsync();

                            }

                        }


                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }




                TempData["Alert"] = "Member have been successfully added!";
                return RedirectToAction("MembersList");
            }



            TempData["Alert"] = "Try Again!";
            return RedirectToAction("MembersList");
            // Redirect to the appropriate action


            // Return the view if the model state is not valid
        }

        public static int ParseLetterToDigit(string letter)
        {
            // Check if the input is null or empty
            if (string.IsNullOrEmpty(letter))
            {
                throw new ArgumentException("Input must not be null or empty");
            }

            // Check if the input has more than one character
            if (letter.Length > 1)
            {
                throw new ArgumentException("Input must be a single character");
            }

            // Extract the first character of the string
            char firstChar = letter[0];

            // Check if the input is a letter
            if (!char.IsLetter(firstChar))
            {
                throw new ArgumentException("Input must be a letter");
            }

            // Convert the letter to lowercase
            firstChar = char.ToLower(firstChar);

            // Calculate the digit value
            int digit = firstChar - 96;

            return digit;
        }

        [HttpGet]
        public async Task<IActionResult> GetGroupsByTeamId(int teamId)
        {
            var groups = await _databaseContext.Groups
                .Where(g => g.TeamID == teamId)
                .Select(g => new
                {
                    g.Id,
                    g.Name,
                    MemberCount = _databaseContext.GroupMembers.Count(gm => gm.GroupId == g.Id)
                })
                .ToListAsync();

            return Ok(groups);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var group = await _databaseContext.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound();
            }

            _databaseContext.Groups.Remove(group);
            await _databaseContext.SaveChangesAsync();

            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request)
        {
            if (ModelState.IsValid)
            {
                var group = new Groups
                {
                    Name = request.GroupName,
                    TeamID = request.TeamId
                };

                _databaseContext.Groups.Add(group);
                await _databaseContext.SaveChangesAsync();


                foreach (var memberId in request.MemberIds)

                {
                    var cleanedMemberId = memberId.Replace("member-", string.Empty);
                    var user = await _userManager.FindByIdAsync(cleanedMemberId);
                    if (user != null)
                    {
                        var groupMember = new GroupMembers
                        {
                            GroupId = group.Id,
                            UserId = user.Id // Ensure the full user ID is used
                        };
                        await _databaseContext.GroupMembers.AddAsync(groupMember);
                    }

                }

                await _databaseContext.SaveChangesAsync();
                return RedirectToAction(nameof(MembersList));
            }

            return BadRequest(ModelState);
        }

        [HttpPost]
        public async Task<IActionResult> GetMembersByTeamId(int teamId)
        {
            var members = await _databaseContext.TeamMembers
                .Where(m => m.TeamId == teamId).ToListAsync();
            List<User> Members = new List<User>();
            foreach (var team in members)
            {
                var user = await _userManager.FindByIdAsync(team.UserId);
                if (user != null)
                {
                    Members.Add(user);
                }
            }
            ;
            Tuple<List<User>, int> tuple = new Tuple<List<User>, int>(Members, teamId);
            return PartialView(nameof(_MembersListPartial), tuple);
        }

        [HttpGet]
        public async Task<JsonResult> GetTeamMembers(int teamId)
        {

            var AtheleteGroup = await _databaseContext.Groups
                .Where(g => g.TeamID == teamId)
                .Where(x => x.Name == "Athletes")
                .FirstOrDefaultAsync();
            if (AtheleteGroup == null)
            {
                var group = new Groups
                {
                    Name = "Athletes",
                    TeamID = teamId,
                };
                await _databaseContext.Groups.AddAsync(group);
                await _databaseContext.SaveChangesAsync();
            }

            var StaffGroup = await _databaseContext.Groups
                .Where(g => g.TeamID == teamId)
                .Where(x => x.Name == "Staff")
                .FirstOrDefaultAsync();
            if (StaffGroup == null)
            {
                var group = new Groups
                {
                    Name = "Staff",
                    TeamID = teamId
                };
                await _databaseContext.Groups.AddAsync(group);
                await _databaseContext.SaveChangesAsync();
            }

            var staff = await _databaseContext.TeamMembers.Where(x => x.TeamId == teamId).ToListAsync();
            foreach (var member in staff)
            {
                var us = await _userManager.FindByIdAsync(member.UserId);
                if (us != null)
                {
                    var role = await _userManager.GetRolesAsync(us);



                    if (role.Contains("Athlete"))
                    {
                        var AthleteGroup = await _databaseContext.Groups.Where(x => x.TeamID == teamId && x.Name == "Athletes").FirstOrDefaultAsync();
                        if (AthleteGroup != null)
                        {
                            var AthleteUser = await _databaseContext.GroupMembers.Where(x => x.GroupId == AthleteGroup.Id && x.UserId == us.Id).FirstOrDefaultAsync();
                            if (AthleteUser == null)
                            {
                                var AthleteUsers = new GroupMembers
                                {
                                    UserId = us.Id,
                                    GroupId = AthleteGroup.Id,
                                };

                                await _databaseContext.GroupMembers.AddAsync(AthleteUsers);
                                await _databaseContext.SaveChangesAsync();
                            }
                        }
                    }
                    else if (role.Contains("Staff"))
                    {
                        var staffGroup = await _databaseContext.Groups.Where(x => x.TeamID == teamId && x.Name == "Staff").FirstOrDefaultAsync();
                        if (staffGroup != null)
                        {
                            var staffUser = await _databaseContext.GroupMembers.Where(x => x.GroupId == staffGroup.Id && x.UserId == us.Id).FirstOrDefaultAsync();
                            if (staffUser == null)
                            {
                                var newUser = new GroupMembers
                                {
                                    UserId = us.Id,
                                    GroupId = staffGroup.Id,
                                };

                                await _databaseContext.GroupMembers.AddAsync(newUser);
                                await _databaseContext.SaveChangesAsync();
                            }
                        }

                    }
                }
            }


            var members = await _databaseContext.TeamMembers
                .Where(m => m.TeamId == teamId).ToListAsync();

            List<object> Members = new List<object>();
            foreach (var team in members)
            {
                var user = await _userManager.FindByIdAsync(team.UserId);

                if (user != null)
                {
                    var userinfo = new { user.Name, user.Surname, user.Id };
                    Members.Add(userinfo);
                }
            }

            // Log the members list to verify the data
            Debug.WriteLine("Members: " + JsonConvert.SerializeObject(Members));

            return Json(Members);
        }


        [HttpGet]
        public IActionResult _MembersListPartial(List<User> Members)
        {
            var members = _databaseContext.Users.ToList(); // Ensure this retrieves the list correctly
            if (members == null)
            {
                members = new List<User>(); // Initialize to an empty list if null
            }
            return PartialView("_MembersListPartial", Members);
        }

        [HttpGet]
        public async Task<IActionResult> AthleteUpload()
        {
            var athletes = await _databaseContext.Users.ToListAsync();
            return View(athletes);
        }

        [HttpPost]
        public async Task<IActionResult> EditMember(string id, string name, string email, string surname)
        {
            if (string.IsNullOrEmpty(name))
            {
                ModelState.AddModelError(string.Empty, "Member name is required.");
                return RedirectToAction("MemberList");
            }

            var athlete = await _userManager.FindByIdAsync(id);
            if (athlete == null)
            {
                return NotFound();
            }

            athlete.Name = name;
            athlete.Surname = surname;
            athlete.Email = email;
            await _userManager.UpdateAsync(athlete);

            await _databaseContext.SaveChangesAsync();

            return RedirectToAction(nameof(MembersList));
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file, int teamId)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Please upload a valid Excel file.");
                return RedirectToAction("AthleteUpload", new { id = teamId });
            }

            if (file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                ModelState.AddModelError(string.Empty, "Invalid file format. Please upload an Excel file.");
                return RedirectToAction("AthleteUpload", new { id = teamId });
            }

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    var data = new List<User>();

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
                            var athlete = new User
                            {
                                Name = worksheet.Rows[row][0]?.ToString() ?? string.Empty,
                                Email = worksheet.Rows[row][1]?.ToString() ?? string.Empty,
                                Password = "defaultPassword",
                            };
                            data.Add(athlete);
                        }
                    }

                    _databaseContext.Users.AddRange(data);
                    await _databaseContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework like Serilog, NLog, etc.)
                Debug.WriteLine($"Error uploading file: {ex.Message}");
                ModelState.AddModelError(string.Empty, "An error occurred while processing the file. Please try again.");
                return RedirectToAction("AthleteUpload");
            }

            return RedirectToAction("SPDashboard", "Users");
        }

        [HttpGet]
        public IActionResult TeamBuilder()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TeamBuilder(TeamVM teamsVM)
        {
            var useremail = HttpContext.Request.Cookies["UserEmail"];
            var user = await _databaseContext.Users.FirstOrDefaultAsync(u => u.Email == useremail);
            if (user == null)
            {
                return NotFound();
            }
            var team = new Teams
            {
                Name = teamsVM.Name,
                UserID = user.Id
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
                Name = "Athletes",
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

            return RedirectToAction(nameof(MembersList));


        }


        [HttpPost]
        public IActionResult Skip()
        {
            return RedirectToAction("SPDashboard", "Users");
        }
        [HttpPost]
        public IActionResult SaveTeam()
        {
            return RedirectToAction("SPDashboard", "Users");
        }


        [HttpGet]
        public async Task<IActionResult> MembersList(int Id)
        {
            var athletes = await _databaseContext.Users.ToListAsync();
            var useremail = HttpContext.Request.Cookies["UserEmail"];
            var user = await _databaseContext.Users.FirstOrDefaultAsync(u => u.Email == useremail);
            if (user == null)
            {
                return NotFound();
            }


            var viewModel = new MembersListViewModel
            {
                Teams = _databaseContext.Teams
                    .Where(t => t.UserID == user.Id)
                    .Select(t => new TeamVModel
                    {
                        Id = t.Id,
                        Name = t.Name,
                        MembersCount = _databaseContext.TeamMembers.Count(tm => tm.TeamId == t.Id),
                        GroupsCount = _databaseContext.Groups.Count(g => g.TeamID == t.Id)
                    })
                    .ToList(),
                Groups = _databaseContext.Groups
                    .Select(g => new GroupVModel
                    {
                        Id = g.Id,
                        Name = g.Name,
                        TeamID = g.TeamID
                    })
                    .ToList(),
            };

            return View(viewModel);
        }



        [HttpGet]
        public async Task<IActionResult> GetGroupDetails(int groupId)
        {
            var group = _databaseContext.Groups.FirstOrDefault(x => x.Id == groupId);
            if (group == null)
            {
                return NotFound();
            }

            var Members = _databaseContext.GroupMembers
                .Where(x => x.GroupId == groupId)
                .ToList();
            foreach (var Member in Members)
            {
                var user = await _databaseContext.Users.FirstOrDefaultAsync(x => x.Id == Member.UserId);
                var User = await _userManager.FindByIdAsync(Member.UserId);
            }

            var groupMembers = _databaseContext.GroupMembers
                .Where(x => x.GroupId == groupId)
                .Select(x => x.UserId)
                .ToList();

            var Groupmembers = _databaseContext.GroupMembers
                .Where(x => x.GroupId == groupId)

                .ToList();
            List<User> TeamMembers = new List<User>();
            var teamMembers = _databaseContext.TeamMembers
                .Where(x => x.TeamId == group.TeamID && !groupMembers.Contains(x.UserId))
                .Select(x => _databaseContext.Users.FirstOrDefault(u => u.Id == x.UserId))
                .ToList();
            foreach (var member in teamMembers)
            {
                var user = await _userManager.FindByIdAsync(member.Id);
                if (user != null)
                {
                    TeamMembers.Add(user);
                }
            }

            List<User> groupMemberDetails = new List<User>();

            foreach (var member in Groupmembers)
            {


                var user = await _userManager.FindByIdAsync(member.UserId);
                if (user != null)
                {
                    groupMemberDetails.Add(user);
                }

                var Group = await _databaseContext.Groups.Where(x => x.Id == groupId).FirstOrDefaultAsync();

                var Team = await _databaseContext.Teams.Where(x => x.Id == Group.TeamID).FirstOrDefaultAsync();

                //foreach (var teamMember in TeamMembers)
                //{
                //    if (teamMember.Id == user.Id)
                //    {

                //        if (teamMember != null)
                //        {
                //            groupMemberDetails.Add(teamMember);
                //        }
                //    }
                //}

            }

            var memberdetails = _databaseContext.Users.Where(u => groupMembers.Contains(u.Id)).ToListAsync();

            var tuple = new Tuple<Groups, List<User>, List<User>>(group, groupMemberDetails, TeamMembers);

            return PartialView("_GroupDetailsPartial", tuple);
        }


        [HttpGet]
        public IActionResult SearchMember(string query)
        {
            var member = _databaseContext.Users
                .Where(e => e.Name.Contains(query))
                .ToList();

            return View("MembersList", member);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMember(string Id, int TeamId)
        {
            var member = await _userManager.FindByIdAsync(Id);
            if (member != null)
            {
                _databaseContext.TeamMembers.RemoveRange(_databaseContext.TeamMembers.Where(tm => tm.UserId == Id & tm.TeamId == TeamId).ToList());
                var groups = _databaseContext.Groups.Where(g => g.TeamID == TeamId).ToList();
                foreach (var group in groups)
                {
                    _databaseContext.GroupMembers.RemoveRange(_databaseContext.GroupMembers.Where(gm => gm.UserId == Id && gm.GroupId == group.Id).ToList());
                }

                await _databaseContext.SaveChangesAsync();

            }

            return RedirectToAction("MembersList");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]
        public IActionResult AddMemberToGroup([FromBody] AddMemberToGroupRequest vm)
        {
            var group = _databaseContext.Groups.FirstOrDefault(g => g.Id == vm.GroupId);
            if (group == null)
            {
                return Json(new { success = false, message = "Group not found" });
            }

            foreach (var memberId in vm.Members)
            {
                var user = _databaseContext.Users.FirstOrDefault(u => u.Id == memberId);
                if (user != null && !_databaseContext.GroupMembers.Any(gm => gm.GroupId == vm.GroupId && gm.UserId == memberId))
                {
                    var groupMember = new GroupMembers
                    {
                        GroupId = vm.GroupId,
                        UserId = memberId
                    };
                    _databaseContext.GroupMembers.Add(groupMember);
                }
            }

            _databaseContext.SaveChanges();

            return Json(new { success = true });
        }
    }

}

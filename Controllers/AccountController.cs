using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Milestone3WebApp.Models;
using Milestone3WebApp.ViewModels;
using System;
using System.Threading.Tasks;
using System.Data;

namespace Milestone3WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        
        private readonly string _connectionString;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;

            _connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new Exception("DATABASE_URL is not set in environment variables.");
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            var loginViewModel = new LoginViewModel
            {
                Password = string.Empty
            };

            return View(loginViewModel);
        }

        [HttpPost]
public async Task<IActionResult> Login(LoginViewModel model)
{
    if (!ModelState.IsValid)
    {
        Console.WriteLine("[ERROR] Model state is invalid.");
        return View(model);
    }

    try
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new SqlCommand("EXEC loginproc @username, @password, @success OUTPUT, @user_type OUTPUT", connection);
            command.Parameters.AddWithValue("@username", model.Username);
            command.Parameters.AddWithValue("@password", model.Password);

            var successParam = new SqlParameter("@success", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var userTypeParam = new SqlParameter("@user_type", SqlDbType.NVarChar, 50) { Direction = ParameterDirection.Output };

            command.Parameters.Add(successParam);
            command.Parameters.Add(userTypeParam);

            await command.ExecuteNonQueryAsync();

            var loginSuccess = (int)successParam.Value == 1;
            var userType = userTypeParam.Value?.ToString();

            if (loginSuccess)
            {
                if (userType == "Admin")
                {
                    HttpContext.Session.SetInt32("AdminID", model.Username);
                    return RedirectToAction("AdminProfile", "Admin", new { adminID = model.Username });
                }
                else if (userType == "Learner")
                {
                    HttpContext.Session.SetInt32("LearnerID", model.Username);
                    return RedirectToAction("Profile", "Learner", new { learnerID = model.Username });
                }
                else if (userType == "Instructor")
                {
                    HttpContext.Session.SetInt32("LearnerID", model.Username);
                    HttpContext.Session.SetInt32("InstructorID", model.Username);
                    return RedirectToAction("Profile", "Instructor", new { instructorID = model.Username });
                }
            }
            else
            {
                Console.WriteLine("[ERROR] Login failed.");
                ModelState.AddModelError("", "Invalid username or password.");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERROR] {ex.Message}");
        ModelState.AddModelError("", "An error occurred. Please try again later.");
    }

    return View(model);
}

        [HttpGet]
        public IActionResult LearnerSignup()
        {
            var SignUpViewModel = new SignUpViewModel
            {
                Password = string.Empty
            };

            return View(SignUpViewModel);
        }

        [HttpPost]
public async Task<IActionResult> LearnerSignup(SignUpViewModel model)
{
    if (ModelState.IsValid)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Insert learner into learner table
                var learnerCommand = new SqlCommand(
                    @"INSERT INTO learner (first_name, last_name, gender, birth_date, country, cultural_background)
                      OUTPUT INSERTED.learnerID 
                      VALUES (@FirstName, @LastName, @Gender, @BirthDate, @Country, @CulturalBackground)", 
                      connection);

                learnerCommand.Parameters.AddWithValue("@FirstName", model.FirstName);
                learnerCommand.Parameters.AddWithValue("@LastName", model.LastName);
                learnerCommand.Parameters.AddWithValue("@Gender", model.Gender);
                learnerCommand.Parameters.AddWithValue("@BirthDate", model.BirthDate);
                learnerCommand.Parameters.AddWithValue("@Country", model.Country);
                learnerCommand.Parameters.AddWithValue("@CulturalBackground", model.CulturalBackground);

                int learnerID = (int)await learnerCommand.ExecuteScalarAsync();

                // Check if learnerID already exists in Users table
                var checkUserCommand = new SqlCommand(
                    @"SELECT COUNT(*) FROM Users WHERE username = @Username", 
                    connection);
                checkUserCommand.Parameters.AddWithValue("@Username", learnerID);

                int existingUserCount = (int)await checkUserCommand.ExecuteScalarAsync();

                if (existingUserCount == 0)
                {
                    // Insert learner into Users table
                    var userCommand = new SqlCommand(
                        @"INSERT INTO Users (username, password, user_type)
                          VALUES (@Username, @Password, @UserType)", 
                          connection);

                    userCommand.Parameters.AddWithValue("@Username", learnerID);
                    userCommand.Parameters.AddWithValue("@Password", model.Password);
                    userCommand.Parameters.AddWithValue("@UserType", "Learner");

                    await userCommand.ExecuteNonQueryAsync();

                    HttpContext.Session.SetInt32("LearnerID", learnerID);
                    return RedirectToAction("Profile", "Learner", new {learnerID });
                }
                else
                {
                    // Handle duplicate username case
                    ModelState.AddModelError("", "A user with this ID already exists.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during signup: {ex.Message}");
            ModelState.AddModelError("", "An error occurred while signing up. Please try again.");
        }
    }
    return View(model);
}

[HttpGet]
        public IActionResult InstructorSignup()
        {
            var InstructorSignUpViewModel = new InstructorSignUpViewModel
            {
                Name = string.Empty,
                LatestQualification = string.Empty,
                ExpertiseArea = string.Empty,
                Email = string.Empty,
                Password = string.Empty,
                ConfirmPassword = string.Empty
            };

            return View(InstructorSignUpViewModel);
        }

        [HttpPost]
public async Task<IActionResult> InstructorSignup(InstructorSignUpViewModel model)
{
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                int instructorID;
                bool isUnique;

                // Generate a unique instructorID
                do
                {
                    // Insert instructor into instructor table
                    var instructorCommand = new SqlCommand(
                        @"INSERT INTO instructor (name, latest_qualification, expertise_area, email)
                          OUTPUT INSERTED.instructorID 
                          VALUES (@Name, @Qualification, @Expertise, @Email)", connection);

                    instructorCommand.Parameters.AddWithValue("@Name", model.Name);
                    instructorCommand.Parameters.AddWithValue("@Qualification", model.LatestQualification);
                    instructorCommand.Parameters.AddWithValue("@Expertise", model.ExpertiseArea);
                    instructorCommand.Parameters.AddWithValue("@Email", model.Email);

                    instructorID = (int)await instructorCommand.ExecuteScalarAsync();

                    // Check if instructorID already exists in Users table
                    var checkUserCommand = new SqlCommand(
                        @"SELECT COUNT(*) FROM Users WHERE username = @Username", connection);
                    checkUserCommand.Parameters.AddWithValue("@Username", instructorID);

                    int existingUserCount = (int)await checkUserCommand.ExecuteScalarAsync();
                    isUnique = existingUserCount == 0;

                } while (!isUnique);

                // Insert instructor into Users table
                var userCommand = new SqlCommand(
                    @"INSERT INTO Users (username, password, user_type)
                      VALUES (@Username, @Password, @UserType)", connection);

                userCommand.Parameters.AddWithValue("@Username", instructorID);
                userCommand.Parameters.AddWithValue("@Password", model.Password);
                userCommand.Parameters.AddWithValue("@UserType", "Instructor");

                await userCommand.ExecuteNonQueryAsync();

                HttpContext.Session.SetInt32("InstructorID", instructorID);
                return RedirectToAction("Profile", "Instructor", new { instructorID });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during signup: {ex.Message}");
            ModelState.AddModelError("", "An error occurred. Please try again.");
        }

    return View(model);
}

public IActionResult DebugConnection()
{
    return Content($"Current Connection String: {_connectionString}");
}
}
}
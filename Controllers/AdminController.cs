using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Milestone3WebApp.Models;
using System;
using System.Threading.Tasks;

namespace Milestone3WebApp.Controllers
{
    public class AdminController : Controller
    {
        
                private readonly string _connectionString;

                public AdminController()
        {
            // Load the connection string from the environment variable
            _connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new Exception("DATABASE_URL is not set in the environment variables.");
            }
        }
        
        // This action fetches and displays the admin's profile based on the admin's ID
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index(int adminID)
        {

            var adminProfile = await GetAdminProfileAsync(adminID);

            if (adminProfile == null)
            {
                return NotFound("Admin profile not found.");
            }
            return View(adminProfile);
        }

[HttpGet]
    public async Task<IActionResult> AdminProfile(int adminID)
    {
        var adminProfile = await GetAdminProfileAsync(adminID);

        if (adminProfile == null)
        {
            return NotFound("Admin profile not found.");
        }
        return View(adminProfile);
    }

    private async Task<Admin> GetAdminProfileAsync(int adminID)
    {
        Admin adminProfile = null;

        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT * FROM Admins WHERE adminID = @adminID", connection);
                command.Parameters.AddWithValue("@adminID", adminID);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        adminProfile = new Admin
                        {
                            adminID = Convert.ToInt32(reader["adminID"]),
                            name = Convert.ToString(reader["name"]),
                            password = reader["password"]?.ToString(),
                        };
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] {ex.Message}");
        }

        return adminProfile;
    }

[HttpPost]
public async Task<IActionResult> RemoveLearnerProfile(int learnerID)
{
    try
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            // Delete personalization profile for the learner
            var command = new SqlCommand("DELETE FROM personalization_profiles WHERE learnerID = @learnerID", connection);
            command.Parameters.AddWithValue("@learnerID", learnerID);

            int rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected > 0)
            {
                return Json(new { success = true, message = $"Personalization profile for Learner ID {learnerID} deleted successfully." });
            }
            else
            {
                return Json(new { success = false, message = $"No personalization profile found for Learner ID {learnerID}." });
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERROR] {ex.Message}");
                return Json(new { success = false, message = "An error occurred while deleting the profile." });
    }
}

[HttpGet]
public async Task<IActionResult> Management(int adminID)
{
    // Verify admin exists
    var adminProfile = await GetAdminProfileAsync(adminID);
    if (adminProfile == null)
    {
        return NotFound("Admin profile not found.");
    }

    // Pass adminID to the view for further actions
    ViewBag.AdminID = adminID;
    return View();
}

[HttpGet]
public IActionResult RemoveUsers()
{
    // Initialize any required view model
    var removeUsersViewModel = new RemoveUsersViewModel
    {
        LearnerID = 0, // Default value
        InstructorID = 0 // Default value
    };

    return View(removeUsersViewModel); // Pass the view model to the view
}

[HttpGet]
public async Task<IActionResult> GetAllUsers()
{
    var users = new List<dynamic>();
    try
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new SqlCommand(@"
                SELECT 
                    CASE 
                        WHEN U.user_type = 'learner' THEN CONCAT(L.first_name, ' ', L.last_name)
                        WHEN U.user_type = 'instructor' THEN I.name
                        ELSE 'Unknown User'
                    END AS Name,
                    U.username AS ID, 
                    U.user_type AS UserType
                FROM Users U
                LEFT JOIN learner L ON U.username = L.learnerID
                LEFT JOIN instructor I ON U.username = I.instructorID", connection);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    users.Add(new
                    {
                        Name = reader["Name"]?.ToString() ?? "Unknown User",
                        ID = reader["ID"]?.ToString() ?? "N/A",
                        UserType = reader["UserType"]?.ToString() ?? "N/A"
                    });
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("[ERROR] " + ex.Message);
        return Json(new { success = false, message = "An error occurred while retrieving users from the database." });
    }

    return Json(new { success = true, users });
}

[HttpPost]
public async Task<IActionResult> DeleteLearnerAccount(int learnerID)
{
    try
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            // Delete learner from learner table
            var deleteLearnerCommand = new SqlCommand("DELETE FROM learner WHERE learnerID = @learnerID", connection);
            deleteLearnerCommand.Parameters.AddWithValue("@learnerID", learnerID);
            int learnerRowsAffected = await deleteLearnerCommand.ExecuteNonQueryAsync();

            // Delete learner from Users table
            var deleteUserCommand = new SqlCommand("DELETE FROM Users WHERE username = @learnerID AND user_type = 'learner'", connection);
            deleteUserCommand.Parameters.AddWithValue("@learnerID", learnerID);
            int userRowsAffected = await deleteUserCommand.ExecuteNonQueryAsync();

            if (learnerRowsAffected > 0 && userRowsAffected > 0)
            {
                return Json(new { success = true, message = $"Learner account {learnerID} deleted successfully." });
            }
            else
            {
                return Json(new { success = false, message = "No learner account found for the provided ID." });
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("[ERROR] " + ex.Message);
        return Json(new { success = false, message = "An error occurred while deleting the learner account." });
    }
}

[HttpPost]
public async Task<IActionResult> DeleteInstructorAccount(int instructorID)
{
    try
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            // Delete instructor from instructor table
            var deleteInstructorCommand = new SqlCommand("DELETE FROM instructor WHERE instructorID = @instructorID", connection);
            deleteInstructorCommand.Parameters.AddWithValue("@instructorID", instructorID);
            int instructorRowsAffected = await deleteInstructorCommand.ExecuteNonQueryAsync();

            // Delete instructor from Users table
            var deleteUserCommand = new SqlCommand("DELETE FROM Users WHERE username = @instructorID AND user_type = 'instructor'", connection);
            deleteUserCommand.Parameters.AddWithValue("@instructorID", instructorID);
            int userRowsAffected = await deleteUserCommand.ExecuteNonQueryAsync();

            if (instructorRowsAffected > 0 && userRowsAffected > 0)
            {
                return Json(new { success = true, message = $"Instructor account {instructorID} deleted successfully." });
            }
            else
            {
                return Json(new { success = false, message = "No instructor account found for the provided ID." });
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("[ERROR] " + ex.Message);
        return Json(new { success = false, message = "An error occurred while deleting the instructor account." });
    }
}

        [HttpGet]
public async Task<IActionResult> ListUsers()
{
    try
    {
        var users = new List<UserViewModel>();

        // Database Connection
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var query = "SELECT username, user_type FROM Users";
            var command = new SqlCommand(query, connection);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    users.Add(new UserViewModel
                    {
                        Username = reader.GetInt32(reader.GetOrdinal("username")),
                        UserType = reader["user_type"]?.ToString() ?? "N/A"
                    });
                }
            }
        }

        // Pass the list to the view
        return View(users);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
        TempData["ErrorMessage"] = "Failed to load users.";
        return RedirectToAction("Management");
    }
}
    }
}
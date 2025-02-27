using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using Milestone3WebApp.Models;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Milestone3WebApp.Controllers
{
    public class LearnerController : Controller
    {
        
private readonly string _connectionString;

public LearnerController()
{
    _connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
                        ?? throw new InvalidOperationException("DATABASE_URL is not set.");
}
        // This action fetches and displays the learner's profile based on the learner's ID
        [HttpGet]
        public async Task<IActionResult> Index(int learnerID)
        {
            var learnerProfile = await GetLearnerProfileAsync(learnerID);

            if (learnerProfile == null)
            {
                return NotFound("Learner profile not found.");
            }

            // Pass the learner's profile to the view
            return View(learnerProfile);
        }


[HttpGet]
public async Task<IActionResult> Profile(int learnerID)
{
    if (learnerID <= 0)
    {
        return BadRequest("Invalid learner ID.");
    }

    try
    {
        var learnerProfile = await GetLearnerProfileAsync(learnerID);

        if (learnerProfile == null)
        {
            Console.WriteLine($"Learner profile not found for ID: {learnerID}");
            return NotFound("Learner profile not found.");
        }

        return View(learnerProfile);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERROR] Exception occurred: {ex.Message}");
        return StatusCode(500, "An error occurred while processing your request.");
    }
}

        private async Task<PersonalizationProfile> GetLearnerProfileAsync(int learnerID)
{
    PersonalizationProfile learnerProfile = null;

    try
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync(); // Open connection asynchronously

            // First, fetch learner's personal details from the `learner` table
            var learnerCommand = new SqlCommand("SELECT * FROM learner WHERE learnerID = @learnerID", connection);
            learnerCommand.Parameters.AddWithValue("@learnerID", learnerID);

            using (var learnerReader = await learnerCommand.ExecuteReaderAsync())
            {
                if (learnerReader.Read())
                {
                    learnerProfile = new PersonalizationProfile
                    {
                        LearnerID = Convert.ToInt32(learnerReader["learnerID"]),
                        Learner = new Learner
                        {
                            LearnerID = Convert.ToInt32(learnerReader["learnerID"]),
                            FirstName = learnerReader["first_name"]?.ToString() ?? string.Empty,
                            LastName = learnerReader["last_name"]?.ToString() ?? string.Empty,
                            Gender = learnerReader["gender"]?.ToString()?.FirstOrDefault() ?? 'U',
                            BirthDate = learnerReader["birth_date"] != DBNull.Value ? Convert.ToDateTime(learnerReader["birth_date"]) : DateTime.MinValue,
                            Country = learnerReader["country"]?.ToString() ?? string.Empty,
                            CulturalBackground = learnerReader["cultural_background"]?.ToString() ?? string.Empty
                        },
                        ProfileID = 0,
                        PreferredContentType = string.Empty,
                        EmotionalState = string.Empty,
                        PersonalityType = string.Empty
                    };
                }
            }

            // Now, fetch learner's profile information from the `personalization_profiles` table
            if (learnerProfile != null)
            {
                var profileCommand = new SqlCommand("EXEC LearnerInfo @learnerID", connection);
                profileCommand.Parameters.AddWithValue("@learnerID", learnerID);

                using (var profileReader = await profileCommand.ExecuteReaderAsync())
                {
                    if (profileReader.Read())
                    {
                        learnerProfile.ProfileID = Convert.ToInt32(profileReader["profileID"]);
                        learnerProfile.PreferredContentType = profileReader["preferred_content_type"]?.ToString() ?? string.Empty;
                        learnerProfile.EmotionalState = profileReader["emotional_state"]?.ToString() ?? string.Empty;
                        learnerProfile.PersonalityType = profileReader["personality_type"]?.ToString() ?? string.Empty;
                    }
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
    return learnerProfile;
}

[HttpPost]
public async Task<IActionResult> RemoveProfile()
{
    try
    {
        // Ensure the session is configured and available
        if (HttpContext.Session == null)
        {
            Console.WriteLine("[ERROR] Session is not configured for this application.");
            return Json(new { success = false, message = "Session is not configured. Please log in again." });
        }

        // Retrieve learner ID from the session
        int? learnerID = HttpContext.Session.GetInt32("LearnerID");

        if (learnerID == null || learnerID <= 0)
        {
            Console.WriteLine($"[ERROR] Invalid learner ID retrieved from session. LearnerID: {(learnerID?.ToString() ?? "null")}");
            return Json(new { success = false, message = "Invalid learner session. Please log in again." });
        }

        Console.WriteLine($"[DEBUG] Retrieved learnerID from session: {learnerID}");

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            Console.WriteLine("[DEBUG] Database connection established.");

            // Delete personalization profile for the learner
            var command = new SqlCommand("DELETE FROM personalization_profiles WHERE learnerID = @learnerID", connection);
            command.Parameters.AddWithValue("@learnerID", learnerID);

            int rowsAffected = await command.ExecuteNonQueryAsync();
            Console.WriteLine($"[DEBUG] Rows affected: {rowsAffected}");

            if (rowsAffected > 0)
            {
                Console.WriteLine($"[DEBUG] Personalization profile for learnerID {learnerID} deleted.");
                return Json(new { success = true, message = "Personalization profile deleted successfully." });
            }
            else
            {
                Console.WriteLine($"[DEBUG] No personalization profile found for learnerID {learnerID}.");
                return Json(new { success = false, message = "No profile found to delete." });
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERROR] Exception occurred: {ex.Message}");
        return Json(new { success = false, message = "An unexpected error occurred while deleting the profile." });
    }
}

[HttpPost]
public async Task<IActionResult> UploadProfilePicture(int learnerID, IFormFile profilePicture)
{
    if (profilePicture == null || profilePicture.Length == 0)
    {
        return BadRequest("No file selected.");
    }

    // Define folder paths and unique file name
    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/learners");
    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(profilePicture.FileName);
    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
    string databasePath = "/uploads/learners/" + uniqueFileName;

    try
    {
        // Ensure the uploads folder exists
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        // Save the file to the server
        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await profilePicture.CopyToAsync(stream);
        }

        // Save file path to the database
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            // Check if the learner already has a profile picture
            var checkCommand = new SqlCommand(
                "SELECT COUNT(*) FROM LearnerProfilePictures WHERE LearnerID = @LearnerID",
                connection);
            checkCommand.Parameters.AddWithValue("@LearnerID", learnerID);
            int count = (int)await checkCommand.ExecuteScalarAsync();

            if (count > 0)
            {
                // Update existing profile picture
                var updateCommand = new SqlCommand(
                    "UPDATE LearnerProfilePictures SET FilePath = @FilePath, UploadedAt = @UploadedAt WHERE LearnerID = @LearnerID",
                    connection);
                updateCommand.Parameters.AddWithValue("@FilePath", databasePath);
                updateCommand.Parameters.AddWithValue("@UploadedAt", DateTime.UtcNow);
                updateCommand.Parameters.AddWithValue("@LearnerID", learnerID);
                await updateCommand.ExecuteNonQueryAsync();
            }
            else
            {
                // Insert new profile picture
                var insertCommand = new SqlCommand(
                    "INSERT INTO LearnerProfilePictures (LearnerID, FilePath, UploadedAt) VALUES (@LearnerID, @FilePath, @UploadedAt)",
                    connection);
                insertCommand.Parameters.AddWithValue("@LearnerID", learnerID);
                insertCommand.Parameters.AddWithValue("@FilePath", databasePath);
                insertCommand.Parameters.AddWithValue("@UploadedAt", DateTime.UtcNow);
                await insertCommand.ExecuteNonQueryAsync();
            }
        }
    }
    catch (SqlException ex)
    {
        Console.WriteLine($"SQL Error: {ex.Message}");
        return StatusCode(500, "A database error occurred while saving the profile picture.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        return StatusCode(500, "An unexpected error occurred while saving the profile picture.");
    }

    // Redirect to the ViewProfilePicture action after successful upload
    return RedirectToAction("ViewLearnerProfilePicture", new { learnerID = learnerID });
}

[HttpGet]
public IActionResult ViewLearnerProfilePicture(int learnerID)
{
    string filePath = null;

    try
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            var command = new SqlCommand("SELECT FilePath FROM LearnerProfilePictures WHERE LearnerID = @LearnerID", connection);
            command.Parameters.AddWithValue("@LearnerID", learnerID);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    filePath = reader["FilePath"]?.ToString();
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }

    if (string.IsNullOrEmpty(filePath))
    {
        ViewBag.FilePath = null; // Ensure no data is sent if nothing is found
        ViewBag.LearnerID = learnerID;
        return View();
    }

    ViewBag.FilePath = filePath;
    ViewBag.LearnerID = learnerID;

    return View();
}

 [HttpGet]
    public IActionResult UploadLearnerProfilePicture(int learnerID)
    {
        ViewBag.LearnerID = learnerID;
        return View(); // Looks for UploadLearnerProfilePicture.cshtml in Views/Learner
    }

    [HttpGet]
public IActionResult ViewDefaultLearnerProfilePicture()
{
    return View();
}
    }
}
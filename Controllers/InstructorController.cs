using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Milestone3WebApp.Models;
using System;
using System.Threading.Tasks;

namespace Milestone3WebApp.Controllers
{
    public class InstructorController : Controller
    {
       // private readonly string _connectionString = "Server=gpepp.database.windows.net;Database=GPEPP;User Id=alioskiller;Password=Ali_2005@5;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
private readonly string _connectionString;

public InstructorController()
{
    _connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
                        ?? throw new InvalidOperationException("DATABASE_URL is not set in environment variables.");
}
        // This action fetches and displays the instructor's profile based on the instructor's ID
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index(int instructorID)
        {
            var instructorProfile = await GetInstructorProfileAsync(instructorID);

            if (instructorProfile == null)
            {
                return NotFound("Instructor profile not found.");
            }

            return View(instructorProfile);
        }

        public async Task<IActionResult> Profile(int instructorID)
{
    var instructorProfile = await GetInstructorProfileAsync(instructorID);

    if (instructorProfile == null)
    {
        return NotFound("instructor profile not found.");
    }

    return View(instructorProfile);
}

        // This method retrieves the instructor's profile from the database asynchronously
        private async Task<Instructor> GetInstructorProfileAsync(int instructorID)
        {
            Instructor instructorProfile = null;

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(); // Open connection asynchronously
                    var command = new SqlCommand("SELECT * FROM instructor WHERE instructorID = @instructorID", connection);
                    command.Parameters.AddWithValue("@instructorID", instructorID);

                    using (var reader = await command.ExecuteReaderAsync()) // Execute asynchronously
                    {
                        if (reader.Read())
                        {
                            instructorProfile = new Instructor
                            {
                                InstructorID = Convert.ToInt32(reader["instructorID"]),
                                Name = reader["name"]?.ToString() ?? string.Empty,
                                LatestQualification = reader["latest_qualification"]?.ToString() ?? string.Empty,
                                ExpertiseArea = reader["expertise_area"]?.ToString() ?? string.Empty,
                                Email = reader["email"]?.ToString() ?? string.Empty
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error: {ex.Message}");
            }

            return instructorProfile;
        }

        [HttpPost]
public async Task<IActionResult> UploadProfilePicture(int instructorID, IFormFile profilePicture)
{
    if (profilePicture == null || profilePicture.Length == 0)
    {
        return BadRequest("No file selected.");
    }

    // Directory to save uploaded files
    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
    string uniqueFileName = Guid.NewGuid().ToString() + "_" + profilePicture.FileName;
    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

    try
    {
        // Ensure the uploads folder exists
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        // Save the file to the server
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await profilePicture.CopyToAsync(stream);
        }

        // Save the file path to the database
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            // Check if the instructor already has a profile picture
            var checkCommand = new SqlCommand("SELECT COUNT(*) FROM ProfilePictures WHERE instructorID = @InstructorID", connection);
            checkCommand.Parameters.AddWithValue("@InstructorID", instructorID);
            int count = (int)await checkCommand.ExecuteScalarAsync();

            if (count > 0)
            {
                // Update existing profile picture
                var updateCommand = new SqlCommand(
                    "UPDATE ProfilePictures SET FilePath = @FilePath, UploadedAt = @UploadedAt WHERE instructorID = @InstructorID",
                    connection);
                updateCommand.Parameters.AddWithValue("@FilePath", "/uploads/" + uniqueFileName);
                updateCommand.Parameters.AddWithValue("@UploadedAt", DateTime.UtcNow);
                updateCommand.Parameters.AddWithValue("@InstructorID", instructorID);
                await updateCommand.ExecuteNonQueryAsync();
            }
            else
            {
                // Insert new profile picture
                var insertCommand = new SqlCommand(
                    "INSERT INTO ProfilePictures (instructorID, FilePath, UploadedAt) VALUES (@InstructorID, @FilePath, @UploadedAt)",
                    connection);
                insertCommand.Parameters.AddWithValue("@InstructorID", instructorID);
                insertCommand.Parameters.AddWithValue("@FilePath", "/uploads/" + uniqueFileName);
                insertCommand.Parameters.AddWithValue("@UploadedAt", DateTime.UtcNow);
                await insertCommand.ExecuteNonQueryAsync();
            }
        }
    }
    catch (SqlException ex)
    {
        Console.WriteLine($"SQL Error: {ex.Message}");
        return StatusCode(500, $"Database error occurred: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
    }

    // Redirect to the ViewProfilePicture action after successful upload
    return RedirectToAction("ViewProfilePicture", new { instructorID = instructorID });
}

[HttpGet]
public async Task<IActionResult> ViewProfilePicture(int instructorID)
{
    string filePath = null;

    try
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            // Query to retrieve the file path for the given instructor
            var command = new SqlCommand("SELECT FilePath FROM ProfilePictures WHERE instructorID = @InstructorID", connection);
            command.Parameters.AddWithValue("@InstructorID", instructorID);

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (reader.Read())
                {
                    filePath = reader["FilePath"]?.ToString();
                }
            }
        }
    }
    catch (SqlException ex)
    {
        Console.WriteLine($"SQL Error: {ex.Message}");
        return StatusCode(500, $"Database error occurred: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
    }

    if (filePath == null)
    {
        return NotFound("Profile picture not found.");
    }

    ViewBag.FilePath = filePath;
    return View();
}

[HttpGet]
public IActionResult UploadProfilePicture(int instructorID)
{
    ViewBag.InstructorID = instructorID; // Pass instructorID to the view
    return View();
}
    }
}
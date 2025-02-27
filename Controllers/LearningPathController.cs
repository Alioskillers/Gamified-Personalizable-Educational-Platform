using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Milestone3WebApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Milestone3WebApp.Controllers
{
    public class LearningPathController : Controller
    {
        
                private readonly string _connectionString;

                public LearningPathController()
        {
            _connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
                                ?? throw new InvalidOperationException("DATABASE_URL is not set.");
        }
        [HttpGet]
        public IActionResult AddLearningPath()
        {
            var learningPathViewModel = new LearningPathViewModel
            {
                CompletionStatus = string.Empty,
                CustomContent = string.Empty,
                AdaptiveRules = string.Empty
            };

            return View(learningPathViewModel); // Pass ViewModel to the view
        }

        [HttpPost]
        public async Task<IActionResult> AddLearningPath(LearningPathViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid input data." });
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var query = "EXEC NewPath @LearnerID, @ProfileID, @CompletionStatus, @CustomContent, @AdaptiveRules";
                    var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@LearnerID", model.LearnerID);
                    command.Parameters.AddWithValue("@ProfileID", model.ProfileID);
                    command.Parameters.AddWithValue("@CompletionStatus", model.CompletionStatus);
                    command.Parameters.AddWithValue("@CustomContent", model.CustomContent ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@AdaptiveRules", model.AdaptiveRules ?? (object)DBNull.Value);

                    await command.ExecuteNonQueryAsync();
                }

                // Return success response with redirect URL
                return Json(new { success = true, redirectUrl = Url.Action("ViewLearningPaths", "LearningPath") });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
                return Json(new { success = false, message = "An error occurred while creating the learning path." });
            }
        }

        [HttpGet]
public async Task<IActionResult> ViewLearningPaths()
{
    int? instructorId = HttpContext.Session.GetInt32("InstructorID");

    var learningPaths = new List<LearningPathViewModel>();

    using (var connection = new SqlConnection(_connectionString))
    {
        await connection.OpenAsync();

        var query = "SELECT * FROM learning_path"; // Instructors view all learning paths
        var command = new SqlCommand(query, connection);

        using (var reader = await command.ExecuteReaderAsync())
        {
            while (reader.Read())
            {
                learningPaths.Add(new LearningPathViewModel
                {
                    PathID = reader.GetInt32(reader.GetOrdinal("pathID")),
                    LearnerID = reader.GetInt32(reader.GetOrdinal("learnerID")),
                    ProfileID = reader.GetInt32(reader.GetOrdinal("profileID")),
                    CompletionStatus = reader.GetString(reader.GetOrdinal("completion_status")),
                    CustomContent = reader.IsDBNull(reader.GetOrdinal("custom_content")) ? string.Empty : reader.GetString(reader.GetOrdinal("custom_content")),
                    AdaptiveRules = reader.IsDBNull(reader.GetOrdinal("adaptive_rules")) ? string.Empty : reader.GetString(reader.GetOrdinal("adaptive_rules"))
                });
            }
        }
    }

    return View(learningPaths); // Pass the list of paths to the view
}
    }
}
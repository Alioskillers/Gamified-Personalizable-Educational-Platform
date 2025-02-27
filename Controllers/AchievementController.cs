using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Milestone3WebApp.Models;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Milestone3WebApp.Controllers
{
    public class AchievementController : Controller
    {
        
                private readonly string _connectionString;

public AchievementController()
        {
            // Load the connection string from the environment variable
            _connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new Exception("DATABASE_URL is not set in the environment variables.");
            }
        }

        [HttpGet]
        public IActionResult CreateAchievement()
        {
            var model = new AchievementViewModel();
            return View(model); // Render the achievement creation page
        }

        [HttpPost]
        public async Task<IActionResult> CreateAchievement(AchievementViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "Invalid input.";
                return View(model);
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Call the NewAchievement procedure
                    var newAchievementCommand = new SqlCommand("NewAchievement", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    newAchievementCommand.Parameters.AddWithValue("@LearnerID", model.LearnerID);
                    newAchievementCommand.Parameters.AddWithValue("@BadgeID", model.BadgeID);
                    newAchievementCommand.Parameters.AddWithValue("@description", model.Description);
                    newAchievementCommand.Parameters.AddWithValue("@date_earned", model.DateEarned);
                    newAchievementCommand.Parameters.AddWithValue("@type", model.Type);

                    await newAchievementCommand.ExecuteNonQueryAsync();

                    // Send notification
                    var notifyCommand = new SqlCommand("NotifyAchievement", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    notifyCommand.Parameters.AddWithValue("@LearnerID", model.LearnerID);
                    notifyCommand.Parameters.AddWithValue("@Message", $"You have earned a new achievement: {model.Description}");
                    notifyCommand.Parameters.AddWithValue("@UrgencyLevel", "High");

                    await notifyCommand.ExecuteNonQueryAsync();

                    ViewBag.Message = "Achievement added and learner notified successfully.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"An error occurred: {ex.Message}";
            }

            return View(model);
        }

        [HttpGet]
public async Task<IActionResult> ViewAchievements()
{
    var learnerID = HttpContext.Session.GetInt32("LearnerID");
    if (learnerID == null)
    {
        TempData["Error"] = "You must be logged in to view your achievements.";
        return RedirectToAction("Login", "Account");
    }

    var achievements = new List<AchievementModel>();

    using (var connection = new SqlConnection(_connectionString))
    {
        await connection.OpenAsync();

        var command = new SqlCommand("GetLearnerAchievements", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@LearnerID", learnerID);

        using (var reader = await command.ExecuteReaderAsync())
        {
            while (reader.Read())
            {
                achievements.Add(new AchievementModel
                {
                    AchievementID = reader.GetInt32(reader.GetOrdinal("achievementID")),
                    BadgeTitle = reader.GetString(reader.GetOrdinal("BadgeTitle")),
                    Description = reader.GetString(reader.GetOrdinal("description")),
                    DateEarned = reader.GetDateTime(reader.GetOrdinal("date_earned")),
                    Type = reader.GetString(reader.GetOrdinal("type"))
                });
            }
        }
    }

    return View(achievements);
}
    }
}
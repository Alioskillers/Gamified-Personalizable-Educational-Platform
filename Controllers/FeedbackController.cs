using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Milestone3WebApp.Models;  // Ensure namespace matches where your models are located

namespace Milestone3WebApp.Controllers
{
    public class FeedbackController : Controller
    {
        
                private readonly string _connectionString;

        public FeedbackController()
        {
            // Load the connection string from the environment variable
            _connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new Exception("DATABASE_URL is not set in the environment variables.");
            }
        }
        
[HttpGet]
public IActionResult SubmitFeedback()
{
    var model = new SubmitFeedbackViewModel
    {
        Timestamp = DateTime.Now.TimeOfDay // current time as default
    };
    return View(model);
}

[HttpPost]
public async Task<IActionResult> SubmitFeedback(SubmitFeedbackViewModel model)
{
    if (!ModelState.IsValid)
    {
        ModelState.AddModelError("", "Please fill out all required fields.");
        return View(model);
    }

    try
    {
        int? learnerID = HttpContext.Session.GetInt32("LearnerID");
        if (learnerID == null || learnerID <= 0)
        {
            TempData["Error"] = "Learner is not logged in or invalid.";
            return View(model);
        }

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            // Use the stored procedure to insert feedback
            var command = new SqlCommand("ActivityEmotionalFeedback", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@ActivityID", model.ActivityID);
            command.Parameters.AddWithValue("@LearnerID", learnerID);
            command.Parameters.AddWithValue("@timestamp", model.Timestamp);  // TIME in SQL maps to TimeSpan in C#
            command.Parameters.AddWithValue("@emotionalstate", model.EmotionalState);

            await command.ExecuteNonQueryAsync();
        }

        TempData["Message"] = "Your feedback has been submitted successfully!";
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERROR]: {ex.Message}");
        TempData["Error"] = "An error occurred while submitting your feedback.";
    }

    return RedirectToAction("ViewFeedbacks");
}

[HttpGet]
    public async Task<IActionResult> ViewFeedbacks()
    {
        var feedbacks = new List<EmotionalFeedbackViewModel>();

        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand(
                    "SELECT * FROM emotional_feedback",
                    connection
                );

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        feedbacks.Add(new EmotionalFeedbackViewModel
                        {
                            FeedbackID = reader.GetInt32(reader.GetOrdinal("feedbackID")),
                            ActivityID = reader.GetInt32(reader.GetOrdinal("activityID")),
                            LearnerID = reader.GetInt32(reader.GetOrdinal("learnerID")),
                            Timestamp = reader.GetDateTime(reader.GetOrdinal("timestamp")),
                            EmotionalState = reader.GetString(reader.GetOrdinal("emotional_state"))
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR]: {ex.Message}");
            TempData["Error"] = "An error occurred while fetching all emotional feedbacks.";
        }

        return View(feedbacks);
    }
    }
}
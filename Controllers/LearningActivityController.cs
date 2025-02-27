using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Milestone3WebApp.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Milestone3WebApp.Controllers
{
    public class LearningActivityController : Controller
    {
        
private readonly string _connectionString;

public LearningActivityController()
{
    _connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
                        ?? throw new InvalidOperationException("DATABASE_URL is not set.");
}
[HttpGet]
public IActionResult AddActivity(int moduleId, int courseId)
{
    var model = new LearningActivityViewModel
    {
        ModuleID = moduleId,
        CourseID = courseId
    };

    return View(model);
}

        [HttpGet]
public async Task<IActionResult> ViewActivities(int moduleId, int courseId, string role)
{
    
    var activities = new List<LearningActivityViewModel>();

    using (var connection = new SqlConnection(_connectionString))
    {
        await connection.OpenAsync();
        var query = "SELECT * FROM learning_activities";
        var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@ModuleID", moduleId);
        command.Parameters.AddWithValue("@CourseID", courseId);

        using (var reader = await command.ExecuteReaderAsync())
        {
            while (reader.Read())
            {
                activities.Add(new LearningActivityViewModel
                {
                    ActivityID = reader.GetInt32(reader.GetOrdinal("activityID")),
                    ModuleID = reader.GetInt32(reader.GetOrdinal("moduleID")),
                    CourseID = reader.GetInt32(reader.GetOrdinal("courseID")),
                    ActivityType = reader.GetString(reader.GetOrdinal("activity_type")),
                    InstructionDetails = reader.GetString(reader.GetOrdinal("instruction_details")),
                    MaxPoints = reader.GetInt32(reader.GetOrdinal("max_points"))
                });
            }
        }
    }

    ViewBag.ModuleId = moduleId;
    ViewBag.CourseId = courseId;

    if (role.ToString() == "Instructor")
    {
        return View("ViewActivities", activities); // Instructor-specific view
    }
    else if (role.ToString() == "Learner")
    {
        return View("ViewActivitiesLearner", activities); // Learner-specific view
    }
    else
    {
        return Json(new { success = false, message = "Invalid role specified." });
    }
}

         [HttpPost]
public async Task<IActionResult> AddActivity([FromBody] LearningActivityViewModel model)
{
    

    try
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new SqlCommand("NewActivity", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@CourseID", model.CourseID);
            command.Parameters.AddWithValue("@ModuleID", model.ModuleID);
            command.Parameters.AddWithValue("@activitytype", model.ActivityType);
            command.Parameters.AddWithValue("@instructiondetails", model.InstructionDetails);
            command.Parameters.AddWithValue("@maxpoints", model.MaxPoints);

            await command.ExecuteNonQueryAsync();
        }

        return Json(new { success = true, message = "Activity added successfully!" });
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = "Error adding activity: " + ex.Message });
    }
}
    }
}
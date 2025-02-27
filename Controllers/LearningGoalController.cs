using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Milestone3WebApp.Models;

public class LearningGoalController : Controller
{
        
private readonly string _connectionString;

public LearningGoalController()
{
    _connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
                        ?? throw new InvalidOperationException("DATABASE_URL is not set.");
}
[HttpGet]
public IActionResult AddLearningGoal()
{
    var learningGoalViewModel = new LearningGoalViewModel
    {
        Status = string.Empty,       // Initialize with an empty status
        Deadline = DateTime.Now,     // Default to current date/time
        Description = string.Empty   // Initialize with an empty description
    };

    return View(learningGoalViewModel); // Pass the initialized ViewModel to the view
}

[HttpGet]
public async Task<IActionResult> LearningGoals()
{
    int? learnerId = HttpContext.Session.GetInt32("LearnerID");
    if (learnerId == null)
    {
        return RedirectToAction("Login", "Account");
    }

    var goals = new List<LearningGoalViewModel>();

    using (var connection = new SqlConnection(_connectionString))
    {
        await connection.OpenAsync();

        var query = "SELECT * FROM learning_goal WHERE learnerID = @LearnerID";
        var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@LearnerID", learnerId);

        using (var reader = await command.ExecuteReaderAsync())
        {
            while (reader.Read())
            {
                goals.Add(new LearningGoalViewModel
                {
                    ID = reader.GetInt32(reader.GetOrdinal("ID")),
                    Status = reader.GetString(reader.GetOrdinal("status")),
                    Deadline = reader.GetDateTime(reader.GetOrdinal("deadline")),
                    Description = reader.GetString(reader.GetOrdinal("description"))
                });
            }
        }
    }

    return View(goals); // Pass the view model to the view
}

    [HttpPost]
public async Task<IActionResult> AddLearningGoal(LearningGoalViewModel model)
{
    int? learnerId = HttpContext.Session.GetInt32("LearnerID");
    if (learnerId == null)
    {
        return Json(new { success = false, message = "Learner ID not found in session." });
    }

    if (!ModelState.IsValid)
    {
        return Json(new { success = false, message = "Invalid input data." });
    }

    try
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var query = "INSERT INTO learning_goal (status, deadline, description, learnerID) VALUES (@Status, @Deadline, @Description, @LearnerID)";
            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Status", model.Status);
            command.Parameters.AddWithValue("@Deadline", model.Deadline);
            command.Parameters.AddWithValue("@Description", model.Description);
            command.Parameters.AddWithValue("@LearnerID", learnerId);

            await command.ExecuteNonQueryAsync();
        }

        // Return success response with the redirect URL
        return Json(new { success = true, redirectUrl = Url.Action("LearningGoals", "LearningGoal") });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERROR] {ex.Message}");
        return Json(new { success = false, message = "An error occurred while adding the learning goal." });
    }
}
}
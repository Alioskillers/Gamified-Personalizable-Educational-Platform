using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Milestone3WebApp.Models;
using System.Data;

namespace Milestone3WebApp.Controllers
{
    public class QuestController : Controller
    {
        
private readonly string _connectionString;

        public QuestController()
        {
            _connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
                                ?? throw new InvalidOperationException("DATABASE_URL is not set.");
        }
        [HttpGet]
public IActionResult AddQuest()
{
    return View();
}

[HttpGet]
public IActionResult SetDeadline()
{
    return View();
}

        [HttpGet]
public async Task<IActionResult> ViewQuests()
{
    var quests = new List<QuestViewModel>();

    using (var connection = new SqlConnection(_connectionString))
    {
        await connection.OpenAsync();
        var query = "SELECT * FROM quest";
        var command = new SqlCommand(query, connection);

        using (var reader = await command.ExecuteReaderAsync())
        {
            while (reader.Read())
            {
                quests.Add(new QuestViewModel
                {
                    QuestID = reader.GetInt32(reader.GetOrdinal("questID")),
                    DifficultyLevel = reader.GetString(reader.GetOrdinal("difficulty_level")),
                    Criteria = reader.GetString(reader.GetOrdinal("criteria")),
                    Description = reader.GetString(reader.GetOrdinal("description")),
                    Title = reader.GetString(reader.GetOrdinal("title")),
                    Deadline = reader.GetDateTime(reader.GetOrdinal("deadline"))
                });
            }
        }
    }

    return View(quests);
}

        [HttpPost]
public async Task<IActionResult> JoinQuest([FromBody] int questID)
{
    try
    {
        // Retrieve LearnerID from the session
        int? learnerID = HttpContext.Session.GetInt32("LearnerID");
        if (learnerID == null)
        {
            return Json(new { success = false, message = "You must be logged in to join a quest." });
        }

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand("JoinQuest", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@LearnerID", learnerID.Value);
                command.Parameters.AddWithValue("@QuestID", questID);

                var returnParameter = command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.ReturnValue;

                await command.ExecuteNonQueryAsync();

                int result = (int)returnParameter.Value;

                if (result == 1)
                {
                    return Json(new { success = true, message = "You successfully joined the quest!" });
                }
                else if (result == 2)
                {
                    return Json(new { success = false, message = "You have already joined this quest." });
                }
                else if (result == 3)
                {
                    return Json(new { success = false, message = "This quest is full." });
                }
                else
                {
                    return Json(new { success = false, message = "An unknown error occurred." });
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        return Json(new { success = false, message = "An unexpected error occurred. Please try again later." });
    }
}

[HttpGet]
public async Task<IActionResult> ViewQuestParticipants(int questID)
{
    var participants = new List<QuestParticipantViewModel>();

    using (var connection = new SqlConnection(_connectionString))
    {
        await connection.OpenAsync();

        using (var command = new SqlCommand("GetQuestParticipants", connection))
        {
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@QuestID", questID);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (reader.Read())
                {
                    participants.Add(new QuestParticipantViewModel
                    {
                        LearnerID = reader.GetInt32(reader.GetOrdinal("learnerID")),
                        LearnerName = reader.GetString(reader.GetOrdinal("LearnerName")),
                        QuestID = reader.GetInt32(reader.GetOrdinal("questID")),
                        QuestTitle = reader.GetString(reader.GetOrdinal("QuestTitle"))
                    });
                }
            }
        }
    }

    if (!participants.Any())
    {
        ViewBag.Message = "No participants found for this quest.";
    }

    return View(participants);
}

[HttpPost]
public async Task<IActionResult> AddQuest([FromBody] QuestViewModel model)
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
            var query = @"
                INSERT INTO quest (difficulty_level, criteria, description, title, deadline)
                VALUES (@DifficultyLevel, @Criteria, @Description, @Title, @Deadline)";
            var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@DifficultyLevel", model.DifficultyLevel);
            command.Parameters.AddWithValue("@Criteria", model.Criteria);
            command.Parameters.AddWithValue("@Description", model.Description);
            command.Parameters.AddWithValue("@Title", model.Title);
            command.Parameters.AddWithValue("@Deadline", model.Deadline);

            await command.ExecuteNonQueryAsync();
        }

        return Json(new { success = true, message = "Quest added successfully!" });
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
    }
}

[HttpPost]
public async Task<IActionResult> SetDeadline([FromBody] SetDeadlineViewModel model)
{
    if (model.QuestID <= 0 || model.NewDeadline == default(DateTime))
    {
        return Json(new { success = false, message = "Invalid input data. Please provide a valid Quest ID and Deadline." });
    }

    try
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand("DeadlineUpdate", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@QuestID", model.QuestID);
                command.Parameters.AddWithValue("@deadline", model.NewDeadline);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
                {
                    return Json(new { success = true, message = "Deadline updated successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = "Quest not found." });
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
    }
}

[HttpGet]
public async Task<IActionResult> ViewQuestsForDeletion()
{
    var quests = new List<QuestViewModel>();

    using (var connection = new SqlConnection(_connectionString))
    {
        await connection.OpenAsync();

        var query = "SELECT * FROM quest";
        var command = new SqlCommand(query, connection);

        using (var reader = await command.ExecuteReaderAsync())
        {
            while (reader.Read())
            {
                quests.Add(new QuestViewModel
                {
                    QuestID = reader.GetInt32(reader.GetOrdinal("questID")),
                    DifficultyLevel = reader.GetString(reader.GetOrdinal("difficulty_level")),
                    Criteria = reader.GetString(reader.GetOrdinal("criteria")),
                    Description = reader.GetString(reader.GetOrdinal("description")),
                    Title = reader.GetString(reader.GetOrdinal("title")),
                    Deadline = reader.GetDateTime(reader.GetOrdinal("deadline"))
                });
            }
        }
    }

    return View(quests);
}

[HttpPost]
public async Task<IActionResult> DeleteQuestByCriteria([FromBody] string criteria)
{
    if (string.IsNullOrWhiteSpace(criteria))
    {
        return Json(new { success = false, message = "Invalid criteria. Please provide valid input." });
    }

    try
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand("CriteriaDelete", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@criteria", criteria);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
                {
                    return Json(new { success = true, message = "Quests matching the criteria were deleted successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = "No quests found for the given criteria." });
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
    }
}

[HttpGet]
public async Task<IActionResult> ViewMyQuests()
{
    var learnerID = HttpContext.Session.GetInt32("LearnerID"); // Retrieve LearnerID from session

    if (learnerID == null)
    {
        return RedirectToAction("Login", "Account"); // Redirect to login if learner is not logged in
    }

    var quests = new List<QuestViewModel>();

    using (var connection = new SqlConnection(_connectionString))
    {
        await connection.OpenAsync();

        using (var command = new SqlCommand("QuestParticipants", connection))
        {
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@LearnerID", learnerID);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (reader.Read())
                {
                    quests.Add(new QuestViewModel
                    {
                        QuestID = reader.GetInt32(reader.GetOrdinal("questID")),
                        Title = reader.GetString(reader.GetOrdinal("QuestTitle")),
                        Description = reader.GetString(reader.GetOrdinal("description")),
                        DifficultyLevel = reader.GetString(reader.GetOrdinal("difficulty_level")),
                        Criteria = reader.GetString(reader.GetOrdinal("criteria")),
                        Deadline = reader.GetDateTime(reader.GetOrdinal("deadline"))
                    });
                }
            }
        }
    }

    return View(quests);
}
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Milestone3WebApp.Models;
using System.Threading.Tasks;

namespace Milestone3WebApp.Controllers
{
    public class LearnerProfileEditController : Controller
    {
        
private readonly string _connectionString;

public LearnerProfileEditController()
{
    _connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
                        ?? throw new InvalidOperationException("DATABASE_URL is not set.");
}
        [HttpGet]
public async Task<IActionResult> EditLearner(int id)
{
    var learner = await GetLearnerById(id);
    if (learner == null)
    {
        return NotFound($"Learner with ID {id} not found.");
    }
    HttpContext.Session.SetInt32("LearnerID", learner.LearnerID);

    var model = new LearnerEditViewModel
    {
        ID = learner.LearnerID,
        FirstName = learner.FirstName,
        LastName = learner.LastName
    };

    return View("~/Views/Profile/EditLearner.cshtml", model);
}

        [HttpPost]
public async Task<IActionResult> EditLearner(LearnerEditViewModel model)
{
    int? learnerId = HttpContext.Session.GetInt32("LearnerID");
    if (learnerId == null)
    {
        return Json(new { success = false, message = "Learner ID not found in session." });
    }

    if (!ModelState.IsValid)
    {
        return Json(new { success = false, message = "Invalid data provided." });
    }

    try
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var query = "UPDATE learner SET first_name = @FirstName, last_name = @LastName WHERE learnerID = @ID";
            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@FirstName", model.FirstName);
            command.Parameters.AddWithValue("@LastName", model.LastName);
            command.Parameters.AddWithValue("@ID", model.ID);

            int rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected == 0)
            {
                return Json(new { success = false, message = "No learner found with the provided ID." });
            }
        }
        return Json(new { success = true, redirectUrl = Url.Action("Profile", "Learner", new { learnerID = model.ID }) });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERROR] {ex.Message}");
        return Json(new { success = false, message = "An error occurred while updating the learner profile." });
    }
}

        private async Task<Learner> GetLearnerById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = "SELECT * FROM learner WHERE learnerID = @ID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ID", id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        return new Learner
                        {
                            LearnerID = reader.GetInt32(reader.GetOrdinal("learnerID")),
                            FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                            LastName = reader.GetString(reader.GetOrdinal("last_name")),
                            Gender = reader.GetString(reader.GetOrdinal("gender"))[0],
                            BirthDate = reader.GetDateTime(reader.GetOrdinal("birth_date")),
                            Country = reader.GetString(reader.GetOrdinal("country")),
                            CulturalBackground = reader.GetString(reader.GetOrdinal("cultural_background"))
                        };
                    }
                }
            }

            return null;
        }
    }
}
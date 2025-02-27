using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Milestone3WebApp.Models;

namespace Milestone3WebApp.Controllers
{
    public class PostController : Controller
    {
        
private readonly string _connectionString;

        public PostController()
        {
            _connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
                                ?? throw new InvalidOperationException("DATABASE_URL is not set.");
        }
[HttpGet]
public IActionResult Post()
{
    return View(); // Returns the Post.cshtml form
}

        [HttpPost]
        public async Task<IActionResult> Post(PostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid form data." });
            }

            int? learnerID = HttpContext.Session.GetInt32("LearnerID");
            if (learnerID == null || learnerID <= 0)
            {
                return Json(new { success = false, message = "Learner is not logged in or invalid." });
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var command = new SqlCommand(
                        "INSERT INTO discussion_forum (moduleID, courseID, post, title, last_active, timestamp, description) " +
                        "VALUES (@ModuleID, @CourseID, @Post, @Title, @LastActive, @Timestamp, @Description)",
                        connection
                    );

                    command.Parameters.AddWithValue("@ModuleID", model.ModuleID);
                    command.Parameters.AddWithValue("@CourseID", model.CourseID);
                    command.Parameters.AddWithValue("@Post", model.Post);
                    command.Parameters.AddWithValue("@Title", model.Title);
                    command.Parameters.AddWithValue("@LastActive", model.LastActive ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Timestamp", model.Timestamp);
                    command.Parameters.AddWithValue("@Description", (object)model.Description ?? DBNull.Value);

                    await command.ExecuteNonQueryAsync();
                }

                return Json(new { success = true, message = "Post added successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult DiscussionForums()
        {
            // Returns a view listing discussion forums (implement as needed)
            return View();
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Milestone3WebApp.Models;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace Milestone3WebApp.Controllers
{
    public class DiscussionForumController : Controller
    {
        
                private readonly string _connectionString;

        public DiscussionForumController()
        {
            // Load the connection string from the environment variable
            _connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new Exception("DATABASE_URL is not set in the environment variables.");
            }
        }
        
        [HttpGet]
        public IActionResult CreateForum()
        {
            var model = new ForumCreationViewModel{
                Post=string.Empty,
                Title=string.Empty,
                Description=string.Empty,
                LastActive=DateTime.Now,
                Timestamp=DateTime.Now
            };
            return View(model);
        }

        [HttpGet]
public async Task<IActionResult> DiscussionForums()
{
    List<DiscussionForumViewModel> forums = new List<DiscussionForumViewModel>();

    using (var connection = new SqlConnection(_connectionString))
    {
        await connection.OpenAsync();
        var command = new SqlCommand("SELECT forumID, moduleID, courseID, post, title, last_active, timestamp, description FROM discussion_forum", connection);

        using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                forums.Add(new DiscussionForumViewModel
                {
                    ForumID = reader.GetInt32(reader.GetOrdinal("forumID")),
                    ModuleID = reader.GetInt32(reader.GetOrdinal("moduleID")),
                    CourseID = reader.GetInt32(reader.GetOrdinal("courseID")),
                    Post = reader.GetString(reader.GetOrdinal("post")),
                    Title = reader.GetString(reader.GetOrdinal("title")),
                    LastActive = reader.IsDBNull(reader.GetOrdinal("last_active")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("last_active")),
                    Timestamp = reader.GetDateTime(reader.GetOrdinal("timestamp")),
                    Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description"))
                });
            }
        }
    }

    return View(forums);
}

        [HttpPost]
public async Task<IActionResult> CreateForum([FromBody] ForumCreationViewModel model)
{
    if (!ModelState.IsValid)
    {
        return Json(new { success = false, message = "Invalid data provided." });
    }

    try
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var query = @"
                INSERT INTO discussion_forum (moduleID, courseID, post, title, last_active, timestamp, description)
                VALUES (@ModuleID, @CourseID, @Post, @Title, GETDATE(), GETDATE(), @Description)";
            var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@ModuleID", model.ModuleID);
            command.Parameters.AddWithValue("@CourseID", model.CourseID);
            command.Parameters.AddWithValue("@Post", (object?)model.Post ?? DBNull.Value);
            command.Parameters.AddWithValue("@Title", model.Title);
            command.Parameters.AddWithValue("@Description", (object?)model.Description ?? DBNull.Value);

            await command.ExecuteNonQueryAsync();
        }

        return Json(new { success = true, message = "Forum created successfully!" });
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = "An error occurred while creating the forum: " + ex.Message });
    }
}
    }
}
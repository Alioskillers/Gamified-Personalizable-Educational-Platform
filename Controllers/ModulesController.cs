using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Milestone3WebApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Milestone3WebApp.Controllers
{
    public class ModulesController : Controller
    {
        
private readonly string _connectionString;

        public ModulesController()
        {
            _connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
                                ?? throw new InvalidOperationException("DATABASE_URL is not set.");
        }
public async Task<IActionResult> ViewModules(string role)
{
    var modules = new List<ModuleViewModel>();

    using (var connection = new SqlConnection(_connectionString))
    {
        await connection.OpenAsync();

        var query = "SELECT * FROM modules"; // Fetch all modules
        var command = new SqlCommand(query, connection);

        using (var reader = await command.ExecuteReaderAsync())
        {
            while (reader.Read())
            {
                modules.Add(new ModuleViewModel
                {
                    ModuleID = reader.GetInt32(reader.GetOrdinal("moduleID")),
                    CourseID = reader.GetInt32(reader.GetOrdinal("courseID")),
                    Title = reader.GetString(reader.GetOrdinal("title")),
                    Difficulty = reader.GetInt32(reader.GetOrdinal("difficulty")),
                    ContentUrl = reader.GetString(reader.GetOrdinal("contenturl"))
                });
            }
        }
    }

    if (modules.Count == 0)
    {
        // Add a message if no modules are found
        ViewBag.Message = "No modules found in the database.";
    }

    ViewBag.Role = role; // Pass role information to the view
    return View("ViewModules", modules); // Ensure the correct view is returned with data
}

        [HttpPost]
public async Task<IActionResult> GetModulesByCourse([FromBody] int courseID)
{
    var modules = new List<ModuleViewModel>();

    using (var connection = new SqlConnection(_connectionString))
    {
        await connection.OpenAsync();
        var query = "SELECT * FROM modules WHERE courseID = @courseID";
        var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@courseID", courseID);

        using (var reader = await command.ExecuteReaderAsync())
        {
            while (reader.Read())
            {
                modules.Add(new ModuleViewModel
                {
                    ModuleID = reader.GetInt32(reader.GetOrdinal("moduleID")),
                    CourseID = reader.GetInt32(reader.GetOrdinal("courseID")),
                    Title = reader.GetString(reader.GetOrdinal("title")),
                    Difficulty = reader.GetInt32(reader.GetOrdinal("difficulty")),
                    ContentUrl = reader.GetString(reader.GetOrdinal("contenturl"))
                });
            }
        }
    }

    return Json(modules);
}
    }
}
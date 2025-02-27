using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Milestone3WebApp.Models;

namespace Milestone3WebApp.Controllers
{
    public class BadgeController : Controller
    {
        
                private readonly string _connectionString;

        public BadgeController()
        {
            // Load the connection string from the environment variable
            _connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new Exception("DATABASE_URL is not set in the environment variables.");
            }
        }
        [HttpGet]
        public async Task<IActionResult> ViewBadges()
        {
            var badges = new List<BadgeViewModel>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT * FROM badge";
                var command = new SqlCommand(query, connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        badges.Add(new BadgeViewModel
                        {
                            BadgeID = reader.GetInt32(reader.GetOrdinal("badgeID")),
                            Title = reader.GetString(reader.GetOrdinal("title")),
                            Description = reader.GetString(reader.GetOrdinal("description")),
                            Criteria = reader.GetString(reader.GetOrdinal("criteria")),
                            Points = reader.GetInt32(reader.GetOrdinal("points"))
                        });
                    }
                }
            }

            return View(badges);
        }
    }
}
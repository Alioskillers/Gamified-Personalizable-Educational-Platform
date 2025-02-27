using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using Milestone3WebApp.Models;

namespace Milestone3WebApp.Controllers
{
    public class TrendAnalysisController : Controller
    {
private readonly string _connectionString;

        public TrendAnalysisController()
        {
            _connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
                                ?? throw new InvalidOperationException("DATABASE_URL is not set.");
        }
        [HttpGet]
        public IActionResult TrendsFilter()
        {
            // Render a form for admin/instructor to input CourseID, ModuleID, and TimePeriod
            var model = new TrendsFilterModel
            {
                TimePeriod = DateTime.Now.AddDays(-7) // default last 7 days
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ViewTrends(TrendsFilterModel filter)
        {
            // Use stored procedure based on whether it's an instructor or admin view
            string storedProcedureName = filter.IsInstructorView 
                ? "InstructorEmotionalTrendAnalysis"
                : "EmotionalTrendAnalysis";

            var results = new List<TrendResultViewModel>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(storedProcedureName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@courseID", filter.CourseID);
                        command.Parameters.AddWithValue("@moduleID", filter.ModuleID);
                        command.Parameters.AddWithValue("@TimePeriod", filter.TimePeriod);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                results.Add(new TrendResultViewModel
                                {
                                    LearnerID = reader.GetInt32(reader.GetOrdinal("learnerID")),
                                    LearnerName = reader.GetString(reader.GetOrdinal("learnerName")),
                                    Timestamp = reader.GetDateTime(reader.GetOrdinal("timestamp")),
                                    EmotionalState = reader.GetString(reader.GetOrdinal("emotional_state"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR]: {ex.Message}");
                TempData["Error"] = "An error occurred while fetching emotional trend data.";
            }

            return View(results);
        }
    }
}
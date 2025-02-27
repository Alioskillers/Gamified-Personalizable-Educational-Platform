using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using Milestone3WebApp.Models;

namespace Milestone3WebApp.Controllers
{
    public class LeaderboardController : Controller
    {
        
private readonly string _connectionString;

public LeaderboardController()
{
    _connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
                        ?? throw new InvalidOperationException("DATABASE_URL is not set.");
}
        [HttpGet]
        public IActionResult FilterLearner()
        {
            // Show a form to input LearnerID
            return View(new LeaderboardFilterModel());
        }

        [HttpPost]
        public async Task<IActionResult> FilterLearner(LeaderboardFilterModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Please provide a valid Learner ID.");
                return View(model);
            }

            var results = new List<LearnerRankingViewModel>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("LeaderboardFilter", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@LearnerID", model.LearnerID);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                results.Add(new LearnerRankingViewModel
                                {
                                    LearnerID = reader.GetInt32(reader.GetOrdinal("learnerID")),
                                    Rank = reader.GetInt32(reader.GetOrdinal("rank")),
                                    Score = reader.GetInt32(reader.GetOrdinal("score"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR]: {ex.Message}");
                TempData["Error"] = "Error retrieving learner ranking data.";
                return View(model);
            }

            return View("LearnerRankingResults", results);
        }

        [HttpGet]
        public IActionResult ViewLeaderboard()
        {
            // Show a form to input LeaderboardID
            return View(new LeaderboardRankModel());
        }

        [HttpPost]
        public async Task<IActionResult> ViewLeaderboard(LeaderboardRankModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Please provide a valid Leaderboard ID.");
                return View(model);
            }

            var results = new List<LeaderboardResultViewModel>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("LeaderboardRank", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@LeaderboardID", model.LeaderboardID);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                results.Add(new LeaderboardResultViewModel
                                {
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Rank = reader.GetInt32(reader.GetOrdinal("rank")),
                                    TotalPoints = reader.GetInt32(reader.GetOrdinal("total_points"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR]: {ex.Message}");
                TempData["Error"] = "Error retrieving leaderboard data.";
                return View(model);
            }

            return View("LeaderboardResults", results);
        }

        [HttpGet]
        public async Task<IActionResult> ViewMyRanking()
        {
            int? learnerID = HttpContext.Session.GetInt32("LearnerID");

            if (learnerID == null || learnerID <= 0)
            {
                TempData["Error"] = "You must be logged in as a learner to view your ranking.";
                return RedirectToAction("Login", "Account"); // Adjust to your login action
            }

            var results = new List<LearnerRankingViewModel>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("LeaderboardFilter", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@LearnerID", learnerID);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                results.Add(new LearnerRankingViewModel
                                {
                                    LearnerID = reader.GetInt32(reader.GetOrdinal("learnerID")),
                                    Rank = reader.GetInt32(reader.GetOrdinal("rank")),
                                    Score = reader.GetInt32(reader.GetOrdinal("score"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR]: {ex.Message}");
                TempData["Error"] = "An error occurred while retrieving your ranking.";
            }

            return View("LearnerRankingResults", results);
        }
    }
}
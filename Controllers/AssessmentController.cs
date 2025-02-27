using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Milestone3WebApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Milestone3WebApp.Controllers
{
    public class AssessmentController : Controller
    {
        
                private readonly string _connectionString;

                public AssessmentController()
        {
            // Load the connection string from environment variables
            _connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new Exception("DATABASE_URL is not set in the environment variables.");
            }
        }
        [HttpGet]
        public IActionResult AssessmentScore()
        {
            return View(); // Returns the input page
        }

        [HttpGet]
public IActionResult ViewAnalytics()
{
    // Initialize an empty view model for input
    var model = new AnalyticsRequestViewModel();

    return View(model); // Pass the model to the view
}

[HttpGet]
public IActionResult UpdateGrade()
{
    var model= new GradeUpdateViewModel(); // Initialize an empty view model
    return View(model); // This returns the UpdateGrade.cshtml page
}

[HttpGet]
public IActionResult CreateAssessment()
{
    var model = new CreateAssessmentViewModel(); // Initialize an empty view model
    return View(model); // Ensure it correctly renders the view
}
        
        [HttpGet]
        public async Task<IActionResult> ViewAllAssessments()
        {
            var assessments = new List<AssessmentViewModel>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT * FROM assessments";
                var command = new SqlCommand(query, connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        assessments.Add(new AssessmentViewModel
                        {
                            ID = reader.GetInt32(reader.GetOrdinal("ID")),
                            AssessmentName = reader.GetString(reader.GetOrdinal("assessmentName")),
                            CourseID = reader.GetInt32(reader.GetOrdinal("courseID")),
                            ModuleID = reader.GetInt32(reader.GetOrdinal("moduleID")),
                            Type = reader.GetString(reader.GetOrdinal("type")),
                            TotalMarks = reader.GetInt32(reader.GetOrdinal("total_marks")),
                            PassingMarks = reader.GetInt32(reader.GetOrdinal("passing_marks")),
                            Description = reader.GetString(reader.GetOrdinal("description"))
                        });
                    }
                }
            }

            return View("AllAssessments", assessments);
        }

public async Task<IActionResult> ViewLearnerAssessments()
{
    int learnerID = HttpContext.Session.GetInt32("LearnerID") ?? 0; // Ensure valid learner ID
    
    if (learnerID == 0)
    {
        ViewBag.Message = "Invalid Learner ID.";
        return View(new List<AssessmentViewModel>());
    }

    var assessments = new List<AssessmentViewModel>();
    
    using (var connection = new SqlConnection(_connectionString))
    {
        await connection.OpenAsync();
        
        var query = @"
            SELECT a.ID, a.assessmentName, a.courseID, a.moduleID, a.type, a.total_marks, 
                   a.passing_marks, a.description, la.score
            FROM assessments a
            INNER JOIN learner_assessments la ON a.ID = la.assessmentID
            WHERE la.learnerID = @learnerID";
        
        using (var command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@learnerID", learnerID);
            
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (reader.Read())
                {
                    assessments.Add(new AssessmentViewModel
                    {
                        ID = reader.GetInt32(0),
                        AssessmentName = reader.GetString(1),
                        CourseID = reader.GetInt32(2),
                        ModuleID = reader.GetInt32(3),
                        Type = reader.GetString(4),
                        TotalMarks = reader.GetInt32(5),
                        PassingMarks = reader.GetInt32(6),
                        Description = reader.GetString(7),
                        Score = reader.GetInt32(8)
                    });
                }
            }
        }
    }

    if (!assessments.Any())
    {
        ViewBag.Message = "No assessments found for this learner.";
    }

    return View(assessments);
}

[HttpGet]
public async Task<IActionResult> AssessmentBreakdown()
{
    int learnerID = HttpContext.Session.GetInt32("LearnerID") ?? 0; // Ensure valid learner ID

    if (learnerID == 0)
    {
        ViewBag.Message = "Invalid Learner ID.";
        return View(new List<LearnerAssessmentBreakdownViewModel>());
    }

    var breakdown = new List<LearnerAssessmentBreakdownViewModel>();

    using (var connection = new SqlConnection(_connectionString))
    {
        await connection.OpenAsync();

        var command = new SqlCommand("AssessmentAnalysis", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@LearnerID", learnerID);

        using (var reader = await command.ExecuteReaderAsync())
        {
            while (reader.Read())
            {
                breakdown.Add(new LearnerAssessmentBreakdownViewModel
                {
                    LearnerID = reader.GetInt32(0),
                    AssessmentName = reader.GetString(1),
                    Score = reader.GetInt32(2),
                    Performance = reader.GetString(3)
                });
            }
        }
    }

    if (!breakdown.Any())
    {
        ViewBag.Message = "No assessment scores found for this learner.";
    }

    return View(breakdown);
}

[HttpPost]
public async Task<IActionResult> ViewAssessmentScore(int assessmentID)
{
    int learnerID = HttpContext.Session.GetInt32("LearnerID") ?? 0; // Fetch the current learner's ID
    if (learnerID == 0)
    {
        ViewBag.Message = "Invalid Learner ID. Please login.";
        return View("ViewAssessmentScore");
    }

    int score = 0;

    try
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand("Viewscore", connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@LearnerID", learnerID);
                command.Parameters.AddWithValue("@AssessmentID", assessmentID);

                var scoreParam = new SqlParameter("@score", System.Data.SqlDbType.Int)
                {
                    Direction = System.Data.ParameterDirection.Output
                };
                command.Parameters.Add(scoreParam);

                await command.ExecuteNonQueryAsync();
                score = (int)(scoreParam.Value ?? 0);
            }
        }

        if (score == 0)
        {
            ViewBag.Message = "No score found for this assessment.";
        }
        else
        {
            ViewBag.Score = score;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        ViewBag.Message = "An error occurred while fetching the score.";
    }

    return View("ViewAssessmentScore");
}

[HttpPost]
        public async Task<IActionResult> FetchAssessmentScore(int assessmentID)
        {
            int learnerID = HttpContext.Session.GetInt32("LearnerID") ?? 0; // Get Learner ID from session
            if (learnerID == 0)
            {
                ViewBag.Message = "Invalid Learner ID. Please login.";
                return View("AssessmentScore");
            }

            int score = 0;

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("Viewscore", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@LearnerID", learnerID);
                        command.Parameters.AddWithValue("@AssessmentID", assessmentID);

                        var scoreParam = new SqlParameter("@score", System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Output
                        };
                        command.Parameters.Add(scoreParam);

                        await command.ExecuteNonQueryAsync();
                        score = (int)(scoreParam.Value ?? 0);
                    }
                }

                if (score == 0)
                {
                    ViewBag.Message = "No score found for this assessment.";
                }
                else
                {
                    ViewBag.Score = score;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                ViewBag.Message = "An error occurred while fetching the score.";
            }

            return View("AssessmentScore");
        }

        [HttpPost]
        public async Task<IActionResult> FetchAssessmentAnalytics([FromBody] AnalyticsRequestViewModel request)
        {
            var analyticsData = new List<AnalyticsViewModel>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var command = new SqlCommand("AssessmentAnalytics", connection)
                    {
                        CommandType = System.Data.CommandType.StoredProcedure
                    };
                    command.Parameters.AddWithValue("@CourseID", request.CourseID);
                    command.Parameters.AddWithValue("@ModuleID", request.ModuleID);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            analyticsData.Add(new AnalyticsViewModel
                            {
                                LearnerID = reader.GetInt32(reader.GetOrdinal("learnerID")),
                                CourseID = reader.GetInt32(reader.GetOrdinal("courseID")),
                                ModuleID = reader.GetInt32(reader.GetOrdinal("moduleID")),
                                AverageScore = reader.GetInt32(reader.GetOrdinal("AverageScore"))
                            });
                        }
                    }
                }

                return Json(analyticsData);
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");
                return Json(new { success = false, message = "Database error occurred while fetching analytics." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Json(new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpPost]
public async Task<IActionResult> AddAssessment([FromBody] CreateAssessmentViewModel model)
{
    try
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var query = "INSERT INTO assessments (assessmentName, moduleID, courseID, type, total_marks, passing_marks, criteria, weightage, description, title) VALUES (@Name, @ModuleID, @CourseID, @Type, @TotalMarks, @PassingMarks, @Criteria, @Weightage, @Description, @Title)";
            var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Name", model.AssessmentName);
            command.Parameters.AddWithValue("@ModuleID", model.ModuleID);
            command.Parameters.AddWithValue("@CourseID", model.CourseID);
            command.Parameters.AddWithValue("@Type", model.Type);
            command.Parameters.AddWithValue("@TotalMarks", model.TotalMarks);
            command.Parameters.AddWithValue("@PassingMarks", model.PassingMarks);
            command.Parameters.AddWithValue("@Criteria", model.Criteria);
            command.Parameters.AddWithValue("@Weightage", model.Weightage);
            command.Parameters.AddWithValue("@Description", model.Description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Title", model.Title);

            await command.ExecuteNonQueryAsync();
        }
        return Json(new { success = true, message = "Assessment added successfully!" });
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = ex.Message });
    }
}

    [HttpPost]
public async Task<IActionResult> UpdateGrade([FromBody] GradeUpdateViewModel model)
{
    if (model == null)
    {
        return Json(new { success = false, message = "Invalid input." });
    }

    try
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new SqlCommand("GradeUpdate", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@LearnerID", model.LearnerID);
            command.Parameters.AddWithValue("@AssessmentID", model.AssessmentID);
            command.Parameters.AddWithValue("@points", model.Points);

            await command.ExecuteNonQueryAsync();

            return Json(new { success = true, message = "Grade updated successfully!" });
        }
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = "An error occurred: " + ex.Message });
    }
}

[HttpGet]
public async Task<IActionResult> ViewHighestGrades()
{
    var highestGrades = new List<HighestGradeViewModel>();

    try
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new SqlCommand("HighestGrade", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (reader.Read())
                {
                    highestGrades.Add(new HighestGradeViewModel
                    {
                        CourseID = reader.GetInt32(reader.GetOrdinal("CourseID")),
                        AssessmentID = reader.GetInt32(reader.GetOrdinal("AssessmentID")),
                        AssessmentTitle = reader.GetString(reader.GetOrdinal("AssessmentTitle")),
                        MaxPoints = reader.GetInt32(reader.GetOrdinal("MaxPoints"))
                    });
                }
            }
        }

        return View(highestGrades); // Pass a list
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        ViewBag.Message = "An error occurred while fetching the highest grades.";
        return View(new List<HighestGradeViewModel>()); // Return an empty list if an error occurs
    }
}
    }
}
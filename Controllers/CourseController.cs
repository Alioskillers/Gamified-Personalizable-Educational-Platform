using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Milestone3WebApp.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Milestone3WebApp.Controllers
{
    public class CourseController : Controller
    {
        
private readonly string _connectionString;

        public CourseController()
        {
            // Load the connection string from the environment variable
            _connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new Exception("DATABASE_URL is not set in the environment variables.");
            }
        }

        public IActionResult CheckPrerequisites()
{
    return View("Prerequisits");
}

public IActionResult Enroll()
{
    return View("Enroll");
}

        [HttpGet]
public async Task<IActionResult> GetCoursesForLearner(int learnerId)
{
    var courses = new List<CourseViewModel>();

    using (var connection = new SqlConnection(_connectionString))
    {
        await connection.OpenAsync();

        var command = new SqlCommand("EnrolledCourses", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@LearnerID", learnerId);

        using (var reader = await command.ExecuteReaderAsync())
        {
            while (reader.Read())
            {
                courses.Add(new CourseViewModel
                {
                    CourseID = reader.GetInt32(reader.GetOrdinal("CourseID")),
                    CourseTitle = reader.GetString(reader.GetOrdinal("CourseTitle")),
                    CourseDescription = reader.GetString(reader.GetOrdinal("CourseDescription")),
                    EnrollmentDate = reader.GetDateTime(reader.GetOrdinal("EnrollmentDate")),
                    EnrollmentStatus = reader.GetString(reader.GetOrdinal("EnrollmentStatus"))
                });
            }
        }
    }

    return View("Courses", courses);
}

    [HttpGet]
        public async Task<IActionResult> ViewPreviousCourses()
        {
            int? learnerId = HttpContext.Session.GetInt32("LearnerID");
            if (learnerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var previousCourses = new List<string>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand("TakenCourses", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@LearnerID", learnerId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        previousCourses.Add(reader.GetString(reader.GetOrdinal("title")));
                    }
                }
            }

            return View("PreviousCourses", previousCourses);
        }

        [HttpGet]
public async Task<IActionResult> InstructorPreviousCourses(int learnerID)
{
    if (learnerID == 0)
    {
        Console.WriteLine("Learner ID not provided.");
        return BadRequest("Learner ID is required.");
    }

    var previousCourses = new List<CourseViewModel>();

    using (var connection = new SqlConnection(_connectionString))
    {
        await connection.OpenAsync();

        var command = new SqlCommand("TakenCourses", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@LearnerID", learnerID);

        using (var reader = await command.ExecuteReaderAsync())
        {
            while (reader.Read())
            {
                previousCourses.Add(new CourseViewModel
                {
                    CourseTitle = reader.GetString(reader.GetOrdinal("title"))
                });
            }
        }
    }
    return View("InstructorPreviousCourses", previousCourses);
}

[HttpPost]
public async Task<IActionResult> CheckPrerequisites([FromBody] PrerequisiteCheckModel model)
{
    if (model == null || model.CourseID <= 0 || model.LearnerID <= 0)
    {
        return Json(new { message = "Invalid input data." });
    }

    string resultMessage;

    try
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new SqlCommand("Prerequisites", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@LearnerID", model.LearnerID);
            command.Parameters.AddWithValue("@CourseID", model.CourseID);

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (reader.Read())
                {
                    resultMessage = reader["ResultMessage"].ToString(); // Read the result from SELECT
                }
                else
                {
                    resultMessage = "No prerequisites information returned.";
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        resultMessage = "An error occurred while checking prerequisites.";
    }

    return Json(new { message = resultMessage });
}

[HttpPost]
public async Task<IActionResult> EnrollInCourse([FromBody] EnrollmentViewModel model)
{
    // Validate the input model
    if (model == null || model.LearnerID <= 0 || model.CourseID <= 0)
    {
        return Json(new { success = false, message = "Invalid input. Please provide valid Learner ID and Course ID." });
    }

    // Validate that the learner is trying to enroll themselves
    int? sessionLearnerID = HttpContext.Session.GetInt32("LearnerID");
    if (sessionLearnerID == null || sessionLearnerID != model.LearnerID)
    {
        return Json(new { success = false, message = "You can only enroll yourself in courses." });
    }

    try
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new SqlCommand("Courseregister", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@LearnerID", model.LearnerID);
            command.Parameters.AddWithValue("@CourseID", model.CourseID);

            var result = await command.ExecuteScalarAsync();

            // Handle messages returned by the stored procedure
            return Json(new { success = true, message = result?.ToString() ?? "Course registration successful." });
        }
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = $"Error: {ex.Message}" });
    }
}

[HttpPost]
public async Task<IActionResult> RemoveCourse([FromBody] int courseId)
{
    try
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            // Call the stored procedure to delete the course
            var command = new SqlCommand("CourseRemove", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@courseID", courseId);

            await command.ExecuteNonQueryAsync();
        }

        return Json(new { success = true, message = "Course deleted successfully." });
    }
    catch (SqlException sqlEx) when (sqlEx.Number == 547) // SQL constraint violation
    {
        return Json(new { success = false, message = "There are already students enrolled in the course. Cannot delete." });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        return Json(new { success = false, message = "An unexpected error occurred." });
    }
}

[HttpGet]
public async Task<IActionResult> ViewAllCourses()
{
     var courses = new List<ViewAllCoursesModel>();

    using (var connection = new SqlConnection(_connectionString))
    {
        await connection.OpenAsync();

        var query = "SELECT * FROM course"; // Fetch all courses
        var command = new SqlCommand(query, connection);

        using (var reader = await command.ExecuteReaderAsync())
        {
            while (reader.Read())
            {
                courses.Add(new ViewAllCoursesModel
                {
                    CourseID = reader.GetInt32(reader.GetOrdinal("courseID")),
                    Title = reader.GetString(reader.GetOrdinal("title")),
                    LearningObjective = reader.GetString(reader.GetOrdinal("learning_objective")),
                    CreditPoints = reader.GetInt32(reader.GetOrdinal("credit_points")),
                    DifficultyLevel = reader.GetInt32(reader.GetOrdinal("difficulty_level")),
                    Prerequisites = reader.GetString(reader.GetOrdinal("pre_requisites")),
                    Description = reader.GetString(reader.GetOrdinal("description"))
                });
            }
        }
    }

    return View("ViewAllCourses", courses); // Ensure ViewAllCourses.cshtml is created
}
    }
}
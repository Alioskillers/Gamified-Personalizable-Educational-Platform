using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Milestone3WebApp.Models;
using System.Threading.Tasks;

namespace Milestone3WebApp.Controllers
{
    public class InstructorProfileEditController : Controller
    {
        
private readonly string _connectionString;

public InstructorProfileEditController()
{
    _connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
                        ?? throw new InvalidOperationException("DATABASE_URL is not set.");
}

        [HttpGet]
        public async Task<IActionResult> EditInstructor(int id)
        {
            Console.WriteLine($"Fetching instructor with ID: {id}");

            var instructor = await GetInstructorById(id);
            if (instructor == null)
            {
                Console.WriteLine($"Instructor with ID {id} not found.");
                return NotFound($"Instructor with ID {id} not found.");
            }

            // Store ID in session
            HttpContext.Session.SetInt32("InstructorID", instructor.InstructorID);

            var model = new InstructorEditViewModel
            {
                ID = instructor.InstructorID,
                Name = instructor.Name,
                LatestQualification = instructor.LatestQualification,
                ExpertiseArea = instructor.ExpertiseArea,
                Email = instructor.Email
            };

            return View("~/Views/Profile/EditInstructor.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditInstructor(InstructorEditViewModel model)
        {
            int? instructorId = HttpContext.Session.GetInt32("InstructorID");
            if (instructorId == null)
            {
                return Json(new { success = false, message = "Instructor ID not found in session." });
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

                    var query = @"UPDATE instructor 
                                  SET name = @Name, 
                                      latest_qualification = @LatestQualification, 
                                      expertise_area = @ExpertiseArea, 
                                      email = @Email 
                                  WHERE instructorID = @ID";

                    var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Name", model.Name);
                    command.Parameters.AddWithValue("@LatestQualification", model.LatestQualification);
                    command.Parameters.AddWithValue("@ExpertiseArea", model.ExpertiseArea);
                    command.Parameters.AddWithValue("@Email", model.Email);
                    command.Parameters.AddWithValue("@ID", model.ID);

                    int rowsAffected = await command.ExecuteNonQueryAsync();

                    if (rowsAffected == 0)
                    {
                        return Json(new { success = false, message = "No instructor found with the provided ID." });
                    }
                }

                // Return JSON response with redirection URL
                return Json(new { success = true, redirectUrl = Url.Action("Profile", "Instructor", new { instructorID = model.ID }) });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
                return Json(new { success = false, message = "An error occurred while updating the instructor profile." });
            }
        }

        private async Task<Instructor> GetInstructorById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = "SELECT * FROM instructor WHERE instructorID = @ID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ID", id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        return new Instructor
                        {
                            InstructorID = reader.GetInt32(reader.GetOrdinal("instructorID")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            LatestQualification = reader.GetString(reader.GetOrdinal("latest_qualification")),
                            ExpertiseArea = reader.GetString(reader.GetOrdinal("expertise_area")),
                            Email = reader.GetString(reader.GetOrdinal("email"))
                        };
                    }
                }
            }

            return null;
        }
    }
}
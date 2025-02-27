using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Milestone3WebApp.Models;
//using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Milestone3WebApp.Controllers
{
    public class NotificationController : Controller
    {
        
private readonly string _connectionString;

        public NotificationController()
        {
            _connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
                                ?? throw new InvalidOperationException("DATABASE_URL is not set.");
        }
        // GET: SendNotification View
        [HttpGet]
        public IActionResult SendNotification()
        {
            var model = new SendNotificationViewModel(); // Initialize an empty view model
            return View(model); // Render the SendNotification.cshtml view
        }

        [HttpGet]
        public IActionResult NotifyUpcomingGoals()
        {
            // Initialize an empty view model
            var model = new NotifyUpcomingGoalsViewModel();
            return View(model);
        }

        [HttpPost]
public async Task<IActionResult> SendNotification(SendNotificationViewModel model)
{
    if (!ModelState.IsValid)
    {
        ViewBag.Message = "All fields are required.";
        return View(model);
    }

    try
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand("AssessmentNot", connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;

                // Add parameters for the stored procedure
                command.Parameters.AddWithValue("@timestamp", DateTime.Now);
                command.Parameters.AddWithValue("@message", model.Message);
                command.Parameters.AddWithValue("@urgencylevel", model.UrgencyLevel);
                command.Parameters.AddWithValue("@LearnerID", model.LearnerID);

                // Execute the stored procedure
                await command.ExecuteNonQueryAsync();
            }
        }

        ViewBag.SuccessMessage = "Notification sent successfully!";
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        ViewBag.ErrorMessage = "An error occurred while sending the notification.";
    }

    return View(model);
}

        [HttpGet]
public async Task<IActionResult> ViewNotifications()
{
    // Retrieve learner ID from session
    int? learnerID = HttpContext.Session.GetInt32("LearnerID");
    if (learnerID == null)
    {
        ViewBag.Message = "You are not logged in. Please log in to view notifications.";
        return View(new List<NotificationViewModel>());
    }

    var notifications = new List<NotificationViewModel>();

    try
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new SqlCommand("ViewNot", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@learnerID", learnerID.Value);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (reader.Read())
                {
                    notifications.Add(new NotificationViewModel
                    {
                        ID = reader.GetInt32(reader.GetOrdinal("ID")),
                        Message = reader.GetString(reader.GetOrdinal("message")),
                        Timestamp = reader.GetDateTime(reader.GetOrdinal("timestamp")),
                        UrgencyLevel = reader.GetString(reader.GetOrdinal("urgency_level")),
                        LearnerName = reader.GetString(reader.GetOrdinal("LearnerName"))
                    });
                }
            }
        }

        if (!notifications.Any())
        {
            ViewBag.Message = "No notifications found for your account.";
        }

        return View(notifications); // Render the ViewNotifications.cshtml view
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        ViewBag.Message = "An error occurred while fetching notifications.";
        return View(new List<NotificationViewModel>());
    }
}

[HttpPost]
        public async Task<IActionResult> NotifyUpcomingGoals(NotifyUpcomingGoalsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // If the model state is not valid, return the view which can display validation errors.
                return View(model);
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("NotifyUpcomingGoals", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@LearnerID", model.LearnerID);
                        command.Parameters.AddWithValue("@Deadline", model.Deadline);
                        command.Parameters.AddWithValue("@Message", model.Message);
                        command.Parameters.AddWithValue("@UrgencyLevel", model.UrgencyLevel);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                ViewBag.SuccessMessage = "Goal reminder notification sent successfully!";
            }
            catch (Exception ex)
            {
                // If there is a server-side error, we can use ModelState to show it:
                ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
            }

            return View(model);
        }

        [HttpPost]
public async Task<IActionResult> DeleteNotification(int notificationID)
{
    // Retrieve learner ID from session
    int? learnerID = HttpContext.Session.GetInt32("LearnerID");
    if (learnerID == null)
    {
        TempData["ErrorMessage"] = "You must be logged in to delete notifications.";
        return RedirectToAction("ViewNotifications");
    }

    try
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand("NotificationUpdate", connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;

                // Add parameters for the stored procedure
                command.Parameters.AddWithValue("@LearnerID", learnerID.Value);
                command.Parameters.AddWithValue("@NotificationID", notificationID);
                command.Parameters.AddWithValue("@ReadStatus", 0); // 0 for delete

                // Execute the stored procedure
                await command.ExecuteNonQueryAsync();
            }
        }

        TempData["SuccessMessage"] = "Notification deleted successfully!";
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        TempData["ErrorMessage"] = "An error occurred while deleting the notification.";
    }

    return RedirectToAction("ViewNotifications");
}

[HttpPost]
public IActionResult MarkNotificationAsRead(int notificationID)
{
    // Optionally: Log or process this action if needed
    TempData["Message"] = $"Notification {notificationID} marked as read.";
    return RedirectToAction("ViewNotifications");
}

[HttpGet]
public async Task<IActionResult> ViewAllNotifications()
{
    var notifications = new List<NotificationViewModel>();

    using (var connection = new SqlConnection(_connectionString))
    {
        await connection.OpenAsync();

        var query = @"SELECT 
                        n.ID AS ID, 
                        l.first_name + ' ' + l.last_name AS LearnerName,
                        n.timestamp, 
                        n.urgency_level AS UrgencyLevel, 
                        n.message 
                      FROM notification n
                      INNER JOIN received_notification rn ON n.ID = rn.notificationID
                      INNER JOIN learner l ON rn.learnerID = l.learnerID";

        var command = new SqlCommand(query, connection);

        using (var reader = await command.ExecuteReaderAsync())
        {
            while (reader.Read())
            {
                notifications.Add(new NotificationViewModel
                {
                    ID = reader.GetInt32(reader.GetOrdinal("ID")),
                    LearnerName = reader.GetString(reader.GetOrdinal("LearnerName")),
                    Timestamp = reader.GetDateTime(reader.GetOrdinal("timestamp")),
                    UrgencyLevel = reader.GetString(reader.GetOrdinal("UrgencyLevel")),
                    Message = reader.GetString(reader.GetOrdinal("message"))
                });
            }
        }
    }

    return View(notifications);
}

[HttpPost]
public async Task<IActionResult> AdminMarkNotificationAsRead(int notificationID)
{
    try
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            // Optional: Add additional logic if needed for admin-specific read marking
            TempData["Message"] = $"Notification {notificationID} marked as read.";
        }

        return RedirectToAction("ViewAllNotifications");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        TempData["ErrorMessage"] = "An error occurred while marking the notification as read.";
        return RedirectToAction("ViewAllNotifications");
    }
}

[HttpPost]
public async Task<IActionResult> AdminDeleteNotification(int notificationID)
{
    try
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var query = "DELETE FROM received_notification WHERE notificationID = @NotificationID";
            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@NotificationID", notificationID);

            await command.ExecuteNonQueryAsync();
        }

        return Json(new { success = true, message = "Notification deleted successfully!" });
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
    }
}

[HttpGet]
public async Task<IActionResult> ViewAdminNotifications()
{
    var notifications = new List<AdminNotificationViewModel>();

    try
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var adminID = HttpContext.Session.GetInt32("AdminID");
            if (!adminID.HasValue)
            {
                ViewBag.Message = "You are not logged in. Please log in to view notifications.";
                return View(notifications); // Return an empty list
            }

            var command = new SqlCommand("GetAdminNotifications", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@AdminID", adminID.Value);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (reader.Read())
                {
                    notifications.Add(new AdminNotificationViewModel
                    {
                        NotificationID = reader.GetInt32(reader.GetOrdinal("NotificationID")),
                        Timestamp = reader.GetDateTime(reader.GetOrdinal("Timestamp")),
                        Message = reader.GetString(reader.GetOrdinal("Message")),
                        UrgencyLevel = reader.GetString(reader.GetOrdinal("UrgencyLevel")),
                        AdminID = reader.GetInt32(reader.GetOrdinal("AdminID"))
                    });
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error fetching notifications: {ex.Message}");
        ViewBag.Message = "An error occurred while fetching notifications.";
    }

    return View(notifications); // Return a non-null model
}
}
}
using Microsoft.AspNetCore.Mvc;
using Backend.Context;
using Backend.Models;
using System.Net.Mail;
using System.Net;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly GameDatabaseContext _context;
        private readonly string _smtpServer = "smtp.mailtrap.io";
        private readonly int _smtpPort = 2525;
        private readonly string _smtpUsername = "d75c3a739b7705";
        private readonly string _smtpPassword = "36d4db722e8b95";
        private readonly string _fromEmail = "MarcBoerdijk@example.com";
        private readonly string _toEmail = "to@example.com";

        public ContactsController(GameDatabaseContext context)
        {
            _context = context;
        }

        // POST: api/Contacts
        [HttpPost]
        public async Task<ActionResult> PostContact(Contact contactRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                using (var client = new SmtpClient(_smtpServer, _smtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);

                    string emailBody = $"From: {contactRequest.Email}\n\n" +
                                       $"By {contactRequest.FirstName} {contactRequest.LastName}\n\n" +
                                       $"Tel Number: {contactRequest.TelNumber}\n\n" +
                                       $"{contactRequest.Question}";

                    var mailMessage = new MailMessage(_fromEmail, _toEmail, contactRequest.Subject, emailBody);

                    client.Send(mailMessage);

                    Console.WriteLine("Email sent successfully");

                    return Ok(new { message = "Your contact request has been sent successfully." });
                }
            }
            catch (Exception ex)
            {   
                Console.WriteLine($"Error sending email: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to send email. Please try again later.");   
            }
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Context;
using Backend.Models;
using System.Net.Mail;
using System.Net;
using System;

namespace Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly GameDatabaseContext _context;

        public UsersController(GameDatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }


        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        //log in a user
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Nickname == user.Nickname && u.Password == user.Password);
            if (userInDb == null)
            {
                return NotFound(new { message = "Invalid login credentials" });
            }

            // Generate and send a 2FA code
            await SendSecurityEmail(userInDb);
            return Ok(new { message = "2FA required", userId = userInDb.Id });
        }
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerificationRequest request)
        {
            var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);

            if (userInDb == null)
            {
                return BadRequest(new { message = "User does not exist" });
            }
            if (userInDb.TwoFactorCode != request.Otp)
            {
                return BadRequest(new { message = "Invalid OTP" });
            }
                if (userInDb.TwoFactorExpiry < DateTime.Now)
            {
                return BadRequest(new { message = "OTP expired" });
            }

            // Log the user in
            userInDb.LoggedIn = true;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Login successful", userInDb.IsAdmin }); 
        }


        // Log out a user
        [HttpPost("logout")]
        public async Task<ActionResult<User>> Logout([FromBody] int id)
        {
            var userInDb = await _context.Users.FindAsync(id); // Find user by Id
            if (userInDb == null)
            {
                return NotFound();
            }

            userInDb.LoggedIn = false; // Update the user's login status
            await _context.SaveChangesAsync();
            return userInDb; // Return the updated user data
        }

        private async Task SendSecurityEmail(User userInDb)
        {
            if (string.IsNullOrEmpty(userInDb.Email))
            {
                throw new ArgumentException("No email address has been set.");
            }
            try
            {
                string _smtpServer = "smtp.mailtrap.io";
                int _smtpPort = 2525;
                string _smtpUsername = "d75c3a739b7705";
                string _smtpPassword = "36d4db722e8b95";
                string _fromEmail = "MarcBoerdijk@example.com";

                using (var client = new SmtpClient(_smtpServer, _smtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    //Generate the code 
                    var rng = new Random();
                    var code = string.Concat(Enumerable.Range(0, 6).Select(_ => rng.Next(0, 10)));
                    string emailBody = $"Your security code is:\n\n{code}";

                    using (var mailMessage = new MailMessage(_fromEmail, userInDb.Email, "Security Code", emailBody))
                    {
                        client.Send(mailMessage);
                    }

                    // Save OTP to user record
                    userInDb.TwoFactorCode = code;
                    userInDb.TwoFactorExpiry = DateTime.Now.AddMinutes(15);
                    _context.Users.Update(userInDb);
                    await _context.SaveChangesAsync();
                }   
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to send email", ex);
            }
        }
    }
}

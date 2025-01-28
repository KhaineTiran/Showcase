using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Context;
using Backend.Models;
using System.Net.Mail;
using System.Net;

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
        public async Task<ActionResult<User>> Login(User user)
        {
            var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Nickname == user.Nickname && u.Password == user.Password);
            if (userInDb == null)
            {
                return NotFound();
            }
            userInDb.LoggedIn = true;
            await _context.SaveChangesAsync();
            return userInDb;
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
        private readonly string _smtpServer = "smtp.mailtrap.io";
        private readonly int _smtpPort = 2525;
        private readonly string _smtpUsername = "d75c3a739b7705";
        private readonly string _smtpPassword = "36d4db722e8b95";
        private readonly string _fromEmail = "MarcBoerdijk@example.com";
        private readonly string _toEmail = "to@example.com";
        //TODO: Implement the SendSecurityEmail method
        public async Task SendSecurityEmail(User user)
        {
            //get email from database
            try
            {
                using (var client = new SmtpClient(_smtpServer, _smtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    Random random = new Random();
                    int[] codeArray = new int[6];
                    for (int i = 0; i < 6; i++)
                    {
                        codeArray[i] = random.Next(0, 10);
                    }
                    string code = string.Join("", codeArray);
                    string emailBody = $"Your Security code is as follows:\n\n{code}";

                    var mailMessage = new MailMessage(_fromEmail, user.Email, "Security Code", emailBody);
                    client.Send(mailMessage);

                    user.TwoFactorCode = code;
                    user.TwoFactorExpiry = DateTime.Now.AddMinutes(5);
                    await _context.SaveChangesAsync();
                    //Save the security code and expiry time to the database
                }
            }
            catch (Exception ex)
            {
                // Handle exception (log it, rethrow it, etc.)
                throw new InvalidOperationException("Failed to send email", ex);
            }
        }
        //TODO: Implement the VerifySecurityCode method
        public async Task<ActionResult<User>> VerifySecurityCode(User user)
        {
            var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email && u.TwoFactorCode == user.TwoFactorCode && u.TwoFactorExpiry > DateTime.Now);
            if (userInDb == null)
            {
                return NotFound();
            }
            return userInDb;
        }
    )
}

using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class User 
    {
        public int Id { get; set; }
        [MaxLength(16)]
        [Required(ErrorMessage = "A nickname is required")]
        public string Nickname { get; set; }
        [Required(ErrorMessage = "A password is required")]
        public string Password { get; set; }
        public bool? LoggedIn { get; set; } = false;
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }

        public bool? IsAdmin { get; set; } = false;

        public string? TwoFactorCode { get; set; }
        public DateTime? TwoFactorExpiry { get; set; }
    }
}

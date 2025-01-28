using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Contact
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "First Name is required")]
        [MaxLength(100, ErrorMessage = "First namee cannot be longer than 100 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [MaxLength(100, ErrorMessage = "last Name cannot be longer than 100 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [MaxLength(100, ErrorMessage = "Email cannot be longer than 100 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Telephone Number is required")]
        [Phone(ErrorMessage = "Invalid Telephone Number")]
        [MaxLength(15, ErrorMessage = "A telephone number cannot be longer than 15 characters")]
        public string TelNumber { get; set; }

        [Required(ErrorMessage = "Subject is required")]
        [MaxLength(100, ErrorMessage = "Subject cannot be longer than 100 characters")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Question is required")]
        [MaxLength(500, ErrorMessage = "Thee question    cannot be longer than 500 characters")]
        public string Question { get; set; }
    }
}

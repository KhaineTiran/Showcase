using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class UserScore
    {
        public int Id { get; set; }
        [Required]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Name must be exactly 3 characters.")]
        public string Name { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Score must be a non-negative integer.")]
        public int Score { get; set; }
    }
}

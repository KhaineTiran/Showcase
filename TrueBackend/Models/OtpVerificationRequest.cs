namespace Backend.Models
{
    public class OtpVerificationRequest
    {
        public int UserId { get; set; }
        public string Otp { get; set; }
    }
}

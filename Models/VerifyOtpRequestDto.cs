namespace LudoGameApi.Models
{
    public class VerifyOtpRequestDto
    {
        public string Email { get; set; }
        public string Otp { get; set; }
        public string FullName { get; set; }
        public string Mobile { get; set; }
    }

}

using LudoGameApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;

namespace LudoGameApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly LudoDbContext _context;
        private readonly AuthenticationService _authService;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _otpExpiryDuration = TimeSpan.FromMinutes(5);
        private readonly EmailSender _emailSender;
        private static readonly Dictionary<string, string> _otpStore = new Dictionary<string, string>();

        public UserController(LudoDbContext context, AuthenticationService authService, IConfiguration configuration, IMemoryCache cache, EmailSender emailSender)
        {
            _context = context;
            _authService = authService;
            _configuration = configuration;
            _cache = cache;
            _emailSender = emailSender;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequestDto signUpRequest)
        {
            // Check if email already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == signUpRequest.Email);
            if (existingUser != null)
            {
                return BadRequest("Email already exists.");
            }

            // Generate OTP
            var otp = new Random().Next(100000, 999999).ToString();
            _otpStore[signUpRequest.Email] = otp;

            // Send OTP via email
            _emailSender.SendEmail(
                senderName: "Ludo King Rishav",
                senderEmail: "bshberpteam01@gmail.com",
                receiverName: signUpRequest.FullName,
                receiverEmail: signUpRequest.Email,
                subject: "Your OTP Code",
                message: $"Your OTP code is {otp}."
            );

            return Ok("OTP sent to email.");
        }

        [HttpPost("verify-otp-signup")]
        public async Task<IActionResult> VerifyOtpSignup([FromBody] VerifyOtpRequestDto verifyOtpRequest)
        {
            // Check if the OTP matches
            if (_otpStore.ContainsKey(verifyOtpRequest.Email) && _otpStore[verifyOtpRequest.Email] == verifyOtpRequest.Otp)
            {
                // Create and save user in the database
                var user = new User
                {
                    FullName = verifyOtpRequest.FullName,
                    Email = verifyOtpRequest.Email,
                    Mobile = verifyOtpRequest.Mobile,
                    DateCreated = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Remove the OTP from the store after successful verification
                _otpStore.Remove(verifyOtpRequest.Email);
                var token = await _authService.GenerateAccessToken(verifyOtpRequest.Email);
                return Ok(new { Token = token, DateCreated = user.DateCreated });
            }

            return BadRequest("Invalid OTP.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginRequest.Email);
            if (user == null)
            {
                return BadRequest("Invalid login credentials.");
            }

            // Generate OTP for login
            var otp = new Random().Next(100000, 999999).ToString();
            _otpStore[loginRequest.Email] = otp;

            // Send OTP via email
            _emailSender.SendEmail(
                senderName: "Ludo King Rishav",
                senderEmail: "bshberpteam01@gmail.com",
                receiverName: user.FullName,
                receiverEmail: loginRequest.Email,
                subject: "Your OTP Code",
                message: $"Your OTP code is {otp}."
            );

            return Ok("OTP sent to email.");
        }

        [HttpPost("verify-otp-login")]
        public async Task<IActionResult> VerifyOtpLogin([FromBody] VerifyOtploginRequestDto verifyOtpRequest)
        {
            // Check if the OTP matches
            if (_otpStore.ContainsKey(verifyOtpRequest.Email) && _otpStore[verifyOtpRequest.Email] == verifyOtpRequest.otp)
            {
                // Remove the OTP from the store after successful verification
                _otpStore.Remove(verifyOtpRequest.Email);
                var token = await _authService.GenerateAccessToken(verifyOtpRequest.Email);
                return Ok(new { Token = token });
            }

            return BadRequest("Invalid OTP.");
        }

        private string GenerateOTP()
        {
            var rng = new RNGCryptoServiceProvider();
            var buffer = new byte[6];
            rng.GetBytes(buffer);
            return BitConverter.ToString(buffer).Replace("-", "").Substring(0, 6);
        }
    }
}

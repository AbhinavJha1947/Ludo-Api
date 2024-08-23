using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LudoGameApi;

public class AuthenticationService
{
    private readonly IConfiguration _configuration;
    private readonly LudoDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticationService(IConfiguration configuration, LudoDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> GenerateAccessToken(string email)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("email", email) }),
            Expires = DateTime.UtcNow.AddMonths(3),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public JwtSecurityToken ValidateToken(string authToken)
    {
        try
        {
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(authToken, validationParameters, out validatedToken);

            return validatedToken as JwtSecurityToken;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception during token validation: {ex}");
            return null;
        }
    }

    public async Task<int> GetIdFromToken()
    {
        var authToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);

        var validatedToken = ValidateToken(authToken);
        if (validatedToken != null)
        {
            var emailClaim = validatedToken.Claims.FirstOrDefault(claim => claim.Type == "email");
            if (emailClaim != null)
            {
                var email = emailClaim.Value;
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user != null)
                {
                    return user.Id;
                }
            }
        }

        return -1;
    }
}

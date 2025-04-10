using Microsoft.AspNetCore.Mvc;
using Turnero.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Turnero.Controllers;
    [Route("api/[controller]")]
    [ApiController]
public class AuthController : Controller
{
    // Context to access the database
    private readonly TurneroDataContext _context;

    // Configuration 
    private readonly IConfiguration _configuration;

    public AuthController(TurneroDataContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // ----- Endpoints -----
    [HttpPost("AdminRegisterEndPoint")]
    public async Task<ActionResult> AdminRegister(AuthDto authDto)
    {
        // Check if the user already exists
        if (_context.Users.Any(u => u.Username == authDto.Username))
        {
            return BadRequest("User already exists");
        }

        // Hash the password
        var hashedPassword = HashPassword(authDto.PasswordHash);

        // Assign the default role (e.g., "User" by default, change as needed)
        var user = new User
        {
            Username = authDto.Username,
            PasswordHash = hashedPassword,
            Role = "Admin", // Automatically assigned role
            CreationDate = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("User registered successfully");
    }
    // /api/auth/register - To register a user
    [Authorize(Roles = "Admin")]
    [HttpPost("Register")]
    public async Task<ActionResult> Register(AuthDto authDto)
    {
        // Check if the user already exists
        if (_context.Users.Any(u => u.Username == authDto.Username))
        {
            return BadRequest("User already exists");
        }

        // Hash the password
        var hashedPassword = HashPassword(authDto.PasswordHash);

        // Assign the default role (e.g., "User" by default, change as needed)
        var user = new User
        {
            Username = authDto.Username,
            PasswordHash = hashedPassword,
            Role = "User", // Automatically assigned role
            CreationDate = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("User registered successfully");
    }

    // /api/auth/login - To log in a user, it retrieve the userId.
    [HttpPost("Login")]
    public async Task<ActionResult> Login(AuthDto authDto)
    {
        var hashedPassword = HashPassword(authDto.PasswordHash);
        var user = _context.Users.FirstOrDefault(u => u.Username == authDto.Username && u.PasswordHash == hashedPassword);

        if (user == null)
            return Unauthorized("Invalid username or password");

        // Generate JWT Token
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role) // Ensure role is included
        }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return Ok(new
        {
            Token = tokenHandler.WriteToken(token),
            UserId = user.Id,
            Role = user.Role // Ensure role is returned
        });
    }


    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }
}

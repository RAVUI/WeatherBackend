using Microsoft.AspNetCore.Mvc;
using Supabase;
using WeatherBackend.Service;
using System.Text.Json;
using WeatherBackend.Models;
using System.Security.Claims;

namespace WeatherBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly Client _supabaseClient;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            JwtService jwtService,
            Client supabaseClient,
            ILogger<AuthController> logger)
        {
            _jwtService = jwtService;
            _supabaseClient = supabaseClient;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromQuery] string email, [FromQuery] string password)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    return BadRequest("Email and password are required");
                }

                var response = await _supabaseClient.Auth.SignIn(email, password);
                if (response.User == null) return Unauthorized("Invalid credentials");

                string name = response.User.UserMetadata?.GetValueOrDefault("name")?.ToString() ?? "";
                var accessToken = _jwtService.GenerateToken(response.User.Id, response.User.Email, name);
                var refreshToken = Guid.NewGuid().ToString(); // Replace with secure refresh token logic

                // Store refresh token in Supabase or a database (not shown here)
                // Example: await _supabaseClient.From<RefreshToken>().Insert(new { UserId = response.User.Id, Token = refreshToken });

                Response.Cookies.Append("access_token", accessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax, // Changed to Lax
                    Expires = DateTime.UtcNow.AddHours(1)
                });

                Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                return Ok(new { Id = response.User.Id, Email = response.User.Email });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return BadRequest(ex.Message);
            }
        }

        //[HttpPost("refresh")]
        //public async Task<IActionResult> RefreshToken()
        //{
        //    var refreshToken = Request.Cookies["refresh_token"];
        //    if (string.IsNullOrEmpty(refreshToken)) return Unauthorized("No refresh token provided");

        //    // Validate refresh token (e.g., check against database)
        //    // For simplicity, assume it's valid and linked to a user
        //    var userId = "user-id-from-refresh-token"; // Replace with actual logic
        //    var email = "user-email-from-refresh-token";
        //    var name = "user-name-from-refresh-token";

        //    var newAccessToken = _jwtService.GenerateToken(userId, email, name);

        //    Response.Cookies.Append("access_token", newAccessToken, new CookieOptions
        //    {
        //        HttpOnly = true,
        //        Secure = true,
        //        SameSite = SameSiteMode.Lax,
        //        Expires = DateTime.UtcNow.AddHours(1)
        //    });

        //    return Ok(new { Message = "Token refreshed" });
        //}

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromQuery] string email, [FromQuery] string password)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    return BadRequest("Email and password are required");
                }

                var signUpResponse = await _supabaseClient.Auth.SignUp(email, password);

                if (signUpResponse.User != null)
                {
                    return Ok(new { Message = "User registered successfully. Please log in." });
                }

                return BadRequest("Unable to register user");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _supabaseClient.Auth.SignOut();
                return Ok(new { Message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("check")]
        public IActionResult CheckAuthentication()
        {
            try
            {
                var accessToken = Request.Cookies["access_token"];
                _logger.LogInformation($"Received access_token: {accessToken ?? "null"}");
                if (string.IsNullOrEmpty(accessToken))
                {
                    return Unauthorized("No access token provided");
                }

                var principal = _jwtService.ValidateToken(accessToken);
                if (principal == null)
                {
                    return Unauthorized("Invalid or expired token");
                }

                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var email = principal.FindFirst(ClaimTypes.Email)?.Value;
                return Ok(new { Id = userId, Email = email });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking authentication");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
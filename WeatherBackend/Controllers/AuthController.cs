using Microsoft.AspNetCore.Mvc;
using Supabase;
using WeatherBackend.Models;
using WeatherBackend.Service;
using System.Text.Json;

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
        public async Task<IActionResult> Login(LoginModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                {
                    return BadRequest("Email and password are required");
                }

                var response = await _supabaseClient.Auth.SignIn(model.Email, model.Password);

                if (response.User != null)
                {
                    string name = response.User.UserMetadata?.GetValueOrDefault("name")?.ToString() ?? "";

                    var token = _jwtService.GenerateToken(response.User.Id, response.User.Email, name);

                    return Ok(new AuthResponse
                    {
                        Token = token,
                        Id = response.User.Id,
                        Email = response.User.Email,
                        
                    });
                }

                return Unauthorized("Invalid credentials");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                {
                    return BadRequest("Email and password are required");
                }

                var signUpResponse = await _supabaseClient.Auth.SignUp(model.Email, model.Password);

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
    }
}

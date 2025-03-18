namespace WeatherBackend.Models
{

    public class RegisterModel
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        
    }

    public class AuthResponse
    {
        public required string Token { get; set; }
        public required string Id { get; set; }
        public required string Email { get; set; }
        
    }
}

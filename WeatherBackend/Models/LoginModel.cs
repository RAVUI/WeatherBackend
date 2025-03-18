namespace WeatherBackend.Models
{
    public class LoginResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    public class MessageResponse
    {
        public string Message { get; set; } = string.Empty;
    }

    public class AuthCheckResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordRequest
    {
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string Token { get; set; }
    }
}

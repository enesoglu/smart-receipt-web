using smart_receipt_web.Models;
using System.Text;
using System.Text.Json;

namespace smart_receipt_web.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthService> _logger;

        private const string TokenSessionKey = "JwtToken";
        private const string UserIdSessionKey = "UserId";
        private const string UsernameSessionKey = "Username";

        public AuthService(
            HttpClient httpClient,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuthService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;

            var baseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7018";
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        public async Task<ApiResponse<AuthResponse>?> LoginAsync(string username, string password)
        {
            try
            {
                var request = new { Username = username, Password = password };
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/auth/login", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"Login response: {response.StatusCode}");

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<ApiResponse<AuthResponse>>(responseContent, options);

                if (result?.Success == true && result.Data != null)
                {
                    // Store token in session
                    var session = _httpContextAccessor.HttpContext?.Session;
                    if (session != null)
                    {
                        session.SetString(TokenSessionKey, result.Data.Token);
                        session.SetInt32(UserIdSessionKey, result.Data.UserId);
                        session.SetString(UsernameSessionKey, result.Data.Username);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Login error: {ex.Message}");
                return new ApiResponse<AuthResponse>
                {
                    Success = false,
                    Message = "Connection error. Please try again."
                };
            }
        }

        public async Task<ApiResponse<AuthResponse>?> RegisterAsync(string username, string password)
        {
            try
            {
                var request = new { Username = username, Password = password };
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/auth/register", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"Register response: {response.StatusCode}");

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<ApiResponse<AuthResponse>>(responseContent, options);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Register error: {ex.Message}");
                return new ApiResponse<AuthResponse>
                {
                    Success = false,
                    Message = "Connection error. Please try again."
                };
            }
        }

        public Task LogoutAsync()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            session?.Clear();
            return Task.CompletedTask;
        }

        public string? GetToken()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString(TokenSessionKey);
        }

        public bool IsAuthenticated()
        {
            var token = GetToken();
            return !string.IsNullOrEmpty(token);
        }
    }
}


using smart_receipt_web.Models;

namespace smart_receipt_web.Services
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthResponse>?> LoginAsync(string username, string password);
        Task<ApiResponse<AuthResponse>?> RegisterAsync(string username, string password);
        Task LogoutAsync();
        string? GetToken();
        bool IsAuthenticated();
    }
}


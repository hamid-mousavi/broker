using Broker.DTOs.Auth;

namespace Broker.Services
{
    public interface IAuthService
    {
        Task<TokenResponseDto?> RegisterAsync(RegisterDto registerDto);
        Task<TokenResponseDto?> LoginAsync(LoginDto loginDto);
        Task<TokenResponseDto?> RefreshTokenAsync(string token);
        Task<bool> VerifyEmailAsync(string email, string token);
        Task<bool> ResendVerificationEmailAsync(string email);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetDto);
    }
}


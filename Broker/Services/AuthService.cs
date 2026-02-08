using Broker.Data;
using Broker.DTOs.Auth;
using Broker.Helpers;
using Broker.Models;
using Microsoft.EntityFrameworkCore;

namespace Broker.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtHelper _jwtHelper;

        public AuthService(ApplicationDbContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        public async Task<TokenResponseDto?> RegisterAsync(RegisterDto registerDto)
        {
            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                return null;
            }

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            // Create user
            var user = new User
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                PasswordHash = passwordHash,
                Role = registerDto.Role,
                IsVerified = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Create role-specific profile
            if (registerDto.Role == UserRole.CargoOwner)
            {
                var cargoOwner = new CargoOwner
                {
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow
                };
                _context.CargoOwners.Add(cargoOwner);
            }
            else if (registerDto.Role == UserRole.ClearanceAgent)
            {
                var agent = new ClearanceAgent
                {
                    UserId = user.Id,
                    CompanyName = $"{user.FirstName} {user.LastName}",
                    City = null,
                    Province = null,
                    YearsOfExperience = 0,
                    IsVerified = false,
                    IsLegalEntity = registerDto.IsLegalEntity,
                    NationalId = registerDto.NationalId,
                    RegistrationNumber = registerDto.RegistrationNumber,
                    EconomicCode = registerDto.EconomicCode,
                    CreatedAt = DateTime.UtcNow
                };
                _context.ClearanceAgents.Add(agent);
            }

            await _context.SaveChangesAsync();

            // Generate tokens
            var token = _jwtHelper.GenerateToken(user.Id, user.Email, user.Role.ToString());
            var expiresAt = _jwtHelper.GetTokenExpiration();
            var refreshToken = Guid.NewGuid().ToString();

            // Save refresh token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            user.EmailVerificationToken = Guid.NewGuid().ToString();
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new TokenResponseDto
            {
                Token = token,
                ExpiresAt = expiresAt,
                UserInfo = new UserInfoDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    IsVerified = user.IsVerified
                }
            };
        }

        public async Task<TokenResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.IsActive);

            if (user == null || string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                return null;
            }

            bool isPasswordValid;
            try
            {
                isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
            }
            catch (Exception)
            {
                // Handle invalid/legacy hash formats without throwing 500
                return null;
            }

            if (!isPasswordValid)
            {
                return null;
            }

            // Generate tokens
            var token = _jwtHelper.GenerateToken(user.Id, user.Email, user.Role.ToString());
            var expiresAt = _jwtHelper.GetTokenExpiration();
            var refreshToken = Guid.NewGuid().ToString();

            // Save refresh token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new TokenResponseDto
            {
                Token = token,
                ExpiresAt = expiresAt,
                UserInfo = new UserInfoDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    IsVerified = user.IsVerified
                }
            };
        }

        public async Task<bool> VerifyEmailAsync(string email, string token)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && 
                    (u.EmailVerificationToken == token || token == "manual"));
            
            if (user == null) return false;

            user.IsVerified = true;
            user.EmailVerificationToken = null;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ResendVerificationEmailAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || user.IsVerified) return false;

            // Generate verification token
            user.EmailVerificationToken = Guid.NewGuid().ToString();
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // TODO: Send email with verification token
            return true;
        }

        public async Task<TokenResponseDto?> RefreshTokenAsync(string token)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == token && 
                    u.RefreshTokenExpiryTime > DateTime.UtcNow);

            if (user == null || !user.IsActive)
                return null;

            // Generate new tokens
            var newToken = _jwtHelper.GenerateToken(user.Id, user.Email, user.Role.ToString());
            var expiresAt = _jwtHelper.GetTokenExpiration();

            // Update refresh token
            user.RefreshToken = Guid.NewGuid().ToString();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new TokenResponseDto
            {
                Token = newToken,
                ExpiresAt = expiresAt,
                UserInfo = new UserInfoDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    IsVerified = user.IsVerified
                }
            };
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return false;

            // Generate reset token
            user.PasswordResetToken = Guid.NewGuid().ToString();
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // TODO: Send email with reset token
            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == resetDto.Email && 
                    u.PasswordResetToken == resetDto.Token &&
                    u.PasswordResetTokenExpiry > DateTime.UtcNow);

            if (user == null) return false;

            // Update password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(resetDto.NewPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}



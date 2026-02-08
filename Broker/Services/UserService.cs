using Broker.Data;
using Broker.DTOs.User;
using Microsoft.EntityFrameworkCore;

namespace Broker.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserProfileDto?> GetUserProfileAsync(int userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);

            if (user == null) return null;

            return new UserProfileDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfileImagePath = user.ProfileImagePath,
                Role = user.Role.ToString(),
                IsVerified = user.IsVerified,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<UserProfileDto?> UpdateUserProfileAsync(int userId, UpdateUserProfileDto updateDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);

            if (user == null) return null;

            if (!string.IsNullOrWhiteSpace(updateDto.FirstName))
                user.FirstName = updateDto.FirstName;

            if (!string.IsNullOrWhiteSpace(updateDto.LastName))
                user.LastName = updateDto.LastName;

            if (!string.IsNullOrWhiteSpace(updateDto.PhoneNumber))
                user.PhoneNumber = updateDto.PhoneNumber;

            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return await GetUserProfileAsync(userId);
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);

            if (user == null) return false;

            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
                return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeactivateAccountAsync(int userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return false;

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}


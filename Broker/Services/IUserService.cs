using Broker.DTOs.User;

namespace Broker.Services
{
    public interface IUserService
    {
        Task<UserProfileDto?> GetUserProfileAsync(int userId);
        Task<UserProfileDto?> UpdateUserProfileAsync(int userId, UpdateUserProfileDto updateDto);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
        Task<bool> DeactivateAccountAsync(int userId);
    }
}



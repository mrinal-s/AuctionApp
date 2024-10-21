using BidRestAPI.Model;

namespace BidRestAPI.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(UserRegisterDto registerDto);
        Task<string> LoginAsync(UserLoginDto loginDto);

        Task<bool> UserExistsAsync(Guid userId);
    }

}
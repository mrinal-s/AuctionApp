using BidRestAPI.Model;
using Models;

namespace BidRestAPI.Interfaces
{
    public interface IAuthService
    {
        Task<LoggedInUserDetail> LoginAsync(UserLoginDto loginDto);
        Task<string> RegisterAsync(UserRegisterDto registerDto);

        Task<bool> UserExistsAsync(Guid userId);
    }

}
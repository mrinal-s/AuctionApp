using BidRestAPI.Model;
using System.Security.Claims;

namespace BidRestAPI.Interfaces
{
    public interface ITokenService
    {
        LoggedInUserDetail CreateToken(User user);
    }
}

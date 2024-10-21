using BidRestAPI.Model;
using System.Security.Claims;

namespace BidRestAPI.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}

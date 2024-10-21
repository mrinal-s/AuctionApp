using BidRestAPI.Model;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public interface IUserService
    {

        Task<string> UpdateUserAsync(UserUpdateDto updateDto);
    }
}

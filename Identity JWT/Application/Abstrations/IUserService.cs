using Application.Dtos.User.Request;
using Application.Dtos.User.Respone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstrations
{
    public interface IUserService
    {
        Task<UserRep> CustomerCreateAsync(CustomerCreateUserReq request);

        Task<UserRep> AdminCreateAsync(CreateUserReq request);

        Task<UserRep?> GetByIdAsync(long id);

        Task<IEnumerable<UserRep>> GetAllAsync();

        Task<UserRep?> UpdateAsync(long id, UpdateUserReq request);

        Task<bool> DeleteAsync(long id);
    }
}

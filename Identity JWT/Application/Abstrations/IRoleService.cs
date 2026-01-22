using Application.Dtos.Role.Request;
using Application.Dtos.Role.Respone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstrations
{
    public interface IRoleService
    {
        Task<RoleRep> CreateAsync(CreateRoleReq request);

        Task<RoleRep?> GetByIdAsync(long id);

        Task<IReadOnlyList<RoleRep>> GetAllAsync();

        Task<RoleRep?> UpdateAsync(long id, UpdateRoleReq request);

        Task<bool> DeleteAsync(long id);
    }
}

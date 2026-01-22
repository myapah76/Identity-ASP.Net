using Application.Persistences.Repositories;
using Domain.Entities;
using Infrastructure.ApplicationDbContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(IAppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Role>> GetAllDetailsAsync()
        {
            return await _dbContext.Roles
                .ToListAsync();
        }

        public async Task<Role?> GetByNameAsync(string name)
        {
            var role = await _dbContext.Roles
                .Where(r => r.Name.ToLower() == name.ToLower())
                .FirstOrDefaultAsync();
            return role;
        }
    }
}
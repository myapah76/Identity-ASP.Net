using Domain.Entities;

namespace Application.Persistences.Repositories
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<Role?> GetByNameAsync(string name);

        Task<IEnumerable<Role>> GetAllDetailsAsync();
    }
}
using Domain.Entities;

namespace Application.Persistences.Repositories;

public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
{
    Task<RefreshToken?> GetRefreshTokenByAccountIdAsync(long Id);
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<RefreshToken?> GetByTokenIncludeRevokedAsync(string token);
    Task RevokeAllUserTokensAsync(long userId);
    Task SaveChangesAsync();
}


using Application.Persistences.Repositories;
using Domain.Entities;
using Infrastructure.ApplicationDbContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories;

public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(IAppDbContext dbContext) : base(dbContext)
    {
    }


    public Task<RefreshToken?> GetRefreshTokenByAccountIdAsync(long Id)
    {
        throw new NotImplementedException();
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _dbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token && rt.Is_revoked != true);
    }

    public async Task<RefreshToken?> GetByTokenIncludeRevokedAsync(string token)
    {
        return await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task RevokeAllUserTokensAsync(long userId)
    {
        var tokens = await _dbContext.RefreshTokens
            .Where(rt => rt.User_id == userId && rt.Is_revoked != true)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.Is_revoked = true;
            token.UpdatedAt = DateTimeOffset.UtcNow;
        }

        if (tokens.Any())
        {
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}


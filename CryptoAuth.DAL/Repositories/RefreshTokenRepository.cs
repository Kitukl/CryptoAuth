using CryptoAuth.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CryptoAuth.DAL.Repositories;

public class RefreshTokenRepository : IRepository<RefreshToken>
{
    private readonly AuthDbContext _dbContext;

    public RefreshTokenRepository(AuthDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public Task<RefreshToken> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<RefreshToken> GetByIdAsync(string id)
    {
        return await _dbContext.RefreshTokens.FirstOrDefaultAsync(token => token.Token == id);
    }

    public async Task<string> CreateAsync(RefreshToken obj)
    {
        await _dbContext.RefreshTokens.AddAsync(obj);
        await _dbContext.SaveChangesAsync();

        return obj.Token;
    }

    public Task<RefreshToken> UpdateAsync(RefreshToken obj, string id)
    {
        throw new NotImplementedException();
    }

    public async Task<string> DeleteAsync(string id)
    {
        await _dbContext.RefreshTokens
            .Where(t => t.Token == id)
            .ExecuteDeleteAsync();
        await _dbContext.SaveChangesAsync();

        return id;
    }
}
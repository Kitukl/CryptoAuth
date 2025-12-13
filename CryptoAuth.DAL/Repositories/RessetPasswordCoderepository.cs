using CryptoAuth.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CryptoAuth.DAL.Repositories;

public class RessetPasswordCodeRepository : IRepository<ResetPasswordCode>
{
    private readonly AuthDbContext _dbContext;

    public RessetPasswordCodeRepository(AuthDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public Task<ResetPasswordCode> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<ResetPasswordCode> GetByIdAsync(string id)
    {
        var code = await _dbContext.ResetPasswordCodes.FirstOrDefaultAsync(t => t.Code == id);
        return code;
    }

    public async Task<string> CreateAsync(ResetPasswordCode obj)
    {
        await _dbContext.ResetPasswordCodes.AddAsync(obj);
        await _dbContext.SaveChangesAsync();

        return obj.UserEmail;
    }

    public Task<ResetPasswordCode> UpdateAsync(ResetPasswordCode obj, string id)
    {
        throw new NotImplementedException();
    }

    public async Task<string> DeleteAsync(string id)
    {
        await _dbContext.ResetPasswordCodes
            .Where(t => t.Code == id)
            .ExecuteDeleteAsync();
        await _dbContext.SaveChangesAsync();

        return String.Empty;
    }
}
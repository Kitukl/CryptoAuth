using Microsoft.AspNetCore.Identity;

namespace CryptoAuth.DAL.Repositories;

public class UserRepository : IRepository<IdentityUser>
{
    private readonly AuthDbContext _dbContext;

    public UserRepository(AuthDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public Task<IdentityUser> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<IdentityUser> GetByIdAsync(string id)
    {
        return await _dbContext.Users.FindAsync(id);
    }

    public Task<string> CreateAsync(IdentityUser obj)
    {
        throw new NotImplementedException();
    }

    public Task<IdentityUser> UpdateAsync(IdentityUser obj, string id)
    {
        throw new NotImplementedException();
    }

    public Task<string> DeleteAsync(string id)
    {
        throw new NotImplementedException();
    }
}
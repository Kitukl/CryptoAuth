using CryptoAuth.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CryptoAuth.DAL.Repositories;

public class TrigersRepository : IRepository<User>
{
    private readonly AuthDbContext _context;

    public TrigersRepository(AuthDbContext context)
    {
        _context = context;
    }
    public Task<User> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<User> GetByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<User> GetByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<List<User>> GetTrigerForNotification(string coin, double? sentiment)
    {
        return _context.Users.Where(x => x.Coin == coin && x.Sentiment == sentiment).ToListAsync();
    }

    public Task<string> CreateAsync(User obj)
    {
        throw new NotImplementedException();
    }

    public Task<User> UpdateAsync(User obj, string id)
    {
        throw new NotImplementedException();
    }

    public Task<string> DeleteAsync(string id)
    {
        throw new NotImplementedException();
    }
}
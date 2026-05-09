namespace CryptoAuth.DAL.Repositories;

public interface IRepository<T>
{
    public Task<T> GetAllAsync();
    public Task<T> GetByIdAsync(string id);
    public Task<T> GetByEmailAsync(string email);
    public Task<List<T>> GetTrigerForNotification(string coin, double? sentiment);
    public Task<string> CreateAsync(T obj);
    public Task<T> UpdateAsync(T obj, string id);
    public Task<string> DeleteAsync(string id);
}
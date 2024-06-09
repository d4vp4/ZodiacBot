using DataAccess.Models;

namespace DataAccess.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserAsync(long id);
    Task<User> CreateUserAsync(long id);
    Task UpdateUserAsync();
}

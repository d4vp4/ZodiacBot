using DataAccess.Models;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Operations;

public class UserRepository(ZodiacDbContext context) : IUserRepository
{
    public async Task<User?> GetUserAsync(long id)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == id);

        return user;
    }

    public async Task<User> CreateUserAsync(long id)
    {
        var user = new User { UserId = id };

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        return user;
    }

    public async Task UpdateUserAsync()
    {
        await context.SaveChangesAsync();
    }
}

using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class ZodiacDbContext(DbContextOptions<ZodiacDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Notification> Notifications { get; set; }
}

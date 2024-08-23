using Microsoft.EntityFrameworkCore;
using LudoGameApi.Models;
namespace LudoGameApi
{

    public class LudoDbContext : DbContext
    {
        public LudoDbContext(DbContextOptions<LudoDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Contest> Contests { get; set; }
        public DbSet<Wallet> Wallets { get; set; } 
    }


}

using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Context
{
    public class GameDatabaseContext : DbContext
    {
        public GameDatabaseContext(DbContextOptions<GameDatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<UserScore> UserScores { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed example users
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1, 
                    Nickname = "admin",
                    Password = "test", 
                    IsAdmin = true,
                    Email = "marcboerdijk@hotmmail.com"
                },
                new User
                {
                    Id = 2,
                    Nickname = "Player2",
                    Password = "test",
                    Email = "marcboerdijk@gmail.com"
                }
            );
        }
    }
}

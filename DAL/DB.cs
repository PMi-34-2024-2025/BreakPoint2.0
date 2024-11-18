using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace DAL
{
    public class ApplicationDbContext : DbContext
    {
        // Рядок підключення
        private const string ConnectionString = "Host=breakdatabase.postgres.database.azure.com;Port=5432;Database=BreakDB;Username=postgres;Password=12345678bp!";

        // Конструктор без параметрів (для спрощення використання)
        public ApplicationDbContext()
            : base(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(ConnectionString) // Використовуємо PostgreSQL
                .Options)
        {
            try
            {
                this.Database.EnsureCreated(); // Автоматичне створення бази даних, якщо її немає
            }
            catch (Exception ex)
            {
                Console.WriteLine("Помилка підключення до бази даних: " + ex.Message);
            }
        }

        // DbSet для таблиць
        public DbSet<User> Users { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Sessions> Sessions { get; set; }
        public DbSet<Game> Games { get; set; }

        // Конфігурація моделей
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sessions>()
                .HasKey(s => s.SessionId);

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.User1)
                .WithMany(u => u.Friendships)
                .HasForeignKey(f => f.User1Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.User2)
                .WithMany()
                .HasForeignKey(f => f.User2Id)
                .OnDelete(DeleteBehavior.Restrict);

             modelBuilder.Entity<Sessions>()
                .HasOne(s => s.User)
                .WithMany(u => u.Sessions)
                .HasForeignKey(s => s.UserId);

             modelBuilder.Entity<Sessions>()
                 .HasOne(s => s.Game)
                 .WithMany(g => g.Sessions)
                 .HasForeignKey(s => s.GameId);
        }
    }
}

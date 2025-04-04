using Microsoft.EntityFrameworkCore;
using Turnero.Models;

namespace Turnero
{
    public class TurneroDataContext : DbContext
    {
        public TurneroDataContext(DbContextOptions<TurneroDataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Window> Windows { get; set; }
        public DbSet<Turn> Turns { get; set; }
        public DbSet<Attend> Attends { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<Window>().HasKey(w => w.Id);
            modelBuilder.Entity<Attend>().HasKey(a => a.Id);

            modelBuilder.Entity<Window>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(w => w.UserId);

            modelBuilder.Entity<Attend>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(a => a.UserId);

            modelBuilder.Entity<Attend>()
                .HasOne<Window>()
                .WithMany()
                .HasForeignKey(a => a.WindowId);
        }
    }
}

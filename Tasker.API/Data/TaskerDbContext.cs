using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tasker.API.Models.Database;

namespace Tasker.API.Data
{
    public class TaskerDbContext : IdentityDbContext<TaskerUser>
    {
        public TaskerDbContext(DbContextOptions<TaskerDbContext> options) : base(options)
        {
        }

        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamUser> TeamUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Keep Identity configurations

            modelBuilder.Entity<Team>()
                .HasOne(t => t.CreatedBy)
                .WithMany()
                .HasForeignKey(t => t.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Team>()
                .HasOne(t => t.Leader)
                .WithMany()
                .HasForeignKey(t => t.LeaderId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<TeamUser>()
                .HasKey(tu => new { tu.TeamId, tu.UserId });

            modelBuilder.Entity<TeamUser>()
                .HasOne(tu => tu.Team)
                .WithMany(t => t.TeamUsers)
                .HasForeignKey(tu => tu.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeamUser>()
                .HasOne(tu => tu.User)
                .WithMany(u => u.Teams)
                .HasForeignKey(tu => tu.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

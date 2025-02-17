using Microsoft.EntityFrameworkCore;
using userService.Models;

namespace userService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<DeviceUserRelation> DeviceUserRelations { get; set; }
        public DbSet<RelationUser> RelationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relación User - RelationUser
            modelBuilder.Entity<RelationUser>()
                .HasOne(r => r.User1)
                .WithMany(u => u.RelationUsersAsUser1)
                .HasForeignKey(r => r.UserId1)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RelationUser>()
                .HasOne(r => r.User2)
                .WithMany(u => u.RelationUsersAsUser2)
                .HasForeignKey(r => r.UserId2)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación DeviceUserRelation - RelationUser
            modelBuilder.Entity<RelationUser>()
                .HasOne(r => r.DeviceUserRelation1)
                .WithMany()
                .HasForeignKey(r => r.DeviceUserRelationId1)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RelationUser>()
                .HasOne(r => r.DeviceUserRelation2)
                .WithMany()
                .HasForeignKey(r => r.DeviceUserRelationId2)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación DeviceUserRelation - User y Device
            modelBuilder.Entity<DeviceUserRelation>()
                .HasOne(du => du.User)
                .WithMany(u => u.DeviceUserRelations)
                .HasForeignKey(du => du.UserId);

            modelBuilder.Entity<DeviceUserRelation>()
                .HasOne(du => du.Device)
                .WithMany(d => d.DeviceUserRelations)
                .HasForeignKey(du => du.DeviceId);
        }
    }
}

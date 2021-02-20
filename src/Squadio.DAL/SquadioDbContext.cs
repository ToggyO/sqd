using Microsoft.EntityFrameworkCore;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Resources;
using Squadio.Domain.Models.Roles;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL
{
    public class SquadioDbContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<UserRestorePasswordRequestModel> UserRestorePasswordRequests { get; set; }
        public DbSet<UserChangeEmailRequestModel> UserChangeEmailRequests { get; set; }
        public DbSet<UserConfirmEmailRequestModel> UserConfirmEmailRequests { get; set; }
        public DbSet<RoleModel> Roles { get; set; }
        public DbSet<ResourceModel> Resources { get; set; }
        

        public SquadioDbContext(DbContextOptions<SquadioDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoleModel>(builder =>
            {
                builder.HasKey(model => model.Id);
                builder.Property(model => model.Name).HasColumnType("varchar(20)").IsRequired();

                builder.HasData(
                    new RoleModel() { Id = RoleGuid.User, Name = Role.User.ToString().ToLower() },
                    new RoleModel() { Id = RoleGuid.Admin, Name = Role.Admin.ToString().ToLower() });
            });
            
            modelBuilder.Entity<UserModel>(item =>
            {
                item.HasKey(c => c.Id);
                item.HasIndex(p => p.Email)
                    .IsUnique();
                item.HasOne(x => x.Avatar)
                    .WithOne()
                    .OnDelete(DeleteBehavior.SetNull);
                item.Property(x => x.RoleId)
                    .HasDefaultValue(RoleGuid.User);
                item.HasOne(p => p.Role)
                    .WithMany()
                    .OnDelete(DeleteBehavior.Restrict);
                item.Property(p => p.UITheme)
                    .HasDefaultValue(UIThemeType.Default);
                item.Property(p => p.SignUpType)
                    .HasDefaultValue(SignUpType.Email);
                item.Property(p => p.Status)
                    .HasDefaultValue(UserStatus.Active);
            });
            
            modelBuilder.Entity<UserRestorePasswordRequestModel>(item =>
            {
                item.HasKey(c => c.Id);
                item.HasOne(p => p.User)
                    .WithMany();
            });
            
            modelBuilder.Entity<UserChangeEmailRequestModel>(item =>
            {
                item.HasKey(c => c.Id);
                item.HasOne(p => p.User)
                    .WithMany();
            });
            
            modelBuilder.Entity<UserConfirmEmailRequestModel>(item =>
            {
                item.HasKey(c => c.Id);
                item.HasOne(p => p.User)
                    .WithMany();
            });
            
            modelBuilder.Entity<ResourceModel>(item =>
            {
                item.HasKey(c => c.Id);
                item.HasIndex(p => p.FileName)
                    .IsUnique();
                item.HasOne(x => x.User)
                    .WithMany()
                    .OnDelete(DeleteBehavior.Cascade);
                item.Property(x => x.IsWithResolution)
                    .IsRequired();
            });
        }
    }
}

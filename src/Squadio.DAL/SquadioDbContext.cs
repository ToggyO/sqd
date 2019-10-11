using Microsoft.EntityFrameworkCore;
using Squadio.Domain.Models.Companies;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL
{
    public class SquadioDbContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<UserPasswordRequestModel> UserPasswordRequests { get; set; }
        public DbSet<CompanyModel> Companies { get; set; }
        

        public SquadioDbContext(DbContextOptions<SquadioDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>(entity =>
            {
            });
        }
    }
}

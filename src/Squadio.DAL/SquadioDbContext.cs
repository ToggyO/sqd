using Microsoft.EntityFrameworkCore;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL
{
    public class SquadioDbContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>(entity =>
            {
            });
        }
    }
}

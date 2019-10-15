using Microsoft.EntityFrameworkCore;
using Squadio.Domain.Models.Companies;
using Squadio.Domain.Models.Invites;
using Squadio.Domain.Models.Projects;
using Squadio.Domain.Models.Teams;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL
{
    public class SquadioDbContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<UserRegistrationStepModel> UsersRegistrationStep { get; set; }
        public DbSet<UserPasswordRequestModel> UserPasswordRequests { get; set; }
        public DbSet<CompanyModel> Companies { get; set; }
        public DbSet<CompanyUserModel> CompaniesUsers { get; set; }
        public DbSet<TeamModel> Teams { get; set; }
        public DbSet<TeamUserModel> TeamsUsers { get; set; }
        public DbSet<ProjectModel> Projects { get; set; }
        public DbSet<ProjectUserModel> ProjectsUsers { get; set; }
        public DbSet<InviteModel> Invites { get; set; }
        

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
